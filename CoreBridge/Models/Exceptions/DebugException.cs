namespace CoreBridge.Models.Exceptions
{
    public class DebugException : Exception
    {
        public DebugException() { }
        public DebugException(string message) : base(message) { }
    }
}
