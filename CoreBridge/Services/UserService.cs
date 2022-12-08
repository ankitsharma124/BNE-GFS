using CoreBridge.Models.DTO;
using CoreBridge.Models.Interfaces;
using CoreBridge.Services.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using CoreBridge.Models.Extensions;
using CoreBridge.Models;

namespace CoreBridge.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unit;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;
        private readonly ISessionStatusService _sss;
        private readonly ILogger _logger;
        public UserService(IUnitOfWork unit, IMapper mapper, IDistributedCache cache,
            ILogger logger, ISessionStatusService sss)
        {
            _unit = unit;
            _mapper = mapper;
            _cache = cache;
            _sss = sss;
            _logger = logger;
        }

        public async Task<GFSUserDto> GetByIdAsync(string id)
        {
            var entity = await _unit.UserRepository.GetByIdAsync(id);
            return _mapper.Map<GFSUserDto>(entity);
        }



        private string GetUserInfoKey()
        {
            if (_sss.IsClientApi)
            {
                return String.Format(SysConsts.UserInfoKeyBase_Client, _sss.UserId);
            }
            else if (_sss.IsServerApi)
            {
                return String.Format(SysConsts.UserInfoKeyBase_Server, _sss.UserId, _sss.SkuType);
            }
            else
            {
                throw new Exception('ここには来ない');
            }
        }

        public async Task<GFSUserDto> LoadCurrentUser()
        {
#if DEBUG
            // memo: must initialize sss with header params before calling this
            if (_sss.UserId == null)
            {
                throw new Exception("initialize sss with header params before calling this");
            }
#endif
            GFSUserDto user;
            var success = _cache.TryGetValue<GFSUserDto>(GetUserInfoKey(), out user);
            if (user == null)
            {
#if DEBUG
                _logger.LogDebug("UserInfoをSpannerから取得");
#endif
                user = await GetByIdAsync(_sss.UserId);
                if (user != null)
                {
                    await _cache.SetAsync(GetUserInfoKey(), user);
                }
            }
#if DEBUG
            else
            {
                _logger.LogDebug("UserInfoをRedisから取得");
            }
#endif
            return user;
        }

    }
}
