using Google.Cloud.EntityFrameworkCore.Spanner.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Diagnostics.CodeAnalysis;

namespace CoreBridge.Models.Ext
{
    public class SpannerContext : DbContext
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
        public override EntityEntry Add([NotNullAttribute] object entity)
        {
            if (entity is SpannerEntity spanner_entity)
            {
                spanner_entity.SetPrimaryKey();
                spanner_entity.CreatedAt = DateTimeOffset.UtcNow.DateTime;
                spanner_entity.UpdatedAt = DateTimeOffset.UtcNow.DateTime;
                return base.Add(spanner_entity);
            }
            else
            {
                return base.Add(entity);
            }
        }

        public override EntityEntry<TEntity> Add<TEntity>([NotNullAttribute] TEntity entity) where TEntity : class
        {
            if (entity is SpannerEntity spanner_entity)
            {
                spanner_entity.SetPrimaryKey();
                spanner_entity.CreatedAt = DateTimeOffset.UtcNow.DateTime;
                spanner_entity.UpdatedAt = DateTimeOffset.UtcNow.DateTime;
                entity = spanner_entity as TEntity;
                return base.Add(entity);
            }
            else
            {
                return base.Add(entity);
            }
        }

        public override ValueTask<EntityEntry> AddAsync([NotNullAttribute] object entity, CancellationToken cancellationToken = default)
        {
            if (entity is SpannerEntity spanner_entity)
            {
                spanner_entity.SetPrimaryKey();
                spanner_entity.CreatedAt = DateTimeOffset.UtcNow.DateTime;
                spanner_entity.UpdatedAt = DateTimeOffset.UtcNow.DateTime;
                entity = spanner_entity;
                return base.AddAsync(entity, cancellationToken);
            }
            else
            {
                return base.AddAsync(entity, cancellationToken);
            }
        }

        public override ValueTask<EntityEntry<TEntity>> AddAsync<TEntity>([NotNullAttribute] TEntity entity, CancellationToken cancellationToken = default) where TEntity : class
        {
            if (entity is SpannerEntity spanner_entity)
            {
                spanner_entity.SetPrimaryKey();
                spanner_entity.CreatedAt = DateTimeOffset.UtcNow.DateTime;
                spanner_entity.UpdatedAt = DateTimeOffset.UtcNow.DateTime;
                entity = spanner_entity as TEntity;
                return base.AddAsync(entity, cancellationToken);
            }
            else
            {
                return base.AddAsync(entity, cancellationToken);
            }
        }


        public override EntityEntry Update([NotNullAttribute] object entity)
        {
            if (entity is SpannerEntity spanner_entity)
            {
                spanner_entity.UpdatedAt = DateTimeOffset.UtcNow.DateTime;
                return base.Update(entity);
            }
            else
            {
                return base.Update(entity);
            }
        }

        public override EntityEntry<TEntity> Update<TEntity>([NotNullAttribute] TEntity entity) where TEntity : class
        {
            if (entity is SpannerEntity spanner_entity)
            {
                spanner_entity.UpdatedAt = DateTimeOffset.UtcNow.DateTime;
                return base.Update(entity);
            }
            else
            {
                return base.Update(entity);
            }
        }
    }
}
