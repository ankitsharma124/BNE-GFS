using CoreBridge.Models.DTO.Requests;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CoreBridgeTest.ApiTests
{
    /// <summary>
    /// 各client api actionに対して行うべきテストのひな形。
    /// コピーして、
    /// _headerはテスト用dbにseedする内容に合わせて変更されたし。
    /// ClientTestParamは各api action用Paramクラスにて置き換え、
    /// post先のcontroller, action nameも変更して使ってください。
    /// 
    /// </summary>
    public class CommonClientApiTests
    {
        private readonly HttpClient _httpClient = new() { BaseAddress = new Uri("https://localhost:44389") };
        private ReqBaseClientServerParamHeader _header = new ReqBaseClientServerParamHeader { UserId = "TestUserId", Session = "testsessionkey", Platform = 1, TitleCode = "TestTitleCode", SkuType = 0 };


        /// <summary>
        /// Api actionにjsonで指定parameterを送り、正常に処理されることを確認。
        /// appsettings.jsonでUseJsonをtrueに設定する必要あり。
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task TestClientApiJsonAccess()
        {
            ClientTestParam param = new ClientTestParam() { Name = "Test" };
            var body = new ReqBag<ReqBaseClientServerParamHeader, ClientTestParam> { Header = _header, Param = param };
            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body),
                         Encoding.UTF8, "application/json");
            // Act.
            var response = await _httpClient.PostAsync("/api/TestTitleCode/client/ClientTester/TestClientApiJson",
                content);

            //Assert - 各自確認ロジックを以下に実装
        }


        /*
        メンテナンス中
        セッションエラー
        セッションタイムアウト
        リクエストエラー
        不正タイトルコード
        不正プラットフォーム種別
        外部デバイス異常
         */



    }
}
