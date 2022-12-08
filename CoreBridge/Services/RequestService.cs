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

namespace CoreBridge.Services
{
    public class RequestService : IRequestService
    {
        private readonly IConfiguration _config;
        private readonly ISessionStatusService _sss;
        private readonly ILogger _logger;
        public RequestService(IConfiguration config, ISessionStatusService sss, ILogger logger)
        {
            _config = config;
            _sss = sss;
            _logger = logger;
        }

        public void CheckUri(HttpRequest req)
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

        public void CheckApi(HttpRequest req)
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

        public void CheckTrialTitleCode()
        {
            if (_sss.SkuType == (int)SysConsts.SkuType.Trial &&
                _sss.TitleInfo.TrialTitleCode == "")
            {
                throw new BNException(_sss.ApiCode, BNException.BNErrorCode.TitleCodeInvalid,
                    $"体験版タイトルコード未登録[{_sss.TitleCode}]");
            }
        }

    }
}
