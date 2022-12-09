using CoreBridge.Models;
using CoreBridge.Models.DTO.Requests;
using CoreBridge.Models.Exceptions;
using CoreBridge.Services.Interfaces;
using MessagePack;
using System.Reflection.Emit;
using System.Text.Json;
using static Google.Rpc.Context.AttributeContext.Types;

namespace CoreBridge.Services
{
    public class ResponseService : IResponseService
    {
        private bool? _useJson = null;

        private const int RESULT_NG = 1;
        private const int RESULT_OK = 0;

        private Action<object[]> customizeResponseInnerHeader;
        private Action<object> customizeResponseContent;

        protected IHostEnvironment _env;
        protected IConfiguration _config;
        private readonly ISessionStatusService _sss;
        private readonly IHashService _hash;
        private readonly ILogger _logger;
        public ResponseService(IHostEnvironment env, IConfiguration config,
            ISessionStatusService sss, IHashService hash, ILogger logger)
        {
            _env = env;
            _config = config;
            _sss = sss;
            _hash = hash;
            _logger = logger;
        }

        public int ResultOK { get { return RESULT_OK; } }
        public int ResultNG { get { return RESULT_NG; } }

        public async Task ReturnBNErrorAsync(HttpResponse response, int statusCode)
        {
            response.StatusCode = statusCode;
            await ReturnBNResponseAsync(response, new object[] { statusCode }, ResultNG, statusCode);
        }

        public async Task ReturnBNResponseAsync(HttpResponse response, object details,
            int result = -1, int status = -1)
        {
            if (result < 0) result = ResultOK;
            if (status < 0) status = (int)BNException.BNErrorCode.OK;


            if (_sss.IsBnIdApi)
            {
                //ReturnHtmlResponse();
                return;
            }

            var innerHeader = new List<object> {
                new{ result = result},
                new{ date = DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss")}
            };

            if (_sss.IsClientApi)
            {
                CustomizeResponseInnerHeader(innerHeader);
            }
            status = GetApiStatus(status);

            if (details.GetType().IsArray)
            {
                ((object[])details)[0] = status;
            }
            else
            {
                details = new object[] { status };
            }

            var responseContent = new object[] { innerHeader.ToArray(), details };
            byte[] responseContentConverted;


            response.Headers.Add("charset", "utf-8");
            response.Headers.Add("Access-Control-Allow-Origin", "*");

            if (_sss.UseJson)
            {
                response.Headers.Add("Content-Type", "application/json");
                var resSerialized = JsonSerializer.Serialize(responseContent);
                if (_sss.IsServerApi)//add hash
                {
                    var hasStr = System.Text.Encoding.UTF8.GetString(_hash.GetHashWithKey(_sss.TitleInfo.HashKey, resSerialized));
                    resSerialized = hasStr + resSerialized;
                }
                response.Headers.ContentLength = resSerialized.Length;
                await response.WriteAsync(resSerialized);
                return;
            }
            responseContentConverted = MessagePackSerializer.Serialize(responseContent);
            if (_sss.IsServerApi)//add hash
            {
                var hasStr = _hash.GetHashWithKey(_sss.TitleInfo.HashKey, responseContentConverted);
                var list = new List<byte>(hasStr);
                list.AddRange(responseContentConverted);
                responseContentConverted = list.ToArray();
            }
            response.Headers.ContentType = "application/x-messagepack";
            response.Headers.ContentLength = ((byte[])responseContentConverted).Length;
            await response.Body.WriteAsync((byte[])responseContentConverted);
        }

        protected void CustomizeResponseInnerHeader(List<object> customHeader)
        {
            var clientParam = (ReqBaseClient)_sss.ReqParam;
            if (clientParam.SessionAvoid() != true)
            {
                bool found = false;
                foreach (IDictionary<string, object> item in customHeader)
                {
                    if (item.ContainsKey("session"))
                    {
                        item["session"] = _sss.SessionKey == null ? "" : _sss.SessionKey;
                        found = true;
                    }
                }
                if (!found)
                {
                    customHeader.Add(new { session = (_sss.SessionKey != null) ? _sss.SessionKey : "" });
                }
            }
        }

        protected int GetApiStatus(int status)
        {
            if (status < (int)BNException.BNErrorCode.ParamExists)
            {
                return status;
            }

            return Convert.ToInt32(_sss.ApiCode.ToString("0000") + status.ToString("0000"));
        }

        protected async void ReturnHtmlResponse(HttpResponse response, object details,
            int result, int status)
        {
            var statusCode = GetApiStatus(status);
#if DEBUG
            _logger.LogInformation("Senging response: result: {0}, status:{1}, uri:{2}",
                result, statusCode, _sss.ReqPath);
#endif
            response.Headers.ContentType = "text/html";
            response.Headers.AccessControlAllowOrigin = "*";
            var body = $"<div>server error</div><div>status:{statusCode}</div>";
            await response.WriteAsync(body);
        }


    }
}
