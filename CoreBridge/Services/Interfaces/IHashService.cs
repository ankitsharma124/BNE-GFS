namespace CoreBridge.Services.Interfaces
{
    public interface IHashService
    {
        byte[] GetHashWithKey(string hashKey, byte[] body);
        byte[] GetHashWithKey(string hashKey, string body);
    }
}