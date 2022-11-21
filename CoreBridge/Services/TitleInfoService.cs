using CoreBridge.Models.DTO;
using CoreBridge.Models.Entity;
using CoreBridge.Models.Interfaces;
using CoreBridge.Services.Interfaces;
using CoreBridge.Specifications;

namespace CoreBridge.Services
{
    public class TitleInfoService : ITitleInfoService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly ILogger<TitleInfoService> _logger;

        public TitleInfoService(IUnitOfWork unitOfWork, ILogger<TitleInfoService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<TitleInfoDto>> ListAsync()
        {
            List<TitleInfoDto> result = new();
            var list = await _unitOfWork.TitleInfoRepository.ListAsync();
            foreach (TitleInfo entity in list)
            {
                result.Add(new(
                    entity.TitleName,
                    entity.TitleCode,
                    entity.TrialTitleCode,
                    entity.Ptype,
                    entity.SwitchAppId,
                    entity.XboxTitleId,
                    entity.PsClientId,
                    entity.PsClientSecoret,
                    entity.SteamAppId,
                    entity.SteamPublisherKey,
                    entity.DevUrl,
                    entity.QaUrl,
                    entity.ProdUrl));
            }

            return result;
        }

        public async Task<TitleInfoDto> GenerateTitleInfo(TitleInfoDto dto)
        {

            TitleInfo entity = new(
                    dto.TitleName,
                    dto.TitleCode,
                    dto.TrialTitleCode,
                    dto.Ptype,
                    dto.SwitchAppId,
                    dto.XboxTitleId,
                    dto.PsClientId,
                    dto.PsClientSecoret,
                    dto.SteamAppId,
                    dto.SteamPublisherKey,
                    dto.DevUrl,
                    dto.QaUrl,
                    dto.ProdUrl);

            await _unitOfWork.TitleInfoRepository.AddAsync(entity);
            await _unitOfWork.CommitAsync();

            return dto;
        }

        public async Task<TitleInfoDto> ActionTitleInfo(TitleInfoDto dto)
        {
            TitleInfoSpecification spec = new();
            //spec.FindByEmail(dto.EMail);

            TitleInfo entity = await _unitOfWork.TitleInfoRepository.GetBySpecAsync(spec);
            TitleInfoDto result = null;
            if (IsValidLogin(dto, entity))
                result = new(
                    entity.TitleName,
                    entity.TitleCode,
                    entity.TrialTitleCode,
                    entity.Ptype,
                    entity.SwitchAppId,
                    entity.XboxTitleId,
                    entity.PsClientId,
                    entity.PsClientSecoret,
                    entity.SteamAppId,
                    entity.SteamPublisherKey,
                    entity.DevUrl,
                    entity.QaUrl,
                    entity.ProdUrl);

            return result;
        }

        private bool IsValidLogin(TitleInfoDto dto, TitleInfo entity)
        {

            if (dto == null || entity == null)
                return false;

            //if (dto.EMail != entity.EMail)
            //    return false;


            //if (dto.Password != entity.Password)
            //    return false;

            return true;
        }
    }
}
