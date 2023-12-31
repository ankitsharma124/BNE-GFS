﻿using Ardalis.Specification;
using CoreBridge.Models.Context;
using CoreBridge.Models.Ext;
using CoreBridge.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoreBridge.Models.Repositories
{
    public class CoreBridgeRepository<T> : CoreBridgeReadOnlyRepository<T>, ICoreBridgeRepository<T> where T : CoreBridgeEntity, IAggregateRoot
    {
        public CoreBridgeRepository(CoreBridgeContext dbContext) : base(dbContext) { }

        public virtual async Task AddAsync(T entity)
        {
            if (entity is SpannerEntity spanner_entity)
            {
                spanner_entity.SetPrimaryKey();
                spanner_entity.CreatedAt = DateTimeOffset.UtcNow.DateTime;
                spanner_entity.UpdatedAt = DateTimeOffset.UtcNow.DateTime;
                entity = spanner_entity as T;
            }
            
            _dbContext.Set<T>().Add(entity);
            await Task.CompletedTask;
        }

        public virtual async Task UpdateAsync(T entity)
        {
            if (entity is SpannerEntity spanner_entity)
            {
                spanner_entity.UpdatedAt = DateTimeOffset.UtcNow.DateTime;
                entity = spanner_entity as T;
            }

            _dbContext.Entry(entity).State = EntityState.Modified;
            await Task.CompletedTask;
        }

        public virtual async Task DetachAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Detached;
            await Task.CompletedTask;
        }

        public virtual async Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await Task.CompletedTask;
        }

        public virtual async Task DeleteRangeAsync(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);
            await Task.CompletedTask;
        }
    }
}
