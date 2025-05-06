using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

public class AuthController : Controller
{
    [HttpGet]
    public IActionResult SignUp()
    {
        return View();
    }


    //[HttpPost]
    //public IActionResult SignUp()
    //{
    //    return View();
    //}
}
