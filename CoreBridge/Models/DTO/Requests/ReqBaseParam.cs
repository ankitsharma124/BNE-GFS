namespace CoreBridge.Models.DTO.Requests
{

    public abstract class ReqBaseParam : ReqBase
    {
        public Dictionary<string, object> ApiSetting = new Dictionary<string, object>()
        {
            { "code", 9999 },
            {  "notCollectParamApi", null },
            {  "maintenanceAvoid", null }
        };

        public ReqBaseParam() : base()
        {

        }
        public override int ApiCode { get { return (int)ApiSetting["code"]; } }

        public bool? NotCollectParamApi { get { return (bool?)ApiSetting["notCollectParamApi"]; } }

        public bool? MaintenanceAvoid { get { return (bool?)ApiSetting["maintenanceAvoid"]; } }
    }

}
