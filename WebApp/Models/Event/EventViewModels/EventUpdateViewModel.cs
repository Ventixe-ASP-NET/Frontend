using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using WebApp.Models.Event.TicketViewModels;
using static System.Net.WebRequestMethods;

namespace WebApp.Models.Event.EventViewModels
{
    public class EventUpdateViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required] public string EventName { get; set; } = null!;

        [JsonPropertyName("description")]
        [Required] public string EventDescription { get; set; } = null!;
        [Required, DataType(DataType.Date)] public DateTime StartDate { get; set; }
        [Required, DataType(DataType.Time)] public TimeSpan StartTime { get; set; }
        [Required, DataType(DataType.Date)] public DateTime EndDate { get; set; }
        [Required, DataType(DataType.Time)] public TimeSpan EndTime { get; set; }

        [Required, Url] public string ImageUrl { get; set; } ="https://placehold.co/600x400";


        [Required] public int CategoryId { get; set; }
        [Required] public Guid EventLocationId { get; set; }

        public List<TicketTypeUpdateViewModel> TicketTypes { get; set; }
            = new List<TicketTypeUpdateViewModel>();
    }
}
