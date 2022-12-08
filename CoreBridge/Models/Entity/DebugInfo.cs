using CoreBridge.Models.Interfaces;

namespace CoreBridge.Models.Entity
{
    public class DebugInfo : CoreBridgeEntity, IAggregateRoot
    {
        public string TitleCode { get; set; } = "";
        public string UserId { get; set; } = "";
        public string RequestUrl { get; set; } = "";
        public string RequestBody { get; set; } = "";
        public string ResponseBody { get; set; } = "";
    }
}
