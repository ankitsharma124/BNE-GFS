namespace CoreBridge.Models.DTO.Requests
{
    public class ReqBaseParamHeader : ReqBaseParam
    {
        public new Dictionary<string, object> ApiSetting = new Dictionary<string, object>()
        {
            { "code", 9999 },
            {  "notCollectParamApi" , null },
            { "temporaryCredential", null },
            {  "maintenanceAvoid" , null }
        };

        public bool HasTemporaryCredential()
        {
            return (bool)this.ApiSetting["temporaryCredential"];
        }


    }
}
