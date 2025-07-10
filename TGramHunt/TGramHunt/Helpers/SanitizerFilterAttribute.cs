using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TGramHunt.Helpers
{
    public class SanitizerFilter : IAsyncPageFilter
    {
        public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context) => Task.CompletedTask;

        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            #region sanitize errors
            var sanitizer = new Sanitizer();
            var errorList = context.ModelState.Where(x => x.Value.ValidationState == ModelValidationState.Invalid);

            foreach (var node in errorList)
            {
                if (node.Value == null ||
                    node.Value.Errors == null ||
                    !node.Value.Errors.Any())
                {
                    continue;
                }

                var errors = node.Value.Errors.Select(x => x.ErrorMessage).ToList();
                context.ModelState.Remove(node.Key);
                foreach (var error in errors)
                {
                    context.ModelState
                        .AddModelError(
                        sanitizer.Sanitize(node.Key),
                        sanitizer.Sanitize(error));
                }
            }
            #endregion

            #region sanitize values            
            var arguments = new Dictionary<string, object>(context.HandlerArguments);
            foreach (var argument in arguments)
            {
                if (argument.Value is string)
                {
                    var sanitizedValue = sanitizer.Sanitize(
                        argument.Value as string,
                        argument.Key,
                        context.ModelState);
                    context.HandlerArguments[argument.Key] =
                        sanitizedValue;
                }
                else
                {
                    sanitizer.Sanitize(
                        argument.Value,
                        context.ModelState);
                }
            }
            #endregion

            await next.Invoke();
        }
    }
}