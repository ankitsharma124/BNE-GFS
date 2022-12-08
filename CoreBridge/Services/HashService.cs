using System.Security.Cryptography;
using System.Text;
using CoreBridge.Services.Interfaces;

namespace CoreBridge.Services
{
    public class HashService : IHashService
    {
        public byte[] GetHashWithKey(string hashKey, string body)
        {
            return GetHashWithKey(hashKey, new UTF8Encoding().GetBytes(body));
        }
        public byte[] GetHashWithKey(string hashKey, byte[] body)
        {
            var key = new UTF8Encoding().GetBytes(hashKey);
            var bytes = new List<byte>(body);
            bytes.AddRange(key);
            return MD5.Create().ComputeHash(bytes.ToArray());
        }

    }
}
