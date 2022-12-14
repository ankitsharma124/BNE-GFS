using CoreBridge.Models.DTO.Requests;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CoreBridgeTest.Old
{
    public class PipelineTest
    {
        private readonly HttpClient _httpClient = new() { BaseAddress = new Uri("https://localhost:44389") };
        private ReqBaseClientServerParamHeader _header = new ReqBaseClientServerParamHeader { UserId = "TestUserId", Session = "testsessionkey", Platform = 1, TitleCode = "TestTitleCode", SkuType = 0 };

        [Fact]
        public async Task TestBnIdControllerFunctions()
        {
            // Act.
            var response = await _httpClient.GetAsync("/api/NewPipelineTest/TestBnIdController?seq=123456789");

            // Assert.

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        //
        [Fact]
        public async Task TestBnIdControllerFunctions_Processing()
        {
            // Act.
            var response = await _httpClient.GetAsync("/api/NewPipelineTest/TestProcessingBnIdController?seq=123456789");

            // Assert.

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        //
        [Fact]
        public async Task TestClientApiJsonAccess()
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
        }

        [Fact]
        public async Task TestClientApiMsgPackAccess()
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
            var response = await _httpClient.PostAsync("/api/client/ClientTester/TestClientMsgpack",
                content);
            // Assert.
            var reqStream = await content.ReadAsStringAsync();
            var resStream = await response.Content.ReadAsStringAsync();
        }

        [Fact]
        public async Task TestServerApiJsonAccess()
        {
            ServerTestParam param = new ServerTestParam() { Name = "Test" };
            var body = new ReqBag<ReqBaseClientServerParamHeader, ServerTestParam> { Header = _header, Param = param };
            var hash = TestHelpers.GetHashWithKey("TEST111111111111", Newtonsoft.Json.JsonConvert.SerializeObject(body));

            var content = new StringContent(Encoding.UTF8.GetString(hash) + Newtonsoft.Json.JsonConvert.SerializeObject(body),
                         Encoding.UTF8, "application/json");
            // Act.
            var response = await _httpClient.PostAsync("/api/server/ServerTest/TestServerApiJson",
                content);
            // Assert.
            var reqStream = await content.ReadAsStringAsync();
            var resStream = await response.Content.ReadAsStringAsync();
        }

        [Fact]
        public async Task TestServerApiMsgPackAccess()
        {
            //change config to UseJson = false
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/x-messagepack"));
            ServerTestParam param = new ServerTestParam() { Name = "Test" };
            var body = new ReqBag<ReqBaseClientServerParamHeader, ServerTestParam> { Header = _header, Param = param };
            var serialized = MessagePackSerializer.Serialize(body);
            var hash = TestHelpers.GetHashWithKey("TEST111111111111", serialized);
            var list = hash.ToList();
            list.AddRange(serialized);
            var content = new ByteArrayContent(list.ToArray());
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-messagepack");
            // Act.
            var response = await _httpClient.PostAsync("/api/server/ServerTest/TestServerMsgpack",
                content);
            // Assert.
            var reqStream = await content.ReadAsStringAsync();
            var resStream = await response.Content.ReadAsStringAsync();
        }

        [Fact]
        public async Task TestClientApiRouteWithTitleCode()
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
            var response = await _httpClient.PostAsync("/api/TestTitleCode/client/ClientTester/TestClientMsgpack",
                content);
            // Assert.
            var reqStream = await content.ReadAsStringAsync();
            var resStream = await response.Content.ReadAsStringAsync();
        }
    }
}
