using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreBridgeTest
{
    public static class TestHelpers
    {
        private const string _jsonMediaType = "application/json";
        private const int _expectedMaxElapsedMilliseconds = 1000;
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

        public static async Task AssertResponseWithContentAsync<T>(
            HttpResponseMessage response, System.Net.HttpStatusCode expectedStatusCode,
            T expectedContent)
        {
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.Equal(_jsonMediaType, response.Content.Headers.ContentType?.MediaType);
            Assert.Equal(expectedContent, await JsonSerializer.DeserializeAsync<T?>(
                await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions));
        }


        public static StringContent GetJsonStringContent<T>(T model)
            => new(JsonSerializer.Serialize(model), Encoding.UTF8, _jsonMediaType);

        public static byte[] GetHashWithKey(string hashKey, string body)
        {
            return GetHashWithKey(hashKey, new UTF8Encoding().GetBytes(body));
        }

        public static byte[] GetHashWithKey(string hashKey, byte[] body)
        {
            var key = new UTF8Encoding().GetBytes(hashKey);
            var bytes = new List<byte>(body);
            bytes.AddRange(key);
            return MD5.Create().ComputeHash(bytes.ToArray());
        }


    }
}
