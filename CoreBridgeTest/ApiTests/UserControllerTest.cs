using CoreBridge.Models.DTO.Requests;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CoreBridgeTest.ApiTests
{
    public class UserControllerTest
    {
        private readonly HttpClient _httpClient = new() { BaseAddress = new Uri("https://localhost:44389") };
        private ReqBaseClientServerParamHeader _header = new ReqBaseClientServerParamHeader { UserId = "TestUserId", Session = "testsessionkey", Platform = 1, TitleCode = "TestTitleCode", SkuType = 0 };

        [Fact]
        public async Task TestGetCountry_Success()
        {
            //change config to UseJson = false
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/x-messagepack"));
            ClientTestParam param = new ClientTestParam() { Name = "Test" };
            var body = new ReqBag<ReqBaseClientServerParamHeader, ClientTestParam> { Header = _header, Param = param };
            var content = new ByteArrayContent(MessagePackSerializer.Serialize(body));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-messagepack");
            // Act.
            var response = await _httpClient.PostAsync("/api/TestTitleCode/client/User/GetCountry",
                content);
            // Assert.
            var reqStream = await content.ReadAsStringAsync();
            var resStream = await response.Content.ReadAsStringAsync();
        }

        [Fact]
        public async Task TestGetCountry_UserNotRegisteredErr()
        {
            ClientTestParam param = new ClientTestParam() { Name = "Test" };
            var body = new ReqBag<ReqBaseClientServerParamHeader, ClientTestParam> { Header = _header, Param = param };
            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body),
                         Encoding.UTF8, "application/json");
            // Act.
            var response = await _httpClient.PostAsync("/api/client/ClientTester/TestClientApiJson",
                content);
            // Assert.
            var reqStream = await content.ReadAsStringAsync();
            var resStream = await response.Content.ReadAsStringAsync();

            //change config to UseJson = false
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/x-messagepack"));
            ClientTestParam param = new ClientTestParam() { Name = "Test" };
            ReqBaseClientServerParamHeader header = new ReqBaseClientServerParamHeader { UserId = "NonRegisteredUser", Session = "testsessionkey", Platform = 1, TitleCode = "TestTitleCode", SkuType = 0 };

            var body = new ReqBag<ReqBaseClientServerParamHeader, ClientTestParam> { Header = header, Param = param };
            var content = new ByteArrayContent(MessagePackSerializer.Serialize(body));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-messagepack");
            // Act.
            var response = await _httpClient.PostAsync("/api/TestTitleCode/client/User/GetCountry", content);
            // Assert.
            var reqStream = await content.ReadAsStringAsync();
            var resStream = await response.Content.ReadAsStringAsync();
        }
    }
}
