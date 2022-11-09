using Newtonsoft.Json;
using System.Diagnostics;

namespace CoreBridgeTest
{
    public class UnitTest1 : IDisposable
    {
        private readonly HttpClient _httpClient = new() { BaseAddress = new Uri("https://localhost:44389/api") };

        [Fact]
        public async Task Test1()
        {
            // Arrange.
            var expected = "\"name\":\"Makoto Nishino\",\"eMail\":\"makoto.nishino@scopenext.jp\",\"password\":\"password\",\"confirmPassword\":\"password\"";

            // Act.
            var response = await _httpClient.GetAsync("/AdminUser/Get");
            // Assert.
            var returned = JsonConvert.SerializeObject(response);
            Assert.Equal(expected, returned);
        }


        public void Dispose()
        {
            _httpClient.DeleteAsync("/state").GetAwaiter().GetResult();
        }
    }
}