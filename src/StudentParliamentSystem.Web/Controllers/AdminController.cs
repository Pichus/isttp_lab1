using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StudentParliamentSystem.Api.Configurations;
using StudentParliamentSystem.Core.Abstractions;
using StudentParliamentSystem.Core.Aggregates.User;
using StudentParliamentSystem.UseCases.Users.Retrieve.All;

using Wolverine;

namespace StudentParliamentSystem.Api.Controllers;

[Authorize(Policy = AuthorizationPolicyNameConstants.CanAccessAdminPanel)]
public class AdminController : Controller
{
    private readonly IMessageBus _bus;

    public AdminController(IMessageBus bus)
    {
        _bus = bus;
    }

    public IActionResult Index()
    {
        return RedirectToAction(nameof(Users));
    }

    public async Task<IActionResult> Users([FromQuery] int page = 1, string? query = null)
    {
        var pageSize = 10;
        var result = await _bus.InvokeAsync<PagedResult<UserPreview>>(new RetrieveAllUsers(page, pageSize, query));

        ViewBag.Query = query;

        return View(result);
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