﻿using CoreBridge.Models.Exceptions;
using CoreBridge.Services.Interfaces;
using MessagePack;
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
        public ResponseService(IHostEnvironment env)
        {
            _env = env;
        }


        public int ResultOK { get { return RESULT_OK; } }
        public int ResultNG { get { return RESULT_NG; } }

        public async Task ReturnBNErrorAsync(HttpResponse response, int statusCode)
        {
            await ReturnBNResponseAsync(response, new object[] { statusCode }, null, null, ResultNG, statusCode);
        }

        public async Task ReturnBNResponseAsync(HttpResponse response, object details,
            Action<object[]> fxCustomizeHeader = null, Action<object> fxCustomizeContent = null, int result = -1, int status = -1)
        {
            if (result < 0) result = ResultOK;
            if (status < 0) status = (int)BNException.BNErrorCode.OK;

            var customHeader = new object[] {
                new{ result = result},
                new{ date = DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss")}
            };

            //継承クラスにおいて必要があればヘッダーをカスタマイズ
            if (fxCustomizeHeader != null) fxCustomizeHeader(customHeader);

            if (details.GetType().IsArray)
            {
                ((object[])details)[0] = status;
            }
            else
            {
                details = new object[] { status };
            }

            var responseContent = new object[] { customHeader, details };
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
            if (fxCustomizeContent != null) fxCustomizeContent(responseContentConverted);
            response.Headers.ContentType = "application/x-messagepack";
            response.Headers.ContentLength = ((byte[])responseContentConverted).Length;
            await response.Body.WriteAsync((byte[])responseContentConverted);
        }


        public bool GetUseJson()
        {
            if (_useJson == null)
            {
                IConfiguration config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.Development.json")
                    .Build();
                try
                {
                    _useJson = (bool)config.GetRequiredSection("DebugConfig")!.GetValue(typeof(bool), "UseJson");
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