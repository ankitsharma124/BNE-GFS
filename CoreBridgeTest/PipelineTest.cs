using CoreBridge.Models.DTO.Requests;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBridgeTest
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
    }
}
