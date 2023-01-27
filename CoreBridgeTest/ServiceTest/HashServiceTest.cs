using System.Security.Cryptography;
using System.Text;
using CoreBridge.Models.DTO;
using CoreBridge.Models.Entity;
using CoreBridge.Models.Exceptions;
using CoreBridge.Services;
using CoreBridge.Services.Interfaces;
using Moq;

namespace CoreBridgeTest.ServiceTest
{
    public class HashServiceTest
	{
        Mock<ISessionStatusService> mockSessionStatusService ;
        HashService hashService ;

        public HashServiceTest()
        {
            mockSessionStatusService = new Mock<ISessionStatusService>();
            hashService = new HashService(mockSessionStatusService.Object);
        }

        [Fact]
        public void GetHashWithKey_StringString_ShouldReturnExpectedHash()
        {
            // Arrange
            var testString = "Test String";
            var testKey = "Test Key";
            var bytes = new List<byte>(new UTF8Encoding().GetBytes(testString));
            bytes.AddRange(new UTF8Encoding().GetBytes(testKey));
            var expectedHash = MD5.Create().ComputeHash(bytes.ToArray()); // Add expected hash value here

            // Act
            var result = hashService.GetHashWithKey(testKey, testString);

            // Assert
            Assert.Equal(expectedHash, result);
        }

        [Fact]
        public void GetHashWithKey_StringByte_ShouldReturnExpectedHash()
        {
            // Arrange
            var testString = "Test String";
            var testKey = "Test Key";

            var bytes = new List<byte>(new UTF8Encoding().GetBytes(testString));
            bytes.AddRange(new UTF8Encoding().GetBytes(testKey));
            var expectedHash = MD5.Create().ComputeHash(bytes.ToArray()); // Add expected hash value here

            // Act
            var result = hashService.GetHashWithKey(testKey, new UTF8Encoding().GetBytes(testString));

            // Assert
            Assert.Equal(expectedHash, result);
        }

        [Fact]
        public void CheckTitleHasHashKey_ShouldThrowException_WhenHashKeyIsNotPresent()
        {
            var mockTitleInfo = new Mock<TitleInfoDto>();
            //mockTitleInfo.SetupGet(ti => ti.HashKey).Returns(string.Empty);

            mockSessionStatusService.SetupGet(sss => sss.TitleInfo).Returns(new TitleInfoDto { HashKey = "" });

            var hashService = new HashService(mockSessionStatusService.Object);

            // Act & Assert
            var ex = Assert.Throws<BNException>(() => hashService.CheckTitleHasHashKey());
            Assert.Contains("ハッシュキーが未登録です", ex.Message);
        }

        [Fact]
        public void CheckTitleHasHashKey_ShouldNotThrowException_WhenHashKeyIsPresent()
        {
            var mockTitleInfo = new Mock<TitleInfoDto>();
            //mockTitleInfo.SetupGet(ti => ti.HashKey).Returns(string.Empty);

            mockSessionStatusService.SetupGet(sss => sss.TitleInfo).Returns(new TitleInfoDto { HashKey = "Valid_Key" });

            var hashService = new HashService(mockSessionStatusService.Object);

            // Act & Assert
            hashService.CheckTitleHasHashKey();
            
        }

        [Fact]
        public void CheckHash_ValidHash_DoesNotThrow()
        {
            // Arrange
            var hashService = new HashService(mockSessionStatusService.Object);
            mockSessionStatusService.Setup(x => x.TitleInfo).Returns(new TitleInfoDto { HashKey = "valid_key" });
            mockSessionStatusService.Setup(x => x.UseJson).Returns(true);
            mockSessionStatusService.Setup(x => x.RequestBody).Returns("valid_request_body");
            mockSessionStatusService.Setup(x => x.RequestHash).Returns(hashService.GetHashWithKey("valid_key", "valid_request_body"));

            // Act
            hashService.CheckHash();

            // Assert
            // No exception was thrown
        }

        [Fact]
        public void CheckHash_InvalidHash_ThrowsBNException()
        {
            // Arrange
            var mockSessionStatusService = new Mock<ISessionStatusService>();
            mockSessionStatusService.Setup(x => x.TitleInfo).Returns(new TitleInfoDto { HashKey = "valid_key" });
            mockSessionStatusService.Setup(x => x.UseJson).Returns(true);
            mockSessionStatusService.Setup(x => x.RequestBody).Returns("valid_request_body");
            mockSessionStatusService.Setup(x => x.RequestHash).Returns(new byte[] { 0, 1, 2 }); // invalid hash
            var hashService = new HashService(mockSessionStatusService.Object);

            // Act & Assert
            var exception = Assert.Throws<BNException>(() => hashService.CheckHash());
            Assert.Contains("hash error", exception.Message);
        }

    }
}

/* Unsupported expression: ti => ti.HashKey
Non-overridable members (here: TitleInfoDto.get_HashKey) may not be used in setup / verification expressions. */

