using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Security.Claims;
using TGramHunt.Contract;
using TGramHunt.Services.Services;

namespace TGramHunt.Services.Test.Services
{
    [TestClass]
    public class AdminUserServiceTests
    {
        private AdminUserService _userService;
        private Mock<UserManager<AdminUser>> _userManagerMoq;
        private Mock<SignInManager<AdminUser>> _signInManagerMoq;

        public AdminUserServiceTests()
        {
            _userManagerMoq = new Mock<UserManager<AdminUser>>(Mock.Of<IUserStore<AdminUser>>(),
                null, null, null, null, null, null, null, null);
            _signInManagerMoq = new Mock<SignInManager<AdminUser>>(_userManagerMoq.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<AdminUser>>().Object,
                null, null, null, null);
            _userService = new AdminUserService(_userManagerMoq.Object, _signInManagerMoq.Object);
        }

        [TestMethod]
        public async Task GetIfAuthorized_NullCurrentUser_ReturnsNull()
        {
            ClaimsPrincipal user = null;

            var result = await _userService.GetIfAuthorized(user);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetIfAuthorized_NotSignInUser_ReturnsNull()
        {
            var user = new ClaimsPrincipal();
            _signInManagerMoq.Setup(manager => manager.IsSignedIn(user)).Returns(false);

            var result = await _userService.GetIfAuthorized(user);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetIfAuthorized_NotFoundUser_ReturnsNull()
        {
            var user = new ClaimsPrincipal();
            _signInManagerMoq.Setup(manager => manager.IsSignedIn(user)).Returns(true);
            _userManagerMoq.Setup(manager => manager.GetUserAsync(user)).ReturnsAsync((AdminUser)null);

            var result = await _userService.GetIfAuthorized(user);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetIfAuthorized_FoundUser_ReturnsUser()
        {
            var user = new ClaimsPrincipal();
            var emptyAdminUser = new AdminUser();
            _signInManagerMoq.Setup(manager => manager.IsSignedIn(user)).Returns(true);
            _userManagerMoq.Setup(manager => manager.GetUserAsync(user)).ReturnsAsync(emptyAdminUser);

            var result = await _userService.GetIfAuthorized(user);

            Assert.AreEqual(emptyAdminUser, result);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow(" ")]
        public async Task GetById_NullOrEmptyId_ReturnsNull(string id)
        {
            var result = await _userService.Get(id);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetById_CorrectId_ReturnsUser()
        {
            string id = "id";
            var emptyAdminUser = new AdminUser();
            _userManagerMoq.Setup(manager => manager.FindByIdAsync(id)).ReturnsAsync(emptyAdminUser);

            var result = await _userService.Get(id);

            Assert.AreEqual(emptyAdminUser, result);
        }

        [TestMethod]
        public async Task GetByClaims_NullClaims_ReturnsNull()
        {
            var result = await _userService.Get((ClaimsPrincipal)null);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetByClaims_CorrectId_ReturnsUser()
        {
            var user = new ClaimsPrincipal();
            var emptyAdminUser = new AdminUser();
            _userManagerMoq.Setup(manager => manager.GetUserAsync(user)).ReturnsAsync(emptyAdminUser);

            var result = await _userService.Get(user);

            Assert.AreEqual(emptyAdminUser, result);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow(" ")]
        public async Task GetByEmail_NullOrEmptyId_ReturnsNull(string email)
        {
            var result = await _userService.Get(email);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetByEmail_CorrectId_ReturnsUser()
        {
            string email = "email";
            var emptyAdminUser = new AdminUser();
            _userManagerMoq.Setup(manager => manager.FindByEmailAsync(email)).ReturnsAsync(emptyAdminUser);

            var result = await _userService.GetByEmail(email);

            Assert.AreEqual(emptyAdminUser, result);
        }
    }
}
