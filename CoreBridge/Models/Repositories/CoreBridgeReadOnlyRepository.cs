using Ardalis.Specification.EntityFrameworkCore;
using Ardalis.Specification;
using CoreBridge.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using CoreBridge.Models.Ext;
using CoreBridge.Models.Context;

namespace CoreBridge.Models.Repositories
{
    public class CoreBridgeReadOnlyRepository<T> : ICoreBridgeReadOnlyRepository<T> where T : CoreBridgeEntity, IAggregateRoot
    {
        protected readonly CoreBridgeContext _dbContext = default;
        protected readonly ISpecificationEvaluator _specificationEvaluator = default;

        public CoreBridgeReadOnlyRepository(CoreBridgeContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _specificationEvaluator = new SpecificationEvaluator();
        }

        public virtual async Task<T> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<T>().AsNoTracking().Where(e => e.Id == id).FirstOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<T> GetBySpecAsync<Spec>(Spec specification, CancellationToken cancellationToken = default) where Spec : ISpecification<T>
        {
            return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<TResult> GetBySpecAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<List<T>> ListAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<T>().AsNoTracking().ToListAsync(cancellationToken);
        }
        public virtual async Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(specification).ToListAsync(cancellationToken);
        }

        public virtual async Task<List<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(specification).ToListAsync(cancellationToken);
        }

        public virtual async Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(specification, true).CountAsync(cancellationToken);
        }

        public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<T>().CountAsync(cancellationToken);
        }

        protected virtual IQueryable<T> ApplySpecification(ISpecification<T> specification, bool evaluateCriteriaOnly = false)
        {
            return _specificationEvaluator.GetQuery(_dbContext.Set<T>().AsNoTracking().AsQueryable(), specification);
        }

        protected virtual IQueryable<TResult> ApplySpecification<TResult>(ISpecification<T, TResult> specification)
        {
            if (specification is null || specification.Selector is null)
            {
                return null;
            }

            return _specificationEvaluator.GetQuery(_dbContext.Set<T>().AsNoTracking().AsQueryable(), specification);
        }

        public async Task<List<T>> ListPaginationAsync(ISpecification<T> specification, int skip, int pageSize, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = ApplySpecification(specification);

            if (!specification.OrderExpressions.Any())
                query.OrderBy(x => x.Id);

            return await query.Skip(skip).Take(pageSize).ToListAsync(cancellationToken);
        }

        public async Task<List<T>> ListPaginationAsync(int skip, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<T>().AsNoTracking().OrderBy(x => x.Id).Skip(skip).Take(pageSize).ToListAsync(cancellationToken);
        }

        public async Task<RType> MaxAsync<RType>(ISpecification<T> specification, Expression<Func<T, RType>> func, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = ApplySpecification(specification);

            if (query.Any())
                return await query.MaxAsync(func, cancellationToken);

            return default;
        }

        public async Task<RType> MaxAsync<RType>(Expression<Func<T, RType>> func, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbContext.Set<T>().AsNoTracking();

            if (query.Any())
                return await query.MaxAsync(func, cancellationToken);

            return default;
        }

        public async Task<bool> AnyAsync(ISpecification<T> specification, Expression<Func<T, bool>> func, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(specification).AnyAsync(func, cancellationToken);
        }

        public async Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(specification).AnyAsync(cancellationToken);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> func, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbContext.Set<T>().AsNoTracking();

            return await query.AnyAsync(func, cancellationToken);
        }

        public async Task<List<T>> RandomAsync(ISpecification<T> specification, int sampleSize, CancellationToken cancellationToken = default)
        {
            List<T> result = new();

            IQueryable<T> query = ApplySpecification(specification);

            List<int> randSelection = Enumerable.Range(0, query.Count()).OrderBy(x => Guid.NewGuid()).ToList();

            while (randSelection.Any())
            {
                result.Add(query.Skip(randSelection.First()).First());
                randSelection.RemoveAt(0);

                if (result.Count >= sampleSize)
                    break;
            }

            return await Task.FromResult(result);
        }

        public async Task<List<TResult>> GroupBy<TResult>(ISpecification<T> specification, Expression<Func<IGrouping<bool, T>, int, TResult>> select,
            Expression<Func<T, bool>> groupBy, CancellationToken cancellationToken = default)
        {
            return await
                ApplySpecification(specification).GroupBy(groupBy).Select(select).ToListAsync(cancellationToken);
        }
    }
}
