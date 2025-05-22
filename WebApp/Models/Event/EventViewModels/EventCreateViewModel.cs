using System.ComponentModel.DataAnnotations;
using WebApp.Dtos.Event;
using WebApp.Models.Event.TicketViewModels;

namespace WebApp.Models.Event.EventViewModels
{
    public class EventCreateViewModel
    {
        [Required] public string EventName { get; set; } = null!;
        [Required]
        //[JsonPropertyName("description")]
        public string EventDescription { get; set; } = null!;
        [Required, DataType(DataType.Date)] public DateTime StartDate { get; set; }
        [Required, DataType(DataType.Time)] public TimeSpan StartTime { get; set; }

        // new:
        [Required, DataType(DataType.Date)] public DateTime EndDate { get; set; }
        [Required, DataType(DataType.Time)] public TimeSpan EndTime { get; set; }

        [Required, Url] public string ImageUrl { get; set; } = "";

        [Required] public int CategoryId { get; set; }
        [Required] public Guid EventLocationId { get; set; }

        public List<TicketTypeCreateViewModel> TicketTypes { get; set; }
            = new List<TicketTypeCreateViewModel>();

        // helper to show it in your list/details:
        public double DurationHours =>
            (EndDate.Date + EndTime - (StartDate.Date + StartTime))
            .TotalHours;
    }
}
