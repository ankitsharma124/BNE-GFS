using CoreBridge.Services.Interfaces;
using System.Drawing.Text;

namespace CoreBridge.Models.Middleware
{
    public class SessionStatusAdminMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly ISessionStatusService _sss;
        private readonly IConfiguration _config;
        public SessionStatusAdminMiddleware(RequestDelegate next, ISessionStatusService sss,
            IConfiguration config)
        {
            _next = next;
            _sss = sss;
            _config = config;
        }
        public async Task InvokeAsync(HttpContext httpContext, IRequestService reqService)
        {
            InitSSS(httpContext);
            await _next(httpContext);
#if DEBUG
            await _sss.SaveSessionDebugInfo();
#endif
        }

        private async void InitSSS(HttpContext httpContext)
        {
            _sss.UseJson = _config.GetValue<bool>("DebugConfig:UseJson");
            var path = httpContext.Request.Path.ToString().ToLower();
            _sss.IsClientApi = path.Contains("api/client/");
            _sss.IsServerApi = path.ToLower().Contains("api/server/");
#if DEBUG
            await _sss.CopyRequestBody(httpContext.Request);
#else
            if (_sss.IsServerApi)
            {
                await _sss.CopyRequestBody(httpContext.Request);   
            }
#endif
        }
    }
}
