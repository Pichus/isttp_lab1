using Microsoft.AspNetCore.Mvc;

using StudentParliamentSystem.Api.ViewModels;

namespace StudentParliamentSystem.Api.Controllers;

public class StatusCodeController : Controller
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [Route("StatusCode/{statusCode}")]
    public IActionResult RenderStatusCode([FromRoute] int statusCode)
    {
        // todo implement a better status code page
        var viewModel = new StatusCodeViewModel() { StatusCode = statusCode };
        return View("StatusCode", viewModel);
    }
}