using Account.Interfaces;
using Account.Models;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

public class AuthController(IAccountService accountService) : Controller
{
    private readonly IAccountService _accountService = accountService;


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
}
