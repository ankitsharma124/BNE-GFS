using CoreBridge.Models.Extensions;
using CoreBridge.Models;
using Microsoft.AspNetCore.Http;
using CoreBridge.Models.Exceptions;
using CoreBridge.Services.Interfaces;
using System.Net.Http;

namespace CoreBridge.Services
{
    public class RequestService : IRequestService
    {
        private readonly IConfiguration _config;
        public RequestService(IConfiguration config)
        {
            _config = config;
        }

        public async Task RemoveHash(HttpRequest req)
        {
            if (GetUseJson() == true)
            {
                await CopyJsonBodyToHeader(req, SysConsts.AddedReqHeaderKey_OriginalBody, true);
            }
            else
            {
                await CopyMsgPackBodyToHeader(req, SysConsts.AddedReqHeaderKey_OriginalBody, true);
            }
        }

        #region read from body => header (bytes to string)

        /// <summary>
        /// HttpRequest.BodyをHeader[SysConsts.AddedReqHeaderKey_OriginalBody]にコピー
        /// (ServerControllerで行うhashチェックのために必要
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task CopyOriginalBodyToHeader(HttpRequest req)
        {
            if (GetUseJson() == true)
            {
                await CopyJsonBodyToHeader(req, SysConsts.AddedReqHeaderKey_OriginalBody);
            }
            else
            {
                await CopyMsgPackBodyToHeader(req, SysConsts.AddedReqHeaderKey_OriginalBody);
            }

        }

#if DEBUG
        /// <summary>
        /// Debug環境のみ。オリジナルのRequest.BodyをHeaderにコピー。
        /// BaseController.CollecthttpParam()で使用
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task Debug_CopyBodyToHeader(HttpRequest req)
        {
            if (GetUseJson() == true)
            {
                await CopyJsonBodyToHeader(req, SysConsts.AddedReqHeaderKey_Debug_ReqBody);
            }
            else
            {
                await CopyMsgPackBodyToHeader(req, SysConsts.AddedReqHeaderKey_Debug_ReqBody);
            }


        }
#endif

        #region helpers
        /// <summary>
        /// Jsonで書かれたHttpRequest.Bodyを指定のHeaderフィールドにコピー
        /// Bodyを読む時はこのサービスのGet系method使用のこと
        /// </summary>
        /// <param name="req"></param>
        /// <param name="bodyKey"></param>
        /// <param name="removeHash"></param>
        /// <returns></returns>
        private async Task CopyJsonBodyToHeader(HttpRequest req, string bodyKey,
            bool removeHash = false)
        {
            req.EnableBuffering();

            using (var streamReader = new System.IO.StreamReader(req.Body, System.Text.Encoding.UTF8))
            {
                var body = await streamReader.ReadToEndAsync();
                req.Headers.Remove(bodyKey);
                req.Headers.Add(bodyKey, body);
                req.Body.Position = 0;
            }


        }


        /// <summary>
        /// MsgPackで書かれたHttpRequest.Bodyを指定のHeaderフィールドにコピー
        /// Bodyを読む時はこのサービスのGet系method使用のこと
        /// </summary>
        /// <param name="req"></param>
        /// <param name="bodyKey"></param>
        /// <param name="removeHash"></param>
        /// <returns></returns>
        private async Task CopyMsgPackBodyToHeader(HttpRequest req, string bodyKey,
            bool removeHash = false)
        {
            byte[] originalContent;
            using (StreamReader stream = new StreamReader(req.Body))
            {
                var ms = new MemoryStream();
                await stream.BaseStream.CopyToAsync(ms);
                originalContent = ms.ToArray();
            }

            if (removeHash)
            {
                var hash = originalContent.Take(16).ToArray();
                req.Headers.Remove(SysConsts.AddedReqHeaderKey_Hash);
                req.Headers.Add(SysConsts.AddedReqHeaderKey_Hash, GetStringFromByteArray(hash));
                originalContent = originalContent.Skip(16).ToArray();
            }

            var strMsgPackBytes = GetStringFromByteArray(originalContent);
            req.Body = new MemoryStream(originalContent);

            req.Headers.Remove(bodyKey);
            req.Headers.Add(bodyKey, strMsgPackBytes);

            //msgPackの長さが奇数バイトである場合、末尾に値0のバイトが足される。
            //それを後に取り除く時のフラグとしてオリジナルbodyの長さを記録
            req.Headers.Remove(SysConsts.AddedReqHeaderKey_ReqBodyLen);
            req.Headers.Add(SysConsts.AddedReqHeaderKey_ReqBodyLen, originalContent.Length + "");

        }


        private string GetStringFromByteArray(byte[] bytes)
        {
            var len = (bytes.Length % sizeof(char) == 0) ? bytes.Length / sizeof(char)
                : bytes.Length / sizeof(char) + 1;
            char[] chars = new char[len];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
        #endregion
        #endregion

        #region Get from header string=>bytes
        public byte[] GetBodyHashInBytesFromHeader(HttpRequest req)
        {
            if (!req.Headers.ContainsKey(SysConsts.AddedReqHeaderKey_Hash))
            {
                throw new Exception("Hash not set in the header");
            }

            return GetByteArrayFromString(req.Headers[SysConsts.AddedReqHeaderKey_Hash]);

        }

        public byte[] GetOriginalBodyInBytesFromHeader(HttpRequest req)
        {
            if (!req.Headers.ContainsKey(SysConsts.AddedReqHeaderKey_OriginalBody))
            {
                throw new Exception("OriginalBody not set in the header");
            }
            return GetBodyByteArrayFromHeaderCopy(req, SysConsts.AddedReqHeaderKey_OriginalBody);
        }

#if DEBUG
        public byte[] GetDebugBodyCopyInBytesFromHeader(HttpRequest req)
        {
            if (!req.Headers.ContainsKey(SysConsts.AddedReqHeaderKey_Debug_ReqBody))
            {
                throw new Exception("Debug copy of ReqBody not set in the header");
            }

            return GetBodyByteArrayFromHeaderCopy(req, SysConsts.AddedReqHeaderKey_Debug_ReqBody);
        }
#endif
        #region helpers
        private byte[] GetBodyByteArrayFromHeaderCopy(HttpRequest req, string key)
        {
            var bytes = GetByteArrayFromString(req.Headers[key]);
            var len = req.Headers[SysConsts.AddedReqHeaderKey_ReqBodyLen];
            if (len == "") throw new Exception("Body length not set in request header @ ReqService");

            if (Convert.ToInt32(len) != bytes.Length)
            {
                return bytes.Take(bytes.Length - (bytes.Length - Convert.ToInt32(len))).ToArray();
            }
            else return bytes;
        }

        private byte[] GetByteArrayFromString(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
        #endregion
        #endregion


        private bool? _useJson = null;
        public bool? GetUseJson()
        {
            if (_useJson == null)
            {
                try
                {
                    _useJson = _config.GetValue<bool>("UseJson");
                }
                catch (Exception ex)
                {
                    _useJson = false;
                }

            }
            return _useJson;
        }


    }
}
