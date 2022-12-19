using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.HttpOverrides;
using MessagePack;

namespace CoreBridge.Models.DTO.Requests
{
    [MessagePackObject]
    public class ReqBaseClient : ReqBaseParamHeader
    {
        [IgnoreMember]
        private new Dictionary<string, object> ApiSetting = new Dictionary<string, object>()
        {
            { "code", 9999 },
            {  "notCollectParamApi" , null },
            { "temporaryCredential", null },
            { "sessionCheckPass", null },
            {  "maintenanceAvoid" , null },
            {  "sessionAvoid", null},
            {  "notSessionUpdate", null},
            {  "mirrorSession", null},
        };

        public override Dictionary<string, object> GetApiSetting()
        { return this.ApiSetting; }

        public bool? SessionCheckPass()
        {
            return (bool?)GetApiSetting()["sessionCheckPass"];
        }

        public bool? SessionAvoid()
        {
            return (bool?)GetApiSetting()["sessionAvoid"];
        }

        public bool? NotSessionUpdate()
        {
            return (bool?)GetApiSetting()["notSessionUpdate"];
        }


        public bool? MirroSession()
        {
            return (bool?)ApiSetting["mirrorSession"];
        }

        public string GetIpAddress(HttpContext context)
        {
            //todo: Test!!
            return context.Connection.RemoteIpAddress.ToString();
        }
    }
}
