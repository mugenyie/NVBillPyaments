using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NVBillPayments.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NVBillPayments.API.Attributes
{
    [AttributeUsage(validOn: AttributeTargets.Class)]
    public class AuthKeysAttribute : Attribute, IAsyncActionFilter
    {
        private const string APIKEYNAME = "x-api-key";
        private const string USERKEY = "user-key";
        private string API_KEY = ConfigurationConstants.API_AUTH_KEY;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "UnAuthorised Client"
                };
                return;
            }

            if (!context.HttpContext.Request.Headers.TryGetValue(USERKEY, out var extractedUserKey))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "UnAuthorised User"
                };
                return;
            }

            if (!API_KEY.Equals(extractedApiKey))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "Invalid Client Key"
                };
                return;
            }

            if (string.IsNullOrEmpty(extractedUserKey))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "Invalid User Key"
                };
                return;
            }

            await next();
        }
    }
}
