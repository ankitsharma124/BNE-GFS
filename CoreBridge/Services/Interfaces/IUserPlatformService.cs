using CoreBridge.Models.DTO;

namespace CoreBridge.Services.Interfaces
{
    public interface IUserPlatformService
    {
        Task<UserPlatformDto> GetByIdAsync(string id);
        Task<UserPlatformDto> GetByUserIdAsync(string userId);
    }
}