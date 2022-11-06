using CoreBridge.Models.Interfaces;

namespace CoreBridge.Models
{
    public class CoreBridgeException : Exception, ICoreBridgeException
    {
        public int Code { get; set; }
        public int Action { get; set; }

        public CoreBridgeException() { }

        public CoreBridgeException(int action)
        {
            Action = action;
        }

        public CoreBridgeException(string message) : base(message) { }
    }
}
