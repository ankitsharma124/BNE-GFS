using Ardalis.Specification;
using System.Linq.Expressions;

namespace CoreBridge.Models.Interfaces
{
    /// <summary>
    /// 読み込み専用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICoreBridgeReadOnlyRepository<T> where T : CoreBridgeEntity, IAggregateRoot
    {
        /// <summary>
        /// Idから検索する
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<T> GetByIdAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// 仕様に合うものを検索する
        /// </summary>
        /// <typeparam name="Spec"></typeparam>
        /// <param name="specification"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<T> GetBySpecAsync<Spec>(Spec specification, CancellationToken cancellationToken = default) where Spec : ISpecification<T>;

        /// <summary>
        /// 仕様に合うものを検索してSelectする
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="specification"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TResult> GetBySpecAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default);

        /// <summary>
        /// 全部を取ってくる
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<T>> ListAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 仕様に合うもの検索して一覧取ってくる
        /// </summary>
        /// <param name="specification"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);

        /// <summary>
        /// 仕様に合うものを検索してSelectする
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="specification"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default);

        /// <summary>
        /// 仕様に合うものを検索して数える
        /// </summary>
        /// <param name="specification"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);

        /// <summary>
        /// 仕様に合うものを検索して数える
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> CountAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// ページに分けて一個ずつのページをSelectする
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="specification"></param>
        /// <param name="skip"></param>
        /// <param name="pageSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<T>> ListPaginationAsync(ISpecification<T> specification, int skip, int pageSize, CancellationToken cancellationToken = default);

        /// <summary>
        /// ページに分けて一個ずつのページをSelectする
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="specification"></param>
        /// <param name="skip"></param>
        /// <param name="pageSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<T>> ListPaginationAsync(int skip, int pageSize, CancellationToken cancellationToken = default);

        Task<RType> MaxAsync<RType>(Expression<Func<T, RType>> func, CancellationToken cancellationToken = default);

        Task<RType> MaxAsync<RType>(ISpecification<T> specification, Expression<Func<T, RType>> func, CancellationToken cancellationToken = default);

        Task<List<T>> RandomAsync(ISpecification<T> specification, int pageSize, CancellationToken cancellationToken = default);

        Task<bool> AnyAsync(ISpecification<T> specification, Expression<Func<T, bool>> func, CancellationToken cancellationToken = default);

        Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);

        Task<bool> AnyAsync(Expression<Func<T, bool>> func, CancellationToken cancellationToken = default);

        public Task<List<TResult>> GroupBy<TResult>(ISpecification<T> specification, Expression<Func<IGrouping<bool, T>, int, TResult>> select,
            Expression<Func<T, bool>> groupBy, CancellationToken cancellationToken = default);
    }

}
