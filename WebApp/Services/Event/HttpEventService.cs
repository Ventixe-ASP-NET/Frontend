using System;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
//using WebApp.Dtos.Event;
using WebApp.Models.Event.CategoryViewModels;
using WebApp.Models.Event.EventViewModels;
using WebApp.Models.Event.TicketViewModels;
using WebApp.Models.Event.VenueViewModels;
using WebApp.Models.Event;
using WebApp.Models.Event.ProfileViewModels;

namespace WebApp.Services.Event
{
    public interface IEventService
    {
        Task<IEnumerable<EventViewModel>> GetAllEventsAsync();
        Task<EventViewModel?> GetEventByIdAsync(Guid id);
        Task<EventViewModel?> CreateEventAsync(EventCreateViewModel dto);
        Task<bool> ActivateEventAsync(Guid id);
        Task<IEnumerable<CategoryViewModel>> GetAllCategoriesAsync();
        Task<IEnumerable<LocationViewModel>> GetLocationsAsync();
        Task<bool> UpdateEventAsync(Guid id, EventUpdateViewModel dto);
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

        public async Task<IEnumerable<EventViewModel>> GetAllEventsAsync()
        {
            try
            {
                var wrapper = await _api
                     .GetFromJsonAsync<EventViewModelWrapper>("/api/Event");


                if (wrapper == null)
                    return Array.Empty<EventViewModel>();

                return wrapper.Events;
            }
            catch
            {

                return Array.Empty<EventViewModel>();
            }
        }

        public async Task<EventViewModel?> GetEventByIdAsync(Guid id)
        {
            try
            {
                var resp = await _api.GetAsync($"/api/Event/{id}");
                if (resp.StatusCode == HttpStatusCode.NotFound)
                    return null;
                resp.EnsureSuccessStatusCode();
                return await resp.Content.ReadFromJsonAsync<EventViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch event {EventId}", id);
                return null;
            }
        }

        public async Task<EventViewModel?> CreateEventAsync(EventCreateViewModel vm)
        {
            
            var start = vm.StartDate.Date.Add(vm.StartTime);
            var end = vm.EndDate.Date.Add(vm.EndTime);

            var payload = new
            {
                EventName = vm.EventName,
                EventDescription = vm.EventDescription,
                StartDate = start.ToString("o"),
                EndDate = end.ToString("o"),
                ImageUrl = vm.ImageUrl,
                CategoryId = vm.CategoryId,
                EventLocationId = vm.EventLocationId,
                TicketTypes = vm.TicketTypes.Select(t => new {
                    TicketType = t.TicketType,
                    Price = t.Price,
                    TotalTickets = t.TotalTickets
                })
            };

            var resp = await _api.PostAsJsonAsync("/api/Event", payload);
            if (resp.StatusCode == HttpStatusCode.BadRequest) return null;
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<EventViewModel>();
        }

        public async Task<bool> UpdateEventAsync(Guid id, EventUpdateViewModel dto)
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
                        ticketType = t.TicketType,
                        price = t.Price,
                        totalTickets = t.TotalTickets
                    }).ToList()
                };
                //payload here letss

                var json = JsonSerializer.Serialize(updatePayload);
                _logger.LogDebug("PUT /api/Event/{Id} payload: {Json}", id, json);
                //error tester
                var resp = await _api.PutAsJsonAsync($"/api/Event/{id}", updatePayload);
                if (!resp.IsSuccessStatusCode)
                {
                    var error = await resp.Content.ReadAsStringAsync();
                    _logger.LogError("Update failed ({StatusCode}): {Error}", resp.StatusCode, error);
                    return false;
                }
                return true;
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
                var req = new ChangeEventStatusViewModel
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

        public async Task<IEnumerable<CategoryViewModel>> GetAllCategoriesAsync()
        {
            try
            {
                return await _api.GetFromJsonAsync<IEnumerable<CategoryViewModel>>("/api/Category")
                       ?? Array.Empty<CategoryViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch categories");
                return Array.Empty<CategoryViewModel>();
            }
        }

        public async Task<IEnumerable<LocationViewModel>> GetLocationsAsync()
        {
            try
            {
                return await _api.GetFromJsonAsync<IEnumerable<LocationViewModel>>("/api/Event/locations")
                       ?? Array.Empty<LocationViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch locations");
                return Array.Empty<LocationViewModel>();
            }
        }


    }

}
