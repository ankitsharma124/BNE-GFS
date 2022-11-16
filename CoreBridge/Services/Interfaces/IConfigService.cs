namespace CoreBridge.Services.Interfaces
{
    public interface IConfigService
    {
        T GetConfigVal<T>(string sectionName, string valueName);
    }
}
