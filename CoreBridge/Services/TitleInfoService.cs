﻿using AutoMapper;
using CoreBridge.Models.DTO;
using CoreBridge.Models.Entity;
using CoreBridge.Models.Interfaces;
using CoreBridge.Models.Repositories;
using CoreBridge.Services.Interfaces;
using CoreBridge.Specifications;
using Hangfire.Server;
using StackExchange.Redis;

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

        public async Task<List<TitleInfoDto>> ListAsync()
        {
            var list = await _unitOfWork.TitleInfoRepository.ListAsync();
            return _mapper.Map<List<TitleInfoDto>>(list);
        }

        public async Task<TitleInfoDto> AddAsync(TitleInfoDto dto)
        {
            await _unitOfWork.TitleInfoRepository.AddAsync(_mapper.Map<TitleInfo>(dto));
            await _unitOfWork.CommitAsync();
            return dto;
        }

        public async Task<TitleInfoDto> UpdateAsync(TitleInfoDto dto)
        {
            return dto;
        }

        public async Task<TitleInfoDto> DetachAsync(TitleInfoDto dto)
        {
            return dto;
        }

        public async Task<TitleInfoDto> DeleteAsync(TitleInfoDto dto)
        {
            return dto;
        }

        //public async Task<TitleInfoDto> GenerateTitleInfo(TitleInfoDto dto)
        //{

        //    TitleInfo entity = _mapper.Map<TitleInfo>(dto);

        //    await _unitOfWork.TitleInfoRepository.AddAsync(entity);
        //    await _unitOfWork.CommitAsync();

        //    return dto;
        //}

        //public async Task<TitleInfoDto> ActionTitleInfo(TitleInfoDto dto)
        //{
        //    TitleInfo spec = new();
        //    //spec.FindByEmail(dto.EMail);

        //    TitleInfo entity = await _unitOfWork.TitleInfoRepository.GetBySpecAsync(spec);
        //    TitleInfoDto result = null;
        //    if (IsValidLogin(dto, entity))
        //        result = new(
        //            entity.TitleName,
        //            entity.TitleCode,
        //            entity.TrialTitleCode,
        //            entity.Ptype,
        //            entity.SwitchAppId,
        //            entity.XboxTitleId,
        //            entity.PsClientId,
        //            entity.PsClientSecoret,
        //            entity.SteamAppId,
        //            entity.SteamPublisherKey,
        //            entity.DevUrl,
        //            entity.QaUrl,
        //            entity.ProdUrl
        //            );

        //    return result;
        //}

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
