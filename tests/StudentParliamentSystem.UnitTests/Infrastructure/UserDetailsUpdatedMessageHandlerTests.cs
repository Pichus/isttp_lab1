using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using StudentParliamentSystem.Infrastructure.Identity.Data.Entities;
using StudentParliamentSystem.Infrastructure.Identity.Handlers;
using StudentParliamentSystem.Shared.Contracts.Users;

namespace StudentParliamentSystem.UnitTests.Infrastructure;

public class UserDetailsUpdatedMessageHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManager;
    private readonly Mock<ILogger<UserDetailsUpdatedMessageHandler>> _logger = new();
    private readonly UserDetailsUpdatedMessageHandler _sut;

    public UserDetailsUpdatedMessageHandlerTests()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        _userManager = new Mock<UserManager<ApplicationUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        _sut = new UserDetailsUpdatedMessageHandler(_userManager.Object, _logger.Object);
    }

    [Fact]
    public async Task HandleAsync_UserNotFound_LogsWarningAndReturns()
    {
        var userId = Guid.NewGuid();
        var message = new UserDetailsUpdated(userId, "New", "Name", new List<string>());
        _userManager.Setup(m => m.FindByIdAsync(userId.ToString())).ReturnsAsync((ApplicationUser?)null);

        await _sut.HandleAsync(message);

        _userManager.Verify(m => m.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_ValidUpdate_SyncsNameAndRoles()
    {
        var userId = Guid.NewGuid();
        var user = new ApplicationUser { Id = userId, FirstName = "Old", LastName = "Name" };
        var message = new UserDetailsUpdated(userId, "New", "Name", new List<string> { "Member" });
        
        _userManager.Setup(m => m.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
        _userManager.Setup(m => m.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);
        _userManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { "OldRole" });
        _userManager.Setup(m => m.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>())).ReturnsAsync(IdentityResult.Success);
        _userManager.Setup(m => m.AddToRolesAsync(user, It.IsAny<IEnumerable<string>>())).ReturnsAsync(IdentityResult.Success);

        await _sut.HandleAsync(message);

        user.FirstName.Should().Be("New");
        user.LastName.Should().Be("Name");
        _userManager.Verify(m => m.UpdateAsync(user), Times.Once);
        _userManager.Verify(m => m.RemoveFromRolesAsync(user, It.Is<IEnumerable<string>>(r => r.Contains("OldRole"))), Times.Once);
        _userManager.Verify(m => m.AddToRolesAsync(user, It.Is<IEnumerable<string>>(r => r.Contains("Member"))), Times.Once);
    }
}
