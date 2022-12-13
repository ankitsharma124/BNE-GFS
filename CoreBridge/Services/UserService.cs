using CoreBridge.Models.DTO;
using CoreBridge.Models.Interfaces;
using CoreBridge.Services.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using CoreBridge.Models.Extensions;
using CoreBridge.Models;
using CoreBridge.Models.DTO.Requests;
using CoreBridge.Models.Exceptions;
using System.Reflection.Emit;
using XAct.Users;

namespace CoreBridge.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unit;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;
        private readonly ISessionStatusService _sss;
        private readonly ILogger<UserService> _logger;
        public UserService(IUnitOfWork unit, IMapper mapper, IDistributedCache cache,
            ILogger<UserService> logger, ISessionStatusService sss)
        {
            _unit = unit;
            _mapper = mapper;
            _cache = cache;
            _sss = sss;
            _logger = logger;
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
                throw new Exception("ここには来ない");
            }
        }

        public async Task LoadStatus_UserInfo()
        {
            _sss.UserInfo = await GetByIdAsync(_sss.UserId);
        }

        public async Task<GFSUserDto> GetByIdAsync(string id)
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
                user = await GetByIdAsyncInner(id);
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

        private async Task<GFSUserDto> GetByIdAsyncInner(string id)
        {
            var entity = await _unit.UserRepository.GetByIdAsync(id);
            return _mapper.Map<GFSUserDto>(entity);
        }

        /// <summary>
        /// memo: user gets loaded here
        /// </summary>
        /// <returns></returns>
        /// <exception cref="BNException"></exception>
        public async Task CheckUserConsistency()
        {
            //todo: check - TEMPORARY_CREDENTIAL_API_LISTの一覧とは？
#if DEBUG
            _logger.LogDebug("debug", "ユーザーの整合性チェック");
#endif
            if (_sss.ReqParam.IsOrDescendantOf(typeof(ReqBaseParamHeader)) &&
                ((ReqBaseParamHeader)_sss.ReqParam).HasTemporaryCredential())
            {
#if DEBUG
                _logger.LogDebug("debug", "ユーザーの整合性チェックをスキップ");
#endif
                return;
            }

            if (_sss.UserId == null)
            {
                throw new BNException(_sss.ApiCode, BNException.BNErrorCode.UserNotExist, BNException.ErrorLevel.Error,
                    "user_consistence:パラメータにユーザーIDが存在しません");
            }

            var titleCode = _sss.SkuType == (int)SysConsts.SkuType.Product ? _sss.TitleCode :
                _sss.TitleInfo.TrialTitleCode;

            await LoadStatus_UserInfo();

            if (_sss.UserInfo == null)
            {
                throw new BNException(_sss.ApiCode, BNException.BNErrorCode.UserNotExist, BNException.ErrorLevel.Error, "user_consistence:error");
            }

            if (_sss.UserInfo.Platform != _sss.Platform)
                throw new BNException(_sss.ApiCode, BNException.BNErrorCode.RequestErr, BNException.ErrorLevel.Error, "user_consistence:ユーザー情報とプラットフォームが相違している");

            // ここに来ることはないはず(SKU種別が変わった場合はセッションID側でエラーにしてる)
            if (_sss.UserInfo.TitleCode != _sss.TitleCode)
                throw new BNException(_sss.ApiCode, BNException.BNErrorCode.UserNotExist, BNException.ErrorLevel.Error, "user_consistence:ユーザー情報とタイトルコードが相違している");
        }
    }
}
