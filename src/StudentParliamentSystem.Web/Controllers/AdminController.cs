using FluentResults;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StudentParliamentSystem.Api.Configurations;
using StudentParliamentSystem.Api.Models;
using StudentParliamentSystem.Core.Abstractions;
using StudentParliamentSystem.Core.Aggregates.Department;
using StudentParliamentSystem.Core.Aggregates.Event;
using StudentParliamentSystem.Core.Aggregates.User;
using StudentParliamentSystem.UseCases.Departments.Retrieve.All;
using StudentParliamentSystem.UseCases.Departments.Retrieve.Members;
using StudentParliamentSystem.UseCases.Departments.Update;
using StudentParliamentSystem.UseCases.Events.Retrieve.ByDepartment;
using StudentParliamentSystem.UseCases.Users.Retrieve.All;
using StudentParliamentSystem.UseCases.Users.Retrieve.ById;
using StudentParliamentSystem.UseCases.Users.Update;

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

    public async Task<IActionResult> Departments()
    {
        var result = await _bus.InvokeAsync<IEnumerable<DepartmentPreview>>(new RetrieveAllDepartments());
        return View(result);
    }
    
    public async Task<IActionResult> ManageDepartment(Guid id, [FromQuery] int usersPage = 1, [FromQuery] int eventsPage = 1, string? query = null)
    {
        var deptResult = await _bus.InvokeAsync<IEnumerable<DepartmentPreview>>(new RetrieveAllDepartments());
        var dept = deptResult.FirstOrDefault(d => d.Id == id);
        if (dept == null) return NotFound();

        var isSuperAdmin = User.IsInRole(StudentParliamentSystem.Core.Aggregates.Role.RoleName.SuperAdmin.ToString());
        var headRoleForDept = GetHeadRoleNameForDepartment(dept.Name);
        var isHead = headRoleForDept != null && User.IsInRole(headRoleForDept);

        if (!isSuperAdmin && !isHead)
        {
            return Forbid();
        }

        var pageSize = 10;
        var usersResult = await _bus.InvokeAsync<PagedResult<UserPreview>>(
            new RetrieveDepartmentMembers(id, usersPage, pageSize, query));

        var eventsResult = await _bus.InvokeAsync<PagedResult<EventPreview>>(
            new RetrieveDepartmentEvents(id, eventsPage, pageSize, query));

        ViewBag.Query = query;
        ViewBag.IsSuperAdmin = isSuperAdmin;
        
        var viewModel = new StudentParliamentSystem.Api.Models.Admin.ManageDepartmentViewModel 
        {
            Department = dept,
            Users = usersResult,
            Events = eventsResult
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddMember(Guid id, string email)
    {
        var deptResult = await _bus.InvokeAsync<IEnumerable<DepartmentPreview>>(new RetrieveAllDepartments());
        var dept = deptResult.FirstOrDefault(d => d.Id == id);
        if (dept == null) return NotFound();

        var isSuperAdmin = User.IsInRole(StudentParliamentSystem.Core.Aggregates.Role.RoleName.SuperAdmin.ToString());
        var headRoleForDept = GetHeadRoleNameForDepartment(dept.Name);
        var isHead = headRoleForDept != null && User.IsInRole(headRoleForDept);

        if (!isSuperAdmin && !isHead) return Forbid();

        var result = await _bus.InvokeAsync<Result>(new AddDepartmentMember(id, email));
        
        if (result.IsFailed)
        {
            TempData["ErrorMessage"] = result.Errors.FirstOrDefault()?.Message ?? "Failed to add member";
        }
        else
        {
            TempData["SuccessMessage"] = "Member added successfully";
        }

        return RedirectToAction(nameof(ManageDepartment), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeHead(Guid id, string email)
    {
        var isSuperAdmin = User.IsInRole(StudentParliamentSystem.Core.Aggregates.Role.RoleName.SuperAdmin.ToString());
        if (!isSuperAdmin) return Forbid();

        var result = await _bus.InvokeAsync<Result>(new ChangeDepartmentHead(id, email));
        
        if (result.IsFailed)
        {
            TempData["ErrorMessage"] = result.Errors.FirstOrDefault()?.Message ?? "Failed to change head";
        }
        else
        {
            TempData["SuccessMessage"] = "Department head changed successfully";
        }

        return RedirectToAction(nameof(ManageDepartment), new { id });
    }

    private string? GetHeadRoleNameForDepartment(string departmentName)
    {
        return departmentName switch
        {
            "Культурний" => StudentParliamentSystem.Core.Aggregates.Role.RoleName.HeadOfCulturalDepartment.ToString(),
            "Науковий" => StudentParliamentSystem.Core.Aggregates.Role.RoleName.HeadOfScienceDepartment.ToString(),
            "Читалкадеп" => StudentParliamentSystem.Core.Aggregates.Role.RoleName.HeadOfCoworkingDepartment.ToString(),
            "Інформаційний" => StudentParliamentSystem.Core.Aggregates.Role.RoleName.HeadOfInformationDepartment.ToString(),
            _ => null
        };
    }
    
    public async Task<IActionResult> Edit(Guid id)
    {
        var result = await _bus.InvokeAsync<Result<User>>(new RetrieveUserById(id));

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

        var result = await _bus.InvokeAsync<Result>(new UpdateUserDetails(
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