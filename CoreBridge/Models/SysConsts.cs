namespace CoreBridge.Models
{
    public class SysConsts
    {
        public const string AddedReqHeaderKey_Hash = "CBHash";
        public const string AddedReqHeaderKey_ReqBodyLen = "CBReqLen";
        public const string AddedReqHeaderKey_OriginalBody = "CBOrigBody";
        public const string AddedReqHeaderKey_Debug_ReqBody = "CBDebug_ReqBody";


        #region ------- システム設定 -------------

        public const string SYSTEM_MAINTENANCE_KEY = "maintenance";


        /// <summary>
        /// メンテ用のキー
        /// </summary>
        public enum ModeType
        {
            AllKey = 1,
            IndividualKey = 2
        }
        #endregion

        /// <summary>
        /// メンテナンスモード判定用のkey
        /// </summary>
        public enum SkuType
        {
            Product = 0,
            Trial = 1
        }

        public enum Platform
        {
            Psn = 1,
            XBox = 2,
            Steam = 3,
            Switch = 4,
            WeGames = 5,
            IOs = 6,
            Android = 7,
            Ps5 = 8,
            XBoxSx = 9,
            Stadia = 10,
            GamePass = 11,
            Data = 98,
            Server = 99
        }

        public static int[] PlatformList = new int[] {
            (int)Platform.Psn,
            (int)Platform.XBox,
            (int)Platform.Steam,
            (int)Platform.Switch,
            (int)Platform.WeGames,
            (int)Platform.IOs,
            (int)Platform.Android,
            (int)Platform.Ps5,
            (int)Platform.XBoxSx,
            (int)Platform.Stadia,
            (int)Platform.GamePass
        };


    };

}

