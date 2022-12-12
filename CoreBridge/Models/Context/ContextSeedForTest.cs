using CoreBridge.Models.Entity;
using Google;
using Microsoft.EntityFrameworkCore;

namespace CoreBridge.Models.Context
{
    public static class ContextSeedForTest
    {
        private static ModelBuilder _modelBuilder;
        public static async Task Seed(ModelBuilder modelBuilder)
        {
            _modelBuilder = modelBuilder;
            await SeedTestTitleInfo();
            await SeedTestUserlinkedTotitleInfo();
        }

        public static async Task SeedTestTitleInfo()
        {
            var info = new TitleInfo
            {
                Id = "TestTitleId",
                TitleCode = "TestTitleCode",
                TitleName = "testTitleName",
                TrialTitleCode = "TestTrialTitleCode",
                HashKey = "TEST111111111111"
            };

            _modelBuilder.Entity<TitleInfo>().HasData(info);
        }

        public static async Task SeedTestUserlinkedTotitleInfo()
        {
            GFSUser user = new GFSUser { Platform = 1, TitleCode = "TestTitleCode", Id = "TestUserId" };
            _modelBuilder.Entity<GFSUser>().HasData(user);
        }

    }
}
