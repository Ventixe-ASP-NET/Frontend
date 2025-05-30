﻿using Account.Entities;
using Account.Interfaces;
using Account.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

public class AuthController(IAccountService accountService, SignInManager<AppUserEntity> signInManager) : Controller
{
    private readonly IAccountService _accountService = accountService;
    private readonly SignInManager<AppUserEntity> _signInManager = signInManager;


    [HttpGet("auth/signup")]
    public IActionResult SignUpEmail()
    {
        var viewModel = new SignUpEmailViewModel();
        return View(viewModel);
    }


    [HttpPost("auth/signup")]
    public async Task<IActionResult> HandleSignUpEmail(SignUpEmailViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View(nameof(SignUpEmail), viewModel);

        var existsResult = await _accountService.AlreadyExistsAsync(viewModel.Email);
        if (existsResult)
        {
            ViewBag.ErrorMessage = "An account already exists";
            return View(nameof(SignUpEmail), viewModel);
        }

        var result = await _accountService.RequestVerificationCodeAsync(viewModel.Email);
        if (!result.Succeeded)
        {
            ViewBag.ErrorMessage = "Unable to send verification code";
            return View(nameof(SignUpEmail), viewModel);
        }

        return RedirectToAction(nameof(SignUpConfirmAccount), new SignUpConfirmAccountViewModel { Email = viewModel.Email });
    }



    [HttpGet("auth/confirm-account")]
    public IActionResult SignUpConfirmAccount(SignUpConfirmAccountViewModel viewModel)
    {
        return View(viewModel);
    }


    [HttpPost("auth/confirm-account")]
    public async Task<IActionResult> HandleSignUpConfirmAccount(SignUpConfirmAccountViewModel viewModel)
    {
        if (string.IsNullOrWhiteSpace(viewModel.Email))
            return RedirectToAction(nameof(SignUpEmail));

        if (string.IsNullOrWhiteSpace(viewModel.Code))
        {
            ViewBag.ErrorMessage = "Verification code is required";
            return View(nameof(SignUpConfirmAccount), viewModel);
        }

        var result = await _accountService.VerifyVerificationCodeAsync(viewModel.Email, viewModel.Code);
        if (!result.Succeeded)
        {
            ViewBag.ErrorMessage = "Invalid or expired verification code";
            return View(nameof(SignUpConfirmAccount), viewModel);
        }

        return RedirectToAction(nameof(SignUpPassword), new SignUpPasswordViewModel { Email = viewModel.Email });
    }



    [HttpGet("auth/password")]
    public IActionResult SignUpPassword(SignUpPasswordViewModel viewModel)
    {
        if (string.IsNullOrWhiteSpace(viewModel.Email))
            return RedirectToAction(nameof(SignUpEmail));

        return View(viewModel);
    }


    [HttpPost("auth/password")]
    public async Task<IActionResult> HandleSignUpPassword(SignUpPasswordViewModel viewModel)
    {
        if (string.IsNullOrWhiteSpace(viewModel.Email))
            return RedirectToAction(nameof(SignUpEmail));

        if (!ModelState.IsValid)
            return View(nameof(SignUpPassword), viewModel);

        var result = await _accountService.SignUpAsync(viewModel.Email, viewModel.Password);
        if (!result.Succeeded)
        {
            ViewBag.ErrorMessage = result.Message;
            return View(nameof(SignUpPassword), viewModel);
        }

        return RedirectToAction(nameof(SignIn), new SignInViewModel { AccountCreated = true });
    }



    [HttpGet]
    public IActionResult SignIn(SignInViewModel viewModel, string returnUrl = "/")
    {
        ViewBag.ReturnUrl = returnUrl;
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
            return View("SignIn", viewModel);
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

        if (!string.IsNullOrEmpty(remoteError))
        {
            ViewBag.ErrorMessage = remoteError;
            return View("SignIn", viewModel);
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
                var accountCreatedSignInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
                if (accountCreatedSignInResult.Succeeded)
                {
                    return LocalRedirect(returnUrl);
                }
            }
            ViewBag.ErrorMessage = accountResult.Message;
            return RedirectToAction("SignIn", "Auth");
        }
    }
}
