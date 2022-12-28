using CoreBridge.Models.DTO.Requests;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CoreBridgeTest
{
    /// <summary>
    /// 各api_actionがmessagepackにきちんと対応していることを確認するテスト
    /// (他テストは全般的にjson形式で行う）
    /// client api用テストひな形 => TestClientApiMsgPackAccess
    /// server api用テストひな形 => TestServerApiMsgPackAccess
    /// ひな形をコピーして、ParameterオブジェクトやURLなどを適宜変更のこと
    /// </summary>
    public class ApiMessagePackTests
    {
        private readonly HttpClient _httpClient = new() { BaseAddress = new Uri("https://localhost:44389") };
        private ReqBaseClientServerParamHeader _header = new ReqBaseClientServerParamHeader { UserId = "TestUserId", Session = "testsessionkey", Platform = 1, TitleCode = "TestTitleCode", SkuType = 0 };

        /// <summary>
        /// サンプル。
        /// client api actionにmsgpackで指定parameterを送り、正常に処理されることを確認。
        /// </summary>
        /// <returns></returns>
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
            var response = await _httpClient.PostAsync("/api/client/ClientTester/TestClientMsgpack", content);

        }

        /// <summary>
        /// サンプル。
        /// server api actionにmsgpackで指定parameterを送り、正常に処理されることを確認。
        /// </summary>
        /// <returns></returns>
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
    }
}
