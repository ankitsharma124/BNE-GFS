using System;
using AutoMapper;
using CoreBridge.Models.DTO;
using CoreBridge.Models.Entity;
using CoreBridge.Models.Interfaces;
using CoreBridge.Services;
using Moq;

namespace CoreBridgeTest.ServiceTest
{
	public class UserPlatformServiceTest
	{
        Mock<IUnitOfWork> mockUnitOfWork;
        Mock<IMapper> mockMapper;
		UserPlatformService service;
        List<UserPlatformDto> expectedDtoList;
        List<UserPlatform> expectedList;
        Mock<ICoreBridgeRepository<UserPlatform>> mockRepository;

        public UserPlatformServiceTest()
		{
            mockUnitOfWork = new Mock<IUnitOfWork>();
            mockMapper = new Mock<IMapper>();
            mockRepository = new Mock<ICoreBridgeRepository<UserPlatform>>();

            expectedDtoList = new List<UserPlatformDto>
            {
                new UserPlatformDto() { UserId = "TestUser1", CountryCode ="testCountry1", PlatformType = 1, PlatformUserId = "TestPlatformUserId1" },
                new UserPlatformDto() { UserId = "TestUser2", CountryCode ="testCountry2", PlatformType = 2, PlatformUserId = "TestPlatformUserId2" },
            };

            expectedList = new List<UserPlatform>
            {
                new UserPlatform() { UserId = "TestUser1", CountryCode ="testCountry1", PlatformType = 1, PlatformUserId = "TestPlatformUserId1" },
                new UserPlatform() { UserId = "TestUser2", CountryCode ="testCountry2", PlatformType = 2, PlatformUserId = "TestPlatformUserId2" },
            };
            service = new UserPlatformService(mockUnitOfWork.Object, mockMapper.Object);
		}

        [Fact]
        public async Task GetByIdAsync_ReturnsExpectedResult()
        {
            // Arrange
            var id = "123";
            expectedList[0].Id = "123";
            expectedList[1].Id = "234";
            var expected = new UserPlatform { Id = id, UserId = "Test User" };
            
            mockRepository.Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(expected);
            mockMapper.Setup(x => x.Map<UserPlatformDto>(expected)).Returns(new UserPlatformDto { UserId = "Test User" });
            mockUnitOfWork.Setup(x => x.UserPlatformRepository).Returns(mockRepository.Object);
            
            // Act
            var result = await service.GetByIdAsync(id);

            // Assert
            Assert.Equal("Test User", result.UserId);
            mockRepository.Verify(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once());
            mockMapper.Verify(x => x.Map<UserPlatformDto>(expected), Times.Once());
        }

        [Fact]
        public async Task GetByIdAsync_WhenIdNotPresent_ReturnsNull()
        {
            // Arrange
            var id = "123";
            expectedList[0].Id = "123";
            expectedList[1].Id = "234";
            var expected = new UserPlatform { Id = "1234", UserId = "Test User" };

            mockRepository.Setup(x => x.GetByIdAsync("1", It.IsAny<CancellationToken>())).ReturnsAsync(expected);
            mockMapper.Setup(x => x.Map<UserPlatformDto>(expected)).Returns(new UserPlatformDto { UserId = "Test User" });
            mockUnitOfWork.Setup(x => x.UserPlatformRepository).Returns(mockRepository.Object);

            // Act
            var result = await service.GetByIdAsync(id);

            // Assert
            Assert.Null(result);
            mockRepository.Verify(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task GetByUserIdAsync_ReturnsExpectedResult()
        {
            // Arrange
            var id = "Test User";
            expectedList[0].Id = "123";
            expectedList[1].Id = "234";
            var expected = new UserPlatform { Id = id, UserId = "Test User" };

            mockRepository.Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(expected);
            mockMapper.Setup(x => x.Map<UserPlatformDto>(expected)).Returns(new UserPlatformDto { UserId = "Test User" });
            mockUnitOfWork.Setup(x => x.UserPlatformRepository).Returns(mockRepository.Object);

            // Act
            var result = await service.GetByIdAsync(id);

            // Assert
            Assert.Equal("Test User", result.UserId);
            mockRepository.Verify(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once());
            mockMapper.Verify(x => x.Map<UserPlatformDto>(expected), Times.Once());
        }

        [Fact]
        public async Task GetByUserIdAsync_WhenIdNotPresent_ReturnsNull()
        {
            // Arrange
            var id = "Test User";
            expectedList[0].Id = "123";
            expectedList[1].Id = "234";
            var expected = new UserPlatform { Id = "1234", UserId = "Test User" };

            mockRepository.Setup(x => x.GetByIdAsync("TestUser1", It.IsAny<CancellationToken>())).ReturnsAsync(expected);
            mockMapper.Setup(x => x.Map<UserPlatformDto>(expected)).Returns(new UserPlatformDto { UserId = "Test User" });
            mockUnitOfWork.Setup(x => x.UserPlatformRepository).Returns(mockRepository.Object);

            // Act
            var result = await service.GetByIdAsync(id);

            // Assert
            Assert.Null(result);
            mockRepository.Verify(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}

