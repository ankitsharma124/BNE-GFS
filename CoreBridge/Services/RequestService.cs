using CoreBridge.Models.Extensions;
using CoreBridge.Models;
using Microsoft.AspNetCore.Http;
using CoreBridge.Models.Exceptions;
using CoreBridge.Services.Interfaces;
using System.Net.Http;
using System.Text;
using MessagePack;
using CoreBridge.Models.Entity;
using System.Security.Policy;
using System.Reflection.Emit;
using CoreBridge.Models.DTO.Requests;
using Microsoft.AspNetCore.Mvc;

namespace CoreBridge.Services
{
    public class RequestService : IRequestService
    {
        private readonly IConfiguration _config;
        private readonly ISessionStatusService _sss;
        private readonly ILogger _logger;
        private readonly IUrlHelper _url;
        private readonly ISessionService _session;
        private readonly IUserService _user;
        private readonly IMaintenanceService _maintenace;
        private readonly IHashService _hash;
        public RequestService(IConfiguration config, ISessionStatusService sss,
            ISessionService session, IUserService user, IMaintenanceService maintenace, IHashService hash,
            IUrlHelper url, ILogger logger)
        {
            _config = config;
            _sss = sss;
            _logger = logger;
            _url = url;
            _session = session;
            _user = user;
            _maintenace = maintenace;
            _hash = hash;
        }

        public async Task ProcessRequest(HttpRequest req, ReqBase reqHeader, ReqBase reqParam)
        {
            _sss.ReqHeader = reqHeader;
            _sss.ReqParam = reqParam;
            if (_sss.ReqParam == null)
            {
                throw new BNException(_sss.ApiCode, BNException.BNErrorCode.RequestErr,
                    "post_param:empty");
            }

            if (!_sss.IsBnIdApi)
            {
                _sss.CopyParamHeader();
                CheckUri(req);
#if DEBUG
                var agent = req.Headers.UserAgent;
                _logger.LogInformation($"HttpHeader[ User-Agent: {agent} ]");
#endif
                await _sss.LoadTitleInfo(_sss.TitleCode);
                if (!_url.IsLocalUrl(req.Headers.Referer))
                {
                    CheckApi(req);
                }
                if (_sss.IsServerApi)
                {
                    _hash.CheckTitleHasHashKey();
                    _hash.CheckHash();
                }

                CheckTrialTitleCode();
            }
            CheckPlatform(req);
            if (_sss.IsClientApi)
            {
                _session.SessionCheck(); //clientOnly
                _session.SessionUpdate(); //clientOnly
            }
            if (!_sss.IsBnIdApi)
            {
                await _user.CheckUserConsistency();
            }
            await _maintenace.CheckMaintenanceStatus(true);

            _sss.ReqParam.Validate();
        }

        private void CheckUri(HttpRequest req)
        {
            var list = _config.GetValue<string[]>("ProhibitedApiList");
            if (list != null && list.Length > 0)
            {
                var path = req.Path;
                if (path.ToString().In(list))
                {
                    throw new BNException(_sss.ApiCode, BNException.BNErrorCode.RequestErr,
                        $"不許可APIが要求されました。ApiName[{path}]");
                }
            }
        }

        private void CheckApi(HttpRequest req)
        {
            var list = new List<string>();
            var dic = ((Dictionary<string, string>[])_sss.TitleInfo.ApiList).First();
            var keys = dic.Keys.ToArray();
            foreach (var key in keys)
            {
                if (req.Path.ToString().Contains(key))
                {
                    _logger.LogError($"使用不可のAPIが要求されました[title_cd:{_sss.TitleCode} ], " +
                        $"ApiName:[{req.Path.ToString()}], HitUrl:{key}");
                    throw new Manual404("");
                }
            }
        }

        private void CheckTrialTitleCode()
        {
            if (_sss.SkuType == (int)SysConsts.SkuType.Trial &&
                _sss.TitleInfo.TrialTitleCode == "")
            {
                throw new BNException(_sss.ApiCode, BNException.BNErrorCode.TitleCodeInvalid,
                    $"体験版タイトルコード未登録[{_sss.TitleCode}]");
            }
        }

        private void CheckPlatform(HttpRequest req)
        {
            var list = _config.GetValue<Dictionary<string, int>>("PlatformlessApiList");

            if (req.Path.ToString().In(list.Keys.ToArray()))
            {
                throw new BNException(_sss.ApiCode, BNException.BNErrorCode.PlatformTypeInvalid,
                    $"不正なプラットフォームが指定されました。platform[{_sss.Platform}]");
            }
            else
            {
                _sss.Platform = list[req.Path.ToString()];
            }
        }

    }
}
