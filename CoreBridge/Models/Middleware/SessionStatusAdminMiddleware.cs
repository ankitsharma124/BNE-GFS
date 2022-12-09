using CoreBridge.Services.Interfaces;
using System.Drawing.Text;

namespace CoreBridge.Models.Middleware
{
    public class SessionStatusAdminMiddleware
    {

        private readonly RequestDelegate _next;
        private ISessionStatusService _sss;
        private IConfiguration _config;
        private IRequestService _req;
        public SessionStatusAdminMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext, ISessionStatusService sss, IRequestService req, IConfiguration config)
        {
            _sss = sss;
            _config = config;
            _req = req;
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
            await _req.LoadStatus_RequestBody(httpContext.Request);
#else
            if (_sss.IsServerApi)
            {
                await _req.LoadStatus_RequestBody(httpContext.Request);   
            }
#endif
        }
    }
}
