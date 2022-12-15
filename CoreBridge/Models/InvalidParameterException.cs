namespace CoreBridge.Models
{
    public class InvalidParameterException : CoreBridgeException
    {
        public InvalidParameterException(string message) : base(message)
        {
            Code = 1001;
            Action = 1;
        }
    }
}
