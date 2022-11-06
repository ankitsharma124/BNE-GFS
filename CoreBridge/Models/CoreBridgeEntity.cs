
using CoreBridge.Models.Ext;

namespace CoreBridge.Models
{
    public class CoreBridgeEntity : SpannerEntity
    {
        public string Id { get; set; }

        public override string GetPrimaryKey()
        {
            return Id;
        }

        public override void SetPrimaryKey()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
