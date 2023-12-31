﻿using AutoMapper;
using CoreBridge.Models.DTO;
using CoreBridge.Models.Entity;
using CoreBridge.Models.Exceptions;
using CoreBridge.Models.Interfaces;
using CoreBridge.Services.Interfaces;

namespace CoreBridge.Services
{
    public class AppUserService : IAppUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AppUserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

            //Auto Mapper Setting.
            //var mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<AppUserDto, AppUser>());
            //_mapper = new Mapper(mapConfig);
            _mapper = mapper;
        }

        public async Task<AppUserDto?> AddAsync(AppUserDto dto)
        {
            //UserIdの重複は阻止する
            var appUserInfo = await _unitOfWork.AppUserRepository.ListAsync();
            var targetTitleCode = appUserInfo.Find(e => e.UserId == dto.UserId);
            if (targetTitleCode != null)
            {
                return null;
            }

            await _unitOfWork.AppUserRepository.AddAsync(_mapper.Map<AppUser>(dto));
            await _unitOfWork.CommitAsync();
            return dto;
        }

        public async Task<AppUserDto> DeleteAsync(AppUserDto dto)
        {
            var appUserInfo = await _unitOfWork.AppUserRepository.ListAsync();
            var targetInfo = appUserInfo.Find(e => e.UserId == dto.UserId);

            await _unitOfWork.AppUserRepository.DeleteAsync(_mapper.Map<AppUser>(targetInfo));
            await _unitOfWork.CommitAsync();
            return dto;
        }

        public async Task<AppUserDto> DetachAsync(AppUserDto dto)
        {
            await _unitOfWork.AppUserRepository.DetachAsync(_mapper.Map<AppUser>(dto));
            await _unitOfWork.CommitAsync();
            return dto;
        }

        public async Task<List<AppUser>> FindAsync()
        {
            var list = await _unitOfWork.AppUserRepository.ListAsync();

            //ここだけAppUserで返したいので、automapperを個別設定します
            var mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<AppUser, AppUserDto>());
            var findMapper = new Mapper(mapConfig);

            return findMapper.Map<List<AppUser>>(list);
        }

        public async Task<List<AppUserDto>> ListAsync()
        {
            var list = await _unitOfWork.AppUserRepository.ListAsync();
            return _mapper.Map<List<AppUserDto>>(list);
        }

        public async Task<AppUser?> GetByIdAsync(string id)
        {
            var appUserInfo = await _unitOfWork.AppUserRepository.ListAsync();
            var targetInfo = appUserInfo.Find(e => e.Id == id);
            if (targetInfo == null)
            {
                return null;
            }
            return await _unitOfWork.AppUserRepository.GetByIdAsync(targetInfo.Id);
        }

        /// <summary>
        /// UserIDの検出
        /// </summary>
        /// <param name="id">UserID</param>
        /// <returns>True:新規 False:既存</returns>
        public async Task<AppUserDto?> GetByUserIdAsync(string id)
        {
            var appUserInfo = await _unitOfWork.AppUserRepository.ListAsync();
            var targetInfo = appUserInfo.Find(e => e.UserId == id);
            if(targetInfo == null)
            {
                return null;
            }

            var entry = await _unitOfWork.AppUserRepository.GetByIdAsync(targetInfo.Id);
            var mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<AppUser, AppUserDto>());
            var retMapper = new Mapper(mapConfig);

            return retMapper.Map<AppUserDto>(entry) ;
        }

        public async Task<AppUserDto?> UpdateAsync(AppUserDto dto)
        {
            var appInfo = await _unitOfWork.AppUserRepository.ListAsync();
            var targetInfo = appInfo.Find(e => e.UserId == dto.UserId);
            if(targetInfo == null)
            {
                return null;
            }

            //データ更新
            targetInfo.UserId = dto.UserId;
            targetInfo.TitleCode = dto.TitleCode;
            targetInfo.Role = dto.Role;
            targetInfo.Email = dto.Email;
            if (dto.Password != null)
            {
                targetInfo.Password = dto.Password;
            }
            targetInfo.UpdateUser = dto.UpdateUser;
            targetInfo.IsDelete = dto.IsDelete;

            await _unitOfWork.AppUserRepository.UpdateAsync(targetInfo);
            await _unitOfWork.CommitAsync();
            return dto;
        }

        public async Task<bool> FindTitleCode(string code)
        {
            var targetInfo = await _unitOfWork.AppUserRepository.ListAsync();
            var targetTitleCode = targetInfo.Find(e => e.TitleCode == code);
            if (targetTitleCode == null)
            {
                return true;
            }
            return false;
        }

        public async Task<AppUserDto> GenerateAppUser(AppUserDto dto)
        {
            AppUser entity = new(dto.UserId, dto.TitleCode, dto.Role, dto.Email, dto.Password, dto.UpdateUser);

            await _unitOfWork.AppUserRepository.AddAsync(entity);
            await _unitOfWork.CommitAsync();

            return dto;
        }
    }
}
