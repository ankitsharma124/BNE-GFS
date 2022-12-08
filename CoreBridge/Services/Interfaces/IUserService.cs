using CoreBridge.Models.DTO;

namespace CoreBridge.Services.Interfaces
{
    public interface IUserService
    {
        Task<GFSUserDto> GetByIdAsync(string id);

        Task<GFSUserDto> LoadCurrentUser()
    }
}
