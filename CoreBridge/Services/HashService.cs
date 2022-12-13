using System.Collections;
using System.Security.Cryptography;
using System.Text;
using CoreBridge.Models.Exceptions;
using CoreBridge.Services.Interfaces;
using MessagePack.Formatters;

namespace CoreBridge.Services
{
    public class HashService : IHashService
    {
        private readonly ISessionStatusService _sss;
        public HashService(ISessionStatusService sss)
        { _sss = sss; }


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

        public void CheckTitleHasHashKey()
        {
            if (_sss.TitleInfo.HashKey == null || _sss.TitleInfo.HashKey == "")
            {
                throw new BNException(_sss.ApiCode, BNException.BNErrorCode.TitleCodeInvalid,
                   $"ハッシュキーが未登録です[{_sss.TitleCode}]");
            }
        }

        public void CheckHash()
        {
            var submittedHash = _sss.RequestHash;
            byte[] calculatedHash = _sss.UseJson ? GetHashWithKey(_sss.TitleInfo.HashKey, (string)_sss.RequestBody) :
                GetHashWithKey(_sss.TitleInfo.HashKey, (byte[])_sss.RequestBody);
            if (!Enumerable.SequenceEqual(submittedHash, calculatedHash))
            {
                throw new BNException(_sss.ApiCode, BNException.BNErrorCode.RequestErr,
                  $"hash error ({System.Text.Encoding.UTF8.GetString(submittedHash)})");
            }
        }
    }
}
