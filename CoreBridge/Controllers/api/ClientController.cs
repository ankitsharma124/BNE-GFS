using CoreBridge.Models.DTO.Requests;
using CoreBridge.Models.Exceptions;
using CoreBridge.Models.Extensions;
using CoreBridge.Services.Interfaces;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Dynamic;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;

namespace CoreBridge.Controllers.api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ClientController : ClientServerControllerBase, IDisposable
    {
        protected const string SessionKeyFormat = "ses_key.{0}";
        private const string Userinfo_ClientKeyFormat = "client.{0}";

        private ReqClientHeader CliendHeader
        {
            get
            {
                return (ReqClientHeader)ReqHeader;
            }
        }

        private ReqBaseClient ClientParam
        {
            get
            {
                return (ReqBaseClient)ReqParam;
            }
        }

        public ClientController(IHostEnvironment env, IResponseService responseService, IDistributedCache cache,
           IConfiguration configService, ILoggerService loggerService) : base(env, responseService, cache, configService, loggerService)
        {
        }

        #region BaseController class methods
        protected override string GetUserInfoKey()
        {
            return String.Format(UserInfoKeyFormat, String.Format(Userinfo_ClientKeyFormat, this.UserId));
        }

        protected override string GetSessionKey()
        {
            return String.Format(SessionKeyFormat, this.UserId);
        }
        protected override void SessionCheck()
        {
            if (_env.IsDevelopment()) return; //dev環境のみセッションチェックしない

            SessionData? session;
            _cache.TryGetValue<SessionData>(GetSessionKey(), out session);

            if (session == null//todo: where does Session get set?
                || session.SessionId == null || session.SkuType == null)
            {
                throw new BNException(CurrActionId, BNException.BNErrorCode.SessionTimeout);
            }
            else if (session.SessionId == this.Session && session.SkuType == (int)this.SkuType)
            {
                //session ok
            }
            else if (session.SessionId != this.Session)
            {
                throw new BNException(CurrActionId, BNException.BNErrorCode.SessionNG, "session err");
            }
            else if (session.SkuType != (int)this.SkuType)
            {
                throw new BNException(CurrActionId, BNException.BNErrorCode.SessionNG, "sku err");
            }
            else
            {
                throw new BNException(CurrActionId, BNException.BNErrorCode.SessionNG, "check err");
            }

        }

        protected override void SessionUpdate()
        {
            if (!ClientParam.NotSessionUpdate() == true)
            {
                // NOT_SESSION_UPDATE_API_LISTで指定されたAPIではセッションを更新せずレスポンスsessionで空文字を返却する
                this.Session = "";
            }
            else if (ClientParam.MirroSession() == true)
            {
                // MIRROR_SESSION_API_LISTで指定されたAPIではリクエストのセッションをそのままレスポンスとして返却する
                // 本来何か処理を記述する必要は無いが、IFを書いた理由を明確化するため以下の記述を残す
                this.Session = this.Session;
            }
            else
            {
                this.Session = DateTime.UtcNow.GenerateUniqId();
                _cache.SetAsync<SessionData>(GetSessionKey(),
                    new SessionData
                    {
                        SessionId = this.Session,
                        TitleCode = this.TitleCode,
                        SkuType = (int)this.SkuType,
                        Platform = this.Platform
                    });
            }

        }


        protected new void CustomizeResponseInnerHeader(List<object> customHeader)
        {
            if (ClientParam.SessionAvoid() != true)
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
            // タイトルコードとURLのタイトルコードが違う場合はエラー
            if (CliendHeader.TitleCd != GetTitleCodeByUrl())
                throw new BNException(CurrActionId, BNException.BNErrorCode.RequestErr,
                    $"post_param:title_cd error header[{CliendHeader.TitleCd}] url[{GetTitleCodeByUrl()}]");

            base.ProcessParams();
        }
    }
}
