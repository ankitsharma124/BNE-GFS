#if DEBUG
using CoreBridge.Models;
using CoreBridge.Models.DTO.Requests;
using CoreBridge.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Cryptography;
using System.Text;

namespace CoreBridge.Controllers.api.server
{
    /// <summary>
    /// local環境でのみ使用、テスト用
    /// </summary>
    [Route("api/server/[controller]/[action]")]
    public class ServerTest_NoBaseController : ControllerBase
    {
        private readonly IRequestService _reqService;
        public ServerTest_NoBaseController(IRequestService reqService)
        {
            _reqService = reqService;
        }

        [HttpPost]
        public IActionResult MiddlewareTest([FromBody] TestParam testParam)
        {
            var hashOK = TestHash(Request);
            return new JsonResult(hashOK);
        }

        private bool TestHash(HttpRequest request)
        {
            var hashKey = "TEST111111111111";
            var originalBytes = _reqService.GetOriginalBodyInBytesFromHeader(request);
            var submittedHash = _reqService.GetBodyHashInBytesFromHeader(request);
            var calculatedHash = GetBodyHash(hashKey, originalBytes);
            return Enumerable.SequenceEqual(submittedHash, calculatedHash);

        }

        /// <summary>
        /// テスト対象であるServerControllerからコピーすべし
        /// </summary>
        /// <param name="hashKey"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        private byte[] GetBodyHash(string hashKey, byte[] body)
        {
            var key = new UTF8Encoding().GetBytes(hashKey);
            var bytes = new List<byte>(body);
            bytes.AddRange(key);
            return MD5.Create().ComputeHash(bytes.ToArray());
        }
    }
}
#endif
