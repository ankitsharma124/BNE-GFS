using CoreBridge.Models;
using CoreBridge.Models.DTO.Requests;
using CoreBridge.Models.Exceptions;
using CoreBridge.Models.Extensions;
using CoreBridge.Models.Interfaces;
using CoreBridge.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text;
using System.Text.Unicode;

namespace CoreBridge.Controllers.api
{
    public class ServerController : BaseControllerForMsgPack, IDisposable
    {

        private const string Userinfo_ServerKeyFormat = "server.{0}_{1}"; //サーバー情報のキャッシュキー 
        private readonly ITitleInfoService _titleInfoService;

        public ServerController(IHostEnvironment env, IResponseService responseService, IDistributedCache cache,
          IConfiguration configService, ILogger loggerService, ITitleInfoService titleInfoService) : base(env, responseService, cache, configService, loggerService)
        {
            _titleInfoService = titleInfoService;
        }


        private ReqServerHeader ServerHeader
        {
            get
            {
                return (ReqServerHeader)ReqHeader;
            }
        }

        private ReqBaseServer ServerParam
        {
            get
            {
                return (ReqBaseServer)ReqParam;
            }
        }

        #region BaseController class methods

        protected override string GetUserInfoKey()
        {
            return String.Format(UserInfoKeyFormat, String.Format(Userinfo_ServerKeyFormat, this.UserId, (int)this.SkuType));
        }

        protected override void SessionCheck()
        {
            _logger.LogDebug("セッションチェック なし");

        }

        protected override void SessionUpdate()
        {
            _logger.LogDebug("セッション更新 なし");
        }

        /// <summary>
        /// _load_header_param()に相当
        /// </summary>
        protected override void ProcessParams()
        {
            //_load_header_param
            var titleCode = GetTitleCodeByUrl();
            this.TitleInfo = _titleInfoService.GetByCodeAsync(titleCode).Result;

            if (this.TitleInfo == null)
            {
                throw new BNException(CurrActionId, BNException.BNErrorCode.TitleCodeInvalid,
                $"不正なタイトルコードが指定されました。 url_title_cd[{titleCode}]");
            }

            CheckApi();
            if (this.TitleInfo.HashKey == null)
            {
                throw new BNException(CurrActionId, BNException.BNErrorCode.TitleCodeInvalid,
                    $"ハッシュキーが未登録です[{titleCode}]");
            }
            GetPostParam();

            /*todo
             // HTTPヘッダーを出力(ユーザエージェントのみ出力)
		        $this->load->library('user_agent');
		        log_message('info',  "HttpHeader[ User-Agent: {$this->agent->agent_string()} ]" );

		        // 体験版の場合、体験版タイトルコードが設定されていない場合エラーにする
		        if ($this->sku_type === Com_define::SKU_TYPE_TRIAL && empty($this->title_info['trial_title_cd'])) {
			        log_message('error', '体験版タイトルコード未登録[' . $this->title_cd . ']');
			        $this->response(null, Com_define::RESULT_NG, Com_define::E_TITLE_CD_INVALID);
			        return;
		        }

		        // 正しいタイトルコードでキャッシュさせるため、一度QueryCacheライブラリを削除して読み込み直す
		        unset($this->querycache);
		        $this->load->library('QueryCache');

		        // postデータの処理終わった後にロードする
		        // redisの読み込み
		        $this->load->library('RedisLib');
             */

            base.ProcessParams();
        }

        protected override void ProcessHeader()
        {

            this.TitleCode = ServerHeader.TitleCd;
            this.UserId = ServerHeader.UserId;
            this.SkuType = (SysConsts.SkuType)Enum.Parse(typeof(SysConsts.SkuType), ServerHeader.SkuType + "");
            this.Session = ServerHeader.Session;
            this.Platform = (int)ServerHeader.Platform;

        }

        private void GetPostParam()
        {
            if (this.ReqParam == null)
            {
                throw new BNException(CurrActionId, BNException.BNErrorCode.RequestErr, "post_param:empty");
            }

            var hashKey = GetHashKey(GetTitleCodeByUrl());
            if (hashKey != "")
            {
                var hash = System.Text.Encoding.UTF8.GetBytes(Request.Headers["InternalHash"].ToString());
                if (hash != null)
                {
                    Request.Headers.Remove("InternalHash");
                }

                var body = System.Text.Encoding.UTF8.GetBytes(Request.Headers["InternalOriginalBody"].ToString());

                if (!hash.SequenceEqual(GetBodyHash(hashKey, body)))
                {
                    throw new BNException(CurrActionId, BNException.BNErrorCode.RequestErr,
                        $"hash error[{Request.Headers["InternalHash"]}]");
                }
            }

            if (this.ServerHeader.TitleCd != GetTitleCodeByUrl())
            {
                throw new BNException(CurrActionId, BNException.BNErrorCode.RequestErr,
                        $"post_param:title_cd error header[{this.ServerHeader.TitleCd}], url[{GetTitleCodeByUrl()}]");
            }


        }

        protected new void CustomizeResponseInnerHeader(List<object> customHeader)
        {
            //if (ServerParam.SessionAvoid() != true)
            {
                bool found = false;

                foreach (IDictionary<string, object> item in customHeader)
                {
                    if (item.ContainsKey("session"))
                    {
                        item["session"] = this.Session != null ? this.Session : "";
                        found = true;
                    }

                }
                if (!found)
                {
                    customHeader.Add(new { session = (this.Session != null) ? this.Session : "" });
                }

            }
        }
        #endregion





        protected new object CustomizeResponseContent(object response)
        {
            var keyStr = GetHashKey(GetTitleCodeByUrl());
            if (keyStr != null && keyStr != "")
            {
                return GetBodyHash(keyStr, (byte[])response);
            }
            return response;
        }

        private string GetHashKey(string titleCode)
        {
            if (Url.IsLocalUrl(Request.Headers.Referer))
            {
                return "TEST111111111111";
            }
            //todo:
            //return TitleInfo.HasKey ?? "";
            return "";
        }

        private byte[] GetBodyHash(string hashKey, byte[] body)
        {
            var key = new UTF8Encoding().GetBytes(hashKey);
            var bytes = new List<byte>(body);
            bytes.AddRange(key);
            return MD5.Create().ComputeHash(bytes.ToArray());
        }

        protected override string GetSessionKey()
        {
            return String.Format("", this.UserId);
        }
    }
}
