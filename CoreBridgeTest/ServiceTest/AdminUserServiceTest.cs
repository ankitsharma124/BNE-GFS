using System;
using CoreBridge.Models.DTO;
using CoreBridge.Models.Entity;
using CoreBridge.Models.Interfaces;
using CoreBridge.Services;
using CoreBridge.Specifications;
using Microsoft.Extensions.Logging;
using Moq;

namespace CoreBridgeTest.ServiceTest
{
	public class AdminUserServiceTest
	{
        Mock<ICoreBridgeRepository<AdminUser>> mocAdminUserRepository;
        Mock<IUnitOfWork> mockUnitOfWork;
        Mock<ILogger<AdminUserService>> mockLogger;
        AdminUserService service;
        List<AdminUser> expectedList;

        public AdminUserServiceTest()
        {
            mocAdminUserRepository = new Mock<ICoreBridgeRepository<AdminUser>>();
            mockUnitOfWork = new Mock<IUnitOfWork>();
            mockLogger = new Mock<ILogger<AdminUserService>>();
            service = new AdminUserService(mockUnitOfWork.Object, mockLogger.Object);
            expectedList = new List<AdminUser>
            {
                new AdminUser("John Smith", "johnsmith@example.com","password1"),
                new AdminUser("Jane Doe","janedoe@example.com", "password2")
            };
        }
        [Fact]
        public async Task ListAsync_ReturnsExpectedResult()
        {
            // Arrange
            mockUnitOfWork.Setup(uow =>  uow.AdminUserRepository.ListAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expectedList);

            // Act
            var result = await service.ListAsync();

            // Assert
            Assert.Equal(expectedList.Count, result.Count);
            for (int i = 0; i < expectedList.Count; i++)
            {
                Assert.Equal(expectedList[i].Name, result[i].Name);
                Assert.Equal(expectedList[i].EMail, result[i].EMail);
                Assert.Equal(expectedList[i].Password, result[i].Password);
            }
        }

        [Fact]
        public async Task GenerateAdminUser_AddsAdminUserToRepository_AndReturnsDto()
        {
            // Arrange
            var dto = new AdminUserDto { Name = "John Doe", EMail = "johndoe@example.com", Password = "password123", ConfirmPassword= "password123" };
            mockUnitOfWork.Setup(uow => uow.AdminUserRepository).Returns(mocAdminUserRepository.Object);

            // Act
            var result = await service.GenerateAdminUser(dto);

            // Assert
            mocAdminUserRepository.Verify(repo => repo.AddAsync(It.IsAny<AdminUser>()), Times.Once());
            mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once());
            Assert.Equal(dto, result);
        }

        [Fact]
        public async Task LoginAdminUser_ReturnsAdminUserDto_WhenEmailAndPasswordAreValid()
        {
            // Arrange
            var adminUser = new AdminUser("John Doe", "johndoe@example.com", "password123");
            mocAdminUserRepository.Setup(repo => repo.GetBySpecAsync(It.IsAny<AdminUserSpecification>(), It.IsAny<CancellationToken>())).ReturnsAsync(adminUser);
            mockUnitOfWork.Setup(uow => uow.AdminUserRepository).Returns(mocAdminUserRepository.Object);
            var dto = new AdminUserDto { EMail = "johndoe@example.com", Password = "password123" };

            // Act
            var result = await service.LoginAdminUser(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(adminUser.Name, result.Name);
            Assert.Equal(adminUser.EMail, result.EMail);
            Assert.Equal(adminUser.Password, result.Password);
        }
    }
}

