using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using TGramHunt.Services.Services.IServices;

namespace TGramHunt.Services.Middleware
{
    public class LoggerMiddleware : IMiddleware
    {
        private readonly ILoggService _loggService;

        public LoggerMiddleware(ILoggService loggService)
        {
            this._loggService = loggService;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            string userName;
            try
            {
                await next(context);
            }
            catch (Exception e)
            {
                userName = context.User?.Identity?.Name ?? string.Empty;
                await this._loggService.Log(e, userName);
                throw;
            }

            userName = context.User?.Identity?.Name ?? string.Empty;
            var request = context.Request;
            var fullUrl = $"{request.Scheme}://{request.Host}{request.PathBase}{request.Path}{request.QueryString}";

            await this._loggService.Log(
                $"{fullUrl} executed with status {context.Response.StatusCode}",
                userName);
        }
    }
}
