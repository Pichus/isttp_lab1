using Microsoft.EntityFrameworkCore;

using StudentParliamentSystem.Core.Aggregates.CoworkingBooking;
using StudentParliamentSystem.Infrastructure.Data;

namespace StudentParliamentSystem.Seeding.Seeders;

public class CoworkingBookingStatusSeeder : ICoworkingBookingStatusSeeder
{
    private static readonly string[] Statuses = ["Pending", "Approved", "Rejected"];

    private readonly ApplicationDatabaseContext _context;

    public CoworkingBookingStatusSeeder(ApplicationDatabaseContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        foreach (var name in Statuses)
        {
            var exists = await _context.Set<CoworkingBookingStatus>()
                .AnyAsync(s => s.Name == name);

            if (!exists)
            {
                _context.Set<CoworkingBookingStatus>().Add(new CoworkingBookingStatus
                {
                    Id = Guid.NewGuid(),
                    Name = name
                });
            }
        }

        await _context.SaveChangesAsync();
    }
}