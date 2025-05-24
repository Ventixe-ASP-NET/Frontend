namespace WebApp.Models.Event.VenueViewModels
{
    public class LocationViewModel
    {
        public Guid Id { get; set; }
        public string VenueName { get; set; } = null!;
        public string City { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string StreetAddress { get; set; } = null!;
    }
}
