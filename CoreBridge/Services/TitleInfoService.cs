using AutoMapper;
using CoreBridge.Models.DTO;
using CoreBridge.Models.Entity;
using CoreBridge.Models.Interfaces;
using CoreBridge.Models.Repositories;
using CoreBridge.Services.Interfaces;
using CoreBridge.Specifications;
using Hangfire.Server;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using StackExchange.Redis;
using System.IO.Enumeration;
using System.Transactions;

namespace CoreBridge.Services
{
    public class TitleInfoService : ITitleInfoService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly ILogger<TitleInfoService> _logger;
        private readonly IMapper _mapper;

        public TitleInfoService(IUnitOfWork unitOfWork, ILogger<TitleInfoService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            //Auto Mapper Setting.
            var mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<TitleInfoDto, TitleInfo>());
            _mapper = new Mapper(mapConfig);
        }

        public async Task<TitleInfoDto> GetByCodeAsync(string titleCode)
        {
            var entity = await _unitOfWork.TitleInfoRepository.GetByIdAsync(titleCode);
            return _mapper.Map<TitleInfoDto>(entity);
        }

        public async Task<List<TitleInfoDto>> FindAsync()
        {
            var list = await _unitOfWork.TitleInfoRepository.ListAsync();
            return _mapper.Map<List<TitleInfoDto>>(list);
        }

        public async Task<TitleInfoDto?> AddAsync(TitleInfoDto dto)
        {
            //タイトルコードの重複は阻止する
            var titleInfos = await _unitOfWork.TitleInfoRepository.ListAsync();
            var targetTitleCode = titleInfos.Find(e => e.TitleCode == dto.TitleCode);
            if(targetTitleCode != null)
            {
                return null;
            }
            var targetTrialTitleCode = titleInfos.Find(e => e.TrialTitleCode == dto.TrialTitleCode);
            if (targetTrialTitleCode != null)
            {
                return null;
            }

            await _unitOfWork.TitleInfoRepository.AddAsync(_mapper.Map<TitleInfo>(dto));
            await _unitOfWork.CommitAsync();
            return dto;
        }

        public async Task<TitleInfo?> GetByIdAsync(string id)
        {
            var titleInfos = await _unitOfWork.TitleInfoRepository.ListAsync();
            var targetInfo = titleInfos.Find(e => e.TitleCode == id);
            if(targetInfo == null)
            {
                return null;
            }
            return await _unitOfWork.TitleInfoRepository.GetByIdAsync(targetInfo.Id);
        }

        public async Task<TitleInfoDto?> UpdateAsync(TitleInfoDto dto)
        {
            var titleInfos = await _unitOfWork.TitleInfoRepository.ListAsync();
            var targetInfo = titleInfos.Find(e => e.TitleCode == dto.TitleCode);
            if(titleInfos == null)
            {
                return null;
            }

            //データ更新
            targetInfo.TitleName = dto.TitleName;
            targetInfo.TitleCode = dto.TitleCode;
            targetInfo.TrialTitleCode = dto.TrialTitleCode;
            targetInfo.Ptype = dto.Ptype;
            targetInfo.SwitchAppId = dto.SwitchAppId;
            targetInfo.XboxTitleId = dto.XboxTitleId;
            targetInfo.PsClientId = dto.PsClientId;
            targetInfo.PsClientSecoret = dto.PsClientSecoret;
            targetInfo.SteamAppId = dto.SteamAppId;
            targetInfo.SteamPublisherKey = dto.SteamPublisherKey;
            targetInfo.DevUrl = dto.DevUrl;
            targetInfo.QaUrl = dto.QaUrl;
            targetInfo.ProdUrl = dto.ProdUrl;

            await _unitOfWork.TitleInfoRepository.UpdateAsync(targetInfo);
            await _unitOfWork.CommitAsync();
            return dto;
        }

        public async Task<TitleInfoDto> DetachAsync(TitleInfoDto dto)
        {
            await _unitOfWork.TitleInfoRepository.DetachAsync(_mapper.Map<TitleInfo>(dto));
            await _unitOfWork.CommitAsync();
            return dto;
        }

        public async Task<TitleInfo> DeleteAsync(TitleInfo dto)
        {
            await _unitOfWork.TitleInfoRepository.DeleteAsync(_mapper.Map<TitleInfo>(dto));
            await _unitOfWork.CommitAsync();
            return dto;
        }
    }
}
