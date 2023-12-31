﻿using Google.Cloud.EntityFrameworkCore.Spanner.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Diagnostics.CodeAnalysis;
using XAct;

namespace CoreBridge.Models.Ext
{
    public class SpannerContext : IdentityDbContext
    {
        public SpannerContext(DbContextOptions<SpannerContext> options) : base(options)
        {
            base.Database.SetCommandTimeout(300);
        }
        protected SpannerContext(DbContextOptions options) : base(options)
        {
            base.Database.SetCommandTimeout(300);
        }

        public static DbContextOptionsBuilder<T> OptionsBuilder<T>(IConfiguration configuration) where T : DbContext
        {
            var optionsBuilder = new DbContextOptionsBuilder<T>();
            optionsBuilder.UseSpanner(AppSetting.GetConnectStringSpanner(configuration));
            return optionsBuilder;
        }
    }
}
