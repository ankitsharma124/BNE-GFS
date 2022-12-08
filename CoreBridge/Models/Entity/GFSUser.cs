using CoreBridge.Models.Interfaces;

namespace CoreBridge.Models.Entity
{
    public class GFSUser : CoreBridgeEntity, IAggregateRoot
    {
        public int Platform { get; set; } //neoではp_type
        public string TitleCode { get; set; }
    }
}
