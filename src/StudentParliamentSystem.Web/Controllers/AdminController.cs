using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StudentParliamentSystem.Api.Configurations;
using StudentParliamentSystem.Api.ViewModels;

namespace StudentParliamentSystem.Api.Controllers;

[Authorize(Policy = AuthorizationPolicyNameConstants.CanAccessAdminPanel)]
public class AdminController : Controller
{
    // GET: /Admin
    // User management and roles
    public IActionResult Index()
    {
        return View("Users", new PaginationViewModel
        {
            CurrentPage = 1,
            TotalPages = 10
        });
    }

    // GET: /Admin/Users
    public IActionResult Users(int page = 1)
    {
        var viewModel = new PaginationViewModel
        {
            CurrentPage = page,
            TotalPages = 10 // placeholder for UI
        };
        
        return View(viewModel);
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
