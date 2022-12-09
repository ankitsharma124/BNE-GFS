using CoreBridge.Models;
using CoreBridge.Models.DTO;
using CoreBridge.Models.DTO.Requests;
using CoreBridge.Models.Entity;
using CoreBridge.Models.Exceptions;
using CoreBridge.Models.Extensions;
using CoreBridge.Services;
using CoreBridge.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using NuGet.Protocol;

namespace CoreBridge.Controllers.api
{
    public abstract class BaseOldController : ControllerBase, IDisposable
    {
        protected readonly IResponseService _responseService;
        protected readonly IConfiguration _configService;
        protected readonly ILogger _logger;
        protected readonly IUserService _userService;
        protected readonly IDistributedCache _cache;
        protected readonly ITitleInfoService _titleInfoService;
        protected IHostEnvironment _env;
#if DEBUG
        protected readonly IRequestService _reqService;
#endif



        protected const string UserInfoKeyFormat = "userInfo_key.{0}";

        public BaseOldController(IHostEnvironment env, IResponseService responseService, IDistributedCache cache,
            IConfiguration configService, ILogger loggerService, ITitleInfoService titleInfoService,
#if DEBUG
            IRequestService reqService,
#endif
            IUserService userService)
        {
            _env = env;
            _responseService = responseService;
            _configService = configService;
            _logger = loggerService;
            _cache = cache;
            _titleInfoService = titleInfoService;
            _userService = userService;
#if DEBUG
            _reqService = reqService;
#endif
        }

        public string? TitleCode { get; set; } = null;
        public string? UserId { get; set; } = null;
        public string? Session { get; set; } = null;
        public int Platform { get; set; } = 0;
        public int? SkuTypeId { get; set; } = null;
        protected int ApiCode { get; set; } = 9999;
        public ReqBase ReqParam { get; set; } = null;
        public ReqBase ReqHeader { get; set; } = null;
        public DateTime StartTime { get; set; }
        public TitleInfoDto TitleInfo { get; set; }

        public GFSUserDto UserInfo { get; set; }

        /// <summary>
        /// 継承クラスにて、これを呼んだら次にModel.IsValid()を呼ぶべし。
        /// </summary>
        /// <param name="reqHeader"></param>
        /// <param name="reqParam"></param>
        /// <returns></returns>
        [NonAction]
        public async Task SetParams(ReqBase reqHeader, ReqBase reqParam)
        {
            ReqHeader = reqHeader;
            ReqParam = reqParam;
            await Init();
        }

        protected async Task Init()
        {

            CheckUri(); //
            ProcessParams(); //
            CheckPlatform();//
            SessionCheck();//
            SessionUpdate();//
            await CheckUserConsistency();//

            // メンテナンス中にAPIを操作できるユーザーであるかを判断する処理
            // 実行APIがリストに存在する場合には処理をスキップ
            if (ReqParam.IsOrDescendantOf(typeof(ReqBaseParam))
                && (bool?)((ReqBaseParam)ReqParam).MaintenanceAvoid == true)
            {
                if (CheckMaintenanceStatus(this.UserId, this.Platform, true))
                {
                    throw new BNException(ApiCode, BNException.BNErrorCode.Maintenance);
                }
            }
        }

        /// <summary>
        /// uriチェック - configで指定した禁止APIの要求を無効化(エラー)とする
        /// </summary>
        protected void CheckUri()
        {
            //requestService.checkApi
        }

        /// <summary>
        /// 受信したパラメーターの処理及びチェック
        /// ClientControllerとその子孫が継承。
        /// Server/BnIdControllerにそれぞれoverride
        /// </summary>
        /// <exception cref="BNException"></exception>
        protected virtual void ProcessParams()
        {
            InitInput(); //see clientApi.InitInput()
#if DEBUG
            //apiDebugservice.LogUserAgent();
#endif
            //sss.loadTitle();

            //CheckApi();
            if (!Url.IsLocalUrl(Request.Headers.Referer))
            {
                RequestService.CheckApi();
            }

            //reqService.CheckTrialTitleCode
            if (this.SkuTypeId == (int)SysConsts.SkuType.Trial &&
                TitleInfo.TrialTitleCode == "")
            {
                throw new BNException(ApiCode, BNException.BNErrorCode.TitleCodeInvalid,
                    $"体験版タイトルコード未登録[{this.TitleCode}]");
            }

        }

        /// <summary>
        /// 不正なPFが指定されていないかのチェック
        /// this is wrong anyway
        /// </summary>
        /// <exception cref="BNException"></exception>
        protected void CheckPlatform()　// RequestService
        {
            var list = _configService.GetValue<Dictionary<string, int>>("PlatformlessApiList");
            if (!Platform.ToString().In(list.Keys.ToArray()))
            {
                throw new BNException(ApiCode, BNException.BNErrorCode.PlatformTypeInvalid,
                    $"不正なプラットフォームが指定されました。platform[{Platform}]");
            }
            else
            {
                Platform = list[Request.Path];
            }
        }

