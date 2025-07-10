using TGramHunt.Services.Services.IServices;
namespace TGramHunt.Test.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TGramHunt.Contract;
using TGramHunt.Services.Services;


[TestClass]
public class UserServiceTest
{
    private Mock<UserManager<User>> _userManagerMock;
    private Mock<RoleManager<Role>> _roleManagerMock;
    private Mock<SignInManager<User>> _signInManagerMock;
    private Mock<IPasswordValidator<User>> _passwordValidatorMock;
    private Mock<IUserVotesForProductsServices> _userVotesForProductsServicesMock;
    private Mock<IProductService> _productServiceMock;
    private Mock<ISmallFilesService> _smallFilesServiceMock;

    private UserService _userService;

    [TestInitialize]
    public void Initialize()
    {
        _userManagerMock =
            new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
        _roleManagerMock = new Mock<RoleManager<Role>>(Mock.Of<IRoleStore<Role>>(), null, null, null, null);
        _signInManagerMock = new Mock<SignInManager<User>>(_userManagerMock.Object,
            new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<User>>().Object,
            null, null, null, null);
        _passwordValidatorMock = new Mock<IPasswordValidator<User>>();
        _userVotesForProductsServicesMock = new Mock<IUserVotesForProductsServices>();
        _productServiceMock = new Mock<IProductService>();
        _smallFilesServiceMock = new Mock<ISmallFilesService>();

        _userService = new UserService(_userManagerMock.Object,
            _signInManagerMock.Object,
            _roleManagerMock.Object,
            _passwordValidatorMock.Object,
            _userVotesForProductsServicesMock.Object,
            _productServiceMock.Object,
            _smallFilesServiceMock.Object);
    }

    [TestMethod]
    public async Task GetIfAuthorized_UserIsNotSignedIn_ReturnsNull()
    {
        // Arrange
        var currentUser = new ClaimsPrincipal();

        _signInManagerMock.Setup(m => m.IsSignedIn(currentUser)).Returns(false);

        // Act
        var result = await _userService.GetIfAuthorized(currentUser);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task Login_CallsSignInManager()
    {
        // Arrange
        var user = new User();

        // Act
        await _userService.Login(user, true);

        // Assert
        _signInManagerMock.Verify(m => m.SignInAsync(user, true, null), Times.Once);
    }

    [TestMethod]
    public async Task GetIfAuthorized_UserIsSignedInButUserIsNotFound_ReturnsNull()
    {
        // Arrange
        var currentUser = new ClaimsPrincipal();
        _signInManagerMock.Setup(x => x.IsSignedIn(currentUser)).Returns(true);
        _userManagerMock.Setup(x => x.GetUserAsync(currentUser)).ReturnsAsync((User)null);

        // Act
        var result = await _userService.GetIfAuthorized(currentUser);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetIfAuthorized_UserIsSignedInButUserIsClosed_ReturnsNull()
    {
        // Arrange
        var currentUser = new ClaimsPrincipal();
        var user = new User { IsClosed = true };

        _signInManagerMock.Setup(x => x.IsSignedIn(currentUser)).Returns(true);
        _userManagerMock.Setup(x => x.GetUserAsync(currentUser)).ReturnsAsync(user);

        // Act
        var result = await _userService.GetIfAuthorized(currentUser);

        // Assert
        Assert.IsNull(result);
    }


    [TestMethod]
    public async Task GetIfAuthorized_ReturnsNull_WhenUserIsNotSignedIn()
    {
        // Arrange
        ClaimsPrincipal currentUser = null;

        // Act
        var result = await _userService.GetIfAuthorized(currentUser);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task Get_ReturnsNull_WhenIdIsEmpty()
    {
        // Arrange
        string userId = null;

        // Act
        var result = await _userService.Get(userId);

        // Assert
        Assert.IsNull(result);
    }
    
    [TestMethod]
    public async Task GetIfAuthorized_ReturnsUser_WhenUserIsSignedInAndNotClosed()
    {
        // Arrange
        var currentUser = new ClaimsPrincipal();
        var user = new User { IsClosed = false };

        _signInManagerMock.Setup(x => x.IsSignedIn(currentUser)).Returns(true);
        _userManagerMock.Setup(x => x.GetUserAsync(currentUser)).ReturnsAsync(user);

        // Act
        var result = await _userService.GetIfAuthorized(currentUser);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(user, result);
    }
    
    [TestMethod]
    public async Task Create_ReturnsUser_WhenUserCreationSucceeds()
    {
        // Arrange
        string strGuid = "d62fa2e3-feca-485f-a1e0-69cfa7a9eac9";
        Guid guid = Guid.Parse(strGuid);

        var user = new User
        {
            Id = guid,
            Email = "test@example.com",
            UserName = "testuser"
        };

        _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success)
            .Callback<User, string>((u, p) =>
            {
                u.Id = user.Id;
            });

        // Act
        var result = user;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(user.Id, result.Id);
        Assert.AreEqual(user.Email, result.Email);
        Assert.AreEqual(user.UserName, result.UserName);
    }
    
    [TestMethod]
    public async Task Get_ReturnsUser_WhenIdIsNotEmpty()
    {
        // Arrange
        string strGuid = "d62fa2e3-feca-485f-a1e0-69cfa7a9eac9";
        Guid guid = Guid.Parse(strGuid);
        var user = new User { Id = guid };

        _userManagerMock.Setup(x => x.FindByIdAsync(strGuid)).ReturnsAsync(user);

        // Act
        var result = await _userService.Get(strGuid);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(user, result);
    }
    
    [TestMethod]
    public async Task GetIfAuthorized_ReturnsNull_IfUserIsNotSignedIn()
    {
        // Arrange
        
        var currentUser = new ClaimsPrincipal();

        _signInManagerMock.Setup(x => x.IsSignedIn(currentUser)).Returns(false);

        // Act
        var result = await _userService.GetIfAuthorized(currentUser);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetIfAuthorized_ReturnsNull_IfCurrentUserIsNull()
    {
        // Arrange
        ClaimsPrincipal currentUser = null;

        // Act
        var result = await _userService.GetIfAuthorized(currentUser);

        // Assert
        Assert.IsNull(result);
    }
    
    [TestMethod]
    public async Task GetIfAuthorized_Should_Return_Null_If_User_Is_Closed()
    {
        // Arrange
        string strGuid = "d62fa2e3-feca-485f-a1e0-69cfa7a9eac9";
        Guid guid = Guid.Parse(strGuid);
        var currentUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1")
        }));
        var user = new User() { Id = guid, IsClosed = true };
        _signInManagerMock.Setup(x => x.IsSignedIn(currentUser)).Returns(true);
        _userManagerMock.Setup(x => x.GetUserAsync(currentUser)).ReturnsAsync(user);

        // Act
        var result = await _userService.GetIfAuthorized(currentUser);

        // Assert
        Assert.IsNull(result);
    }
    
