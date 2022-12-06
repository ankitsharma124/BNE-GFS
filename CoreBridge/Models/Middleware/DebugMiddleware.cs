using CoreBridge.Models.Extensions;
using CoreBridge.Services.Interfaces;
using System.Diagnostics;

namespace CoreBridge.Models.Middleware
{
    public class DebugMiddleware
    {
        private readonly RequestDelegate _next;
        public DebugMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext, IRequestService reqService)
        {
            var headers = httpContext.Request.Headers;
            if (headers.ContainsKey(SysConsts.AddedReqHeaderKey_OriginalBody)) //call to server-api. copy already exists.
            {
                httpContext.Request.Headers.Remove(SysConsts.AddedReqHeaderKey_Debug_ReqBody);
                headers.Add(SysConsts.AddedReqHeaderKey_Debug_ReqBody,
                    headers[SysConsts.AddedReqHeaderKey_OriginalBody]);
            }
            else
            {
                await reqService.Debug_CopyBodyToHeader(httpContext.Request);
            }
            await _next(httpContext);
        }
    }
}
