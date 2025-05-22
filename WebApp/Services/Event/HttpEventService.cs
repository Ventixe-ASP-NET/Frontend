using System;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WebApp.Dtos.Event;

namespace WebApp.Services.Event
{
    public interface IEventService
    {
        Task<IEnumerable<DisplayEventsDto>> GetAllEventsAsync();
        Task<DisplayEventsDto?> GetEventByIdAsync(Guid id);
        Task<DisplayEventsDto?> CreateEventAsync(EventCreateDto dto);
        Task<bool> ActivateEventAsync(Guid id);
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<IEnumerable<LocationDto>> GetLocationsAsync();
        Task<bool> UpdateEventAsync(Guid id, EventUpdateDto dto);
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
                _logger.LogError(ex, "Failed to fetch event {EventId}", id);
                return null;
            }
        }

        public async Task<DisplayEventsDto?> CreateEventAsync(EventCreateDto dto)
        {
            try
            {
                var payload = BuildCreatePayload(dto);
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

        public async Task<bool> UpdateEventAsync(Guid id, EventUpdateDto dto)
        {
            try
            {
                var start = dto.StartDate.Date.Add(dto.StartTime);
                var end = dto.EndDate.Date.Add(dto.EndTime);

                var updatePayload = new
                {
                    eventName = dto.EventName,
                    eventDescription = dto.EventDescription,
                    startDate = start.ToString("o"),
                    endDate = end.ToString("o"),
                    imageUrl = dto.ImageUrl,
                    categoryId = dto.CategoryId,
                    eventLocationId = dto.EventLocationId,
                    ticketTypes = dto.TicketTypes.Select(t => new {
                        id = t.Id,
                        ticketType_ = t.TicketType,
                        price = t.Price,
                        totalTickets = t.TotalTickets
                    }).ToList()
                };
                //payload here letss

                var resp = await _api.PutAsJsonAsync($"/api/Event/{id}", updatePayload);
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

        private object BuildCreatePayload(EventCreateDto dto)
        {
            var start = dto.StartDate.Date.Add(dto.StartTime);
            var end = dto.EndDate.Date.Add(dto.EndTime);

            return new
            {
                EventName = dto.EventName,
                EventDescription = dto.EventDescription,
                StartDate = start.ToString("o"),
                EndDate = end.ToString("o"),
                ImageUrl = dto.ImageUrl,
                CategoryId = dto.CategoryId,
                EventLocationId = dto.EventLocationId,
                TicketTypes = dto.TicketTypes.Select(t => new
                {
                    TicketType = t.TicketType,
                    Price = t.Price,
                    TotalTickets = t.TotalTickets
                })
            };
        }
    }
}
//return new
//{
//    EventName = dto.EventName,
//    Description = dto.EventDescription,
//    StartDate = start.ToString("o"),
//    EndDate = end.ToString("o"),
//    ImageUrl = dto.ImageUrl,
//    CategoryId = dto.CategoryId,
//    EventLocationId = dto.EventLocationId,
//    TicketTypes = dto.TicketTypes.Select(t => new
//    {
//        TicketType = t.TicketType,
//        Price = t.Price,
//        TotalTickets = t.TotalTickets
//    })
//};