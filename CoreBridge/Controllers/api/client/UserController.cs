using CoreBridge.Models;
using CoreBridge.Models.DTO.Requests;
using CoreBridge.Models.DTO.Requests.client.user;
using CoreBridge.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreBridge.Controllers.api.client
{
    [Route("api/{titleCode}/client/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ISessionStatusService _sss;
        private readonly IRequestService _req;
        private readonly IResponseService _res;

        public UserController(ISessionStatusService sss, IRequestService req, IResponseService res)
        {
            _sss = sss;
            _req = req;
            _res = res;
        }

        [HttpPost]
        public async Task GetCountry([FromBody] ReqBag<ReqClientHeader, ClientUserGetCountryParam> bag)
        {
            await _req.ProcessRequest(Request, bag.Header, bag.Param);
            //do service
            await _res.ReturnBNResponseAsync(Response, new Dictionary<string, string> { { "responseParamName1", "responseParamVal" } });

        }
    }
}
