namespace CoreBridge.Services.Interfaces
{
    public interface IMaintenanceService
    {
        /// <summary>
        /// 現在のメンテ状況を確認。
        /// メンテ中にてアクセス不可であればexception（BNErrorCode.Maintenance）
        /// </summary>
        /// <param name="isCommon"></param>
        /// <returns></returns>
        Task CheckMaintenanceStatus(bool isCommon);
    }
}