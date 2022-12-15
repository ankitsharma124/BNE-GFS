using CoreBridge.Models.DTO;
using CoreBridge.Models.Entity;


namespace CoreBridge.Models.Interfaces
{
    public interface IUnitOfWork
    {
        // Repository List
        ICoreBridgeRepository<AdminUser> AdminUserRepository { get; }

        ICoreBridgeRepository<TitleInfo> TitleInfoRepository { get; }

        ICoreBridgeRepository<GFSUser> UserRepository { get; }
        ICoreBridgeRepository<UserPlatform> UserPlatformRepository { get; }

        ICoreBridgeRepository<DebugInfo> DebugInfoRepository { get; }

        public Task<bool> CommitAsync();
        public string GetTableName<T>() where T : IAggregateRoot;
        public string GetTableName(string className);
    }
}
