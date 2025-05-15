using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Views.Bookings.BookingModels
{
    public class CreateBookingViewModel
    {
        public Guid SelectedEventId { get; set; }
        public List<SelectListItem> Events { get; set; } = new();
        public List<EventDto> EventData { get; set; } = new();
    }
}
