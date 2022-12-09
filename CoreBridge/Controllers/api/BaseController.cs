using CoreBridge.Models.DTO.Requests;
using CoreBridge.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.PortableExecutable;

namespace CoreBridge.Controllers.api
{
    public class BaseController : ControllerBase
    {
        private readonly ISessionStatusService _sss;
        private readonly IRequestService _req;
        public BaseController(ISessionStatusService sss, IRequestService req)
        {
            _sss = sss;
            _req = req;
        }

        public async Task SampleProcess(ReqBase reqHeader, ReqBase reqParam)
        {
            _sss.ApiCode = 9999;
            //_sss.BnIdApi = true;
            //req.ProcessRequest(Request, reaHeader, reqParam);

        }


    }
}
