using Google.Rpc;

namespace CoreBridge.Models.Exceptions
{
    public class BNException : CoreBridgeException
    {

        public enum BNErrorCode
        {
            //共通
            OK = 0,
            NG = 1,
            Maintenance = 2,
            SessionNG = 3,
            SessionTimeout = 4,
            VersionInvalid = 5,
            RequestErr = 6,
            ConnectStaging = 7,
            TitleCodeInvalid = 8,
            PlatformTypeInvalid = 9,
            ExternalDevice = 10,
            //Params
            ParamExists = 101,
            ParamType = 102,
            Num_LLimit = 103,
            Num_ULimit = 104,
            StrLen_LLimit = 105,
            StrLen_ULimit = 106,
            CharType = 107,
            NGWord = 108,
            OutOfRange = 109,
            //User
            UserNotExist = 201,
            RegisteredUser = 202, //登録済ユーザ
            BlackListUser = 203,
            TokenErr = 204,
            UIdDuplicate = 205,
            NotBNageService = 206,
            //Sys
            FieldAgree = 501,
            Fieled_TitleCode = 502,
            Kpi_NoEntryData = 503,
            //BNID
            BNId_NotLinked = 1501,
            BNId_Linked = 1502,
            NotService = 1503,
            NoExistingEntry = 1504, //COSMOSID未登録
            BNId_EraseFailed = 1505,
            //購入 - clientOnly
            Common_OtherPurchaseInProgress = 1601,
            Common_NotReflectedRight = 1602,
            Common_MoneyLimit = 1603,
            Common_MasterNotRegistered = 1604,
            //PSN
            Psn_ReqFailure_Token = 1701,
            Psn_ReqFailure_Container = 1702, //clientOnly
            Psn_ReqFailure_GetEntitlement = 1703, //clientOnly
            Psn_ReqFailure_Consume = 1704,//clientOnly
            Psn_ReqFailure_BaseUrl = 1705,//clientOnly
            Psn_ReqFailure_AccId = 1706,
            //Steam
            Steam_ReqFailure_Info = 1801, //clientOnly
            Steam_ReqFailure_Init = 1802,//clientOnly
            Steam_ReqFailure_Query = 1803,//clientOnly
            Steam_ReqFailure_Finalize = 1804,//clientOnly
            Steam_ReqFailure_AuthTicket = 1805,
            Steam_ReqFailure_Ownership = 1806, //clientOnly
            //Xbox - clientOnly
            Xbox_ReqFailure_SToken = 1901,
            Xbox_ReqFailure_XToken = 1902,
            Xbox_ReqFailure_Inventory = 1903,
            Xbox_ReqFailure_EDS = 1904,
            Xbox_ReqFailure_Different = 1905,
            //Switch - clientOnly
            Switch_ReqFailure_Get = 2001,
            Switch_ReqFailure_Consume = 2002,
            Switch_ReqFailure_SugarMaintenance = 2003,
            Switch_ReqFailure_BaasMaintenance = 2004,
            Switch_ReqFailure_RightsEmpty = 2005,
            Switch_ErrCode_SugarMaintenance = 1502,
            Switch_ErrCode_BaasMaintenance = 9501
        }


        public BNException(int action, BNErrorCode code)
        {
            Code = (int)code;
            Action = action;
        }

        public BNException(int action, BNErrorCode code, string message) : base(message)
        {
            Code = (int)code;
            Action = action;
        }


        public int StatusCode
        {
            get
            {
                return Convert.ToInt32(StatusCodeStr);
            }
        }

        public string StatusCodeStr
        {
            get
            {
                return Action.ToString("0000") + Code.ToString("0000");
            }
        }
    }
}
