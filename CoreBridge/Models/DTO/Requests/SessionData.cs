namespace CoreBridge.Models.DTO.Requests
{
    public class SessionData
    {
        public string? SessionId { get; set; }
        public string? TitleCode { get; set; }
        public int? SkuType { get; set; }
        public int? Platform { get; set; }

        public SessionData(string? sessionId = null, string? titleCode = null, int? skuType = null,
            int? platform = null)
        {
            SessionId = sessionId;
            TitleCode = titleCode;
            SkuType = skuType;
            Platform = platform;
        }
    }
}
