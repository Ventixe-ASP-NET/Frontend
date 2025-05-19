namespace WebApp.Dtos.Event
{
    public class LocationDto
    {
        public Guid Id { get; set; }
        public string VenueName { get; set; } = "";
        public string City { get; set; } = "";
        public string PostalCode { get; set; } = "";
        public string Country { get; set; } = "";
        public string StreetAddress { get; set; } = "";
    }
}
