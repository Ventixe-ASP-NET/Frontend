using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

public class AuthController : Controller
{
    [HttpGet]
    public IActionResult SignUp()
    {
        var viewModel = new SignUpViewModel();
        return View(viewModel);
    }


    [HttpPost]
    public IActionResult SignUp(SignUpFormModel formData)
    {
        var viewModel = new SignUpViewModel();

        if (!ModelState.IsValid)
        {
            viewModel.FormData = formData;
            return View(viewModel);
        }



        return View(viewModel);
    }
}
