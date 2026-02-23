using Microsoft.AspNetCore.Mvc;

namespace StudentParliamentSystem.Api.Controllers;

public class StatisticsController : Controller
{
    // GET: /Statistics
    // Department / Global Stats and Dean Report
    public IActionResult Index()
    {
        return View();
    }
}
