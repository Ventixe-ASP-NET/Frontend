using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using WebApp.Services.Event;
using WebApp.Models.Event;
using WebApp.Models.Event.EventViewModels;
using WebApp.Models.Event.TicketViewModels;
using WebApp.Models.Event.VenueViewModels;
using WebApp.Models.Event.CategoryViewModels;

namespace WebApp.Controllers.Event
{
    [Route("Event")]
    public class EventController : Controller
    {
        private readonly IEventService _events;
        private readonly ILogger<EventController> _logger;

        public EventController(IHttpClientFactory http, ILogger<EventController> logger, IEventService events)
        {
            _logger = logger;
            _events = events;
        }
        [HttpGet("")]
        public async Task<IActionResult> Index(Models.Event.EventStatus? status, int page = 1)
        {
            const int PageSize = 8;

            // 1) Fetch & log
            var all = (await _events.GetAllEventsAsync()).ToList();
            _logger.LogInformation("Index: fetched {Count} events", all.Count);

            // 2) Sidebar counts & “current” status
            ViewData["CountAll"] = all.Count;
            ViewData["CountDraft"] = all.Count(e => e.StatusEnum == Models.Event.EventStatus.Draft);
            ViewData["CountActive"] = all.Count(e => e.StatusEnum == Models.Event.EventStatus.Active);
            ViewData["CountPast"] = all.Count(e => e.StatusEnum == Models.Event.EventStatus.Past);
            ViewData["CurrentStatus"] = status?.ToString() ?? "All";

            // 3) Filter
            var filtered = status.HasValue
                ? all.Where(e => e.StatusEnum == status.Value).ToList()
                : all;

            // 4) Paging math
            var totalItems = filtered.Count;
            var totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            // 5) Slice out current page
            var pageItems = filtered
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            // 6) Project into your existing EventViewModel
            var vm = new EventListViewModel
            {
                Events = pageItems.Select(dto => new EventViewModel
                {
                    Id = dto.Id,
                    EventName = dto.EventName,
                    EventDescription = dto.EventDescription,
                    ImageUrl = dto.ImageUrl,
                    Status = (int)dto.StatusEnum,            // ← set the int
                    Category = new CategoryViewModel
                    {
                        Id = dto.Category.Id,
                        CategoryName = dto.Category.CategoryName
                    },
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    Location = new LocationViewModel
                    {
                        Id = dto.Location.Id,
                        VenueName = dto.Location.VenueName,
                        City = dto.Location.City
                    },
                    TicketTypes = dto.TicketTypes.Select(t => new TicketTypeViewModel
                    {
                        Id = t.Id,
                        TicketType = t.TicketType,
                        Price = t.Price,
                        TicketsLeft = t.TicketsLeft
                    }).ToList(),
                    TotalTickets = dto.TicketTypes.Sum(t => t.TicketsLeft + t.TicketsSold)
                }).ToList(),

                PageNumber = page,
                TotalPages = totalPages,
                CurrentStatus = status
            };

            return View(vm);
        }

        // GET /Event
        //[HttpGet("")]
        //public async Task<IActionResult> Index(Models.Event.EventStatus? status)
        //{
        //    var all = (await _events.GetAllEventsAsync()).ToList();
        //    _logger.LogInformation("Index: fetched {Count} events", all.Count);

        //    ViewData["CountAll"] = all.Count;
        //    ViewData["CountDraft"] = all.Count(e => e.StatusEnum == Models.Event.EventStatus.Draft);
        //    ViewData["CountActive"] = all.Count(e => e.StatusEnum == Models.Event.EventStatus.Active);
        //    ViewData["CountPast"] = all.Count(e => e.StatusEnum == Models.Event.EventStatus.Past);

        //    var filtered = status.HasValue
        //         ? all.Where(e => e.StatusEnum == status.Value)
        //         : all;
        //    ViewData["CurrentStatus"] = status?.ToString() ?? "All";

