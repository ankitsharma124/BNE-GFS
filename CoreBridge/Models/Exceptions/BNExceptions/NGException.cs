namespace CoreBridge.Models.Exceptions.BNExceptions
{
    public class NGException : CoreBridgeException
    {
        public NGException(int action, string message) : base(message)
        {
            Code = 1;
            Action = action;
        }
    }
}
