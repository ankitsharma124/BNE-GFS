using MessagePack;
using XAct;

namespace CoreBridge.Models.DTO.Requests.client.user
{
    [MessagePackObject]
    public class ClientUserGetCountryParam : ReqBaseClient
    {
        [IgnoreMember]
        private new Dictionary<string, object> ApiSetting = new Dictionary<string, object>()
        {
            { "code", 1204 },
            {  "notCollectParamApi" , false },
            { "sessionCheckPass", false},
            { "temporaryCredential", false },
            {  "maintenanceAvoid" , false },
            {  "sessionAvoid" , false },
            {  "notSessionUpdate" , false },
            {  "mirrorSession" , false }
        };

        public override Dictionary<string, object> GetApiSetting()
        { return this.ApiSetting; }



    }
}
