namespace WebApp.Dtos.Event
{
    public enum EventStatus
    {
        Draft  = 0,
        Active = 1,
        Past   = 2

    }
    public class ChangeStatusRequest
    {
        public string Id { get; set; } = null!;
        public int Status { get; set; }
    }
}
