using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoreBridge.Models.lib;

namespace CoreBridge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JwtTestController : ControllerBase
    {
        private JWTManager _jwtcontent = new JWTManager();

        [HttpGet]
        public async Task<ActionResult<string>> GetJWTtems()
        {
            //テストクラスで実験.
            PayloadSample payloadSample = new PayloadSample();
            payloadSample.foo = "foo";
            payloadSample.bar = "bar";

            //Tokenを作る
            _jwtcontent.CreateToken(payloadSample);

            //TokenをDecode
            return await _jwtcontent.DecToken();
        }



    }

}
