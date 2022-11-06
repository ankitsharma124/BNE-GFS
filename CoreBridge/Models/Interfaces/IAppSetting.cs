namespace CoreBridge.Models.Interfaces
{
    public interface IAppSetting
    {
        public string GetConnectStringMySQL(IConfiguration configuration);

        public string GetConnectStringRedis(IConfiguration configuration);

        public string GetConnectStringSpanner(IConfiguration configuration);

    }
}
