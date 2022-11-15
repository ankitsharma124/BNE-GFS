using CoreBridge.Models;
using CoreBridge.Models.Exceptions;

namespace CoreBridge.Services.Interfaces
{
    public interface ILoggerService
    {
        void LogDebug(string message);
        void LogDebug(string message, Exception ex);
        void LogError(string message);
        void LogError(string message, Exception ex);
        void LogError(BNException ex);
        void LogInfo(string message);
        void LogWarn(string message);
        void LogWarn(string message, Exception ex);
    }
}
