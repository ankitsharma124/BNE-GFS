using MessagePack;

namespace CoreBridge.Models.DTO.Requests
{
    [MessagePackObject]
    public class ReqBaseParamHeader : ReqBaseParam
    {
        [Key(40)]
        public new Dictionary<string, object> ApiSetting = new Dictionary<string, object>()
        {
            { "code", 9999 },
            {  "notCollectParamApi" , null },
            { "temporaryCredential", null },
            {  "maintenanceAvoid" , null }
        };
        public bool HasTemporaryCredential()
        {
            return this.ApiSetting["temporaryCredential"] == null ? false : (bool)this.ApiSetting["temporaryCredential"];
        }


    }
}
