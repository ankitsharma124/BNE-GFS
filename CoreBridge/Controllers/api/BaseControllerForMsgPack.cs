using Ardalis.Specification;
using CoreBridge.Models;
using CoreBridge.Models.DTO.Requests;
using CoreBridge.Models.Entity;
using CoreBridge.Models.Exceptions;
using CoreBridge.Models.Extensions;
using CoreBridge.Services.Interfaces;
using Google.Api;
using Google.Api.Gax;
using Google.Apis.Http;
using Google.Apis.Util;
using Google.Type;
using Hangfire.Storage.Monitoring;
using MessagePack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System;
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
    public abstract class BaseControllerForMsgPack : ControllerBase, IDisposable
    {
        protected readonly IResponseService _responseService;
        protected readonly IConfiguration _configService;
        protected readonly ILoggerService _loggerService;
        protected readonly IDistributedCache _cache;
        protected IHostEnvironment _env;
        protected const string UserInfoKeyFormat = "userInfo_key.{0}";

        public BaseControllerForMsgPack(IHostEnvironment env, IResponseService responseService, IDistributedCache cache,
            IConfiguration configService, ILoggerService loggerService)
        {
            _env = env;
            _responseService = responseService;
            _configService = configService;
            _loggerService = loggerService;
            _cache = cache;

        }


        #region Properties
        //仕様書のエラーコード表にあわせて、各アクションがコールされた時点でsetすべし
        //共通機能内でのエラー発生時に使用
        protected int CurrActionId { get; set; } = 0;
        public int Platform { get; set; } = 0;
        public string RequestClassName { get; set; }
        public SysConsts.SkuType SkuType { get; set; }
        public string? Session { get; set; } = null;
        public string TitleCode { get; set; }

        public string? UserId { get; set; } = null;

        //todo: type?
        public object UserInfo { get; set; } = null;

        public TitleInfo TitleInfo { get; set; }

        //各アクションでParameter bindingで受け取った後、セットすべし
        private ReqBase _reqParam;
        public ReqBase ReqParam
        {
            get { return _reqParam; }
            set { _reqParam = value; }
        }

        private ReqBase _reqHeader;
        public ReqBase ReqHeader
        {
            get { return _reqHeader; }
            set { _reqHeader = value; }
        }

        #endregion
        [NonAction]
        public void SetParams(ReqBase reqHeader, ReqBase reqParam)
        {
            _reqHeader = reqHeader;
            _reqParam = reqParam;
            Init();
        }

        private void Init()
        {
            //todo? 
            //$this->start_time = microtime(true);
            //設定値の読み込み
            //$this->contents['base_url'] = $this->config->base_url();


            CheckUri();
            ProcessParams();
            CheckPlatform();
            SessionCheck();
            SessionUpdate();
            CheckUserConsistency();

            // メンテナンス中にAPIを操作できるユーザーであるかを判断する処理
            // 実行APIがリストに存在する場合には処理をスキップ

            if (ReqParam.IsOrDescendantOf(typeof(ReqBaseParam))
                && (bool)((ReqBaseParam)ReqParam).ApiSetting["code"] == true)
            {
                if (JudgeMaintenance(this.UserId, this.Platform, true))
                {
                    throw new BNException(CurrActionId, BNException.BNErrorCode.Maintenance);
                }
            }

            // データのバリデーションとプロパティへの設定
            ReqParam.Validate();
        }

        protected virtual void ProcessParams()
        {
            //パラメーターの処理自体は各APIアクションの入り口で行う
            ProcessHeader();

            /* todo:Assign this.TitleInfo
             * 
             Check title_cd exists vs db.Titles

		        // タイトルマスタ情報の取得
		        $this->title_info = $this->Com_model->get_title_master($this->title_cd);
		        if (empty($this->title_info)) {
			        log_message('error', '不正なタイトルコードが指定されました。title_cd[' . $this->title_cd . ']');
			        $this->response(null, Com_define::RESULT_NG, Com_define::E_TITLE_CD_INVALID);
			        return;
		        }
             * 
             */

            CheckApi();

            // 体験版の場合、体験版タイトルコードが設定されていない場合エラーにする
            //Base_Controller.php:196
            if (this.SkuType == SysConsts.SkuType.Trial)//todo: && TitleInfo.TrialTitleCd is empty
            {
                throw new BNException(CurrActionId, BNException.BNErrorCode.TitleCodeInvalid,
                    $"体験版タイトルコード未登録[{this.TitleCode}]");
            }

        }

        protected abstract void ProcessHeader();
        [NonAction]
        public bool JudgeMaintenance(string? userId, int platform, bool isCommon = false)
        {
            var permissionFlag = false;
            if (isCommon)
            {
                if (JudgeTitle(platform, (int)SysConsts.ModeType.AllKey))
                {
                    permissionFlag = true;
                }
            }
            else
            {
                if (JudgeTitle(platform, (int)SysConsts.ModeType.AllKey))
                {
                    permissionFlag = true;
                }
                //todo: wtf????
                //BaseController 339-344

                if (JudgeTitle(platform, (int)SysConsts.ModeType.IndividualKey))
                {
                    permissionFlag = true;
                }
            }
            // API個別のメンテナンス確認
            //api_result = $this->__request_api_judgment($platform);
            if (JudgeReqApi(platform))
            {
                permissionFlag = true;
            }

            // メンテナンス中アクセス許可ユーザー登録情報確認
            if (permissionFlag && JudgeUser(userId))
            {
                permissionFlag = false;
            }

            return permissionFlag;
        }

        [NonAction]
        private bool JudgeUser(string? userId)
        {
            object code = null;
            var scs = _cache.TryGetValue(SysConsts.SYSTEM_MAINTENANCE_KEY + ":" + userId,
                out code);

            if (scs && code != null)
            {
                return true;
            }
            return false;
        }

        private bool JudgeReqApi(int platform)
        {
            var maintenanceKey = Request.Path;
            object code = null;
            var scs = _cache.TryGetValue(SysConsts.SYSTEM_MAINTENANCE_KEY + ":" + maintenanceKey + ":" + platform,
                out code);

            if (scs && code != null)
            {
                return true;
            }
            return false;
        }

        private bool JudgeTitle(int platform, int modeKey)
        {
            int getMode;
            //todo: Q6
            //check where cache is being read in from the src
            var success = _cache.TryGetValue<int>(SysConsts.SYSTEM_MAINTENANCE_KEY + ":" + platform, out getMode);
            if (success && getMode == modeKey) return true;
            return false;
        }

        protected void CheckApi()
        {
            if (Url.IsLocalUrl(Request.Headers.Referer))
            {
                return;
            }

            var list = new List<string>();//todo: Title.APIList_Jsonの型確認
                                          //$no_api_list = array_keys(array_filter($this->title_info['api_list'], function($t) { return $t === 0; }));

            // 該当APIが、登録されている使用不可のAPIの一部がヒットする場合、使用不可のAPI
            foreach (var item in list)
            {
                if (item.Contains(Request.Path))
                {
                    throw new Manual404($"不許可APIが要求されました。ApiName[{Request.Path}]");
                }
            }

        }

        //todo: check - TEMPORARY_CREDENTIAL_API_LISTの一覧とは？
        protected void CheckUserConsistency()
        {
            //log_message('debug', 'ユーザーの整合性チェック');
            if (ReqParam.IsOrDescendantOf(typeof(ReqBaseParamHeader))
                && ((ReqBaseParamHeader)ReqParam).HasTemporaryCredential())
            {
                //log_message('debug', 'ユーザーの整合性チェックをパス');
                return;
            }

            if (this.UserId == null)
            {
                throw new BNException(CurrActionId, BNException.BNErrorCode.UserNotExist, "user_consistence:パラメータにユーザーIDが存在しません");
            }

            var titleCode = this.SkuType == SysConsts.SkuType.Product ? this.TitleCode :
                //todo: $this->title_info['trial_title_cd']
                this.TitleInfo.ToString();

            // ユーザ情報の取得
            //$result = $this->redislib->get($this->_get_user_info_key());
            //var user;
            //var success = _cache.TryGetValue<User>(GetUserInfoKey(), out user);
            //if (user == null) 
            {
                //log_message('debug', 'user_infoをSpannerから取得');
                //todo: user = UserRepository.Get(this.UserId);
            }

            //skipped: redisのttlを延長 - 
            //$this->redislib->save($this->_get_user_info_key(), json_encode($this->user_info), Com_define::SQL_CACHE_TIME);

            if (this.UserInfo == null)
            {
                throw new BNException(CurrActionId, BNException.BNErrorCode.UserNotExist, "user_consistence:error");
            }

            //todo:
            /*
             * if(this.UserInfo.PType !== this.Platform) - should be int
             * throw new BNException(CurrActionId, BNException.BNErrorCode.RequestErr, "user_consistence:ユーザー情報とプラットフォームが相違している");
             
             // ここに来ることはないはず(SKU種別が変わった場合はセッションID側でエラーにしてる)
                if ($this->user_info['title_cd'] !== $title_cd) {
                    throw new BNException(CurrActionId, BNException.BNErrorCode.UserNotExist, "user_consistence:ユーザー情報とタイトルコードが相違している");
                }
             */
        }

        /// <summary>
        /// uriチェック - configで指定した禁止APIの要求を無効化(エラー)とする
        /// </summary>
        protected void CheckUri()
        {
            var list = _configService.GetValue<string[]>("ProhibitedApiList");
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
            var list = _configService.GetValue<Dictionary<string, int>>("PlatformlessApiList");
            if (!this.Platform.IsNumber())//todo: check - Platform assigned anywhere else?
            {
                if (!Platform.ToString().In(list.Keys.ToArray()))
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

        protected int GetApiStatus(int status)
        {
            if (status < (int)BNException.BNErrorCode.ParamExists)
            {
                return status;
            }

            return Convert.ToInt32(CurrActionId.ToString("0000") + status.ToString("0000"));
        }


        /**
         * 
         * todo:
         * 
        * URLからタイトルコードを取得する
        *
        * @return string
        
           protected function _get_title_cd_by_url(): string
       {
           $protocol = $this->config->item('uri_protocol');
           empty($protocol) && $protocol = 'REQUEST_URI';

           $uris = explode('/', rtrim(str_replace($this->uri->uri_string(), '', $this->input->server($protocol)), '/'));
           $title_cd = end($uris);
           log_message('debug', "title_cd by url title_cd[{$title_cd}]");
           return $title_cd;
       }
           */


        protected string GetTitleCodeByUrl()
        {
            //todo:　uri_protocolの他の設定値の可能性は？　Q7
            //$protocol = $this->config->item('uri_protocol');
            //empty($protocol) && $protocol = 'REQUEST_URI';

            return Request.Path.ToString().Split("/").Last();
        }

        protected abstract string GetSessionKey();

        protected abstract string GetUserInfoKey();

        protected abstract void SessionCheck();

        protected abstract void SessionUpdate();


        protected void CustomizeResponseInnerHeader(List<object> customHeader)
        {
            //必要に応じて継承クラスでoverride
            //php BaseControllerの_response_custom_headerに相当
        }

        protected void CustomizeResponseContent(object response)
        {
            //必要に応じて継承クラスでoverride
            //php BaseControllerの_response_custom_dataに相当
        }


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
                CustomizeResponseContent, GetApiStatus, result, status);
        }
        [NonAction]
        public void Dispose()
        {
            //todo: transactions?
        }

    }
}
