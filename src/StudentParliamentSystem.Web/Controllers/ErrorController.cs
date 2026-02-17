using Microsoft.AspNetCore.Mvc;

namespace StudentParliamentSystem.Api.Controllers;

[Route("/Error")]
public class ErrorController : Controller
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        // todo implement a better error page
        return View();
    }
}