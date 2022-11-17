using Microsoft.EntityFrameworkCore;
using CoreBridge.Models.Context;
using CoreBridge.Models.Exceptions;
using CoreBridge.Models.Interfaces;
using CoreBridge.Models.Entity;

namespace CoreBridge.Models.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CoreBridgeContext _dbContext;

        public UnitOfWork(CoreBridgeContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        //Entry List
        private readonly CoreBridgeRepository<AdminUser> _adminUserRepository = null;
        public ICoreBridgeRepository<AdminUser> AdminUserRepository { get { return GetInstance(_adminUserRepository); } }
        private readonly CoreBridgeRepository<TitleInfo> _platformInfoRepository = null;
        public ICoreBridgeRepository<TitleInfo> PlatformRepository { get { return GetInstance(_platformInfoRepository); } }

        private CoreBridgeRepository<T> GetInstance<T>(CoreBridgeRepository<T> repository) where T : CoreBridgeEntity, IAggregateRoot
        {
            repository ??= new CoreBridgeRepository<T>(_dbContext);
            return repository;
        }

        public async Task<bool> CommitAsync()
        {
            try
            {
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public string? GetTableName<T>() where T : IAggregateRoot
        {
            var entityType = _dbContext.Model.FindEntityType(typeof(T));

            if (entityType == null)
                throw new DataNotFoundException($"The class {nameof(T)} is not specified in the context.");

            return entityType.GetTableName();
        }

        public string? GetTableName(string className)
        {
            var entityType = _dbContext.Model.FindEntityType(className);

            if (entityType == null)
                throw new DataNotFoundException($"The class {className} is not specified in the context.");

            return entityType.GetTableName();
        }
    }
}
