using CoreBridge.Models.DTO.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreBridge.Controllers.api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class Test_NoBaseController : ControllerBase
    {
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

    }
}
