using CoreBridge.Models.DTO;

namespace CoreBridge.Services.Interfaces
{
    public interface ITitleInfoService
    {
        Task<TitleInfoDto> GetByCodeAsync(string titleCode);


    }
}
