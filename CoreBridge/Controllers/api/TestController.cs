#if DEBUG

using CoreBridge.Models.DTO.Requests;
using CoreBridge.Models.Exceptions;
using CoreBridge.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace CoreBridge.Controllers.api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : BaseControllerForMsgPack
    {
        private ReqClientHeader _clientHeader = new ReqClientHeader { Platform = 1, Session = "Session", UserId = "UserId", TitleCd = "Code", SkuType = 0 };

        public TestController(ILogger<TestController> logger, IAdminUserService adminUserService, IDistributedCache cache,
            IWebHostEnvironment env, IResponseService resp, IConfiguration config) : base(env, resp, cache, config, logger)
        {

        }

        [HttpPost]
        public IActionResult CollectParamTest([FromBody] TestParam testParam)
        {
            var name = testParam.name;
            return new JsonResult(new { name = name });
        }




        [HttpPost]
        public IActionResult JsonTest([FromBody] TestParam testParam)
        {
            var name = testParam.name;
            return new JsonResult(new { name = name });
        }



        [HttpGet]
        public IActionResult Test()
        {
            return new JsonResult(new { name = "Hello" });
        }

        [HttpPost]
        public void TestMsgPackLoad([FromBody] TestParam testParam)
        {
            SetParams(_clientHeader, testParam);
            ReturnBNResponse(new object[] { 1, testParam }).Wait();

        }

        [HttpPost]
        public void TestMsgPackLoad1(string strParam)
        {
            SetParams(_clientHeader, new TestParam { name = strParam });
            ReturnBNResponse(new object[] { 1, strParam }).Wait();

        }

        [HttpPost]
        public void ParamTestObject([FromHeader] ReqClientHeader header, [FromForm] TestParam testParam)
        {
            SetParams(header, testParam);
        }

        #region BaseController class methods

        protected override void ProcessHeader()
        {
            // throw new NotImplementedException();
        }
        protected override string GetUserInfoKey() { return ""; }
        protected override string GetSessionKey() { return ""; }
        protected override void SessionCheck()
        {
        }

        protected override void SessionUpdate()
        {
        }
        #endregion

        #region trials
        [HttpPost]
        public void HeaderTest([FromHeader] ReqClientHeader header)
        {
            //Console.WriteLine(header.UserId);
        }

        [HttpPost]
        public void ParamTest([FromHeader] ReqClientHeader header, [FromForm] string name)
        {
            //Console.WriteLine(header.UserId);
            Console.WriteLine(name);
        }



        [HttpGet]
        public void GetResponseWrite()
        {
            Response.Headers.Add("Content-Type", "text/plain");
            ReturnBNResponse(new object[] { 1, "TestString" }).Wait();
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

        #endregion

        /// <summary>
        /// コントローラー上でBNエラーを投げるとerror responseが返ってくることを確認
        /// </summary>
        /// <exception cref="BNException"></exception>
        [HttpGet]
        public void GetBNErrorTest()
        {
            throw new BNException(1011, BNException.BNErrorCode.TokenErr);
        }

        /// <summary>
        /// コントローラー上でBNエラーを投げるとresponseが返ってくることを確認
        /// </summary>
        /// <exception cref="BNException"></exception>
        [HttpGet]
        public void GetBNResponseTest()
        {
            ReturnBNResponse(new object[] { 0 }).Wait();
        }



    }
}
#endif
