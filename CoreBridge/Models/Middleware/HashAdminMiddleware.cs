using CoreBridge.Models.Exceptions;
using CoreBridge.Services.Interfaces;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Net;
using System.Text.Unicode;
using System.Text;

namespace CoreBridge.Models.Middleware
{
    public class HashAdminMiddleware
    {
        private readonly RequestDelegate _next;
        public HashAdminMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext.Request.Path.StartsWithSegments("/api/server"))
            {
                byte[] originalContent = null;
                using (StreamReader stream = new StreamReader(httpContext.Request.Body))
                {
                    var ms = new MemoryStream();
                    stream.BaseStream.CopyTo(ms);
                    originalContent = new byte[ms.Length];
                    ms.GetBuffer().CopyTo(originalContent, ms.Length);
                }
                var hash = originalContent.Take(16).ToArray();
                var bodyWithoutHash = originalContent.Skip(16).ToArray();

                MemoryStream newBody = new MemoryStream(bodyWithoutHash);
                httpContext.Request.Body = newBody;

                //todo: rewrite request

                httpContext.Request.Headers.Remove("InternalHash");
                httpContext.Request.Headers.Add("InternalHash", System.Text.Encoding.UTF8.GetString(hash, 0, hash.Length));
                httpContext.Request.Headers.Remove("InternalOriginalBody");
                httpContext.Request.Headers.Add("InternalOriginalBody", System.Text.Encoding.UTF8.GetString(bodyWithoutHash));
            }
            await _next(httpContext);
        }

    }
}
