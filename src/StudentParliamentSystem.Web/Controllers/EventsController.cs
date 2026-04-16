using System.Security.Claims;

using FluentResults;
using FluentResults;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StudentParliamentSystem.Api.Models.Events;
using StudentParliamentSystem.Core.Abstractions;
using StudentParliamentSystem.Core.Aggregates.Department;
using StudentParliamentSystem.Core.Aggregates.Event;
using StudentParliamentSystem.Core.Aggregates.Role;
using StudentParliamentSystem.UseCases.Departments.Retrieve.ForUser;
using StudentParliamentSystem.UseCases.Events.Create;
using StudentParliamentSystem.UseCases.Events.Delete;
using StudentParliamentSystem.UseCases.Events.Register;
using StudentParliamentSystem.UseCases.Events.Retrieve.ById;
using StudentParliamentSystem.UseCases.Events.Retrieve.Published;
using StudentParliamentSystem.UseCases.Events.Retrieve.Tags;
using StudentParliamentSystem.UseCases.Events.Update;

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

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, string? query = null, string? tag = null, string? sortOrder = "date_desc")
    {
        var result = await _bus.InvokeAsync<PagedResult<EventPreview>>(
            new RetrievePublishedEvents(page, 12, query, tag, sortOrder));

        var tags = await _bus.InvokeAsync<IEnumerable<EventTag>>(new RetrieveAllEventTags());

        ViewBag.Query = query;
        ViewBag.Tag = tag;
        ViewBag.SortOrder = sortOrder;
        ViewBag.Tags = tags.Select(t => t.Name).ToList();

        return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var result = await _bus.InvokeAsync<Result<Event>>(new RetrieveEventById(id));

        if (result.IsFailed)
            return NotFound();

        var @event = result.Value;


        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userIdStr, out var currentUserId))
        {
            var eventWithRegs = await _bus.InvokeAsync<Result<Event>>(new RetrieveEventByIdWithRegistrations(id));
            if (eventWithRegs.IsSuccess)
            {
                @event = eventWithRegs.Value;
                ViewBag.IsRegistered = @event.Registrations?.Any(r => r.UserId == currentUserId) ?? false;
                ViewBag.RegistrationCount = @event.Registrations?.Count ?? 0;
            }
        }
        else
        {
            ViewBag.IsRegistered = false;
            ViewBag.RegistrationCount = 0;
        }

        return View(@event);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(Guid id)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var result = await _bus.InvokeAsync<Result>(new RegisterForEvent(id, userId));

        if (result.IsFailed)
            TempData["ErrorMessage"] = result.Errors.FirstOrDefault()?.Message ?? "Registration failed.";
        else
            TempData["SuccessMessage"] = "Ви успішно зареєструвались на захід!";

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelRegistration(Guid id)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var result = await _bus.InvokeAsync<Result>(new CancelEventRegistration(id, userId));

        if (result.IsFailed)
            TempData["ErrorMessage"] = result.Errors.FirstOrDefault()?.Message ?? "Cancellation failed.";
        else
            TempData["SuccessMessage"] = "Реєстрацію скасовано.";

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
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

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var result = await _bus.InvokeAsync<Result<Event>>(new RetrieveEventById(id));
        if (result.IsFailed) return NotFound();

        var @event = result.Value;

        if (@event.CreatedByUserId != userId && !User.IsInRole(nameof(RoleName.SuperAdmin)))
            return Forbid();

        var departments = await _bus.InvokeAsync<IEnumerable<DepartmentPreview>>(
            new RetrieveUserDepartments(userId));
        var existingTags = await _bus.InvokeAsync<IEnumerable<EventTag>>(new RetrieveAllEventTags());

        ViewBag.Departments = departments.ToList();
        ViewBag.ExistingTags = existingTags.Select(t => t.Name).ToList();

        var model = new EditEventViewModel
        {
            Id = @event.Id,
            Title = @event.Title,
            Description = @event.Description,
            Location = @event.Location,
            StartTime = @event.StartTimeUtc.ToLocalTime(),
            EndTime = @event.EndTimeUtc.ToLocalTime(),
            DepartmentId = @event.DepartmentId,
            MaxParticipants = @event.MaxParticipants,
            IsPublished = @event.IsPublished,
            TagsCsv = string.Join(",", @event.Tags.Select(t => t.Name))
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditEventViewModel model)
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

        var resultEvent = await _bus.InvokeAsync<Result<Event>>(new RetrieveEventById(model.Id));
        if (resultEvent.IsFailed) return NotFound();
        var @event = resultEvent.Value;

        if (@event.CreatedByUserId != userId && !User.IsInRole(nameof(RoleName.SuperAdmin)))
            return Forbid();

        var tags = string.IsNullOrWhiteSpace(model.TagsCsv)
            ? new List<string>()
            : model.TagsCsv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

        var result = await _bus.InvokeAsync<Result>(new UpdateEvent(
            model.Id,
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
            ModelState.AddModelError(string.Empty, "Failed to update event");
            return View(model);
        }

        return RedirectToAction(nameof(Details), new { id = model.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var resultEvent = await _bus.InvokeAsync<Result<Event>>(new RetrieveEventById(id));
        if (resultEvent.IsFailed) return NotFound();
        var @event = resultEvent.Value;

        if (@event.CreatedByUserId != userId && !User.IsInRole(nameof(RoleName.SuperAdmin)))
            return Forbid();

        var result = await _bus.InvokeAsync<Result>(new DeleteEvent(id));

        var referer = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(referer))
        {
            return Redirect(referer);
        }

        return RedirectToAction(nameof(Index));
    }
}