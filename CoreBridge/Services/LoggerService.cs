using CoreBridge.Models;
using CoreBridge.Services.Interfaces;
using NLog;

namespace CoreBridge.Services
{
    public class LoggerService : ILoggerService
    {
        private static NLog.ILogger logger = LogManager.GetCurrentClassLogger();



        public void LogDebug(string message)
        {
            logger.Debug(message);
        }
        public void LogDebug(string message, Exception ex)
        {
            logger.Debug(ex, message);
        }
        public void LogError(string message)
        {
            logger.Error(message);
        }
        public void LogError(string message, Exception ex)
        {
            logger.Error(ex, message);
        }

        public void LogError(string message, CoreBridgeException ex)
        {
            logger.Error(ex, $"ErrCode[{ex.Code}] | Action[{ex.Action}] | " + message);
        }

        public void LogError(int errorCode, string message, Exception ex)
        {
            logger.Error(ex, $"Code[{errorCode}] |" + message);
        }


        public void LogInfo(string message)
        {
            logger.Info(message);
        }
        public void LogWarn(string message)
        {
            logger.Warn(message);
        }
        public void LogWarn(string message, Exception ex)
        {
            logger.Warn(ex, message);
        }
    }
}
