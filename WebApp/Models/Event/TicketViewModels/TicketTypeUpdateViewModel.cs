using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.Event.TicketViewModels
{
    public class TicketTypeUpdateViewModel
    {
        //MAYBE CHANGE IT 
        public Guid Id { get; set; }
        //[Required, JsonPropertyName("TicketType_")]
        public string TicketType { get; set; } = null!;

        [Required]
        [Range(0.0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int TotalTickets { get; set; }
    }
}
