using CoreBridge.Models.Interfaces;

namespace CoreBridge.Models.Entity
{

    //タイトルコード管理テーブル（仮）
    public class TitleInfo : CoreBridgeEntity, IAggregateRoot
    {
        internal TitleInfo() { }

        public TitleInfo(string titleName, string titleCode, string trialTitleCode, int pType,
            string switchAppId, string xboxTitleId, string psClientId, string psClientSecret,
            string steamAppId, string steamPublisherKey, string devUrl, string qaUrl, string prodUrl)
        {
            TitleName = titleName;
            TitleCode = titleCode;
            TrialTitleCode = trialTitleCode;
            Ptype = pType;
            SwitchAppId = switchAppId;
            XboxTitleId = xboxTitleId;
            PsClientId = psClientId;
            PsClientSecoret = psClientSecret;
            SteamAppId = steamAppId;
            SteamPublisherKey = steamPublisherKey;
            DevUrl = devUrl;
            QaUrl = qaUrl;
            ProdUrl = prodUrl;
            //CreatedAt = createTime;
            //UpdatedAt = updateTime;
            //Id = id;
        }

        public string TitleName { get; set; }
        public string TitleCode { get; set; }
        public string TrialTitleCode { get; set; }
        public int Ptype { get; set; } = 0;
        public string? SwitchAppId { get; set; }
        public string? XboxTitleId { get; set; }
        public string? PsClientId { get; set; }
        public string? PsClientSecoret { get; set; }
        public string? SteamAppId { get; set; }
        public string? SteamPublisherKey { get; set; }
        public string? DevUrl { get; set; }
        public string? TestUrl { get; set; }
        public string? QaUrl { get; set; }
        public string? ProdUrl { get; set; }

        public string? HashKey { get; set; } = null;

        //public object[]? ApiList { get; set; } //Dictionary<string, string>[]?


        //public DateTime CreateAt { get; set; }
        //public DateTime UpdateAt { get; set; }
        //public string Id { get; set; }
    }



}