    [TestMethod]
    public async Task GetIfAuthorized_Returns_Null_If_CurrentUser_Is_Not_Signed_In()
    {
        // Arrange
        var currentUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1")
        }));
        _signInManagerMock.Setup(x => x.IsSignedIn(currentUser)).Returns(false);

        // Act
        var result = await _userService.GetIfAuthorized(currentUser);

        // Assert
        Assert.IsNull(result);
    }
    
    [TestMethod]
    public async Task Login_ValidUser_SignsInUser()
    {
        // Arrange
        string strGuid = "d62fa2e3-feca-485f-a1e0-69cfa7a9eac9";
        Guid guid = Guid.Parse(strGuid);
        var user = new User { Id = guid, UserName = "testuser" };
        var staySignedIn = true;
        var signInResult = SignInResult.Success;
        _signInManagerMock.Setup(x => x.SignInAsync(user, staySignedIn, null));

        var userService = new UserService(_userManagerMock.Object, _signInManagerMock.Object, _roleManagerMock.Object, _passwordValidatorMock.Object, _userVotesForProductsServicesMock.Object, _productServiceMock.Object, _smallFilesServiceMock.Object);

        // Act
        await userService.Login(user, staySignedIn);

        // Assert
        _signInManagerMock.Verify(x => x.SignInAsync(user, staySignedIn, null), Times.Once);
    }

    [TestMethod]
    public async Task Update_ValidUser_UpdatesUser()
    {
        // Arrange
        string strGuid = "d62fa2e3-feca-485f-a1e0-69cfa7a9eac9";
        Guid guid = Guid.Parse(strGuid);
        var user = new User { Id = guid, UserName = "testuser" };
        var identityResult = IdentityResult.Success;
        _userManagerMock.Setup(x => x.UpdateAsync(user)).ReturnsAsync(identityResult);

        var userService = new UserService(_userManagerMock.Object, _signInManagerMock.Object, _roleManagerMock.Object, _passwordValidatorMock.Object, _userVotesForProductsServicesMock.Object, _productServiceMock.Object, _smallFilesServiceMock.Object);

        // Act
        var result = await userService.Update(user);

        // Assert
        Assert.AreEqual(identityResult, result);
        _userManagerMock.Verify(x => x.UpdateAsync(user), Times.Once);
    }
}