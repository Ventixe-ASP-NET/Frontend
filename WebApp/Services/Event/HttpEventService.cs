using WebApp.Dtos.Event;

namespace WebApp.Services.Event
{

    public interface IEventService
    {
        Task<IEnumerable<DisplayEventsDto>> GetAllEventsAsync();

        // ? or not
        Task<DisplayEventsDto?> GetEventByIdAsync(Guid id);

        Task<DisplayEventsDto?> CreateEventAsync(EventCreateDto dto);

        Task<bool> ActivateEventAsync(Guid id);

        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<IEnumerable<LocationDto>> GetLocationsAsync();

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

        public async Task<DisplayEventsDto?> CreateEventAsync(EventCreateDto dto)
        {
            try
            {
                
                var response = await _api.PostAsJsonAsync("/api/Event", dto);
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                   
                    return null;
                }
                response.EnsureSuccessStatusCode();

                
                return await response.Content.ReadFromJsonAsync<DisplayEventsDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create event");
                return null;
            }
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

        public async Task<bool> ActivateEventAsync(Guid id)
        {
            try
            {
                var reqDto = new ChangeStatusRequest
                {
                    Id = id.ToString(),
                    Status = (int)EventStatus.Active
                };

             
                var response = await _api.PatchAsJsonAsync(
                    $"api/event/{id}/status",
                    reqDto
                );

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to activate event {EventId}", id);
                return false;
            }
        }
        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var resp = await _api.GetAsync("/api/Category");
            resp.EnsureSuccessStatusCode();
            return (await resp.Content
                              .ReadFromJsonAsync<IEnumerable<CategoryDto>>())
                   ?? Enumerable.Empty<CategoryDto>();
        }

        public async Task<IEnumerable<LocationDto>> GetLocationsAsync()
        {
            var resp = await _api.GetAsync("/api/locations");
            resp.EnsureSuccessStatusCode();
            return (await resp.Content
                              .ReadFromJsonAsync<IEnumerable<LocationDto>>())
                   ?? Enumerable.Empty<LocationDto>();
        }
    }
}
