
using CoreBridge.Models.Ext;
using System.ComponentModel.DataAnnotations;

namespace CoreBridge.Models
{
    public class CoreBridgeEntity : SpannerEntity
    {
        [Key]
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
