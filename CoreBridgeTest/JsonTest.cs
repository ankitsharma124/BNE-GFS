using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static CoreBridgeApiTest.MsgPackTEst;

namespace CoreBridgeApiTest
{
    public class JsonTest
    {
        private readonly HttpClient _httpClient = new() { BaseAddress = new Uri("https://localhost:44389") };


        [Fact]
        public async Task TestCollectHttpParamJson()/// config useJson = trueで設定のこと
        {
            // Arrange.
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(new TestParam { name = "testName" }),
              Encoding.UTF8, "application/json");
            // Act.
            var response = await _httpClient.PostAsync("/api/Test/CollectParamTest",
                content);
            // Assert.
            var reqStream = await content.ReadAsStringAsync();
            var resStream = await response.Content.ReadAsStringAsync();
            Assert.Equal(reqStream, resStream);
        }


        /// <summary>
        /// config useJson = trueで設定のこと
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task JsonThroughBaseController()
        {
            // Arrange.
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(new TestParam { name = "testName" }),
              Encoding.UTF8, "application/json");
            // Act.
            var response = await _httpClient.PostAsync("/api/Test/JsonTest",
                content);
            // Assert.
            var reqStream = await content.ReadAsStringAsync();
            var resStream = await response.Content.ReadAsStringAsync();
            Assert.Equal(reqStream, resStream);
        }

        [Fact]
        public async Task JsonTestNoBase()
        {
            // Arrange.
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(new TestParam { name = "testName" }),
               Encoding.UTF8, "application/json");
            // Act.
            var response = await _httpClient.PostAsync("/api/Test_NoBase/JsonTest",
                content);
            // Assert.
            var reqStream = await content.ReadAsStringAsync();
            var resStream = await response.Content.ReadAsStringAsync();
            Assert.Equal(reqStream, resStream);
        }
    }
}
