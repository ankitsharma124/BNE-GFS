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
        private readonly ILogger _logger;

        public UnitOfWork(CoreBridgeContext dbContext, ILogger<UnitOfWork> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        //Entry List
        private readonly CoreBridgeRepository<AdminUser> _adminUserRepository = null;
        public ICoreBridgeRepository<AdminUser> AdminUserRepository { get { return GetInstance(_adminUserRepository); } }
        private readonly CoreBridgeRepository<TitleInfo> _titleInfoRepository = null;
        public ICoreBridgeRepository<TitleInfo> TitleInfoRepository { get { return GetInstance(_titleInfoRepository); } }
        private readonly CoreBridgeRepository<AppUser> _appUserRepository = null;
        public ICoreBridgeRepository<AppUser> AppUserRepository { get { return GetInstance(_appUserRepository); } }

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
                _logger.LogError(ex, "UnitOfWork:DB Commit Err");
                //Console.WriteLine(ex.Message);
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
