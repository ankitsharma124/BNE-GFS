namespace CoreBridge.Models.Extensions
{
    public static class BasicTypeExtensions
    {
        public static bool IsNumber(this object value)
        {
            return value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal;
        }

        public static bool In(this string toLookFor, string[] arr)
        {
            if (Array.IndexOf(arr, toLookFor) >= 0) return true;
            else return false;
        }
    }
}
