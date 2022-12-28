using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace CoreBridgeTest.Old
{
    //public class MiddlewareTest
    //{
    //    private readonly HttpClient _httpClient = new() { BaseAddress = new Uri("https://localhost:44389") };

    //    /// <summary>
    //    /// DebugMiddlewareのdebug用テスト・自動走行対象外
    //    /// </summary>
    //    /// <returns></returns>
    //    //[Fact]
    //    public async Task TestDebugMiddleware()
    //    {
    //        // Arrange.
    //        _httpClient.DefaultRequestHeaders.Accept.Clear();
    //        _httpClient.DefaultRequestHeaders.Accept.Add(
    //            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/x-messagepack"));
    //        var content = new ByteArrayContent(MessagePackSerializer.Serialize(new { name = "testName" }));
    //        content.Headers.ContentType = new MediaTypeHeaderValue("application/x-messagepack");
    //        // Act.
    //        var response = await _httpClient.PostAsync("/api/Test_NoBase/MiddlewareTest",
    //            content);
    //        // Assert.
    //        var reqStream = await content.ReadAsStreamAsync();
    //        var resStream = await response.Content.ReadAsStreamAsync();
    //        Assert.Equal(reqStream.Length, resStream.Length);
    //        Assert.Equal(reqStream.ReadByte(), resStream.ReadByte());
    //    }

    //    /// <summary>
    //    /// HashMiddlewareのテスト
    //    /// </summary>
    //    /// <returns></returns>
    //    [Fact]
    //    public async Task TestHashMiddleware()
    //    {
    //        // Arrange.
    //        _httpClient.DefaultRequestHeaders.Accept.Clear();
    //        _httpClient.DefaultRequestHeaders.Accept.Add(
    //            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/x-messagepack"));

    //        var msgPack = MessagePackSerializer.Serialize(new { name = "testName" });
    //        var hashKey = "TEST111111111111";
    //        var keyBytes = msgPack.Concat(Encoding.UTF8.GetBytes(hashKey)).ToArray();
    //        var hashed = System.Security.Cryptography.MD5.HashData(keyBytes);
    //        var body = hashed.Concat(msgPack).ToArray();
    //        var content = new ByteArrayContent(body);
    //        content.Headers.ContentType = new MediaTypeHeaderValue("application/x-messagepack");
    //        // Act.
    //        var response = await _httpClient.PostAsync("/api/server/ServerTest_NoBase/MiddlewareTest",
    //            content);
    //        // Assert.
    //        var resContent = await response.Content.ReadAsStringAsync();

    //        Assert.Equal(resContent, "true");
    //    }
    //}
}
