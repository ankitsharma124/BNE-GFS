using System;
using CoreBridge.Models.Interfaces;
using CoreBridge.Services.Interfaces;

namespace CoreBridge.Models.lib
{
    public class UserOperation
    {
        private const int CREATE_ROOP_MAX = 10;
        private readonly IAppUserService _appUserService;

        public UserOperation(IAppUserService appUserService)
        {
            _appUserService = appUserService;
        }

        /// <summary>
        /// ユーザーID作成
        /// </summary>
        /// <returns>生成したユーザーID</returns>
        public async Task<string> CreateUserId()
        {
            string retId = string.Empty;
            Random rand = new Random();

            int val = CREATE_ROOP_MAX;
            do
            {
                //自動生成
                DateTime dt = DateTime.Now;
                // 0000～9999 + 年月日時分秒(COSMOS踏襲)
                var creteId = rand.Next(10000).ToString("D4") + dt.ToString("yyyymmddHHmmss");

                //既存IDとの不一致確認
                var checkok = await _appUserService.GetByUserIdAsync(creteId);
                if (checkok == null)
                {
                    return creteId;
                }

                val--;

            } while (val > 0);

            return retId;
        }
    }
}
