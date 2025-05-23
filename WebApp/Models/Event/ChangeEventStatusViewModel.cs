namespace WebApp.Models.Event
{
    public class ChangeEventStatusViewModel
    {
        public string Id { get; set; } = null!;
        public int Status { get; set; }
    }

    public enum EventStatus
    {
        Draft = 0,
        Active = 1,
        Past = 2

    }
}
