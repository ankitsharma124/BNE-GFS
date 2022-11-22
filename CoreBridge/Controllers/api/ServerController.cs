using CoreBridge.Models.DTO.Requests;
using CoreBridge.Models.Exceptions;
using CoreBridge.Models.Extensions;
using CoreBridge.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Unicode;

namespace CoreBridge.Controllers.api
{
    public class ServerController : ClientServerControllerBase, IDisposable
    {

        private const string Userinfo_ServerKeyFormat = "server.{0}_{1}"; //サーバー情報のキャッシュキー 

        public ServerController(IHostEnvironment env, IResponseService responseService, IDistributedCache cache,
          IConfiguration configService, ILoggerService loggerService) : base(env, responseService, cache, configService, loggerService)
        {
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
            _loggerService.LogDebug("セッションチェック なし");

        }

        protected override void SessionUpdate()
        {
            _loggerService.LogDebug("セッション更新 なし");
        }


        protected new void CustomizeResponseInnerHeader(List<object> customHeader)
        {
            if (ServerParam.SessionAvoid() != true)
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

        protected override void ProcessParams()
        {
            var titleCode = GetTitleCodeByUrl();

            //todo: Get TitleInfo from repository
            //this.TitleInfo = 

            if (titleCode != null)
            {
                //todo: uncomment
                //throw new BNException(CurrActionId, BNException.BNErrorCode.TitleCodeInvalid,
                //$"不正なタイトルコードが指定されました。 url_title_cd[{titleCode}]");
            }

            CheckApi();
            //todo:
            /*
            if (this.TitleInfo.HashKey == null)
            {
                throw new BNException(CurrActionId, BNException.BNErrorCode.TitleCodeInvalid,
                    $"ハッシュキーが未登録です[{titleCode}]");
            }*/
            GetPostParam();


            /*
             
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


        }

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
            var hashKeyEncoded = new UTF8Encoding().GetBytes(hashKey);
            hashKeyEncoded.ToList().AddRange(body);
            return hashKeyEncoded.ToArray();
        }
    }
}
