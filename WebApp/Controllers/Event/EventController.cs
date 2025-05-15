using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using WebApp.Dtos.Event;
using WebApp.Services.Event;

namespace WebApp.Controllers.Event
{
    [Route("Event")]
    public class EventController : Controller
    {
        private readonly HttpClient _api;
        private readonly IEventService _events;
        private readonly ILogger<EventController> _logger;

        public EventController(IHttpClientFactory http, ILogger<EventController> logger, IEventService events)
        {
            _api = http.CreateClient("EventApi");
            _logger = logger;
            _events = events;
        }


        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var allEvents = await _events.GetAllEventsAsync();
            return View(allEvents);

        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> DetailsBySlug(string slug)
        {
            var all = await _events.GetAllEventsAsync();
            var ev = all.FirstOrDefault(e => e.Slug == slug);

            if (ev is null)
            {
                return NotFound();
            }
            return View("Details", ev);
        }
    }
}
