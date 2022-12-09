using CoreBridge.Models.Exceptions;
using CoreBridge.Services.Interfaces;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Net;
using System.Text.Unicode;
using System.Text;
using CoreBridge.Models.Extensions;

namespace CoreBridge.Models.Middleware
{
    /*
    public class HashAdminMiddleware
    {
        private readonly RequestDelegate _next;
        public HashAdminMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext, IRequestService reqService)
        {
            if (httpContext.Request.Path.StartsWithSegments("/api/server"))
            {
                await reqService.RemoveHash(httpContext.Request);
            }
            await _next(httpContext);
        }

    }
    */
}
