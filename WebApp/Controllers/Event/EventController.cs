using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
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

        [HttpGet("")]
        public async Task<IActionResult> Index(EventStatus? status)
        {
            var allEvents = (await _events.GetAllEventsAsync())
                .ToList();

            ViewData["CountAll"] = allEvents.Count();
            ViewData["CountDraft"] = allEvents.Count(e => e.StatusEnum == EventStatus.Draft);
            ViewData["CountActive"] = allEvents.Count(e => e.StatusEnum == EventStatus.Active);
            ViewData["CountPast"] = allEvents.Count(e => e.StatusEnum == EventStatus.Past);

            
            var filtered = status.HasValue
            ? allEvents.Where(e => e.OutDatedEvents == status.Value)
            : allEvents;

            ViewData["CurrentStatus"] = status?.ToString() ?? "All";
            return View(filtered);
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> DetailsBySlug(string slug)
        {
            var all = await _events.GetAllEventsAsync();
            var ev = all.FirstOrDefault(e => e.Slug == slug);
            if (ev is null)
                return NotFound();
            return View("Details", ev);
        }

        [HttpPost("{slug}/Activate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(string slug)
        {
            var all = await _events.GetAllEventsAsync();
            var ev = all.FirstOrDefault(e => e.Slug == slug);
            if (ev == null)
                return NotFound();

            if (ev.StatusEnum != EventStatus.Draft)
                return BadRequest();

            var success = await _events.ActivateEventAsync(ev.Id);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Unable to activate event.");
            }

            return RedirectToAction(nameof(DetailsBySlug), new { slug });
        }

        [HttpGet("New")]
        public async Task<IActionResult> New()
        {
            await PopulateSelectLists();
            return View(new EventCreateDto());
        }

        [HttpPost("New")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New(EventCreateDto vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var created = await _events.CreateEventAsync(vm);
            if (created == null)
            {
                ModelState.AddModelError(string.Empty, "Could not create event");
                return View(vm);
            }


            return RedirectToAction(nameof(Index));
        }
        //private or public
        private async Task PopulateSelectLists()
        {
            var categories = await _events.GetAllCategoriesAsync();
            ViewData["Categories"] = new SelectList(
                categories, "CategoryId", "CategoryName");

            var location = await _events.GetLocationsAsync();
            ViewData["Locations"] = new SelectList(location, "Id", "VenueName");

        
        }
    }


   
}
