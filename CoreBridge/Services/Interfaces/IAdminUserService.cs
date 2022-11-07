using CoreBridge.Models.DTO;

namespace CoreBridge.Services.Interfaces
{
    public interface IAdminUserService
    {
        public Task<AdminUserDto> GenerateAdminUser(AdminUserDto dto);
        public Task<AdminUserDto> LoginAdminUser(AdminUserDto dto);
        public Task<List<AdminUserDto>> ListAsync();

    }
}
