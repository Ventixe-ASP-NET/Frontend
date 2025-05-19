namespace WebApp.Dtos.Event;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

    public class EventCreateDto
    {


    [Required]
    public string EventName { get; set; } = null!;

            [Required]
            public string EventDescription { get; set; } = null!;

            [Required]
            [DataType(DataType.Date)]
            public DateTime StartDate { get; set; }

            [Required]
            [DataType(DataType.Time)]
            public TimeSpan StartTime { get; set; }

            [Required]
            [Range(1, 168, ErrorMessage = "Duration must be between 1 and 168 hours.")]
            public int DurationHours { get; set; }

            [Required]
            [Url]
            public string ImageUrl { get; set; } = "";

            [Required]
            public int CategoryId { get; set; } 

            [Required]
            public Guid EventLocationId { get; set; }

            public List<TicketTypeCreateDto> TicketTypes { get; set; }
                = new List<TicketTypeCreateDto>();
        }

        public class TicketTypeCreateDto
        {
            [Required]
            public string TicketType { get; set; } = "";

            [Required]
            [Range(0.0, double.MaxValue)]
            public decimal Price { get; set; }

            [Required]
            [Range(1, int.MaxValue)]
            public int TotalTickets { get; set; }
        }
    



