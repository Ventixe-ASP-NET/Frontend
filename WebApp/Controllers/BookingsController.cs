using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using WebApp.Views.Bookings;

namespace WebApp.Controllers
{
    public class BookingsController : Controller
    {
        private readonly HttpClient _httpClient;

        public BookingsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://bookingserviceventixe-fbb7amdzfsh4b4d6.swedencentral-01.azurewebsites.net/");
        }

        public async Task<IActionResult> Index()
        {
            var bookings = await _httpClient.GetFromJsonAsync<List<BookingModel>>("/api/bookings");
            return View(bookings);
        }
    }
}
