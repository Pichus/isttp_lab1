using Microsoft.AspNetCore.Mvc;

namespace StudentParliamentSystem.Api.Controllers;

public class EventsController : Controller
{
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
    public IActionResult Create()
    {
        return View();
    }

    // GET: /Events/Edit/5
    public IActionResult Edit(Guid? id)
    {
        return View();
    }
}
