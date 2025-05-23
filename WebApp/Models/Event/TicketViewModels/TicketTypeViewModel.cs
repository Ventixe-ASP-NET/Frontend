using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApp.Models.Event.TicketViewModels
{
    public class TicketTypeViewModel
    {
        public Guid Id { get; set; }

        [JsonPropertyName("ticketType_")]
        public string TicketType { get; set; } = null!; //Change the name 
        public decimal Price { get; set; }
        public int TotalTickets { get; set; }
        public int TicketsSold { get; set; }
        public int TicketsLeft { get; set; }
    }
}