        /// <summary>
        /// 使用不可のAPIにアクセスを試みていないかのチェック
        /// </summary>
        /// <exception cref="Manual404"></exception>
        protected void CheckApi()
        {


            var list = new List<string>();
            var dic = ((Dictionary<string, string>[])TitleInfo.ApiList).First();
            var keys = dic.Keys.ToArray();
            foreach (var key in keys)
            {
                if (Request.Path.ToString().Contains(key))
                {
                    _logger.LogError($"使用不可のAPIが要求されました[title_cd:{TitleCode} ], " +
                        $"ApiName:[{Request.Path.ToString()}], HitUrl:{key}");
                    throw new Manual404("");
                }
            }
        }


        /// <summary>
        /// ヘッダーパラメーターとして送信されてきたUserIdに基づくUser情報と
        /// 同様に送信されたTitleCode, Platformとの整合性を確認
        /// </summary>
        /// <returns></returns>
        /// <exception cref="BNException"></exception>
        protected async Task CheckUserConsistency()
        {
            //todo: check - TEMPORARY_CREDENTIAL_API_LISTの一覧とは？
#if DEBUG
            _logger.LogDebug("debug", "ユーザーの整合性チェック");
#endif
            if (ReqParam.IsOrDescendantOf(typeof(ReqBaseParamHeader)) &&
                ((ReqBaseParamHeader)ReqParam).HasTemporaryCredential())
            {
                _logger.LogDebug("debug", "ユーザーの整合性チェックをスキップ");
                return;
            }

            if (this.UserId == null)
            {
                throw new BNException(ApiCode, BNException.BNErrorCode.UserNotExist,
                    "user_consistence:パラメータにユーザーIDが存在しません");
            }

            var titleCode = this.SkuTypeId == (int)SysConsts.SkuType.Product ? this.TitleCode :
                this.TitleInfo.TrialTitleCode;

            // ユーザ情報の取得

            //UserService.LoadUser();

            if (this.UserInfo == null)
            {
                throw new BNException(ApiCode, BNException.BNErrorCode.UserNotExist, "user_consistence:error");
            }

            if (this.UserInfo.Platform != this.Platform)
                throw new BNException(ApiCode, BNException.BNErrorCode.RequestErr, "user_consistence:ユーザー情報とプラットフォームが相違している");

            // ここに来ることはないはず(SKU種別が変わった場合はセッションID側でエラーにしてる)
            if (UserInfo.TitleCode != this.TitleCode)
                throw new BNException(ApiCode, BNException.BNErrorCode.UserNotExist, "user_consistence:ユーザー情報とタイトルコードが相違している");

        }

        protected abstract void ProcessHeader();

        [NonAction]
        public bool CheckMaintenanceStatus(string? userId, int platform, bool isCommon = false)
        {
            var permissionFlag = false;
            if (isCommon)
            {
                if (CheckTitleMaintenance(platform, (int)SysConsts.ModeType.AllKey))
                {
                    permissionFlag = true;
                }
            }
            else
            {
                if (CheckTitleMaintenance(platform, (int)SysConsts.ModeType.IndividualKey))
                {
                    permissionFlag = true;
                }
            }
            // API個別のメンテナンス確認
            if (CheckReqApiMaintenance(platform))
            {
                permissionFlag = true;
            }

            // メンテナンス中アクセス許可ユーザー登録情報確認
            if (permissionFlag && CheckUserMaintenance(userId))
            {
                permissionFlag = false;
            }

            return permissionFlag;
        }

        [NonAction]
        private bool CheckUserMaintenance(string? userId)
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

        private bool CheckReqApiMaintenance(int platform)
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


        protected async Task ReturnBNResponse(object details, int result = -1, int status = -1)
        {
            await _responseService.ReturnBNResponseAsync(ApiCode, Response, details, CustomizeResponseInnerHeader,
                CustomizeResponseContent, result, status);
        }


        /// <summary>
        /// ClientControllerでoverride
        /// </summary>
        protected virtual void InitInput()
        { }

        protected abstract string GetSessionKey();

        protected abstract string GetUserInfoKey();

        protected abstract void SessionCheck();

        protected abstract void SessionUpdate();

        protected void CustomizeResponseInnerHeader(List<object> customHeader)
        {
            //必要に応じて継承クラスでoverride
            //php BaseControllerの_response_custom_headerに相当
        }

        /// <summary>
        /// 必要に応じて継承クラスでoverride
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        protected object CustomizeResponseContent(object response)
        {
            return new object();
        }

        [NonAction]
        public void Dispose()
        {
#if DEBUG
            _logger.LogInformation("Dispose");

            if (ReqParam.IsOrDescendantOf(typeof(ReqBaseParam))
               && (bool?)((ReqBaseParam)ReqParam).NotCollectParamApi == true)
            {
                _logger.LogDebug($"集計対象外API [{Request.Path}]");
                return;
            }

            var configFlag = _configService.GetValue<bool?>("DebugApiCollect");
            if (configFlag == false)
            {
                return;
            }

            CollectHttpParamForDebug();
        }


        protected void CollectHttpParamForDebug() //done
        {
            var skuType = (int)SysConsts.SkuType.Product;
            skuType = this.SkuTypeId ?? skuType;
            var titleCode = skuType == (int)SysConsts.SkuType.Product ? this.TitleCode :
                this.TitleInfo.TrialTitleCode;






        }
#endif
    }
}

