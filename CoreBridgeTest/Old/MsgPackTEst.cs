using MessagePack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace CoreBridgeTest.Old
{
    public class MsgPackTEst : IDisposable
    {
        private readonly HttpClient _httpClient = new() { BaseAddress = new Uri("https://localhost:44389") };

        //ターゲットプロジェクトのJson（appconfig.development.jsonにDebugConfig/UseJson=trueを設定

        #region plain access
        [Fact]
        //Passed
        public async Task PlainAccessGetTest()
        {

            // Act.
            var response = await _httpClient.GetAsync("/api/Test_NoBase/Test");
            // Assert.
            var actual = await response.Content.ReadAsStringAsync();

            Assert.Equal("", actual);
        }

        [Fact]
        //Passed
        public async Task PlainPostStrTest()
        {

            // Act.
            var response = await _httpClient.PostAsync("/api/Test_NoBase/PostTestStr?testParam=Test", null);


            // Assert.
            var des = await response.Content.ReadAsStringAsync();
            Console.WriteLine(des);

            Assert.Equal("", des.ToString());
        }

        public class TestParam
        {
            public string name { get; set; }
        }

        [Fact]
        public async Task NoBaseMsgPackPostTest()
        {

            // Act.
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/x-messagepack"));
            var content = new ByteArrayContent(MessagePackSerializer.Serialize(new { name = "testName" }));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-messagepack");
            // Act.
            var response = await _httpClient.PostAsync("/api/Test_NoBase/PostTest",
                content);

            Assert.Equal("", response.Content.ToString());
        }

        [Fact]
        public async Task PlainPostTestParamFromBodyTest()
        {

            // Act.
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            var content = new StringContent(JsonConvert.SerializeObject(new TestParam { name = "testName" }),
                Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            // Act.
            var response = await _httpClient.PostAsync("/api/Test_NoBase/PostTest",
                content);

            Assert.Equal("", response.Content.ToString());
        }

        [Fact]
        //  404
        public async Task PlainPostStrFromBodyTest()
        {

            // Act.
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            var content = new StringContent(@"{""testParam""}", Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            // Act.
            var response = await _httpClient.PostAsync("/api/Test_NoBase/PostStrFromBodyTest",
                content);

            Assert.Equal("", response.Content.ToString());
        }
        #endregion

        [Fact]
        public async Task TestMsgPackNoPack1()
        {
            // Arrange.
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            // Act.
            var response = await _httpClient.PostAsync("/api/Test2/PostTestStr?testParam=Test", null);


            // Assert.
            var des = (object[])MessagePackSerializer.Deserialize(typeof(object[]), await response.Content.ReadAsByteArrayAsync());
            Console.WriteLine(des);
            Assert.Equal("ok", "ok");
        }

        [Fact]
        public async Task TestMsgPackNoPack()
        {
            // Arrange.
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            var content = new StringContent(JsonConvert.SerializeObject(new { name = "testNameValue" }), Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            // Act.
            var response = await _httpClient.PostAsync("/api/Test2/PostTest",
                content);

            // Assert.
            var des = (object[])MessagePackSerializer.Deserialize(typeof(object[]), await response.Content.ReadAsByteArrayAsync());
            Console.WriteLine(des);
            Assert.Equal("ok", "ok");
        }

        [Fact]
        public async Task TestStr()
        {
            // Arrange.
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            var content = new StringContent("testParam");

            // Act.
            var response = await _httpClient.PostAsync("/api/Test/TestMsgPackLoad1",
                content);

            // Assert.
            var des = (object[])MessagePackSerializer.Deserialize(typeof(object[]), await response.Content.ReadAsByteArrayAsync());
            Console.WriteLine(des);
            Assert.Equal("ok", "ok");
        }


        [Fact]
        public async Task TestMsgPack()
        {
            // Arrange.
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));


            // Act.
            var response = await _httpClient.PostAsync("/api/Test2/PostTest",
                new ByteArrayContent(MessagePackSerializer.Serialize(new { name = "Test" })));
            // Assert.
            var des = (object[])MessagePackSerializer.Deserialize(typeof(object[]), await response.Content.ReadAsByteArrayAsync());
            Console.WriteLine(des);
            Assert.Equal("ok", "ok");
        }

        [Fact]
        public async Task TestGetJson()
        {
            // Arrange.
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, "/api/Test/TestMsgPackLoad");
            msg.Content = new ByteArrayContent(MessagePackSerializer.Serialize(new { name = "Test" }));

            // Act.
            var response = await _httpClient.SendAsync(msg);
            // Assert.
            var des = (object[])MessagePackSerializer.Deserialize(typeof(object[]), await response.Content.ReadAsByteArrayAsync());

            Assert.Equal("ok", response.Content.ReadAsStringAsync().Result);
        }



        public void Dispose()
        {
            _httpClient.DeleteAsync("/state").GetAwaiter().GetResult();
        }
    }
}
