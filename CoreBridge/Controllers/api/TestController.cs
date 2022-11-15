using CoreBridge.Models.Exceptions;
using CoreBridge.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CoreBridge.Controllers.api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : BaseControllerForMsgPack
    {

        public TestController(IHostEnvironment env, IResponseService responseService) : base(env, responseService)
        {

        }


        [HttpGet]
        public void GetBNErrorTest()
        {
            throw new BNException(1011, BNException.BNErrorCode.TokenErr);
        }


        [HttpGet]
        public void GetBNResponseTest()
        {
            ReturnBNResponse(new object[] { 0 }).Wait();
        }


        [HttpGet]
        public void GetResponseWrite()
        {
            Response.Headers.Add("Content-Type", "text/plain");
            Response.WriteAsync("Test").Wait();
        }

        [HttpGet]
        public void GetResponseWriteJson()
        {
            Response.Headers.Add("Content-Type", "application/json");
            Response.WriteAsJsonAsync(new { Test = "test" }).Wait();
        }

        [HttpGet]
        public void GetResponseWriteJsonByNormalWrite()
        {
            Response.Headers.Add("Content-Type", "application/json");
            var json = JsonSerializer.Serialize(new { Test = "test" });
            Response.Headers.ContentLength = json.Length;
            Response.WriteAsync(json).Wait();
        }

        [HttpGet]
        public void GetResponseWriteJsonByNormalWriteUtf()
        {
            Response.Headers.Add("Content-Type", "application/json");
            Response.Headers.Add("charset", "utf-8");
            var json = JsonSerializer.Serialize(new { Test = "test" });
            Response.Headers.ContentLength = json.Length;
            Response.WriteAsync(json).Wait();
        }

        [HttpGet]
        public void GetJson_Void()
        {
            Response.Headers.Add("Content-Type", "application/json");
        }


        [HttpGet]
        public IActionResult GetJson()
        {

            return new JsonResult(new { test = "test" });
        }
    }
}
