namespace WebApp.Models.Event.EventViewModels
{
    public class EventListViewModel
    {
        public IEnumerable<EventViewModel> Events { get; set; } = Enumerable.Empty<EventViewModel>();
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public Models.Event.EventStatus? CurrentStatus { get; set; }

    }
}