        //    _logger.LogInformation("Index: showing {Count} after filtering", filtered.Count());
        //    return View(filtered);
        //}


        // GET /Event/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var ev = await _events.GetEventByIdAsync(id);
            if (ev == null) return NotFound();
            return View("Details", ev);
        }

        // POST /Event/{id}/Activate
        [HttpPost("{id:guid}/Activate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(Guid id)
        {
            var ok = await _events.ActivateEventAsync(id);
            if (!ok) ModelState.AddModelError("", "Unable to activate event.");
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET /Event/New
        [HttpGet("New")]
        public async Task<IActionResult> New()
        {
            await PopulateSelectLists();

            var vm = new EventCreateViewModel();
          
            vm.TicketTypes = new List<TicketTypeCreateViewModel> {
            new TicketTypeCreateViewModel { Id = Guid.Empty }
        };

            return View(vm);

         
        }

        // POST /Event/New
        [HttpPost("New")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New(EventCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await PopulateSelectLists();
                return View(vm);
            }

            var created = await _events.CreateEventAsync(vm);
            if (created == null)
            {
                ModelState.AddModelError("", "Could not create event");
                await PopulateSelectLists();
                return View(vm);
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpGet("Edit/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var ev = await _events.GetEventByIdAsync(id);
            if (ev == null) return NotFound();

        
            var vm = new EventUpdateViewModel
            {
                EventName = ev.EventName,
                EventDescription = ev.EventDescription,
                ImageUrl = ev.ImageUrl,
                CategoryId = ev.Category.Id,
                EventLocationId = ev.Location.Id,

                StartDate = ev.StartDate.Date,
                StartTime = ev.StartDate.TimeOfDay,
                EndDate = ev.EndDate.Date,
                EndTime = ev.EndDate.TimeOfDay,

                TicketTypes = ev.TicketTypes
                                .Select(t => new TicketTypeUpdateViewModel
                                {
                                    Id = t.Id,
                                    TicketType = t.TicketType,
                                    Price = t.Price,
                                    TotalTickets = t.TicketsSold + t.TicketsLeft
                                })
                                .ToList()
            };

            await PopulateSelectLists();

            ViewData["IsEdit"] = true;
            ViewData["Id"] = id;
            ViewData["AllowTicketEdit"] = true;

            return View("Edit", vm);   
        }


        // POST /Event/Edit/{id}
        [HttpPost("Edit/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EventUpdateViewModel vm)
        {
            var existing = await _events.GetEventByIdAsync(id);
            if (existing == null) return NotFound();

            ViewData["IsEdit"] = true;
            ViewData["Id"] = id;
            ViewData["AllowTicketEdit"] = true;

            if (!ModelState.IsValid)
            {
                await PopulateSelectLists();
                return View("Edit", vm);
            }

            _logger.LogInformation("POST Edit: bound {Count} ticket(s)", vm.TicketTypes.Count);
            foreach (var t in vm.TicketTypes)
            {
                _logger.LogInformation(" → Ticket idx: Id={Id}, Type=\"{Type}\", Price={Price}, Total={Total}",
                    t.Id, t.TicketType, t.Price, t.TotalTickets);
            }

            var ok = await _events.UpdateEventAsync(id, vm);
            if (!ok)
            {
                ModelState.AddModelError("", "Unable to update event.");
                await PopulateSelectLists();
                return View("Edit", vm);
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // POST /Event/Delete/{id}
        [HttpPost("Delete/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _events.DeleteEventAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // ──────────────────────────────────────────────────────────────────────

        private async Task PopulateSelectLists()
        {
            var categories = await _events.GetAllCategoriesAsync();
            ViewData["Categories"] = new SelectList(categories, "Id", "CategoryName");

            var locations = await _events.GetLocationsAsync();
            ViewData["Locations"] = new SelectList(locations, "Id", "VenueName");
        }
    }
}
