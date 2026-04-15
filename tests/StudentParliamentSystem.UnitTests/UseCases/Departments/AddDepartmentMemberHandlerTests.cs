using FluentResults;
using Microsoft.Extensions.Logging;
using Moq;
using StudentParliamentSystem.Core.Aggregates.Department;
using StudentParliamentSystem.Core.Aggregates.Role;
using StudentParliamentSystem.Core.Aggregates.User;
using StudentParliamentSystem.UseCases.Abstractions;
using StudentParliamentSystem.UseCases.Departments.Update;

namespace StudentParliamentSystem.UnitTests.UseCases.Departments;

public class AddDepartmentMemberHandlerTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IDepartmentRepository> _deptRepo = new();
    private readonly Mock<IRoleRepository> _roleRepo = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<ILogger<AddDepartmentMemberHandler>> _logger = new();
    private readonly AddDepartmentMemberHandler _sut;

    public AddDepartmentMemberHandlerTests()
    {
        _sut = new AddDepartmentMemberHandler(_userRepo.Object, _deptRepo.Object, _roleRepo.Object, _uow.Object, _logger.Object);
    }

    [Fact]
    public async Task HandleAsync_DepartmentNotFound_ReturnsFail()
    {
        var command = new AddDepartmentMember(Guid.NewGuid(), "a@b.com");
        _deptRepo.Setup(r => r.GetByIdAsync(command.DepartmentId)).ReturnsAsync((Department?)null);

        var result = await _sut.HandleAsync(command);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("Department not found");
    }

    [Fact]
    public async Task HandleAsync_UserNotFound_ReturnsFail()
    {
        var dept = new Department { Id = Guid.NewGuid(), Name = "Культурний" };
        var command = new AddDepartmentMember(dept.Id, "a@b.com");
        _deptRepo.Setup(r => r.GetByIdAsync(dept.Id)).ReturnsAsync(dept);
        _userRepo.Setup(r => r.GetByEmailAsync(command.Email)).ReturnsAsync((User?)null);

        var result = await _sut.HandleAsync(command);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("User not found");
    }

    [Fact]
    public async Task HandleAsync_ValidNewMember_AddsDeptAndRoleAndSaves()
    {
        var dept = new Department { Id = Guid.NewGuid(), Name = "Культурний" };
        var user = User.Create(Guid.NewGuid(), "a@b.com", "A", "B");
        var role = new Role { Id = Guid.NewGuid(), Name = RoleName.CulturalDepartmentMember };
        var command = new AddDepartmentMember(dept.Id, user.Email);
        
        _deptRepo.Setup(r => r.GetByIdAsync(dept.Id)).ReturnsAsync(dept);
        _userRepo.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);
        _roleRepo.Setup(r => r.GetByNameAsync(RoleName.CulturalDepartmentMember)).ReturnsAsync(Result.Ok(role));
        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _sut.HandleAsync(command);

        result.IsSuccess.Should().BeTrue();
        user.Departments.Should().Contain(dept);
        user.Roles.Should().Contain(role);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
