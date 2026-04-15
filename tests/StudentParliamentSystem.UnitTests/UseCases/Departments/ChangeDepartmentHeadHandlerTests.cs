using FluentResults;
using Microsoft.Extensions.Logging;
using Moq;
using StudentParliamentSystem.Core.Aggregates.Department;
using StudentParliamentSystem.Core.Aggregates.Role;
using StudentParliamentSystem.Core.Aggregates.User;
using StudentParliamentSystem.UseCases.Abstractions;
using StudentParliamentSystem.UseCases.Departments.Update;

namespace StudentParliamentSystem.UnitTests.UseCases.Departments;

public class ChangeDepartmentHeadHandlerTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IDepartmentRepository> _deptRepo = new();
    private readonly Mock<IRoleRepository> _roleRepo = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<ILogger<ChangeDepartmentHeadHandler>> _logger = new();
    private readonly ChangeDepartmentHeadHandler _sut;

    public ChangeDepartmentHeadHandlerTests()
    {
        _sut = new ChangeDepartmentHeadHandler(_userRepo.Object, _deptRepo.Object, _roleRepo.Object, _uow.Object, _logger.Object);
    }

    [Fact]
    public async Task HandleAsync_DepartmentNotFound_ReturnsFail()
    {
        var command = new ChangeDepartmentHead(Guid.NewGuid(), "a@b.com");
        _deptRepo.Setup(r => r.GetByIdAsync(command.DepartmentId)).ReturnsAsync((Department?)null);

        var result = await _sut.HandleAsync(command);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("Department not found");
    }

    [Fact]
    public async Task HandleAsync_UserNotFound_ReturnsFail()
    {
        var dept = new Department { Id = Guid.NewGuid(), Name = "Культурний" };
        var command = new ChangeDepartmentHead(dept.Id, "a@b.com");
        _deptRepo.Setup(r => r.GetByIdAsync(dept.Id)).ReturnsAsync(dept);
        _userRepo.Setup(r => r.GetByEmailAsync(command.Email)).ReturnsAsync((User?)null);

        var result = await _sut.HandleAsync(command);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("User not found");
    }

    [Fact]
    public async Task HandleAsync_ValidNewHead_RemovesOldHeadsAndAddsNewOne()
    {
        var dept = new Department { Id = Guid.NewGuid(), Name = "Культурний" };
        var headRole = new Role { Id = Guid.NewGuid(), Name = RoleName.HeadOfCulturalDepartment };
        var memberRole = new Role { Id = Guid.NewGuid(), Name = RoleName.CulturalDepartmentMember };
        
        var oldHead = User.Create(Guid.NewGuid(), "old@b.com", "Old", "Head");
        oldHead.Roles.Add(headRole);
        
        var newHead = User.Create(Guid.NewGuid(), "new@b.com", "New", "Head");
        
        var command = new ChangeDepartmentHead(dept.Id, newHead.Email);

        _deptRepo.Setup(r => r.GetByIdAsync(dept.Id)).ReturnsAsync(dept);
        _userRepo.Setup(r => r.GetByEmailAsync(newHead.Email)).ReturnsAsync(newHead);
        _roleRepo.Setup(r => r.GetByNameAsync(RoleName.HeadOfCulturalDepartment)).ReturnsAsync(Result.Ok(headRole));
        _roleRepo.Setup(r => r.GetByNameAsync(RoleName.CulturalDepartmentMember)).ReturnsAsync(Result.Ok(memberRole));
        _userRepo.Setup(r => r.GetUsersByRoleAsync(headRole.Id)).ReturnsAsync(new List<User> { oldHead });
        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _sut.HandleAsync(command);

        result.IsSuccess.Should().BeTrue();
        oldHead.Roles.Should().NotContain(headRole);
        newHead.Roles.Should().Contain(headRole);
        newHead.Roles.Should().Contain(memberRole);
        newHead.Departments.Should().Contain(dept);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
