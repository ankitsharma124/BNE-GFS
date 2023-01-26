using System;
using System.Collections.Generic;
using AutoMapper;
using Castle.Components.DictionaryAdapter.Xml;
using CoreBridge.Models.DTO;
using CoreBridge.Models.Entity;
using CoreBridge.Models.Interfaces;
using CoreBridge.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CoreBridgeTest.ServiceTest
{
	public class AppUserServiceTest
	{
        Mock<ICoreBridgeRepository<AppUser>> mockAppUserRepository;
        Mock<IUnitOfWork> mockUnitOfWork;
        Mock<ILogger<AppUserService>> mockLogger;
        AppUserService service;
        Mock<IMapper> mockMapper;
        List<AppUser> expectedList;
        List<AppUserDto> expectedDtoList;

        public AppUserServiceTest()
        {
            mockAppUserRepository = new Mock<ICoreBridgeRepository<AppUser>>();
            mockUnitOfWork = new Mock<IUnitOfWork>();
            mockMapper = new Mock<IMapper>();
            mockLogger = new Mock<ILogger<AppUserService>>();
            expectedDtoList = new List<AppUserDto>
            {
                new AppUserDto("abc123", "title1", CoreBridge.Models.AdminUserRoleEnum.AdminUser, "password1","abc123@email.com","password1",""),
                new AppUserDto("bcd123", "title1", CoreBridge.Models.AdminUserRoleEnum.AdminUser, "password2","bcd123@email.com","password2","")
            };

            expectedList = new List<AppUser>
            {
                new AppUser("abc123", "title1", CoreBridge.Models.AdminUserRoleEnum.AdminUser,"abc123@email.com","password1",""),
                new AppUser("bcd123", "title1", CoreBridge.Models.AdminUserRoleEnum.AdminUser,"bcd123@email.com","password2","")
            };
            service = new AppUserService(mockUnitOfWork.Object, mockMapper.Object);

        }

        [Fact]
        public async Task ListAsync_ReturnsExpectedResult()
        {
            // Arrange
            mockUnitOfWork.Setup(x => x.AppUserRepository.ListAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expectedList);
            mockMapper.Setup(x => x.Map<List<AppUserDto>>(expectedList)).Returns(expectedDtoList);
            
            // Act
            var result = await service.ListAsync();

            // Assert
            Assert.Equal(expectedList.Count, result.Count);
            for (int i = 0; i < expectedList.Count; i++)
            {
                Assert.Equal(expectedList[i].UserId, result[i].UserId);
                Assert.Equal(expectedList[i].TitleCode, result[i].TitleCode);
                Assert.Equal(expectedList[i].Password, result[i].Password);
            }
        }
        [Fact]
        public async Task AddAsync_ReturnsExpectedResult()
        {
            //Arrange
            mockUnitOfWork.Setup(x => x.AppUserRepository).Returns(mockAppUserRepository.Object);
            var appUserDto = new AppUserDto("abc123", "title1", CoreBridge.Models.AdminUserRoleEnum.AdminUser, "password1", "abc123@email.com", "password1", "");

            //Act
            var result = await service.AddAsync(appUserDto);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(appUserDto.UserId, result?.UserId);
            Assert.Equal(appUserDto.TitleCode, result?.TitleCode);
            Assert.Equal(appUserDto.Password, result?.Password);
        }

        [Fact]
        public async Task AddAsync_UserAlreadyExists_ReturnsNull()
        {
            //Arrange
            var appUserDto = new AppUserDto("abc123", "title1", CoreBridge.Models.AdminUserRoleEnum.AdminUser, "password1", "abc123@email.com", "password1", "");
            mockUnitOfWork.Setup(x => x.AppUserRepository.ListAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<AppUser> { new AppUser { UserId = "abc123" } });

            //Act
            var result = await service.AddAsync(appUserDto);

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsExpectedResult()
        {
            //Arrange
            var appUserDto = new AppUserDto("abc123", "title1", CoreBridge.Models.AdminUserRoleEnum.AdminUser, "password1", "abc123@email.com", "password1", "");
            mockUnitOfWork.Setup(x => x.AppUserRepository.ListAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<AppUser> { new AppUser { UserId = "abc123" } });

            //Act
            var result = await service.DeleteAsync(appUserDto);

            //Assert
            mockUnitOfWork.Verify(x => x.AppUserRepository.DeleteAsync(It.IsAny<AppUser>()), Times.Once);
            mockUnitOfWork.Verify(x => x.CommitAsync(), Times.Once);
            Assert.Equal(appUserDto, result);
        }

        [Fact]
        public async Task DetachAsync_ReturnsExpectedResult()
        {
            //Arrange
            var appUserDto = new AppUserDto("abc123", "title1", CoreBridge.Models.AdminUserRoleEnum.AdminUser, "password1", "abc123@email.com", "password1", "");
            mockUnitOfWork.Setup(x => x.AppUserRepository.ListAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<AppUser> { new AppUser { UserId = "abc123" } });

            //Act
            var result = await service.DetachAsync(appUserDto);

            //Assert
            mockUnitOfWork.Verify(x => x.AppUserRepository.DetachAsync(It.IsAny<AppUser>()), Times.Once);
            mockUnitOfWork.Verify(x => x.CommitAsync(), Times.Once);
            Assert.Equal(appUserDto, result);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsExpectedResult()
        {
            // Arrange
            var appUser = new AppUser("abc123", "title1", CoreBridge.Models.AdminUserRoleEnum.AdminUser, "abc123@email.com", "password1", "");
            expectedList[0].Id = "1234";
            expectedList[1].Id = "4321";
            appUser.Id = "1234";
            string expectedUserId = "1234";
            mockUnitOfWork.Setup(x => x.AppUserRepository.ListAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expectedList);
            mockMapper.Setup(x => x.Map<List<AppUserDto>>(expectedList)).Returns(expectedDtoList);
            mockUnitOfWork.Setup(x => x.AppUserRepository.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(appUser);
            // Act
            var result = await service.GetByIdAsync(expectedUserId);

            // Assert
            mockUnitOfWork.Verify(x => x.AppUserRepository.ListAsync(It.IsAny<CancellationToken>()), Times.Once);
            mockUnitOfWork.Verify(x => x.AppUserRepository.GetByIdAsync(expectedUserId, It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal(expectedUserId, result?.Id);
        }

        [Fact]
        public async Task GetByIdAsync_IdNot_Exist_ReturnsNull()
        {
            // Arrange
            var appUser = new AppUser("abc123", "title1", CoreBridge.Models.AdminUserRoleEnum.AdminUser, "abc123@email.com", "password1", "");
            expectedList[0].Id = "1234";
            expectedList[1].Id = "4321";
            string expectedUserId = "1236";
            mockUnitOfWork.Setup(x => x.AppUserRepository.ListAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expectedList);
            mockMapper.Setup(x => x.Map<List<AppUserDto>>(expectedList)).Returns(expectedDtoList);
            mockUnitOfWork.Setup(x => x.AppUserRepository.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(appUser);
            // Act
            var result = await service.GetByIdAsync(expectedUserId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_ValidInput_ReturnsExpectedResult()
        {
            //Arrange
            mockUnitOfWork.Setup(x => x.AppUserRepository).Returns(mockAppUserRepository.Object);
            mockUnitOfWork.Setup(x => x.AppUserRepository.ListAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expectedList);
            var appUserDto = new AppUserDto("abc123", "title1", CoreBridge.Models.AdminUserRoleEnum.AdminUser, "password1", "abc123_update@email.com", "password1", "");

            //Act
            var result = await service.UpdateAsync(appUserDto);

            //Assert
            Assert.NotNull(result);
            mockUnitOfWork.Verify(x => x.AppUserRepository.UpdateAsync(It.IsAny<AppUser>()), Times.Once);
            mockUnitOfWork.Verify(x => x.CommitAsync(), Times.Once);
            Assert.Equal(appUserDto, result);
        }

        [Fact]
        public async Task UpdateAsync_UserNotExist_ReturnsNull()
        {
            //Arrange
            mockUnitOfWork.Setup(x => x.AppUserRepository).Returns(mockAppUserRepository.Object);
            mockUnitOfWork.Setup(x => x.AppUserRepository.ListAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expectedList);
            var appUserDto = new AppUserDto("xyz123", "title1", CoreBridge.Models.AdminUserRoleEnum.AdminUser, "password1", "abc123_update@email.com", "password1", "");

            //Act
            var result = await service.UpdateAsync(appUserDto);

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task FindTitleCode_ReturnsFalse()
        {
            // Arrange
            string titleCode = "title1";
            mockUnitOfWork.Setup(x => x.AppUserRepository.ListAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expectedList);
            mockMapper.Setup(x => x.Map<List<AppUserDto>>(expectedList)).Returns(expectedDtoList);
            // Act
            var result = await service.FindTitleCode(titleCode);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task FindTitleCode_TitleNotExist_ReturnsTrue()
        {
            // Arrange
            string titleCode = "title5";
            mockUnitOfWork.Setup(x => x.AppUserRepository.ListAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expectedList);
            mockMapper.Setup(x => x.Map<List<AppUserDto>>(expectedList)).Returns(expectedDtoList);
            // Act
            var result = await service.FindTitleCode(titleCode);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task GenerateAppUser_ReturnsExpectedResult()
        {
            //Arrange
            mockUnitOfWork.Setup(x => x.AppUserRepository).Returns(mockAppUserRepository.Object);
            var appUserDto = new AppUserDto("abc123", "title1", CoreBridge.Models.AdminUserRoleEnum.AdminUser, "password1", "abc123@email.com", "password1", "");

            //Act
            var result = await service.GenerateAppUser(appUserDto);

            //Assert
            mockUnitOfWork.Verify(x => x.AppUserRepository.AddAsync(It.IsAny<AppUser>()), Times.Once);
            mockUnitOfWork.Verify(x => x.CommitAsync(), Times.Once);
            Assert.Equal(appUserDto, result);
        }

        //[Fact] 
        //public async Task DeleteAsync_UserNotExists_ReturnsNull()
        //{
        //    //Arrange
        //    var appUserDto = new AppUserDto("abc123", "title1", CoreBridge.Models.AdminUserRoleEnum.AdminUser, "password1", "abc123@email.com", "password1", "");
        //    mockUnitOfWork.Setup(x => x.AppUserRepository.ListAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<AppUser> { new AppUser { UserId = "bcd123" } });

        //    //Act
        //    var result = await service.DeleteAsync(appUserDto);

        //    //Assert
        //    Assert.Null(result);
        //}
    }
}

