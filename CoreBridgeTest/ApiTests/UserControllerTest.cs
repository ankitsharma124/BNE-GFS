using CoreBridge.Models.DTO.Requests;
using Google.Cloud.Spanner.V1;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static CoreBridge.Models.SysConsts;
using XAct.Users;
using Newtonsoft.Json;

namespace CoreBridgeTest.ApiTests
{
    public class UserControllerTest
    {
        private readonly HttpClient _httpClient = new() { BaseAddress = new Uri("https://localhost:44389") };
        private ReqBaseClientServerParamHeader _header = new ReqBaseClientServerParamHeader { UserId = "TestUserId", Session = "testsessionkey", Platform = 1, TitleCode = "TestTitleCode", SkuType = 0 };

        [Fact]
        public async Task TestGetCountry_Success()
        {
            ClientTestParam param = new ClientTestParam() { Name = "Test" };
            var body = new ReqBag<ReqBaseClientServerParamHeader, ClientTestParam> { Header = _header, Param = param };
            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body),
                         Encoding.UTF8, "application/json");

            // Act.
            var response = await _httpClient.PostAsync("/api/TestTitleCode/client/User/GetCountry",
                content);
            // Assert.

            var resStream = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task TestGetCountry_UserNotRegisteredErr()
        {
            ClientTestParam param = new ClientTestParam() { Name = "Test" };
            var header = new ReqBaseClientServerParamHeader { UserId = "UnregId", Session = "testsessionkey", Platform = 1, TitleCode = "TestTitleCode", SkuType = 0 };
            var body = new ReqBag<ReqBaseClientServerParamHeader, ClientTestParam> { Header = header, Param = param };
            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body),
                         Encoding.UTF8, "application/json");
            // Act.

            try
            {
                var response = await _httpClient.PostAsync("/api/TestTitleCode/client/User/GetCountry",
                    content);
            }
            catch (Exception ex)
            {
                return;
            }
            // Assert.

            throw new Exception();

        }
    }
}
