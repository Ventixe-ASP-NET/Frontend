using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

[Authorize(Roles = "Admin")]
public class BookingsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
