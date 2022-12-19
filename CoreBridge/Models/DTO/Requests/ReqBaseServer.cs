using MessagePack;
using Microsoft.AspNetCore.HttpOverrides;
namespace CoreBridge.Models.DTO.Requests
{
    [MessagePackObject]
    public class ReqBaseServer : ReqBaseParamHeader
    {
        [IgnoreMember]
        private new Dictionary<string, object> ApiSetting = new Dictionary<string, object>()
        {
            { "code", 9999 },
            {  "notCollectParamApi" , null },
            { "temporaryCredential", null },
            {  "maintenanceAvoid" , null }
        };

        public override Dictionary<string, object> GetApiSetting()
        { return this.ApiSetting; }


        public string GetIpAddress(HttpContext context)
        {
            //todo: Test!!
            return context.Connection.RemoteIpAddress.ToString();
        }
    }
}
