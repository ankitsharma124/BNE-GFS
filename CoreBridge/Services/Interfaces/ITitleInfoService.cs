using CoreBridge.Models.DTO;
using CoreBridge.Models.Entity;

namespace CoreBridge.Services.Interfaces
{
    public interface ITitleInfoService 
    {
        public Task<List<TitleInfoDto>> ListAsync();
        public Task<TitleInfoDto> AddAsync(TitleInfoDto titleInfo);
        public Task<TitleInfoDto> UpdateAsync(TitleInfoDto titleInfo);
        public Task<TitleInfoDto> DetachAsync(TitleInfoDto titleInfo);
        public Task<TitleInfoDto> DeleteAsync(TitleInfoDto titleInfo);
    }
}
