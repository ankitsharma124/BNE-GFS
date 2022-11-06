namespace CoreBridge.Models.Interfaces
{
    /// <summary>
    /// 書き込み可能
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICoreBridgeRepository<T> : ICoreBridgeReadOnlyRepository<T> where T : CoreBridgeEntity, IAggregateRoot
    {
        /// <summary>
        /// 新規追加
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task AddAsync(T entity);

        /// <summary>
        /// 更新(追跡させる)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task UpdateAsync(T entity);

        /// <summary>
        /// 削除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task DeleteAsync(T entity);

        Task DetachAsync(T entity);

        /// <summary>
        /// まとめて削除
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task DeleteRangeAsync(IEnumerable<T> entities);
    }
}
