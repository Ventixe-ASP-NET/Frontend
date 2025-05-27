using Account.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.ViewComponents;

[ViewComponent(Name = "HeaderViewComponent")]
public class HeaderViewComponent : ViewComponent
{
    private readonly IAccountService _accountService;

    public HeaderViewComponent(IAccountService accountService)
    {
        _accountService = accountService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var signedInUser = await _accountService.GetSignedInAppUserAsync((System.Security.Claims.ClaimsPrincipal?)User);
        var viewModel = new HeaderViewModel { AppUser = signedInUser };
        return View(viewModel);
    }
}
