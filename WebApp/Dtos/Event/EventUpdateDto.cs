using System.ComponentModel.DataAnnotations;

namespace WebApp.Dtos.Event
{
    public class EventUpdateDto
    {
        [Required] public string EventName { get; set; } = null!;
        [Required] public string EventDescription { get; set; } = null!;
        [Required, DataType(DataType.Date)] public DateTime StartDate { get; set; }
        [Required, DataType(DataType.Time)] public TimeSpan StartTime { get; set; }
        [Required, DataType(DataType.Date)] public DateTime EndDate { get; set; }
        [Required, DataType(DataType.Time)] public TimeSpan EndTime { get; set; }

        [Required, Url] public string ImageUrl { get; set; } = "";

        [Required] public int CategoryId { get; set; }
        [Required] public Guid EventLocationId { get; set; }

        public List<TicketTypeUpdateDto> TicketTypes { get; set; }
            = new List<TicketTypeUpdateDto>();
    }
    public class TicketTypeUpdateDto
    {
        [Required] public Guid Id { get; set; }                // existing ticket’s GUID
        [Required] public string TicketType { get; set; } = null!;
        [Required, Range(0, double.MaxValue)] public decimal Price { get; set; }
        [Required, Range(1, int.MaxValue)] public int TotalTickets { get; set; }
    }
}
