using CoreBridge.Models.Ext;
using Microsoft.EntityFrameworkCore;

namespace CoreBridge.Models.Context
{
    public class CoreBridgeContext : SpannerContext
    {
        public CoreBridgeContext(DbContextOptions<CoreBridgeContext> options) : base(options) { }
        protected CoreBridgeContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
}
