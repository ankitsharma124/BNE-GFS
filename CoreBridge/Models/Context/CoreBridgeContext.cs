using CoreBridge.Models.DTO;
using CoreBridge.Models.Entity;

using CoreBridge.Models.Ext;
using Microsoft.AspNetCore.Identity;
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
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<GFSUser> Users { get; set; }
#if DEBUG
        public DbSet<DebugInfo> DebugInfoList { get; set; }
#endif

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TitleInfo>().HasIndex(t => t.TitleCode);
            modelBuilder.Entity<AppUser>().HasIndex(a => a.UserId);
            modelBuilder.Entity<IdentityUser>().HasIndex(a => a.Id);
            modelBuilder.Entity<IdentityRole>().HasIndex(a => a.Id);
#if DEBUG
            ContextSeedForTest.Seed(modelBuilder).Wait();
#endif
        }
    }
}
