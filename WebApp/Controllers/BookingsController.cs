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
            _bookingGatewayClient = factory.CreateClient("bookingGateway");
            _eventClient = factory.CreateClient("eventApi");
            _bookingClient = factory.CreateClient("bookingApi");
        }

        public async Task<IActionResult> Index(string sort = "date", string order = "desc", int page = 1, int pageSize = 8)
        {
            var bookings = await _bookingGatewayClient.GetFromJsonAsync<List<BookingWithEventModel>>("api/bookingwithevents");
            var stats = await _bookingClient.GetFromJsonAsync<BookingStatsModel>("api/bookings/stats");
            var chart = await _bookingClient.GetFromJsonAsync<BookingChartModel>("api/bookings/stats/overview?range=week");

            if (sort == "none")
            {
                var modelNoSort = new BookingIndexViewModel
                {
                    Bookings = bookings,
                    Stats = stats,
                    Chart = chart
                };
                return View(modelNoSort);
            }

            bookings = sort.ToLower() switch
            {
                "invoice" => order == "asc"
                    ? bookings.OrderBy(b => b.InvoiceId).ToList()
                    : bookings.OrderByDescending(b => b.InvoiceId).ToList(),

                "date" => order == "asc"
                    ? bookings.OrderBy(b => b.CreatedAt).ToList()
                    : bookings.OrderByDescending(b => b.CreatedAt).ToList(),

                "name" => order == "asc"
                    ? bookings.OrderBy(b => b.BookingName).ToList()
                    : bookings.OrderByDescending(b => b.BookingName).ToList(),

                "event" => order == "asc"
                    ? bookings.OrderBy(b => b.EventName).ToList()
                    : bookings.OrderByDescending(b => b.EventName).ToList(),

                "category" => order == "asc"
                    ? bookings.OrderBy(b => b.Category).ToList()
                    : bookings.OrderByDescending(b => b.Category).ToList(),

                "price" => order == "asc"
                    ? bookings.OrderBy(b => b.BookedTickets.Min(t => t.PricePerTicket)).ToList()
                    : bookings.OrderByDescending(b => b.BookedTickets.Max(t => t.PricePerTicket)).ToList(),

                "qty" => order == "asc"
                    ? bookings.OrderBy(b => b.BookedTickets.Sum(t => t.Quantity)).ToList()
                    : bookings.OrderByDescending(b => b.BookedTickets.Sum(t => t.Quantity)).ToList(),

                "amount" => order == "asc"
                    ? bookings.OrderBy(b => b.BookedTickets.Sum(t => t.TotalPrice)).ToList()
                    : bookings.OrderByDescending(b => b.BookedTickets.Sum(t => t.TotalPrice)).ToList(),

                "status" => order == "asc"
                    ? bookings.OrderBy(b => "Confirmed").ToList()
                    : bookings.OrderByDescending(b => "Confirmed").ToList(),

                "evoucher" => order == "asc"
                    ? bookings.OrderBy(b => "–").ToList()
                    : bookings.OrderByDescending(b => "–").ToList(),

                _ => bookings.OrderByDescending(b => b.CreatedAt).ToList()
            };

            var totalCount = bookings.Count;
            var pagedBookings = bookings
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new BookingIndexViewModel
            {
                Bookings = pagedBookings,
                Stats = stats,
                Chart = chart // ✅ Nyckeln
            };

            ViewBag.TotalCount = totalCount;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;

            return View(model);
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
