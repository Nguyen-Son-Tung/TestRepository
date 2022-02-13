using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Producers
{
    public class GenerateXRequestIdMiddleware
    {
        private readonly RequestDelegate _next;
        private const string X_REQUEST_ID_KEY = "x-request-id";
        public GenerateXRequestIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            string xRequestId = null;
            if (httpContext.Request.Headers.TryGetValue(X_REQUEST_ID_KEY, out StringValues xRequestIds))
            {
                xRequestId = xRequestIds.FirstOrDefault(k => k.Equals(X_REQUEST_ID_KEY));
            }
            else
            {
                xRequestId = Guid.NewGuid().ToString();
                httpContext.Request.Headers.Add(X_REQUEST_ID_KEY, xRequestId);
            }
            httpContext.Response.OnStarting(() =>
            {
                if (!httpContext.Response.Headers.TryGetValue(X_REQUEST_ID_KEY, out xRequestIds))
                    httpContext.Response.Headers.Add(X_REQUEST_ID_KEY, xRequestId);
                return Task.CompletedTask;
            });

            // Call the next delegate/middleware in the pipeline.
            await _next(httpContext);
        }
    }
}
