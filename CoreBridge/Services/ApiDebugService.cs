using CoreBridge.Services.Interfaces;

namespace CoreBridge.Services
{
    public class ApiDebugService
    {
        private readonly IConfiguration _config;
        private readonly ISessionStatusService _sss;
        private readonly ILogger _logger;
        public ApiDebugService(IConfiguration config, ISessionStatusService sss, ILogger logger)
        {
            _config = config;
            _sss = sss;
            _logger = logger;
        }

        public DateTime Started { get; set; } = DateTime.MinValue;

        public void StartTimer()
        {
            Started = DateTime.Now;
        }

        public void LogUserAgent(HttpRequest req)
        {
            var agent = req.Headers.UserAgent;
            _logger.LogInformation($"HttpHeader[ User-Agent: {agent} ]");
        }


    }
}
