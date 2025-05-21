using WebApp.Views.Bookings.BookingModels;

namespace WebApp.Models;

public class DashboardViewModel
{
    public BookingStatsModel? BookingStats { get; set; } = new();
    public int UpcomingEvents { get; set; } = 0;
    public List<EventDto> Events { get; set; } = new();
    public List<BookingWithEventModel> RecentBookings { get; set; } = new();
}
