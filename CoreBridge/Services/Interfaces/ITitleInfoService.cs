using CoreBridge.Models.DTO;
using CoreBridge.Models.Entity;

namespace CoreBridge.Services.Interfaces
{
    public interface ITitleInfoService
    {
        public Task<List<TitleInfoDto>> FindAsync();
        public Task<TitleInfoDto?> AddAsync(TitleInfoDto dto);
        public Task<TitleInfo?> GetByIdAsync(string id);
        public Task<TitleInfoDto> UpdateAsync(TitleInfoDto dto);
        public Task<TitleInfoDto> DetachAsync(TitleInfoDto dto);
        public Task<TitleInfo> DeleteAsync(TitleInfo dto);
        public Task<bool> FindTitleCode(string titlecode);
        Task<TitleInfoDto> GetByCodeAsync(string titleCode);

        Task LoadStatus_TitleInfo(string titleCode);


    }
}
