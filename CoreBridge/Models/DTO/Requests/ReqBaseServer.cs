using MessagePack;
using Microsoft.AspNetCore.HttpOverrides;
namespace CoreBridge.Models.DTO.Requests
{
    [MessagePackObject]
    public class ReqBaseServer : ReqBaseParamHeader
    {
        [Key(50)]
        public new Dictionary<string, object> ApiSetting = new Dictionary<string, object>()
        {
            { "code", 9999 },
            {  "notCollectParamApi" , null },
            { "temporaryCredential", null },
            {  "maintenanceAvoid" , null }
        };


        public string GetIpAddress(HttpContext context)
        {
            //todo: Test!!
            return context.Connection.RemoteIpAddress.ToString();
        }
    }
}
