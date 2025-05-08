using Account.Interfaces;
using Account.Models;
using Authentication.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

public class AuthController(IAccountService accountService, SignInManager<AppUserEntity> signInManager) : Controller
{
    private readonly IAccountService _accountService = accountService;
    private readonly SignInManager<AppUserEntity> _signInManager = signInManager;


    [HttpGet]
    public IActionResult SignUp()
    {
        var viewModel = new SignUpViewModel();
        return View(viewModel);
    }


    [HttpPost]
    public async Task<IActionResult> SignUp(SignUpFormModel formData)
    {
        var viewModel = new SignUpViewModel();

        if (!ModelState.IsValid)
        {
            viewModel.FormData = formData;
            return View(viewModel);
        }

        var result = await _accountService.SignUpAsync(formData);
        if (!result.Succeeded)
        {
            ViewBag.ErrorMessage = result.Message;
            return View(viewModel);
        }

        ModelState.Clear();
        viewModel.AccountCreated = true;
        return View(viewModel);
    }



    [HttpGet]
    public IActionResult SignIn(string returnUrl = "/")
    {
        ViewBag.ReturnUrl = returnUrl;
        var viewModel = new SignInViewModel();
        return View(viewModel);
    }


    [HttpPost]
    public async Task<IActionResult> SignIn(SignInFormModel formData, string returnUrl = "/")
    {
        ViewBag.ReturnUrl = returnUrl;
        var viewModel = new SignInViewModel();

        if (ModelState.IsValid)
        {
            var result = await _accountService.SignInAsync(formData);
            if (result.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                ViewBag.ErrorMessage = result.Message;
            }
        }

        viewModel.FormData = formData;
        return View(viewModel);
    }


    [HttpGet]
    public new async Task<IActionResult> SignOut()
    {
        await _accountService.SignOutAsync();
        return RedirectToAction("SignIn", "Auth");
    }



    [HttpPost]
    public IActionResult ExternalSignIn(string provider, string returnUrl = null!)
    {
        ViewBag.ReturnUrl = returnUrl;
        var viewModel = new SignInViewModel();

        if (string.IsNullOrEmpty(provider))
        {
            ViewBag.ErrorMessage = "Invalid provider";
            return View("Auth/SignIn", viewModel);
        }

        var redirectUrl = Url.Action("ExternalSignInCallback", "Auth", new { returnUrl })!;
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }


    public async Task<IActionResult> ExternalSignInCallback(string returnUrl = null!, string remoteError = null!)
    {
        returnUrl ??= Url.Content("/");
        ViewBag.ReturnUrl = returnUrl;
        var viewModel = new SignInViewModel();

        if (string.IsNullOrEmpty(remoteError))
        {
            ViewBag.ErrorMessage = remoteError;
            return View("Auth/SignIn", viewModel);
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
            return RedirectToAction("SignIn", "Auth");

        var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
        if (signInResult.Succeeded)
        {
            return LocalRedirect(returnUrl);
        }
        else
        {
            var accountResult = await _accountService.SignUpExternalAsync(info);
            if (accountResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                ViewBag.ErrorMessage = accountResult.Message;
                return View("Auth/SignIn", viewModel);
            }
        }
    }
}
