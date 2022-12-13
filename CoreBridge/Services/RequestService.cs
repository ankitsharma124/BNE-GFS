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
using Google.Cloud.Spanner.V1;
using static CoreBridge.Models.SysConsts;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Reflection.PortableExecutable;
using XAct.Users;

namespace CoreBridge.Services
{
    public class RequestService : IRequestService
    {
        private readonly IConfiguration _config;
        private readonly ISessionStatusService _sss;
        private readonly ILogger<RequestService> _logger;
        private readonly ISessionDataService _session;
        private readonly IUserService _user;
        private readonly IMaintenanceService _maintenace;
        private readonly IHashService _hash;
        private readonly ITitleInfoService _title;
        public RequestService(IConfiguration config, ISessionStatusService sss, ITitleInfoService title,
            ISessionDataService session, IUserService user, IMaintenanceService maintenace, IHashService hash,
            ILogger<RequestService> logger)
        {
            _config = config;
            _sss = sss;
            _logger = logger;
            _session = session;
            _user = user;
            _maintenace = maintenace;
            _hash = hash;
            _title = title;
        }

        public async Task ProcessRequest(HttpRequest req, ReqBase reqHeader, ReqBase reqParam)
        {
            _sss.ReqHeader = reqHeader;
            _sss.ReqParam = reqParam;
            if (_sss.ReqParam == null)
            {
                throw new BNException(_sss.ApiCode, BNException.BNErrorCode.RequestErr, BNException.ErrorLevel.Error,
                    "post_param:empty");
            }

            if (!_sss.IsBnIdApi)
            {
                LoadStatus_ParamHeader();
                CheckUri(req);
#if DEBUG
                var agent = req.Headers.UserAgent;
                _logger.LogInformation($"HttpHeader[ User-Agent: {agent} ]");
#endif
                await _title.LoadStatus_TitleInfo(_sss.TitleCode);
                if (req.Host.Host != "localhost")
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
            var list = _config.GetValue<string[]>("CheckLists:ProhibitedApiList");
            if (list != null && list.Length > 0)
            {
                var path = req.Path;
                if (path.ToString().In(list))
                {
                    throw new BNException(_sss.ApiCode, BNException.BNErrorCode.RequestErr, BNException.ErrorLevel.Error,
                        $"不許可APIが要求されました。ApiName[{path}]");
                }
            }
        }

        private void CheckApi(HttpRequest req)
        {
            var list = new List<string>();
            var dic = ((Dictionary<string, string>[])_sss.TitleInfo.ApiList)?.First();
            if (dic != null)
            {
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
            var list = _config.GetValue<Dictionary<string, int>>("CheckLists:PlatformlessApiList");
            if (_sss.Platform == null && list != null)
            {
                if (!req.Path.ToString().In(list.Keys.ToArray()))
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

        public void LoadStatus_ParamHeader()
        {
            if (!_sss.ReqHeader.IsOrDescendantOf(typeof(ReqBaseClientServerParamHeader)))
            {
                throw new Exception("smtg wrong with the header");
            }

            var header = (ReqBaseClientServerParamHeader)_sss.ReqHeader;
            _sss.TitleCode = header.TitleCode;
            _sss.UserId = header.UserId;
            _sss.SkuType = header.SkuType;
            _sss.Session = header.Session;
            _sss.Platform = header.Platform;
        }

        /// <summary>
        /// copy request body from req and save to Json/MsgPackRequestbody
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task LoadStatus_RequestBody(HttpRequest req)
        {
            if (req.Path.ToString().ToLower().Contains("api") && req.Method.ToUpper() == "GET")
            {
                _sss.Query = req.Query;
            }
            else if (req.Path.ToString().ToLower().Contains("api"))
            {
                byte[] originalContent;
                using (StreamReader stream = new StreamReader(req.Body))
                {
                    var ms = new MemoryStream();
                    await stream.BaseStream.CopyToAsync(ms);
                    originalContent = ms.ToArray();
                }
                _sss.ReqPath = req.Path.ToString();
                if (_sss.UseHash)
                {
                    _sss.RequestHash = originalContent.Take(16).ToArray();
                    originalContent = originalContent.Skip(16).ToArray();
                }

                if (_sss.UseJson)
                {
                    _sss.JsonRequest = originalContent.ToString();
                }
                else
                {
                    _sss.MsgPackRequest = originalContent;
                }
                req.Body = new MemoryStream(originalContent);
            }

        }

    }
}
