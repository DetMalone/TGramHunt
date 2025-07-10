using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;
using TGramHunt.Services.Services.IServices;

namespace TGramHunt.Helpers
{
    public class PermissionHandler : IAuthorizationHandler
    {
        private readonly IUserService _userService;

        public PermissionHandler(
            IUserService userService)
        {
            this._userService = userService;
        }

        public async Task HandleAsync(AuthorizationHandlerContext context)
        {
            // https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-3.1
            var isAllowed = context.PendingRequirements
                .FirstOrDefault(x => x is AllowForCloseRequirement);

            var user = await _userService.Get(context.User);
            if (user == null ||
                isAllowed == null && user.IsClosed)
            {
                context.Fail(new AuthorizationFailureReason(this, "Account is closed"));
            }
            else if (isAllowed != null)
            {
                context.Succeed(isAllowed);
            }
        }
    }
}