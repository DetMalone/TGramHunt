using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using TGramHunt.Helpers;
using TGramHunt.Services.Helpers;

namespace TGramHunt.Middleware
{
    public class AuthorizationMiddlewareResultHandlerCust : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();
        private readonly LogOutHelper _logOutHelper;

        public AuthorizationMiddlewareResultHandlerCust(
            LogOutHelper _logOutHelper)
        {
            this._logOutHelper = _logOutHelper;
        }

        public async Task HandleAsync(
            RequestDelegate next,
            HttpContext context,
            AuthorizationPolicy policy,
            PolicyAuthorizationResult authorizeResult)
        {
            var authorizationFailureReason = authorizeResult
                .AuthorizationFailure?
                .FailureReasons
                .FirstOrDefault();

            var message = authorizationFailureReason?.Message;
            var handler = authorizationFailureReason?.Handler;

            if (handler is PermissionHandler)
            {
                await this._logOutHelper.Logout(context);
                context.Response.StatusCode =
                    StatusCodes.Status401Unauthorized;
                await Results
                    .Json(message)
                    .ExecuteAsync(context);
                return;
            }

            if (!authorizeResult.Succeeded)
            {
                var unauthorizedPolicy = policy.Requirements.FirstOrDefault(x => x is DenyAnonymousAuthorizationRequirement);
                if (unauthorizedPolicy is DenyAnonymousAuthorizationRequirement)
                {
                    await this._logOutHelper
                        .Logout(context);
                    context.Response.StatusCode =
                        StatusCodes.Status401Unauthorized;

                    await Results
                        .Redirect(LinkHelper.rootLink)
                        .ExecuteAsync(context);

                    return;
                }
            }

            // Fall back to the default implementation.
            await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
        }
    }
}