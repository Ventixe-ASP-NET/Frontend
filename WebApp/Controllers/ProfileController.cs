using Microsoft.AspNetCore.Mvc;
using WebApp.Models.Event.ProfileViewModels;
using WebApp.Models.ProfileViewModels;
using WebApp.Services.Event;
using WebApp.Services.Profile;

namespace WebApp.Controllers.Event
{
    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await _profileService.GetProfileAsync();

            if (model == null)
            {
                TempData["ERROR_MESSAGE"] = "Could not load profile data.";
                return View(new SaveProfileViewModel());
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(SaveProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ERROR_MESSAGE"] = "One or more fields are invalid.";
                return View(model);
            }

            var response = await _profileService.SaveProfileAsync(model);

            if (response.StatusCode == (int)ApiStatusCodes.SUCCESS)
            {
                TempData["RESP_MESSAGE"] = "Profile saved successfully.";
            }
            else
            {
                TempData["ERROR_MESSAGE"] = response.Message ?? "Error saving profile.";
            }

            return View(model);
        }
    }
}
