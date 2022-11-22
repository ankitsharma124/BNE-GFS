using Microsoft.AspNetCore.HttpOverrides;
namespace CoreBridge.Models.DTO.Requests
{
    public class ReqBaseServer : ReqBaseParamHeader
    {
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
