#if DEBUG
using CoreBridge.Models;
using CoreBridge.Models.DTO.Requests;
using CoreBridge.Models.Extensions;
using CoreBridge.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Diagnostics;

namespace CoreBridge.Controllers.api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class Test_NoBaseController : ControllerBase
    {

        private readonly IRequestService _reqService;

        public Test_NoBaseController(IRequestService reqService)
        {
            _reqService = reqService;
        }

        [HttpPost]
        public IActionResult JsonTest([FromBody] TestParam testParam)
        {
            var name = testParam.name;
            return new JsonResult(new { name = name });
        }

        /*

        [HttpPost]
        public IActionResult MiddlewareTest([FromBody] TestParam testParam)
        {
            var header = _reqService.GetDebugBodyCopyInBytesFromHeader(Request);
            //Debug.WriteLine(header);

            return new JsonResult(header);
        }


        [HttpGet]
        public IActionResult GetTest()
        {
            return new JsonResult(new { name = "Hello" });
        }


        [HttpPost]
        public IActionResult PostTest([FromBody] TestParam testParam)
        {
            return new JsonResult(new { name = "Hello" });
        }

        [HttpPost]
        public IActionResult PostStrFromBodyTest([FromBody] string testParam)
        {
            return new JsonResult(new { name = "Hello" });
        }

        [HttpPost]
        public IActionResult PostTestStr(string testParam)
        {
            return new JsonResult(new { name = "Hello" });
        }
        */

    }
}
#endif
