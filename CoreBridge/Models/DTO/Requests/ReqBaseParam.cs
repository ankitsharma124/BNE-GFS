using MessagePack;

namespace CoreBridge.Models.DTO.Requests
{
    [MessagePackObject]
    public class ReqBaseParam : ReqBase
    {
        private Dictionary<string, object> ApiSetting = new Dictionary<string, object>()
        {
            { "code", 9999 },
            {  "notCollectParamApi", null },
            {  "maintenanceAvoid", null }
        };

        public ReqBaseParam() : base()
        {

        }

        public virtual Dictionary<string, object> GetApiSetting()
        { return this.ApiSetting; }

        [IgnoreMember]
        public int ApiCode { get { return (int)GetApiSetting()["code"]; } }
        [IgnoreMember]
        public bool? NotCollectParamApi { get { return (bool?)GetApiSetting()["notCollectParamApi"]; } }
        [IgnoreMember]
        public bool? MaintenanceAvoid { get { return (bool?)GetApiSetting()["maintenanceAvoid"]; } }
    }

}
