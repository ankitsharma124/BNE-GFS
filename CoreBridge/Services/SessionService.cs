using CoreBridge.Models.DTO.Requests;
using CoreBridge.Models.Exceptions;
using CoreBridge.Models.Extensions;
using CoreBridge.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace CoreBridge.Services
{
    public class SessionDataService : ISessionDataService
    {
        private readonly ISessionStatusService _sss;
        private readonly IWebHostEnvironment _env;
        private readonly IDistributedCache _cache;
        public SessionDataService(ISessionStatusService sss, IWebHostEnvironment env, IDistributedCache cache)
        {
            _sss = sss;
            _env = env;
            _cache = cache;
        }

        public void SessionCheck()
        {
            if (_env.IsDevelopment() || !_sss.IsClientApi) return;

            SessionData? session;
            var success = _cache.TryGetValue<SessionData>(GetSessionKey(), out session);

            if (!success || session == null || session.SessionId == null || session.SkuType == null)
            {
                throw new BNException(_sss.ApiCode, BNException.BNErrorCode.SessionTimeout);
            }
            else if (session.SessionId == _sss.Session && session.SkuType == (int)_sss.SkuType)
            {
                //session ok
            }
            else if (session.SessionId != _sss.Session)
            {
                throw new BNException(_sss.ApiCode, BNException.BNErrorCode.SessionNG, "session err");
            }
            else if (session.SkuType != _sss.SkuType)
            {
                throw new BNException(_sss.ApiCode, BNException.BNErrorCode.SessionNG, "sku err");
            }
            else
            {
                throw new BNException(_sss.ApiCode, BNException.BNErrorCode.SessionNG, "check err");
            }
        }

        private string GetSessionKey()
        {
            return $"ses_key.{_sss.UserId}";
        }

        public void SessionUpdate()
        {
            var clientParam = (ReqBaseClient)_sss.ReqParam;
            if (!clientParam.NotSessionUpdate() == true)
            {
                // NOT_SESSION_UPDATE_API_LISTで指定されたAPIではセッションを更新せずレスポンスsessionで空文字を返却する
                _sss.Session = "";
            }
            else if (clientParam.MirroSession() == true)
            {
                // MIRROR_SESSION_API_LISTで指定されたAPIではリクエストのセッションをそのままレスポンスとして返却する
            }
            else
            {
                _sss.Session = DateTime.UtcNow.GenerateUniqId();
                _cache.SetAsync<SessionData>(GetSessionKey(),
                    new SessionData
                    {
                        SessionId = _sss.Session,
                        TitleCode = _sss.TitleCode,
                        SkuType = _sss.SkuType,
                        Platform = _sss.Platform
                    });
            }

        }
    }
}
