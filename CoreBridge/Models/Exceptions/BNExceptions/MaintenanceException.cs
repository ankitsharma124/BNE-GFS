using Google.Rpc;

namespace CoreBridge.Models.Exceptions.BNExceptions
{
    public class MaintenanceException : CoreBridgeException
    {
        public MaintenanceException(int action, string message) : base(message)
        {
            Code = 2;
            Action = action;
        }
    }
}
