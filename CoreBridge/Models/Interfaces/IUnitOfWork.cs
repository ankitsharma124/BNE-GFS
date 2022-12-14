using CoreBridge.Models.DTO;
using CoreBridge.Models.Entity;


namespace CoreBridge.Models.Interfaces
{
    public interface IUnitOfWork
    {
        // Repository List
        public ICoreBridgeRepository<AdminUser> AdminUserRepository { get; }
        public ICoreBridgeRepository<TitleInfo> TitleInfoRepository { get; }
        public ICoreBridgeRepository<AppUser> AppUserRepository { get; }
        //ICoreBridgeRepository<GFSUser> UserRepository { get; }
        ICoreBridgeRepository<DebugInfo> DebugInfoRepository { get; }

        public Task<bool> CommitAsync();
        public string GetTableName<T>() where T : IAggregateRoot;
        public string GetTableName(string className);
    }
}
