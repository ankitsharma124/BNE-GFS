namespace CoreBridge.Models.DTO.Requests
{

    public abstract class ReqBaseParam : ReqBase
    {
        public Dictionary<string, object> ApiSetting = new Dictionary<string, object>()
        {
            { "code", 9999 },
            {  "notCollectParamApi" , null },
            {  "MaintenanceAvoid" , null }
        };

        public ReqBaseParam() : base()
        {

        }
        public override int ApiCode { get { return (int)ApiSetting["code"]; } }
    }

}
