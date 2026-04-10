using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using StudentParliamentSystem.Core.Aggregates.CoworkingBooking;
using StudentParliamentSystem.Core.Aggregates.Department;
using StudentParliamentSystem.Core.Aggregates.Event;
using StudentParliamentSystem.Core.Aggregates.Role;
using StudentParliamentSystem.Core.Aggregates.User;
using StudentParliamentSystem.Infrastructure.Data;
using StudentParliamentSystem.Infrastructure.Identity.Data.Entities;
using StudentParliamentSystem.UseCases.Abstractions;

namespace StudentParliamentSystem.Seeding.Seeders;

public class RealisticDataSeeder : IRealisticDataSeeder
{
    private readonly ApplicationDatabaseContext _context;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public RealisticDataSeeder(ApplicationDatabaseContext context, UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager, IRoleRepository roleRepository, IUnitOfWork unitOfWork)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task SeedAsync()
    {
        // 1. Departments
        var deptNames = new[] { "Культурний", "Науковий", "Читалкадеп", "Інформаційний" };
        var depts = new List<Department>();
        foreach (var dn in deptNames)
        {
            var dept = await _context.Departments.FirstOrDefaultAsync(d => d.Name == dn);
            if (dept == null)
            {
                dept = new Department { Id = Guid.NewGuid(), Name = dn };
                await _context.Departments.AddAsync(dept);
            }

            depts.Add(dept);
        }

        await _context.SaveChangesAsync();

        var coworkingDept = depts.First(d => d.Name == "Читалкадеп");
        var scienceDept = depts.First(d => d.Name == "Науковий");
        var culturalDept = depts.First(d => d.Name == "Культурний");
        var infoDept = depts.First(d => d.Name == "Інформаційний");

        // 2. Users
        var seedUsers = new[]
        {
            // Coworking
            new { Email = "taras.shevchenko@knu.ua", First = "Тарас", Last = "Шевченко", Role = RoleName.CoworkingDepartmentMember, Dept = coworkingDept },
            new { Email = "lesia.ukrainka@knu.ua", First = "Леся", Last = "Українка", Role = RoleName.CoworkingDepartmentMember, Dept = coworkingDept },
            new { Email = "ivan.nechuy@knu.ua", First = "Іван", Last = "Нечуй-Левицький", Role = RoleName.CoworkingDepartmentMember, Dept = coworkingDept },
            new { Email = "borys.hrinchenko@knu.ua", First = "Борис", Last = "Грінченко", Role = RoleName.HeadOfCoworkingDepartment, Dept = coworkingDept },

            // Science
            new { Email = "ivan.franko@knu.ua", First = "Іван", Last = "Франко", Role = RoleName.ScienceDepartmentMember, Dept = scienceDept },
            new { Email = "olha.kobylianska@knu.ua", First = "Ольга", Last = "Кобилянська", Role = RoleName.ScienceDepartmentMember, Dept = scienceDept },
            new { Email = "panteleimon.kulish@knu.ua", First = "Пантелеймон", Last = "Куліш", Role = RoleName.ScienceDepartmentMember, Dept = scienceDept },
            new { Email = "mykola.amosov@knu.ua", First = "Микола", Last = "Амосов", Role = RoleName.HeadOfScienceDepartment, Dept = scienceDept },

            // Cultural
            new { Email = "mariya.zankovetska@knu.ua", First = "Марія", Last = "Заньковецька", Role = RoleName.HeadOfCulturalDepartment, Dept = culturalDept },
            new { Email = "solomiya.krushelnytska@knu.ua", First = "Соломія", Last = "Крушельницька", Role = RoleName.CulturalDepartmentMember, Dept = culturalDept },
            new { Email = "marko.vovchok@knu.ua", First = "Марко", Last = "Вовчок", Role = RoleName.CulturalDepartmentMember, Dept = culturalDept },
            new { Email = "lina.kostenko@knu.ua", First = "Ліна", Last = "Костенко", Role = RoleName.CulturalDepartmentMember, Dept = culturalDept },

            // Information
            new { Email = "mykhailo.hrushevskyi@knu.ua", First = "Михайло", Last = "Грушевський", Role = RoleName.HeadOfInformationDepartment, Dept = infoDept },
            new { Email = "vasyl.stus@knu.ua", First = "Василь", Last = "Стус", Role = RoleName.InformationDepartmentMember, Dept = infoDept },
            new { Email = "vyacheslav.chornovil@knu.ua", First = "В'ячеслав", Last = "Чорновіл", Role = RoleName.InformationDepartmentMember, Dept = infoDept },
            new { Email = "mykhailo.kotsiubynskyi@knu.ua", First = "Михайло", Last = "Коцюбинський", Role = RoleName.InformationDepartmentMember, Dept = infoDept }
        };

        var appUsers = new List<User>();
        foreach (var u in seedUsers)
        {
            var existingAppUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == u.Email);
            if (existingAppUser != null)
            {
                appUsers.Add(existingAppUser);
                continue;
            }

            var userId = Guid.NewGuid();

            // Identity
            if (await _userManager.FindByEmailAsync(u.Email) == null)
            {
                var idUser = new ApplicationUser
                {
                    Id = userId,
                    UserName = u.Email,
                    Email = u.Email,
                    FirstName = u.First,
                    LastName = u.Last
                };
                await _userManager.CreateAsync(idUser, "Qwert123!");
                await _userManager.AddToRoleAsync(idUser, u.Role.ToString());
            }

            // Application User
            var role = (await _roleRepository.GetByNameAsync(u.Role)).Value;
            var appUser = new User
            {
                Id = userId,
                Email = u.Email,
                FirstName = u.First,
                LastName = u.Last,
                CreatedAtUtc = DateTimeOffset.UtcNow,
                Roles = new List<Role> { role },
                Departments = new List<Department> { u.Dept }
            };

            await _context.Users.AddAsync(appUser);
            appUsers.Add(appUser);
        }

