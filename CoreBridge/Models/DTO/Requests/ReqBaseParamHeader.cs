using MessagePack;

namespace CoreBridge.Models.DTO.Requests
{
    [MessagePackObject]
    public class ReqBaseParamHeader : ReqBaseParam
    {

        private new Dictionary<string, object> ApiSetting = new Dictionary<string, object>()
        {
            { "code", 9999 },
            {  "notCollectParamApi" , null },
            { "temporaryCredential", null },
            {  "maintenanceAvoid" , null }
        };
        public override Dictionary<string, object> GetApiSetting()
        { return this.ApiSetting; }

        public bool HasTemporaryCredential()
        {
            return this.GetApiSetting()["temporaryCredential"] == null ? false : (bool)this.GetApiSetting()["temporaryCredential"];
        }


    }
}
