using CoreBridge.Models.DTO.Requests;
using CoreBridge.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace CoreBridge.Controllers.api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestForBaseController : BaseControllerForMsgPack
    {
        public TestForBaseController(IHostEnvironment env, IResponseService responseService, IDistributedCache cache,
            IConfiguration configService, ILoggerService loggerService) : base(env, responseService, cache,
                configService, loggerService)
        { }

        #region BaseController class methods
        protected override void ProcessHeader()
        {
            // throw new NotImplementedException();
        }
        protected override string GetUserInfoKey() { return "UserInfoKey"; }
        protected override string GetSessionKey() { return "SessionKey"; }
        protected override void SessionCheck()
        {
        }

        protected override void SessionUpdate()
        {
        }
        #endregion


        [HttpPost]
        public void ParamTestObject([FromHeader] ReqClientHeader header, [FromForm] TestParam testParam)
        {
            ReqParam = testParam;
        }

    }
}
