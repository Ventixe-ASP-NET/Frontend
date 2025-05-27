using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using WebApp.Models.Event.ProfileViewModels;
using WebApp.Models.ProfileViewModels;

namespace WebApp.Services.Profile
{
    public interface IProfileService
    {
        Task<SaveProfileViewModel?> GetProfileAsync(string id);
        Task<BaseResponse> SaveProfileAsync(SaveProfileViewModel model);
    }

    public class HttpProfileService : IProfileService
    {
        private readonly HttpClient _api;
        private readonly ILogger<HttpProfileService> _logger;

        public HttpProfileService(IHttpClientFactory httpClientFactory, ILogger<HttpProfileService> logger)
        {
            _api = httpClientFactory.CreateClient("ProfileApi");
            _logger = logger;
        }

        public async Task<SaveProfileViewModel?> GetProfileAsync(string id)
        {
            try
            {
                var response = await _api.GetAsync("/api/profile/getProfile");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("GetProfile failed with status code {Status}", response.StatusCode);
                    return null;
                }

                var result = await response.Content.ReadFromJsonAsync<UserProfileResponse>();
                return result?.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch profile");
                return null;
            }
        }

        public async Task<BaseResponse> SaveProfileAsync(SaveProfileViewModel model)
        {
            try
            {
                var response = await _api.PostAsJsonAsync("/api/profile/saveProfile", model);

                return await response.Content.ReadFromJsonAsync<BaseResponse>() ?? new BaseResponse
                {
                    StatusCode = 500,
                    Message = "Unexpected error"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save profile");
                return new BaseResponse
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
            }
        }
    }
}
