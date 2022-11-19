using CoreBridge.Controllers.api;
using CoreBridge.Models;
using CoreBridge.Models.DTO.Requests;
using CoreBridge.Models.Extensions;
using CoreBridge.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace CoreBridge.Controllers
{

    public class ClientServerControllerBase : BaseControllerForMsgPack, IDisposable
    {
        public ClientServerControllerBase(IHostEnvironment env, IResponseService responseService, IDistributedCache cache,
            IConfiguration configService, ILoggerService loggerService) : base(env, responseService, cache, configService, loggerService)
        {
        }

        protected override void ProcessHeader()
        {
            if (!ReqHeader.IsOrDescendantOf(typeof(ReqBaseClientServerParamHeader)))
            {
                throw new Exception("smtg wrong with the header");
            }

            var header = (ReqBaseClientServerParamHeader)ReqHeader;
            this.TitleCode = header.TitleCd;
            this.UserId = header.UserId;
            this.SkuType = (SysConsts.SkuType)Enum.Parse(typeof(SysConsts.SkuType), header.SkuType + "");
            this.Session = header.Session;
            this.Platform = (int)header.Platform;

        }

        #region BaseController class methods


        protected override string GetUserInfoKey() { return ""; }
        protected override string GetSessionKey() { return ""; }
        protected override void SessionCheck()
        {
        }

        protected override void SessionUpdate()
        {
        }
        #endregion

    }
}
