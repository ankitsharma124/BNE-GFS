#if DEBUG

using CoreBridge.Models.DTO.Requests;
using CoreBridge.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace CoreBridge.Controllers.api.server
{

    [Route("api/server/[controller]/[action]")]
    [ApiController]
    public class ServerTestController : ControllerBase
    {
        private readonly IRequestService _reqService;
        private readonly ISessionStatusService _sss;
        private readonly IResponseService _res;
        public ServerTestController(
          IRequestService reqService, ISessionStatusService session, IResponseService res)
        {
            _reqService = reqService;
            _sss = session;
            _res = res;

        }


        [HttpPost]
        public IActionResult MiddlewareTest([FromBody] TestParam testParam)
        {
            //var header = _reqService.GetDebugBodyCopyInBytesFromHeader(Request);
            //Debug.WriteLine(header);

            return new JsonResult("OK");
        }

        [HttpPost]
        public async Task TestServerApiJson([FromBody] ReqBag<ReqBaseClientServerParamHeader, ServerTestParam> bag)
        {
            _sss.ApiCode = 9999;
            await _reqService.ProcessRequest(Request, bag.Header, bag.Param);
            //perform service
            await _res.ReturnBNResponseAsync(Response, 1);

        }

        [HttpPost]
        public async Task TestServerMsgpack([FromBody] ReqBag<ReqBaseClientServerParamHeader, ServerTestParam> bag)
        {
            _sss.ApiCode = 9999;
            await _reqService.ProcessRequest(Request, bag.Header, bag.Param);
            //perform service
            await _res.ReturnBNResponseAsync(Response, 1);

        }
    }
}
#endif