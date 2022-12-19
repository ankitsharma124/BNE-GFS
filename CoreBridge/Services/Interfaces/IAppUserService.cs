using CoreBridge.Models.DTO;
using CoreBridge.Models.Entity;

namespace CoreBridge.Services.Interfaces
{
    public interface IAppUserService
    {
        public Task<AppUserDto> GenerateAdminUser(AppUserDto dto);
        public Task<List<AppUser>> FindAsync();
        public Task<AppUserDto?> AddAsync(AppUserDto dto);
        public Task<AppUser?> GetByIdAsync(string id);
        public Task<AppUserDto?> UpdateAsync(AppUserDto dto);
        public Task<AppUserDto> DetachAsync(AppUserDto dto);
        public Task<AppUserDto> DeleteAsync(AppUserDto dto);
        public Task<bool> FindTitleCode(string code);
        public Task<AppUserDto?> GetByUserIdAsync(string id);
    }
}
