using Microsoft.AspNetCore.Mvc;

namespace StudentParliamentSystem.Api.Controllers;

public class AdminController : Controller
{
    // GET: /Admin
    // User management and roles
    public IActionResult Index()
    {
        return View("Users");
    }

    // GET: /Admin/Users
    public IActionResult Users()
    {
        return View();
    }
}
