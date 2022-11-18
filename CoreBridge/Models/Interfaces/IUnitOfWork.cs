using CoreBridge.Models.Entity;

namespace CoreBridge.Models.Interfaces
{
    public interface IUnitOfWork
    {
        // Repository List
        public ICoreBridgeRepository<AdminUser> AdminUserRepository { get; }
        public ICoreBridgeRepository<TitleInfo> TitleInfoRepository { get; }

        public Task<bool> CommitAsync();
        public string GetTableName<T>() where T : IAggregateRoot;
        public string GetTableName(string className);
    }
}
