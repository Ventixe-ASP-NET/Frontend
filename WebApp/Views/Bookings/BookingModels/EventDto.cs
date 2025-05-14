using System.Text.Json.Serialization;

namespace WebApp.Views.Bookings.BookingModels
{
    public class EventDto
    {
        public Guid Id { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public int Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public EventLocationModel Location { get; set; } = new();
        public List<EventTicketTypeModel> TicketTypes { get; set; } = new();

        public EventCategoryModel Category { get; set; } = new();
    }
    public class EventCategoryModel
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }

    public class EventTicketTypeModel
    {
        public Guid Id { get; set; }
        [JsonPropertyName("ticketType_")]
        public string TicketType { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int TotalTickets { get; set; }
        public int TicketsSold { get; set; }
        public int TicketsLeft { get; set; }
    }
}
