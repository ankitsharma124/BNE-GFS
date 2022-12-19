using AutoMapper;
using CoreBridge.Models.DTO;
using CoreBridge.Models.Entity;
using CoreBridge.Models.Interfaces;
using CoreBridge.Services.Interfaces;
using CoreBridge.Specifications;

namespace CoreBridge.Services
{
    public class UserPlatformService : IUserPlatformService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UserPlatformService(IUnitOfWork unit, IMapper mapper)
        {
            _unitOfWork = unit;
            _mapper = mapper;
        }

        public async Task<UserPlatformDto> GetByIdAsync(string id)
        {
            return _mapper.Map<UserPlatformDto>(await _unitOfWork.UserPlatformRepository.GetByIdAsync(id));
        }

        public async Task<UserPlatformDto> GetByUserIdAsync(string userId)
        {
            UserPlatformSpecification spec = new();
            spec.FindByUserId(userId);

            UserPlatform entity = await _unitOfWork.UserPlatformRepository.GetBySpecAsync(spec);
            UserPlatformDto result = _mapper.Map<UserPlatformDto>(entity);
            return result;
        }
    }
}
