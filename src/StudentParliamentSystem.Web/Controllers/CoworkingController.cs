using Microsoft.AspNetCore.Mvc;

namespace StudentParliamentSystem.Api.Controllers;

public class CoworkingController : Controller
{
    // GET: /Coworking
    // View available slots
    public IActionResult Index()
    {
        return View();
    }

    // GET: /Coworking/Book
    // Request booking
    public IActionResult Book()
    {
        return View();
    }

    // GET: /Coworking/Manage
    // Administer space / bookings
    public IActionResult Manage()
    {
        return View();
    }
}
