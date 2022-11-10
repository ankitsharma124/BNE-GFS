using CoreBridge.Models;

namespace CoreBridge.Services.Interfaces
{
    public interface ILoggerService
    {
        void LogDebug(string message);
        void LogDebug(string message, Exception ex);
        void LogError(string message);
        void LogError(string message, Exception ex);
        void LogError(string message, CoreBridgeException ex);
        void LogError(int errorCode, string message, Exception ex);
        void LogInfo(string message);
        void LogWarn(string message);
        void LogWarn(string message, Exception ex);
    }
}
