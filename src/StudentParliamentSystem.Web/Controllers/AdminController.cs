using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StudentParliamentSystem.Api.Configurations;

namespace StudentParliamentSystem.Api.Controllers;

[Authorize(Policy = AuthorizationPolicyNameConstants.CanAccessAdminPanel)]
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

    // GET: /Admin/Edit/5
    public IActionResult Edit(string id)
    {
        return View();
    }

    // POST: /Admin/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(string id, IFormCollection collection)
    {
        // For demonstration, simply redirect back to the user list.
        return RedirectToAction(nameof(Users));
    }
}
