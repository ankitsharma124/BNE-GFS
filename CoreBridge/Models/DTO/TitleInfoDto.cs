using CoreBridge.Models.Interfaces;
using MessagePack;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;

namespace CoreBridge.Models.DTO
{
    [Index(nameof(TitleCode))]
    [MessagePackObject]
    public class TitleInfoDto
    {
        public TitleInfoDto() { }

        public TitleInfoDto(string titleName, string titleCode, string trialTitleCode, int pType,
            string switchAppId, string xboxTitleId, string psClientId, string psClientSecret,
            string steamAppId, string steamPublisherKey,
            string devUrl, string testUrl, string qaUrl, string prodUrl)
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
            TestUrl = testUrl;
            QaUrl = qaUrl;
            ProdUrl = prodUrl;
        }

        [Key(0)]
        public String TitleName { get; set; }
        [Key(1)]
        public String TitleCode { get; set; }
        [Key(2)]
        public String TrialTitleCode { get; set; }
        [Key(3)]
        public Int32 Ptype { get; set; }
        [Key(4)]
        public String? SwitchAppId { get; set; }
        [Key(5)]
        public String? XboxTitleId { get; set; }
        [Key(6)]
        public String? PsClientId { get; set; }
        [Key(7)]
        public String? PsClientSecoret { get; set; }
        [Key(8)]
        public String? SteamAppId { get; set; }
        [Key(9)]
        public String? SteamPublisherKey { get; set; }
        [Key(10)]
        public String? DevUrl { get; set; }
        [Key(11)]
        public String? TestUrl { get; set; }
        [Key(12)]
        public String? QaUrl { get; set; }
        [Key(13)]
        public String? ProdUrl { get; set; }
        [IgnoreMember]
        public String? HashKey { get; set; } = null;
        [IgnoreMember]
        public object[]? ApiList { get; set; } //Dictionary<string, string>[]?
        //[Key(13)]
        //[System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
        //public DateTime CreateAt { get; set; }
        //[Key(14)]
        //[System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
        //public DateTime UpdateAt { get; set; }

        public bool IsValid()
        {
            return true;
        }
    }
}
