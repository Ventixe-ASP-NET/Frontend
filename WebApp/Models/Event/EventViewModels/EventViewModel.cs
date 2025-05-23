//using WebApp.Dtos.Event;
using WebApp.Models.Event.TicketViewModels;
using WebApp.Models.Event.VenueViewModels;
using WebApp.Models.Event.CategoryViewModels;
using WebApp.Models.Event;

using System.Text.Json.Serialization;

namespace WebApp.Models.Event.EventViewModels
{
    public class EventViewModelWrapper
    {

        public List<EventViewModel> Events { get; set; } = new();
    }
    public class EventViewModel
    {
        public Guid Id { get; set; }
        public string EventName { get; set; } = null!;

        [JsonPropertyName("description")]
        public string EventDescription { get; set; } = null!;

        // Status as an int or use the enum name [Required]
        //ADDED IMAGEURL
        public string ImageUrl { get; set; } = "";
        public int Status { get; set; }
        public EventStatus StatusEnum => (EventStatus)Status;

        public EventStatus OutDatedEvents =>
            StatusEnum == EventStatus.Active && StartDate < DateTime.Now
            ? EventStatus.Past
            : StatusEnum;


        public CategoryViewModel Category { get; set; } = null!;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public double DurationHours =>
             (EndDate - StartDate).TotalHours;

        public LocationViewModel Location { get; set; } = null!;

        public List<TicketTypeViewModel> TicketTypes { get; set; } = new();
        public int TotalTickets { get; set; }

    }
}
