using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Views.Bookings.BookingModels;

namespace WebApp.Controllers
{
    public class BookingsController : Controller
    {
        private readonly HttpClient _bookingClient;
        private readonly HttpClient _eventClient;

        public BookingsController(IHttpClientFactory factory)
        {
            _bookingClient = factory.CreateClient("bookingGateway");
            _eventClient = factory.CreateClient("eventApi");
        }

        public async Task<IActionResult> Index()
        {
            var bookings = await _bookingClient.GetFromJsonAsync<List<BookingWithEventModel>>("api/bookingwithevents");
            return View(bookings);
        }

        public async Task<IActionResult> Create()
        {
            var wrapper = await _eventClient.GetFromJsonAsync<EventListWrapper>("api/event");

            var activeEvents = wrapper?.Events
                .Where(e => e.Status == 1)
                .ToList() ?? new List<EventDto>();

            var viewModel = new CreateBookingViewModel
            {
                Events = activeEvents.Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = $"{e.EventName} ({e.StartDate:yyyy-MM-dd})"
                }).ToList(),
                EventData = activeEvents // 👈 detta används i dina boxes
            };

            return View(viewModel);
        }
    }
}
