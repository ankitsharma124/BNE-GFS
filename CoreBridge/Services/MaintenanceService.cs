using CoreBridge.Models;
using CoreBridge.Models.DTO.Requests;
using CoreBridge.Models.Exceptions;
using CoreBridge.Models.Extensions;
using CoreBridge.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Reflection.Emit;
using System.Security.Permissions;

namespace CoreBridge.Services
{
    public class MaintenanceService : IMaintenanceService
    {
        private readonly ISessionStatusService _sss;
        private readonly IDistributedCache _cache;
        public MaintenanceService(ISessionStatusService sss, IDistributedCache cache)
        {
            _sss = sss;
            _cache = cache;
        }

        public async Task CheckMaintenanceStatus(bool isCommon)
        {
            if (!_sss.ReqParam.IsOrDescendantOf(typeof(ReqBaseParam))
                || (bool?)((ReqBaseParam)_sss.ReqParam).MaintenanceAvoid != true)
            {
                return;
            }
            var inMaintenance = false;
            if (isCommon)
            {
                inMaintenance = CheckTitleMaintenance((int)SysConsts.ModeType.AllKey);
            }
            else
            {
                inMaintenance = CheckTitleMaintenance((int)SysConsts.ModeType.IndividualKey);
            }

            inMaintenance = CheckReqApiMaintenance() ? true : inMaintenance;

            if (inMaintenance)
            {
                inMaintenance = CheckUserMaintenanceOverride(_sss.UserId) ? false : inMaintenance;
            }
            if (inMaintenance)
            {
                throw new BNException(_sss.ApiCode, BNException.BNErrorCode.Maintenance);
            }
        }

        private bool CheckTitleMaintenance(int modeKey)
        {
            int getMode;
            var success = _cache.TryGetValue<int>(SysConsts.SYSTEM_MAINTENANCE_KEY + ":" + _sss.Platform, out getMode);
            if (success && getMode == modeKey) return true;
            return false;
        }

        private bool CheckReqApiMaintenance()
        {
            var maintenanceKey = _sss.ReqPath;
            object code = null;
            var scs = _cache.TryGetValue(SysConsts.SYSTEM_MAINTENANCE_KEY + ":" + maintenanceKey + ":" + platform,
                out code);

            if (scs && code != null)
            {
                return true;
            }
            return false;
        }

        private bool CheckUserMaintenanceOverride(string? userId)
        {
            object code = null;
            var scs = _cache.TryGetValue(SysConsts.SYSTEM_MAINTENANCE_KEY + ":" + userId,
                out code);

            if (scs && code != null)
            {
                return true;
            }
            return false;
        }

    }
}
