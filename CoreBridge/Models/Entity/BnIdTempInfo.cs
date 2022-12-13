namespace CoreBridge.Models.Entity
{
    public class BnIdTempInfo : CoreBridgeEntity
    {
        public string TitleCode { get; set; } = "";
        public int Platform { get; set; }
        public int SkuType { get; set; }
        public string UserId { get; set; } = "";

    }
}
