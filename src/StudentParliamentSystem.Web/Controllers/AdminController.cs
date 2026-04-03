using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StudentParliamentSystem.Api.Configurations;
using StudentParliamentSystem.Api.Models;
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
    public async Task<IActionResult> Edit(Guid id)
    {
        var result = await _bus.InvokeAsync<FluentResults.Result<User>>(new StudentParliamentSystem.UseCases.Users.Retrieve.ById.RetrieveUserById(id));

        if (result.IsFailed)
        {
            return NotFound();
        }

        var user = result.Value;
        var viewModel = new UpdateUserViewModel
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = user.Roles.Select(r => r.Name.ToString()).ToList()
        };

        return View(viewModel);
    }

    // POST: /Admin/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateUserViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _bus.InvokeAsync<FluentResults.Result>(new StudentParliamentSystem.UseCases.Users.Update.UpdateUserDetails(
            model.Id, 
            model.FirstName, 
            model.LastName, 
            model.Roles));

        if (result.IsFailed)
        {
            ModelState.AddModelError(string.Empty, "Update failed.");
            return View(model);
        }

        return RedirectToAction(nameof(Users));
    }
}