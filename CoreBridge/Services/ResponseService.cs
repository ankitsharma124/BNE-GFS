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
        public ResponseService(IHostEnvironment env, IConfiguration config)
        {
            _env = env;
            _config = config;
        }

        public int ResultOK { get { return RESULT_OK; } }
        public int ResultNG { get { return RESULT_NG; } }

        public async Task ReturnBNErrorAsync(int apiCode, HttpResponse response, int statusCode)
        {
            response.StatusCode = statusCode;
            await ReturnBNResponseAsync(apiCode, response, new object[] { statusCode }, null, null,
                ResultNG, statusCode);
        }

        public async Task ReturnBNResponseAsync(int apiCode, HttpResponse response, object details,
            Action<List<object>> fxCustomizeHeader = null, Func<object, object> fxCustomizeContent = null,
            int result = -1, int status = -1)
        {
            if (result < 0) result = ResultOK;
            if (status < 0) status = (int)BNException.BNErrorCode.OK;

            var customHeader = new List<object> {
                new{ result = result},
                new{ date = DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss")}
            };

            //継承クラスにおいて必要があればヘッダーをカスタマイズ
            if (fxCustomizeHeader != null) fxCustomizeHeader(customHeader);

            status = GetApiStatus(apiCode, status);

            if (details.GetType().IsArray)
            {
                ((object[])details)[0] = status;
            }
            else
            {
                details = new object[] { status };
            }

            var responseContent = new object[] { customHeader.ToArray(), details };
            object responseContentConverted;


            response.Headers.Add("charset", "utf-8");
            response.Headers.Add("Access-Control-Allow-Origin", "*");

            if (!_env.IsProduction())
            {

                if (GetUseJson())
                {
                    response.Headers.Add("Content-Type", "application/json");
                    var resSerialized = JsonSerializer.Serialize(responseContent);
                    if (fxCustomizeContent != null) fxCustomizeContent(resSerialized);
                    response.Headers.ContentLength = resSerialized.Length;
                    await response.WriteAsync(resSerialized);
                    return;
                }
            }

            responseContentConverted = MessagePackSerializer.Serialize(responseContent);
            if (fxCustomizeContent != null) responseContentConverted = fxCustomizeContent(responseContentConverted);
            response.Headers.ContentType = "application/x-messagepack";
            response.Headers.ContentLength = ((byte[])responseContentConverted).Length;
            await response.Body.WriteAsync((byte[])responseContentConverted);
        }
        protected int GetApiStatus(int apiCode, int status)
        {
            if (status < (int)BNException.BNErrorCode.ParamExists)
            {
                return status;
            }

            return Convert.ToInt32(apiCode.ToString("0000") + status.ToString("0000"));
        }

        public bool GetUseJson()
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
            return (bool)_useJson;
        }

    }
}
