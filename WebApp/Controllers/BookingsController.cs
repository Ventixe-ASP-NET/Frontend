using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Views.Bookings.BookingModels;

namespace WebApp.Controllers
{
    public class BookingsController : Controller
    {
        private readonly HttpClient _bookingGatewayClient;
        private readonly HttpClient _eventClient;
        private readonly HttpClient _bookingClient;

        public BookingsController(IHttpClientFactory factory)
        {
            _bookingGatewayClient = factory.CreateClient("BookingGateway");
            _eventClient = factory.CreateClient("EventApi");
            _bookingClient = factory.CreateClient("BookingApi");
        }

        public async Task<IActionResult> Index(string sort = "date", string order = "desc", int page = 1, int pageSize = 8)
        {
            var response = await _bookingGatewayClient.GetFromJsonAsync<PagedResponse<BookingWithEventModel>>(
                $"api/bookingwithevents/paged?sort={sort}&order={order}&page={page}&pageSize={pageSize}");

            var bookings = response?.Items ?? new List<BookingWithEventModel>();
            var totalCount = response?.TotalCount ?? 0;

            var stats = await _bookingClient.GetFromJsonAsync<BookingStatsModel>("api/bookings/stats");
            var chart = await _bookingClient.GetFromJsonAsync<BookingChartModel>("api/bookings/stats/overview?range=week");
            var topCategories = await _bookingGatewayClient.GetFromJsonAsync<TopCategoriesModel>("api/bookingwithevents/stats/top-categories");

            var model = new BookingIndexViewModel
            {
                Bookings = bookings,
                Stats = stats,
                Chart = chart,
                TopCategories = topCategories
            };

            ViewBag.TotalCount = totalCount;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;

            return View("Dashboard/Index", model);
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
                EventData = activeEvents 
            };

            return View("CreateBooking/Create", viewModel);
        }
    }
}
