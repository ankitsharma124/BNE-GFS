using System;
using CoreBridge.Models.DTO.Requests;
using CoreBridge.Models.Exceptions;
using CoreBridge.Services;
using CoreBridge.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Moq;

namespace CoreBridgeTest.ServiceTest
{
    public class TestReqBaseParam : ReqBaseParam
    {
        public new bool? MaintenanceAvoid { get; set; }
    }

    public class MaintenanceServiceTest
	{
        Mock<ISessionStatusService> mockISessionStatusService;
        Mock<IDistributedCache> mockIDistributedCache;
        MaintenanceService maintenanceService;
        public MaintenanceServiceTest()
        { 
            mockIDistributedCache = new Mock<IDistributedCache>();
            mockISessionStatusService = new Mock<ISessionStatusService>();
            maintenanceService = new MaintenanceService(mockISessionStatusService.Object, mockIDistributedCache.Object);
        }

        [Fact]
        public async Task CheckMaintenanceStatus_ReturnsNull()
        {
            try
            {
                // Arrange
                var cacheMock = new Mock<IDistributedCache>();
                mockISessionStatusService.Setup(x => x.ReqParam).Returns(new TestReqBaseParam { MaintenanceAvoid = false });
                mockISessionStatusService.Setup(x => x.UserId).Returns("abc123");
                mockISessionStatusService.Setup(x => x.ApiCode).Returns(1);

                // Act and Assert
                await maintenanceService.CheckMaintenanceStatus(true);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        //[Fact]
        //public async Task CheckMaintenanceStatus_ThrowException()
        //{
        //    try
        //    {
        //        // Arrange
        //        var cacheMock = new Mock<IDistributedCache>();
        //        mockISessionStatusService.Setup(x => x.ReqParam).Returns(new ReqBaseParam{ MaintenanceAvoid = false });
        //        mockISessionStatusService.Setup(x => x.UserId).Returns("abc123");
        //        mockISessionStatusService.Setup(x => x.ApiCode).Returns(1);

        //        // Act and Assert
        //        await maintenanceService.CheckMaintenanceStatus(true);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

    }
}

