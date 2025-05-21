namespace WebApp.Dtos.Event;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class EventCreateDto
    {

    [Required] public string EventName { get; set; } = null!;
    [Required] public string EventDescription { get; set; } = null!;
    [Required, DataType(DataType.Date)] public DateTime StartDate { get; set; }
    [Required, DataType(DataType.Time)] public TimeSpan StartTime { get; set; }

    // new:
    [Required, DataType(DataType.Date)] public DateTime EndDate { get; set; }
    [Required, DataType(DataType.Time)] public TimeSpan EndTime { get; set; }

    [Required, Url] public string ImageUrl { get; set; } = "";

    [Required] public int CategoryId { get; set; }
    [Required] public Guid EventLocationId { get; set; }

    public List<TicketTypeCreateDto> TicketTypes { get; set; }
        = new List<TicketTypeCreateDto>();

    // helper to show it in your list/details:
    public double DurationHours =>
        (EndDate.Date + EndTime - (StartDate.Date + StartTime))
        .TotalHours;
}

        public class TicketTypeCreateDto
            {
            public Guid Id { get; set; }
            [Required, JsonPropertyName("TicketType_")]
            public string TicketType { get; set; } = null!;

            [Required]
            [Range(0.0, double.MaxValue)]
            public decimal Price { get; set; }

            [Required]
            [Range(1, int.MaxValue)]
            public int TotalTickets { get; set; }
        }
    



