using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using TGramHunt.Contract;
using TGramHunt.Services.Services.IServices;

namespace TGramHunt.Test.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TGramHunt.Helpers;

[TestClass]
public class PermissionHandlerTests
{
    private readonly User _openUser = new User { IsClosed = false };
    private readonly User _closedUser = new User { IsClosed = true };

    [TestMethod]
    public async Task HandleAsync_AllowForCloseRequirement_Success()
    {
        // Arrange
        var requirement = new AllowForCloseRequirement();
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "testuser")
        }));
        var context = new AuthorizationHandlerContext(
            new[] { requirement }, user, null);
        var userService = new FakeUserService();
        var permissionHandler = new PermissionHandler(userService);

        // Act
        await permissionHandler.HandleAsync(context);

        // Assert
        Assert.IsFalse(context.HasSucceeded);
    }

    [TestMethod]
    public async Task HandleAsync_AccountNotClosed_Success()
    {
        // Arrange
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "testuser")
        }));
        var context = new AuthorizationHandlerContext(
            new[] { new AllowForCloseRequirement() }, user, null);
        var userService = new FakeUserService();
        userService.Users.Add("testuser", _openUser);
        userService.Principal = user;
        var permissionHandler = new PermissionHandler(userService);

        // Act
        await permissionHandler.HandleAsync(context);

        // Assert
        Assert.IsFalse(context.HasSucceeded);
    }

    [TestMethod]
    public async Task HandleAsync_AccountClosed_Fail()
    {
        // Arrange
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "testuser")
        }));
        var context = new AuthorizationHandlerContext(
            new[] { new AllowForCloseRequirement() }, user, null);
        var userService = new FakeUserService();
        userService.Users.Add("testuser", _closedUser);
        userService.Principal = user;
        var permissionHandler = new PermissionHandler(userService);

        // Act
        await permissionHandler.HandleAsync(context);

        // Assert
        Assert.IsTrue(context.HasFailed);
    }
}

public class FakeUserService : IUserService
{
    public Dictionary<string, User> Users { get; } = new Dictionary<string, User>();

    public ClaimsPrincipal Principal { get; set; }

    public Task<string?> BaseRegister(User user, string password, string confirmPassword)
    {
        throw new NotImplementedException();
    }

    public Task<User?> Get(string id)
    {
        return Task.FromResult(Users.TryGetValue(id, out var result) ? result : null);
    }

    public Task<User?> GetIfAuthorized(ClaimsPrincipal user)
    {
        if (user == Principal)
        {
            return Get(user.Identity?.Name ?? string.Empty);
        }
        else
        {
            return Task.FromResult<User?>(null);
        }
    }

    public Task Login(User user, bool rememberMe)
    {
        throw new NotImplementedException();
    }

    public Task<string?> Register(User user, string password, string confirmPassword)
    {
        throw new NotImplementedException();
    }

    public Task Remove(User user)
    {
        throw new NotImplementedException();
    }

    public Task<IdentityResult> Update(User user)
    {
        throw new NotImplementedException();
    }
    
    public Task<User?> Get(ClaimsPrincipal claimsPrincipal)
    {
        return GetIfAuthorized(claimsPrincipal);
    }

}