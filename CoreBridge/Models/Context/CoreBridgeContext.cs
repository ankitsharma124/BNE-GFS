using CoreBridge.Models.Entity;
using CoreBridge.Models.Ext;
using Microsoft.EntityFrameworkCore;

namespace CoreBridge.Models.Context
{
    public class CoreBridgeContext : SpannerContext
    {
        public CoreBridgeContext(DbContextOptions<CoreBridgeContext> options) : base(options) { }
        protected CoreBridgeContext(DbContextOptions options) : base(options) { }

        // Entity Entry
        public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<TitleInfo> TitleInfo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TitleInfo>().HasKey(t => t.TitleCode);
        }
    }
}
