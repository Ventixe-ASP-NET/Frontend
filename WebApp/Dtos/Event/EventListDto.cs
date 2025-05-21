using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApp.Dtos.Event
{
    public class EventListDtoWrapper
    {

        public List<DisplayEventsDto> Events { get; set; } = new();
    }

    public class DisplayEventsDto
    {
        public Guid Id { get; set; }
        public string EventName { get; set; } = null!;
        public string Description { get; set; } = null!;

        // Status as an int or use the enum name [Required]
       //ADDED IMAGEURL
        public string ImageUrl { get; set; } = "";
        public int Status { get; set; }
        public EventStatus StatusEnum => (EventStatus)Status;

        public EventStatus OutDatedEvents =>
            StatusEnum == EventStatus.Active && StartDate < DateTime.Now
            ? EventStatus.Past
            : StatusEnum;


        public CategoryDto Category { get; set; } = null!;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public double DurationHours =>
             (EndDate - StartDate).TotalHours;

        // Now a nested object
        public LocationDto Location { get; set; } = null!;

        public List<TicketTypeDto> TicketTypes { get; set; } = new();
        public int TotalTickets { get; set; }

        //Slug Test
        //public string Slug
        //        => EventName
        //        .ToLowerInvariant()
        //        .Replace(" ", "-")
        //        .Replace(".", "")
        //        .Replace(",", "")
        //        ;
    }
}

    public class LocationDto
    {
        public Guid Id { get; set; }
        public string VenueName { get; set; } = null!;
        public string StreetAddress { get; set; } = null!;
        public string City { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
        public string Country { get; set; } = null!;
    }

    public class TicketTypeDto
    {
        public Guid Id { get; set; }

        [JsonPropertyName("ticketType_")]
        public string TicketType { get; set; } = null!; //Change the name 
        public decimal Price { get; set; }
        public int TotalTickets { get; set; }
        public int TicketsSold { get; set; }
        public int TicketsLeft { get; set; }
    }
    public class CategoryDto
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = null!;
    }


