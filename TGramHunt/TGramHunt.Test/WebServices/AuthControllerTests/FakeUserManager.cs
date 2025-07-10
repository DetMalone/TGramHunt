using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using TGramHunt.Contract;

namespace TGramHunt.Test.WebServices.AuthControllerTests
{
    public class FakeUserManager : UserManager<User>
    {
        public FakeUserManager()
            : base(Substitute.For<IUserStore<User>>(),
              Substitute.For<IOptions<IdentityOptions>>(),
              Substitute.For<IPasswordHasher<User>>(),
              Array.Empty<IUserValidator<User>>(),
              Array.Empty<IPasswordValidator<User>>(),
              Substitute.For<ILookupNormalizer>(),
              Substitute.For<IdentityErrorDescriber>(),
              Substitute.For<IServiceProvider>(),
              Substitute.For<ILogger<UserManager<User>>>())
        { }

        public override Task<IdentityResult> CreateAsync(User user, string password)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public override Task<IdentityResult> AddToRoleAsync(User user, string role)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public override Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return Task.FromResult(Guid.NewGuid().ToString());
        }
    }
}
