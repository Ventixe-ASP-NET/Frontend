namespace WebApp.Views.Bookings
{
    public class BookingModel
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public string BookingName { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid EventId { get; set; }
    }
}
