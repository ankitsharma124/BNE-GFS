namespace CoreBridge.Models.Interfaces
{
    public interface IUnitOfWork
    {
        public Task<bool> CommitAsync();
        public string GetTableName<T>() where T : IAggregateRoot;
        public string GetTableName(string className);
    }
}
