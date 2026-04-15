using FluentResults;
using Microsoft.Extensions.Logging;
using Moq;
using StudentParliamentSystem.Core.Abstractions;
using StudentParliamentSystem.Core.Aggregates.Role;
using StudentParliamentSystem.Core.Aggregates.User;
using StudentParliamentSystem.Shared.Contracts.Users;
using StudentParliamentSystem.UseCases.Abstractions;
using StudentParliamentSystem.UseCases.Users.Retrieve.All;
using StudentParliamentSystem.UseCases.Users.Retrieve.ById;
using StudentParliamentSystem.UseCases.Users.Retrieve.ByRole;
using StudentParliamentSystem.UseCases.Users.Update;
using Wolverine;

namespace StudentParliamentSystem.UnitTests.UseCases.Users;

public class UserTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IRoleRepository> _roleRepo = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IMessageBus> _bus = new();
    private readonly Mock<ILogger<UpdateUserDetailsHandler>> _logger = new();

    [Fact]
    public async Task RetrieveAllUsersHandler_DelegatesToRepository()
    {
        var sut = new RetrieveAllUsersHandler(_userRepo.Object);
        var expected = new PagedResult<UserPreview> { Items = new List<UserPreview>(), TotalCount = 0, CurrentPage = 1, PageSize = 10 };
        _userRepo.Setup(r => r.RetrieveAllAsync(1, 10, "q")).ReturnsAsync(expected);

        var result = await sut.HandleAsync(new RetrieveAllUsers(1, 10, "q"));

        result.Should().BeSameAs(expected);
    }

    [Fact]
    public async Task RetrieveUserByIdHandler_Found_ReturnsOk()
    {
        var sut = new RetrieveUserByIdHandler(_userRepo.Object);
        var id = Guid.NewGuid();
        var user = User.Create(id, "a@b.com", "A", "B");
        _userRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(user);

        var result = await sut.HandleAsync(new RetrieveUserById(id));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(user);
    }

    [Fact]
    public async Task RetrieveUserByIdHandler_NotFound_ReturnsFail()
    {
        var sut = new RetrieveUserByIdHandler(_userRepo.Object);
        _userRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

        var result = await sut.HandleAsync(new RetrieveUserById(Guid.NewGuid()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task RetrieveUsersByRoleHandler_ValidRole_ReturnsPreviews()
    {
        var sut = new RetrieveUsersByRoleHandler(_userRepo.Object, _roleRepo.Object);
        var role = new Role { Id = Guid.NewGuid(), Name = RoleName.SuperAdmin };
        var user = User.Create(Guid.NewGuid(), "a@b.com", "A", "B");
        user.Roles.Add(role);
        
        _roleRepo.Setup(r => r.GetByNameAsync(RoleName.SuperAdmin)).ReturnsAsync(Result.Ok(role));
        _userRepo.Setup(r => r.GetUsersByRoleAsync(role.Id)).ReturnsAsync(new List<User> { user });

        var result = await sut.HandleAsync(new RetrieveUsersByRole(RoleName.SuperAdmin));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value.First().Email.Should().Be("a@b.com");
    }

    [Fact]
    public async Task UpdateUserDetailsHandler_ValidUpdate_SavesAndPublishes()
    {
        var sut = new UpdateUserDetailsHandler(_userRepo.Object, _roleRepo.Object, _uow.Object, _bus.Object, _logger.Object);
        var userId = Guid.NewGuid();
        var user = User.Create(userId, "old@b.com", "Old", "Name");
        var role = new Role { Id = Guid.NewGuid(), Name = RoleName.SuperAdmin };
        
        _userRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _roleRepo.Setup(r => r.GetByNameAsync(RoleName.SuperAdmin)).ReturnsAsync(Result.Ok(role));
        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var command = new UpdateUserDetails(userId, "New", "Name", new List<string> { "SuperAdmin" });
        var result = await sut.HandleAsync(command);

        result.IsSuccess.Should().BeTrue();
        user.FirstName.Should().Be("New");
        user.Roles.Should().Contain(role);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _bus.Verify(b => b.PublishAsync(It.Is<UserDetailsUpdated>(m => m.UserId == userId && m.FirstName == "New"), default), Times.Once);
    }

    [Fact]
    public async Task UpdateUserDetailsHandler_UserNotFound_ReturnsFail()
    {
        var sut = new UpdateUserDetailsHandler(_userRepo.Object, _roleRepo.Object, _uow.Object, _bus.Object, _logger.Object);
        _userRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

        var result = await sut.HandleAsync(new UpdateUserDetails(Guid.NewGuid(), "A", "B", new List<string>()));

        result.IsFailed.Should().BeTrue();
    }
}
