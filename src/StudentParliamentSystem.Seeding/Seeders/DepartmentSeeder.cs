using Microsoft.EntityFrameworkCore;

using StudentParliamentSystem.Core.Aggregates.Department;
using StudentParliamentSystem.Infrastructure.Data;
using StudentParliamentSystem.UseCases.Abstractions;

namespace StudentParliamentSystem.Seeding.Seeders;

public class DepartmentSeeder : IDepartmentSeeder
{
    private readonly ApplicationDatabaseContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public DepartmentSeeder(ApplicationDatabaseContext dbContext, IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task SeedAsync()
    {
        var departmentsToSeed = new[]
        {
            new { Id = Guid.NewGuid(), Name = "Культурний", Description = "Культурний департамент" },
            new { Id = Guid.NewGuid(), Name = "Науковий", Description = "Науковий департамент" },
            new { Id = Guid.NewGuid(), Name = "Читалкадеп", Description = "Читалкадеп" },
            new { Id = Guid.NewGuid(), Name = "Інформаційний", Description = "Інформаційний департамент" }
        };

        foreach (var depData in departmentsToSeed)
        {
            var exists = await _dbContext.Departments.AnyAsync(d => d.Name == depData.Name);
            if (!exists)
            {
                var department = new Department
                {
                    Id = depData.Id,
                    Name = depData.Name,
                    Description = depData.Description
                };

                _dbContext.Departments.Add(department);
            }
        }

        await _unitOfWork.SaveChangesAsync();
    }
}