        await _context.SaveChangesAsync();

        var coworkingManager = appUsers.First(u => u.Email == "taras.shevchenko@knu.ua");
        var organizer = appUsers.First(u => u.Email == "ivan.franko@knu.ua");

        // 3. Events
        var evtList = new List<Event>();
        var eventData = new[]
        {
            new
            {
                Title = "Абітурієнтська Олімпіада",
                Start = DateTime.UtcNow.AddDays(1),
                End = DateTime.UtcNow.AddDays(1).AddHours(4),
                Loc = "Аудиторія 01"
            },
            new
            {
                Title = "IT Meetup: Architecture",
                Start = DateTime.UtcNow.AddDays(2),
                End = DateTime.UtcNow.AddDays(2).AddHours(2),
                Loc = "Ректорат"
            },
            new
            {
                Title = "Вечір Настільних Ігор",
                Start = DateTime.UtcNow.AddDays(5),
                End = DateTime.UtcNow.AddDays(5).AddHours(3),
                Loc = "Аудиторія 01"
            }
        };

        foreach (var ed in eventData)
        {
            if (await _context.Set<Event>().AnyAsync(e => e.Title == ed.Title))
            {
                continue;
            }

            var evt = new Event
            {
                Id = Guid.NewGuid(),
                Title = ed.Title,
                Description = "Цікавий захід для студентів ФКНК.",
                Location = ed.Loc,
                MaxParticipants = 50,
                IsPublished = true,
                StartTimeUtc = ed.Start,
                EndTimeUtc = ed.End,
                DepartmentId = scienceDept.Id,
                CreatedByUserId = organizer.Id,
                CreatedAtUtc = DateTime.UtcNow,
                EventOrganizers = new List<EventOrganizer> { new() { Id = Guid.NewGuid(), UserId = organizer.Id } }
            };

            await _context.Set<Event>().AddAsync(evt);
            evtList.Add(evt);
        }

        await _context.SaveChangesAsync();

        // 4. Coworking Bookings
        var statuses = await _context.CoworkingBookingStatuses.ToListAsync();
        var approvedStatus = statuses.First(s => s.Name == "Approved");
        var pendingStatus = statuses.First(s => s.Name == "Pending");

        foreach (var evt in evtList.Where(e => e.Location.Contains("01")))
        {
            if (await _context.CoworkingBookings.AnyAsync(b => b.EventId == evt.Id))
            {
                continue;
            }

            var booking = new CoworkingBooking
            {
                Id = Guid.NewGuid(),
                EventId = evt.Id,
                StatusId = evt.Title.Contains("Олімпіада") ? approvedStatus.Id : pendingStatus.Id,
                SpaceManagerId = evt.Title.Contains("Олімпіада") ? coworkingManager.Id : null,
                StartTimeUtc = evt.StartTimeUtc,
                EndTimeUtc = evt.EndTimeUtc,
                Notes = "Прошу виділити аудиторію"
            };

            await _context.CoworkingBookings.AddAsync(booking);
        }

        await _context.SaveChangesAsync();
    }
}