using CoreBridge.Models.Interfaces;

namespace CoreBridge.Models
{
    public class AppSetting : IAppSetting
    {
        // AppSettings Parameter
        /// <summary>
        /// ConnectionStrings:データベース接続先定義
        /// </summary>
        public const string ConnectionStringsKey = "ConnectionStrings";
        /// <summary>
        /// ConnectionStrings:データベース接続先定義:MySQL定義
        /// </summary>
        public const string ConnectionStringMySQL = "MySQL";
        /// <summary>
        /// ConnectionStrings:データベース接続先定義:MySQL Version
        /// </summary>
        public const string ConnectionStringMySQLVersion = "8.0.21-mysql";

        /// <summary>
        /// ConnectionStrings:データベース接続先定義:InMemoryDB
        /// </summary>
        public const string ConnectionStringMyInMemoryDb = "InMemoryDb";
        /// <summary>
        /// ConnectionStrings:データベース接続先定義:Redis
        /// </summary>
        public const string ConnectionStringRedis = "Redis";
        /// <summary>
        /// ConnectionStrings:データベース接続先定義:Spanner
        /// </summary>
        public const string ConnectionStringSpanner = "Spanner";

        /// <summary>
        /// InMemory SecondCashe定義
        /// </summary>
        public const string InMemoryProviderName = "InMemoryQuerySecondCache";

        /// <summary>
        /// Reviewサーバ定義
        /// </summary>
        public const string ReviewServer = "ReviewServer";

        // Enviroment Parameter
        /// <summary>
        /// 環境変数定義：MｙSQL接続先
        /// </summary>
        public const string EnviromentStringMySQL = "ConnectionStrings_MySQL";
        /// <summary>
        /// 環境変数定義：Redis接続先
        /// </summary>
        public const string EnviromentStringRedis = "ConnectionStrings_Redis";
        /// <summary>
        /// 環境変数定義：Spanner接続先
        /// </summary>
        public const string EnviromentStringSpanner = "ConnectionStrings_Spanner";


        string IAppSetting.GetConnectStringMySQL(IConfiguration configuration) => GetConnectStringMySQL(configuration);

        /// <summary>
        /// MySQL接続文字列取得
        /// </summary>
        /// <param name="configuration">IConfiguration</param>
        /// <returns>string</returns>
        public static string GetConnectStringMySQL(IConfiguration configuration)
        {
            string res = Environment.GetEnvironmentVariable(EnviromentStringMySQL);
            if (res == null)
                res = configuration.GetConnectionString(ConnectionStringMySQL);
            return res;
        }

        string IAppSetting.GetConnectStringRedis(IConfiguration configuration) => GetConnectStringRedis(configuration);

        /// <summary>
        /// Redis接続文字列取得
        /// </summary>
        /// <param name="configuration">IConfiguration</param>
        /// <returns>string</returns>
        public static string GetConnectStringRedis(IConfiguration configuration)
        {
            string res = Environment.GetEnvironmentVariable(EnviromentStringRedis);
            if (res == null)
                res = configuration.GetConnectionString(ConnectionStringRedis);
            return res;
        }

        string IAppSetting.GetConnectStringSpanner(IConfiguration configuration) => GetConnectStringSpanner(configuration);

        /// <summary>
        /// Spanner接続文字列取得
        /// </summary>
        /// <param name="configuration">IConfiguration</param>
        /// <returns>string</returns>
        public static string GetConnectStringSpanner(IConfiguration configuration)
        {
            string res = Environment.GetEnvironmentVariable(EnviromentStringSpanner);
            if (res == null)
                res = configuration.GetConnectionString(ConnectionStringSpanner);
            return res;
        }

    }
}
