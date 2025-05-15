using WebApp.Dtos.Event;

namespace WebApp.Services.Event
{

    public interface IEventService
    {
        Task<IEnumerable<DisplayEventsDto>> GetAllEventsAsync();

        // ? or not
        Task<DisplayEventsDto?> GetEventByIdAsync(Guid id);
    }


    public class HttpEventService : IEventService
    {
        private readonly HttpClient _api;
        private readonly ILogger<HttpEventService> _logger;

        public HttpEventService(IHttpClientFactory http, ILogger<HttpEventService> logger)
        {
            _api = http.CreateClient("EventApi");
            _logger = logger;
        }
        public async Task<IEnumerable<DisplayEventsDto>> GetAllEventsAsync()
        {
            try
            {
                var response = await _api.GetAsync("/api/Event");
                response.EnsureSuccessStatusCode();
                var wrapper = await response.Content.ReadFromJsonAsync<EventListDtoWrapper>();
                return wrapper?.Events ?? Enumerable.Empty<DisplayEventsDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch events");
                return Enumerable.Empty<DisplayEventsDto>();
            }
        }

        public async Task<DisplayEventsDto?> GetEventByIdAsync(Guid id)
        {
            try
            {
                var response = await _api.GetAsync($"/api/Event/{id}");
                if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<DisplayEventsDto>();

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch event with id {Id}", id);
                return null;
            }

        }
    }
}
