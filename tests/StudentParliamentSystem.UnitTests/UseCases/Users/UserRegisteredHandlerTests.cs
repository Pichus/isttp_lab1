using FluentResults;
using Microsoft.Extensions.Logging;
using Moq;
using StudentParliamentSystem.Core.Aggregates.Role;
using StudentParliamentSystem.Core.Aggregates.User;
using StudentParliamentSystem.Shared.Contracts.Users;
using StudentParliamentSystem.UseCases.Abstractions;
using StudentParliamentSystem.UseCases.Users.Register;

namespace StudentParliamentSystem.UnitTests.UseCases.Users;

public class UserRegisteredHandlerTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IRoleRepository> _roleRepo = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<ILogger<UserRegisteredHandler>> _logger = new();
    private readonly UserRegisteredHandler _sut;

    public UserRegisteredHandlerTests()
    {
        _sut = new UserRegisteredHandler(_userRepo.Object, _logger.Object, _uow.Object, _roleRepo.Object);
    }

    [Fact]
    public async Task HandleAsync_UserAlreadyExists_ReturnsEarly()
    {
        var userId = Guid.NewGuid();
        var message = new UserRegistered(userId, "a@b.com", "A", "B", new List<string>());
        _userRepo.Setup(r => r.ExistsAsync(userId)).ReturnsAsync(true);

        await _sut.HandleAsync(message);

        _userRepo.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_ValidNewUser_CreatesUserWithRoles()
    {
        var userId = Guid.NewGuid();
        var message = new UserRegistered(userId, "a@b.com", "A", "B", new List<string> { "SuperAdmin" });
        var role = new Role { Id = Guid.NewGuid(), Name = RoleName.SuperAdmin };
        
        _userRepo.Setup(r => r.ExistsAsync(userId)).ReturnsAsync(false);
        _roleRepo.Setup(r => r.GetByNameAsync(RoleName.SuperAdmin)).ReturnsAsync(Result.Ok(role));
        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _sut.HandleAsync(message);

        _userRepo.Verify(r => r.Add(It.Is<User>(u => 
            u.Id == userId && 
            u.Email == "a@b.com" && 
            u.Roles.Contains(role))), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_InvalidRoleName_SkipsRoleButCreatesUser()
    {
        var userId = Guid.NewGuid();
        var message = new UserRegistered(userId, "a@b.com", "A", "B", new List<string> { "NonExistentRole" });
        
        _userRepo.Setup(r => r.ExistsAsync(userId)).ReturnsAsync(false);
        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _sut.HandleAsync(message);

        _userRepo.Verify(r => r.Add(It.Is<User>(u => u.Id == userId && u.Roles.Count == 0)), Times.Once);
    }
}
