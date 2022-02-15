using Microsoft.AspNetCore.Http;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Middlewares
{
    public class InjectedIdentityMiddleware
    {
        private readonly RequestDelegate _next;
        public InjectedIdentityMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public Task Invoke(HttpContext context)
        {
            LogContext.PushProperty("UserName", "Tung 123");
            LogContext.PushProperty("x-request-id", context.Request.Headers.First());
            return _next(context);
        }
    }
}
