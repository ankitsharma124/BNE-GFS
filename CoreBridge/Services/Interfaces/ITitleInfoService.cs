using CoreBridge.Models.DTO;

namespace CoreBridge.Services.Interfaces
{
    public interface ITitleInfoService
    {
        public Task<TitleInfoDto> GenerateTitleInfo(TitleInfoDto dto);
        public Task<TitleInfoDto> ActionTitleInfo(TitleInfoDto dto);
        public Task<List<TitleInfoDto>> ListAsync();
    }
}
