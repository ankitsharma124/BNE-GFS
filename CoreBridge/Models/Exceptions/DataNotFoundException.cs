using Google.Rpc;

namespace CoreBridge.Models.Exceptions
{
    public class DataNotFoundException : CoreBridgeException
    {
        public DataNotFoundException(string message) : base(message)
        {
            Code = 1002;
            Action = 2;
        }
    }
}
