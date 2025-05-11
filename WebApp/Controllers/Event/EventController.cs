using Microsoft.AspNetCore.Mvc;
using WebApp.Dtos.Event;

namespace WebApp.Controllers.Event
{
    public class EventController : Controller
    {
        private readonly HttpClient _api;

        public EventController(IHttpClientFactory http)
        {
            _api = http.CreateClient("EventApi");
        }



        public async Task<IActionResult> Index()
        {
            
            var wrapper = await _api.GetFromJsonAsync<EventListDtoWrapper>("/api/Event");
            var events = wrapper?.Events ?? new List<DisplayEventsDto>();
            return View(events);
        }
    }
}
