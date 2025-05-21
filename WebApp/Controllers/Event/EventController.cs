using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Dtos.Event;
using WebApp.Services.Event;

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

        // GET /Event
        [HttpGet("")]
        public async Task<IActionResult> Index(EventStatus? status)
        {
            var all = (await _events.GetAllEventsAsync()).ToList();
            _logger.LogInformation("Index: fetched {Count} events", all.Count);

            ViewData["CountAll"] = all.Count;
            ViewData["CountDraft"] = all.Count(e => e.StatusEnum == EventStatus.Draft);
            ViewData["CountActive"] = all.Count(e => e.StatusEnum == EventStatus.Active);
            ViewData["CountPast"] = all.Count(e => e.StatusEnum == EventStatus.Past);

            var filtered = status.HasValue
                 ? all.Where(e => e.StatusEnum == status.Value)
                 : all;
            ViewData["CurrentStatus"] = status?.ToString() ?? "All";

            _logger.LogInformation("Index: showing {Count} after filtering", filtered.Count());
            return View(filtered);
        }

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
            return View(new EventCreateDto());
        }

        // POST /Event/New
        [HttpPost("New")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New(EventCreateDto vm)
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
            if (ev == null)
                return NotFound();

   
            var vm = new EventCreateDto
            {
                EventName = ev.EventName,
                EventDescription = ev.Description,
                ImageUrl = ev.ImageUrl,
                CategoryId = ev.Category.Id,
                EventLocationId = ev.Location.Id,

                StartDate = ev.StartDate.Date,
                StartTime = ev.StartDate.TimeOfDay,
                EndDate = ev.EndDate.Date,
                EndTime = ev.EndDate.TimeOfDay,

                TicketTypes = ev.TicketTypes
                                      .Select(t => new TicketTypeCreateDto
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

            // ← NEW: only allow ticket editing on Draft events
            //ViewData["AllowTicketEdit"] = (ev.StatusEnum == EventStatus.Draft);
            //new
            ViewData["AllowTicketEdit"] = true;

        
            return View("New", vm);
        }

        [HttpPost("Edit/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EventCreateDto vm)
        {
            var existing = await _events.GetEventByIdAsync(id);
            if (existing == null) return NotFound();

            ViewData["IsEdit"] = true;
            ViewData["Id"] = id;
            ViewData["AllowTicketEdit"] = true;    // always allow

            if (!ModelState.IsValid)
            {
                await PopulateSelectLists();
                return View("New", vm);
            }


            var ok = await _events.UpdateEventAsync(id, vm);
            if (!ok)
            {
                ModelState.AddModelError("", "Unable to update event.");
                await PopulateSelectLists();
                return View("New", vm);
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
