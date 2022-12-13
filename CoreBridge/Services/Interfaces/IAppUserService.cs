using CoreBridge.Models.DTO;
using CoreBridge.Models.Entity;

namespace CoreBridge.Services.Interfaces
{
    public interface IAppUserService
    {
        public Task<List<AppUser>> FindAsync();
        public Task<AppUserDto?> AddAsync(AppUserDto dto);
        public Task<AppUser?> GetByIdAsync(string id);
        public Task<AppUserDto?> UpdateAsync(AppUserDto dto);
        public Task<AppUserDto> DetachAsync(AppUserDto dto);
        public Task<AppUser> DeleteAsync(AppUser dto);
        public Task<bool> FindTitleCode(string code);
        public Task<bool> GetByUserIdAsync(string id);
    }
}
