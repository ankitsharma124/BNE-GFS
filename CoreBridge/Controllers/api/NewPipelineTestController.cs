using CoreBridge.Models.DTO.Requests;
using CoreBridge.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreBridge.Controllers.api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class NewPipelineTestController : ControllerBase
    {
        private readonly ISessionStatusService _sss;
        private readonly IRequestService _req;
        private readonly IResponseService _res;

        public NewPipelineTestController(ISessionStatusService sss, IRequestService req, IResponseService res)
        {
            _sss = sss;
            _req = req;
            _res = res;
        }

        [HttpGet]
        public async Task TestBnIdController([FromQuery] BnIdTestParam param)
        {
            _sss.ApiCode = 9999;
            _sss.IsBnIdApi = true;
            //await _req.ProcessRequest(Request, null, param);

        }

        [HttpGet]
        public async Task TestProcessingBnIdController([FromQuery] BnIdTestParam param)
        {
            _sss.ApiCode = 9999;
            _sss.IsBnIdApi = true;
            await _req.ProcessRequest(Request, null, param);
            await _res.ReturnBNResponseAsync(Response, 1);

        }



    }
}
