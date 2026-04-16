using System.Security.Claims;

using FluentResults;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StudentParliamentSystem.Api.Configurations;
using StudentParliamentSystem.Core.Aggregates.CoworkingBooking;
using StudentParliamentSystem.Core.Aggregates.Event;
using StudentParliamentSystem.UseCases.CoworkingBookings.Request;
using StudentParliamentSystem.UseCases.CoworkingBookings.Schedule;
using StudentParliamentSystem.UseCases.Events.Retrieve.CreatedByUser;

using Wolverine;

namespace StudentParliamentSystem.Api.Controllers;

[Authorize]
public class CoworkingController : Controller
{
    private readonly IMessageBus _bus;

    public CoworkingController(IMessageBus bus)
    {
        _bus = bus;
    }

    public async Task<IActionResult> Index()
    {
        var schedule = await _bus.InvokeAsync<IEnumerable<CoworkingBookingSlot>>(new GetCoworkingSchedule());
        return View(schedule);
    }

    [Authorize(Policy = AuthorizationPolicyNameConstants.CanManageDepartment)]
    public async Task<IActionResult> RequestBooking()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var events = await _bus.InvokeAsync<IEnumerable<Event>>(new RetrieveEventsCreatedByUser(userId));
        ViewBag.Events = events;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicyNameConstants.CanManageDepartment)]
    public async Task<IActionResult> RequestBooking(Guid eventId, DateTime startTime, DateTime endTime, string? notes)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var result = await _bus.InvokeAsync<Result>(new RequestCoworkingBooking(
            eventId,
            userId,
            startTime.ToUniversalTime(),
            endTime.ToUniversalTime(),
            notes));

        if (result.IsFailed)
        {
            TempData["ErrorMessage"] = result.Errors.FirstOrDefault()?.Message ?? "Failed to request booking";
            var events = await _bus.InvokeAsync<IEnumerable<Event>>(new RetrieveEventsCreatedByUser(userId));
            ViewBag.Events = events;
            return View();
        }

        TempData["SuccessMessage"] = "Booking request submitted successfully";
        return RedirectToAction(nameof(Index));
    }
}