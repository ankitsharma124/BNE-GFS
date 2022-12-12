using MessagePack;

namespace CoreBridge.Models.DTO.Requests
{
    [MessagePackObject]
    public class ReqBaseParam : ReqBase
    {
        [Key(30)]
        public Dictionary<string, object> ApiSetting = new Dictionary<string, object>()
        {
            { "code", 9999 },
            {  "notCollectParamApi", null },
            {  "maintenanceAvoid", null }
        };

        public ReqBaseParam() : base()
        {

        }
        [IgnoreMember]
        public override int ApiCode { get { return (int)ApiSetting["code"]; } }
        [IgnoreMember]
        public bool? NotCollectParamApi { get { return (bool?)ApiSetting["notCollectParamApi"]; } }
        [IgnoreMember]
        public bool? MaintenanceAvoid { get { return (bool?)ApiSetting["maintenanceAvoid"]; } }
    }

}
