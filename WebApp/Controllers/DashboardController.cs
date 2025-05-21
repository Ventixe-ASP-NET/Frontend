using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Views.Bookings.BookingModels;

namespace WebApp.Controllers;

[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{

    private readonly HttpClient _bookingClient;
    private readonly HttpClient _eventClient;

    public DashboardController(IHttpClientFactory factory)
    {
        _eventClient = factory.CreateClient("eventApi");
        _bookingClient = factory.CreateClient("bookingApi");
    }

    public async Task<IActionResult> Index()
    {
        var bookingStats = await _bookingClient.GetFromJsonAsync<BookingStatsModel>("api/bookings/stats");
        var events = await _eventClient.GetFromJsonAsync<EventListWrapper>("api/event");

        List<EventDto> firstEvents = [];
        int activeEvents = 0;

        if (events != null)
        {
            firstEvents = events.Events.OrderByDescending(e => e.TicketTypes.Sum(t => t.TicketsSold)).Take(3).ToList();
            activeEvents = events.Events.Count(e => e.Status == 1);
        }

        var viewModel = new DashboardViewModel
        {
            BookingStats = bookingStats,
            UpcomingEvents = activeEvents,
            Events = firstEvents
        };

        return View(viewModel);
    }
}
