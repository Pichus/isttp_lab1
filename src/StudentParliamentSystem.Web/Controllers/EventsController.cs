using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentParliamentSystem.Api.Models.Events;
using StudentParliamentSystem.Core.Aggregates.Department;
using StudentParliamentSystem.Core.Aggregates.Event;
using StudentParliamentSystem.UseCases.Departments.Retrieve.ForUser;
using StudentParliamentSystem.UseCases.Events.Create;
using StudentParliamentSystem.UseCases.Events.Retrieve.Tags;
using Wolverine;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentParliamentSystem.Api.Models.Events;
using StudentParliamentSystem.Core.Aggregates.Department;
using StudentParliamentSystem.UseCases.Departments.Retrieve.ForUser;
using StudentParliamentSystem.UseCases.Events.Create;
using Wolverine;

namespace StudentParliamentSystem.Api.Controllers;

[Authorize]
public class EventsController : Controller
{
    private readonly IMessageBus _bus;

    public EventsController(IMessageBus bus)
    {
        _bus = bus;
    }

    // GET: /Events/
    public IActionResult Index()
    {
        return View();
    }

    // GET: /Events/Archive
    public IActionResult Archive()
    {
        return View();
    }

    // GET: /Events/Details/5
    public IActionResult Details(Guid? id)
    {
        return View();
    }

    // GET: /Events/Create
    public async Task<IActionResult> Create()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var departments = await _bus.InvokeAsync<IEnumerable<DepartmentPreview>>(
            new RetrieveUserDepartments(userId));

        var tags = await _bus.InvokeAsync<IEnumerable<EventTag>>(new RetrieveAllEventTags());

        ViewBag.Departments = departments.ToList();
        ViewBag.ExistingTags = tags.Select(t => t.Name).ToList();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateEventViewModel model)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        if (!ModelState.IsValid)
        {
            var departments = await _bus.InvokeAsync<IEnumerable<DepartmentPreview>>(
                new RetrieveUserDepartments(userId));
            var existingTags = await _bus.InvokeAsync<IEnumerable<EventTag>>(new RetrieveAllEventTags());
            ViewBag.Departments = departments.ToList();
            ViewBag.ExistingTags = existingTags.Select(t => t.Name).ToList();
            return View(model);
        }

        var tags = string.IsNullOrWhiteSpace(model.TagsCsv) 
            ? new List<string>() 
            : model.TagsCsv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

        var result = await _bus.InvokeAsync<FluentResults.Result>(new CreateEvent(
            model.Title,
            model.Description,
            model.Location,
            model.StartTime.ToUniversalTime(),
            model.EndTime.ToUniversalTime(),
            model.DepartmentId,
            model.MaxParticipants,
            model.IsPublished,
            tags,
            userId
        ));

        if (result.IsFailed)
        {
            var departments = await _bus.InvokeAsync<IEnumerable<DepartmentPreview>>(
                new RetrieveUserDepartments(userId));
            var existingTags = await _bus.InvokeAsync<IEnumerable<EventTag>>(new RetrieveAllEventTags());
            ViewBag.Departments = departments.ToList();
            ViewBag.ExistingTags = existingTags.Select(t => t.Name).ToList();
            ModelState.AddModelError(string.Empty, "Failed to create event");
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: /Events/Edit/5
    public IActionResult Edit(Guid? id)
    {
        return View();
    }
}
