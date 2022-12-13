using CoreBridge.Models.DTO.Requests;
using CoreBridge.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreBridge.Controllers.api.client
{
    [Route("api/client/[controller]/[action]")]
    [ApiController]
    public class ClientTesterController : ControllerBase
    {
        private readonly ISessionStatusService _sss;
        private readonly IRequestService _req;
        private readonly IResponseService _res;

        public ClientTesterController(ISessionStatusService sss, IRequestService req, IResponseService res)
        {
            _sss = sss;
            _req = req;
            _res = res;
        }

        [HttpPost]
        public async Task TestClientApiJson([FromBody] ReqBag<ReqBaseClientServerParamHeader, ClientTestParam> bag)
        {
            _sss.ApiCode = 9999;
            await _req.ProcessRequest(Request, bag.Header, bag.Param);
            //perform service
            await _res.ReturnBNResponseAsync(Response, 1);

        }

        [HttpPost]
        public async Task TestClientMsgpack([FromBody] ReqBag<ReqBaseClientServerParamHeader, ClientTestParam> bag)
        {
            _sss.ApiCode = 9999;
            await _req.ProcessRequest(Request, bag.Header, bag.Param);
            //perform service
            await _res.ReturnBNResponseAsync(Response, 1);

        }
    }
}
