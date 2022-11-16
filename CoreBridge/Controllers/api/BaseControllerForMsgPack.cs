using Ardalis.Specification;
using CoreBridge.Models;
using CoreBridge.Models.Exceptions;
using CoreBridge.Models.Extensions;
using CoreBridge.Services.Interfaces;
using Google.Api;
using Google.Api.Gax;
using Google.Type;
using MessagePack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using static Google.Rpc.Context.AttributeContext.Types;
using DateTime = System.DateTime;

namespace CoreBridge.Controllers.api
{
    public class BaseControllerForMsgPack : ControllerBase
    {
        private readonly IResponseService _responseService;
        private readonly IConfigService _configService;
        private readonly ILoggerService _loggerService;
        protected IHostEnvironment _env;



        public BaseControllerForMsgPack(IHostEnvironment env, IResponseService responseService,
            IConfigService configService, ILoggerService loggerService)
        {
            _env = env;
            _responseService = responseService;
            _configService = configService;
            _loggerService = loggerService;
            Init();
        }

        private void Init()
        {
            //todo? 
            //$this->start_time = microtime(true);
            //設定値の読み込み
            //$this->contents['base_url'] = $this->config->base_url();

            // リクエストレスポンスクラスのロード
            this.RequestClass = "Req" + Request.Path.ToString().Replace("/", "");

            CheckUri();
            CheckPlatform();
            SessionCheck();
            SessionUpdate();


        }

        //仕様書のエラーコード表にあわせて、各アクションがコールされた時点でsetすべし
        //共通機能内でのエラー発生時に使用
        protected int CurrActionId { get; set; } = 0;
        public object Platform { get; set; } = null;
        public string RequestClass { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="details">基本、object[]で送り込む方向, details[0]はstatusCode</param>
        /// <param name="result"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        protected async Task ReturnBNResponse(object details, int result = -1, int status = -1)
        {
            await _responseService.ReturnBNResponseAsync(Response, details, CustomizeResponseInnerHeader,
                CustomizeResponseContent, result, status);
        }



        private void CheckUserConsistency()
        {
            //todo: 
            //TitleCodeとか色々。

            //TEMPORARY_CREDENTIAL_API_LISTの一覧とは？
            //if ($this->request_param->temporary_credential()) {
            //RequestParameterにtemporary_credentialが含められている？

        }

        /// <summary>
        /// uriチェック - configで指定した禁止APIの要求を無効化(エラー)とする
        /// </summary>
        protected void CheckUri()
        {
            var list = _configService.GetConfigVal<string[]>("CheckLists", "ProhibitedApiList");
            if (list != null && list.Length > 0)
            {
                var path = Request.Path;
                if (path.ToString().In(list))
                {
                    throw new BNException(CurrActionId, BNException.BNErrorCode.RequestErr,
                        $"不許可APIが要求されました。ApiName[{path}]");
                }
            }


        }

        /// <summary>
        /// platformチェック
        /// </summary>
        protected void CheckPlatform()
        {
            var list = _configService.GetConfigVal<Dictionary<string, int>>("CheckLists", "PlatformlessApiList");
            if (!this.Platform.IsNumber())
            {
                if (Platform.ToString().In(list.Keys.ToArray()))
                {
                    throw new BNException(CurrActionId, BNException.BNErrorCode.PlatformTypeInvalid,
                        $"不正なプラットフォームが指定されました。platform[{Platform}]");
                }
                else
                {
                    Platform = list[Request.Path];
                }
            }
            else
            {
                Platform = (int)Platform;
            }
        }

        protected void SessionCheck()
        {
            throw new Exception("継承してください");
        }

        protected void SessionUpdate()
        {
            throw new Exception("継承してください");
        }


        protected void CustomizeResponseInnerHeader(object[] customHeader)
        {
            //必要に応じて継承クラスでoverride
            //php BaseControllerの_response_custom_headerに相当
        }

        protected void CustomizeResponseContent(object response)
        {
            //必要に応じて継承クラスでoverride
            //php BaseControllerの_response_custom_dataに相当
        }

    }
}
