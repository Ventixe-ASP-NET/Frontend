using System.Net;
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

        Task<bool> UpdateEventAsync(Guid id, EventCreateDto dto);
        Task<bool> DeleteEventAsync(Guid id);

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
                var wrapper = await _api
                     .GetFromJsonAsync<EventListDtoWrapper>("/api/Event");

                
                if (wrapper == null)
                    return Array.Empty<DisplayEventsDto>();

                return wrapper.Events;
            }
            catch
            {
           
                return Array.Empty<DisplayEventsDto>();
            }
        }

        public async Task<DisplayEventsDto?> GetEventByIdAsync(Guid id)
        {
            try
            {
                var resp = await _api.GetAsync($"/api/Event/{id}");
                if (resp.StatusCode == HttpStatusCode.NotFound)
                    return null;

                resp.EnsureSuccessStatusCode();
                return await resp.Content.ReadFromJsonAsync<DisplayEventsDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch event {Id}", id);
                return null;
            }
        }

        public async Task<DisplayEventsDto?> CreateEventAsync(EventCreateDto dto)
        {
            try
            {
                var payload = BuildPayload(dto);
                var resp = await _api.PostAsJsonAsync("/api/Event", payload);
                if (resp.StatusCode == HttpStatusCode.BadRequest)
                    return null;

                resp.EnsureSuccessStatusCode();
                return await resp.Content.ReadFromJsonAsync<DisplayEventsDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create event");
                return null;
            }
        }

        public async Task<bool> UpdateEventAsync(Guid id, EventCreateDto dto)
        {
            try
            {
                var payload = BuildPayload(dto);
                var resp = await _api.PutAsJsonAsync($"/api/Event/{id}", payload);
                return resp.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update event {EventId}", id);
                return false;
            }
        }

        public async Task<bool> DeleteEventAsync(Guid id)
        {
            try
            {
                var resp = await _api.DeleteAsync($"/api/Event/{id}");
                return resp.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete event {EventId}", id);
                return false;
            }
        }

        public async Task<bool> ActivateEventAsync(Guid id)
        {
            try
            {
                var req = new ChangeStatusRequest
                {
                    Id = id.ToString(),
                    Status = (int)EventStatus.Active
                };
                var resp = await _api.PatchAsJsonAsync($"/api/Event/{id}/status", req);
                return resp.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to activate event {EventId}", id);
                return false;
            }
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            try
            {
                return await _api.GetFromJsonAsync<IEnumerable<CategoryDto>>("/api/Category")
                       ?? Array.Empty<CategoryDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch categories");
                return Array.Empty<CategoryDto>();
            }
        }

        public async Task<IEnumerable<LocationDto>> GetLocationsAsync()
        {
            try
            {
                return await _api.GetFromJsonAsync<IEnumerable<LocationDto>>("/api/Event/locations")
                       ?? Array.Empty<LocationDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch locations");
                return Array.Empty<LocationDto>();
            }
        }

        // helper to merge date/time + duration => create the API payload
        private object BuildPayload(EventCreateDto dto)
        {
            var start = dto.StartDate.Date.Add(dto.StartTime);
            var end = dto.EndDate.Date.Add(dto.EndTime);

            return new
            {
                EventName = dto.EventName,
                EventDescription = dto.EventDescription,
                StartDate = start,
                EndDate = end,
                ImageUrl = dto.ImageUrl,
                CategoryId = dto.CategoryId,
                EventLocationId = dto.EventLocationId,
                TicketTypes = dto.TicketTypes
            };
        }
    }
}

