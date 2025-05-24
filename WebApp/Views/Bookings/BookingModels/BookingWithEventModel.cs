using System.Text.Json.Serialization;

namespace WebApp.Views.Bookings.BookingModels
{
    public class BookingWithEventModel
    {
        // Booking info
        public int Id { get; set; }
        public string? EvoucherId { get; set; }
        public string BookingName { get; set; }
        public string InvoiceId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string EventId { get; set; }

        // Event info
        public string? EventName { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public EventLocationModel? Location { get; set; }
        public List<BookedTicketModel> BookedTickets { get; set; } = new();
    }

    public class EventLocationModel
    {
        public string VenueName { get; set; } = string.Empty;
        public string StreetAddress { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }

    public class BookedTicketModel
    {
        public Guid TicketTypeId { get; set; }
        public string TicketType { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal PricePerTicket { get; set; }
        public decimal TotalPrice => Quantity * PricePerTicket;
    }
}
