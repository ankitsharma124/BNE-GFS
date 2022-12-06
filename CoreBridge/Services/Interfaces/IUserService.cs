using CoreBridge.Models.DTO;

namespace CoreBridge.Services.Interfaces
{
    public interface IUserService
    {
        public Task<GFSUserDto> GetByIdAsync(string id);
    }
}
