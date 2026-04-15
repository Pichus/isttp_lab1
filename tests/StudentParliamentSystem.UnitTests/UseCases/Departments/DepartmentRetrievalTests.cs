using FluentResults;
using Moq;
using StudentParliamentSystem.Core.Abstractions;
using StudentParliamentSystem.Core.Aggregates.Department;
using StudentParliamentSystem.Core.Aggregates.Role;
using StudentParliamentSystem.Core.Aggregates.User;
using StudentParliamentSystem.UseCases.Departments.Retrieve.All;
using StudentParliamentSystem.UseCases.Departments.Retrieve.ForUser;
using StudentParliamentSystem.UseCases.Departments.Retrieve.Members;
using StudentParliamentSystem.UseCases.Users.Retrieve.ById;
using Wolverine;

namespace StudentParliamentSystem.UnitTests.UseCases.Departments;

public class DepartmentRetrievalTests
{
    private readonly Mock<IDepartmentRepository> _deptRepo = new();
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IMessageBus> _bus = new();

    [Fact]
    public async Task RetrieveAllDepartmentsHandler_DelegatesToRepository()
    {
        var sut = new RetrieveAllDepartmentsHandler(_deptRepo.Object);
        var expected = new List<DepartmentPreview>();
        _deptRepo.Setup(r => r.RetrieveAllPreviewsAsync()).ReturnsAsync(expected);

        var result = await sut.HandleAsync(new RetrieveAllDepartments());

        result.Should().BeSameAs(expected);
    }

    [Fact]
    public async Task RetrieveDepartmentMembersHandler_DelegatesToRepository()
    {
        var sut = new RetrieveDepartmentMembersHandler(_userRepo.Object);
        var deptId = Guid.NewGuid();
        var expected = new PagedResult<UserPreview> { Items = new List<UserPreview>(), TotalCount = 0, CurrentPage = 1, PageSize = 10 };
        _userRepo.Setup(r => r.RetrieveByDepartmentAsync(deptId, 1, 10, "query")).ReturnsAsync(expected);

        var result = await sut.HandleAsync(new RetrieveDepartmentMembers(deptId, 1, 10, "query"));

        result.Should().BeSameAs(expected);
    }

    [Fact]
    public async Task RetrieveUserDepartmentsHandler_SuperAdmin_ReturnsAll()
    {
        var sut = new RetrieveUserDepartmentsHandler(_deptRepo.Object, _bus.Object);
        var userId = Guid.NewGuid();
        var user = User.Create(userId, "a@b.com", "A", "B");
        user.Roles.Add(new Role { Id = Guid.NewGuid(), Name = RoleName.SuperAdmin });
        
        var depts = new List<DepartmentPreview> { new(Guid.NewGuid(), "D1", "", null, 0), new(Guid.NewGuid(), "D2", "", null, 0) };
        
        _bus.Setup(b => b.InvokeAsync<Result<User>>(It.IsAny<RetrieveUserById>(), default, null))
            .ReturnsAsync(Result.Ok(user));
        _deptRepo.Setup(r => r.RetrieveAllPreviewsAsync()).ReturnsAsync(depts);

        var result = await sut.HandleAsync(new RetrieveUserDepartments(userId));

        result.Should().BeEquivalentTo(depts);
    }

    [Fact]
    public async Task RetrieveUserDepartmentsHandler_DeptMember_ReturnsOnlyAllowed()
    {
        var sut = new RetrieveUserDepartmentsHandler(_deptRepo.Object, _bus.Object);
        var userId = Guid.NewGuid();
        var user = User.Create(userId, "a@b.com", "A", "B");
        user.Roles.Add(new Role { Id = Guid.NewGuid(), Name = RoleName.CulturalDepartmentMember });
        
        var depts = new List<DepartmentPreview> 
        { 
            new(Guid.NewGuid(), "Культурний", "", null, 0), 
            new(Guid.NewGuid(), "Науковий", "", null, 0) 
        };
        
        _bus.Setup(b => b.InvokeAsync<Result<User>>(It.IsAny<RetrieveUserById>(), default, null))
            .ReturnsAsync(Result.Ok(user));
        _deptRepo.Setup(r => r.RetrieveAllPreviewsAsync()).ReturnsAsync(depts);

        var result = await sut.HandleAsync(new RetrieveUserDepartments(userId));

        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Культурний");
    }

    [Fact]
    public async Task RetrieveUserDepartmentsHandler_UserNotFound_ReturnsEmpty()
    {
        var sut = new RetrieveUserDepartmentsHandler(_deptRepo.Object, _bus.Object);
        _bus.Setup(b => b.InvokeAsync<Result<User>>(It.IsAny<RetrieveUserById>(), default, null))
            .ReturnsAsync(Result.Fail("Not found"));

        var result = await sut.HandleAsync(new RetrieveUserDepartments(Guid.NewGuid()));

        result.Should().BeEmpty();
    }
}
