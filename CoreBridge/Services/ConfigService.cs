using CoreBridge.Services.Interfaces;

namespace CoreBridge.Services
{
    public class ConfigService : IConfigService
    {
        private readonly IWebHostEnvironment _env;
        private IConfiguration? _config = null;

        private string _filenameBase = "appsettings.{0}json";

        public ConfigService(IWebHostEnvironment env)
        {
            _env = env;
            if (_env.IsDevelopment())
            {
                _config ??= new ConfigurationBuilder()
                    .AddJsonFile(String.Format(_filenameBase, "Development."))
                    .Build();

            }
            else if (_env.IsStaging())
            {
                _config ??= new ConfigurationBuilder()
                   .AddJsonFile(String.Format(_filenameBase, "Staging."))
                   .Build();
            }
            else if (_env.IsProduction())
            {
                _config ??= new ConfigurationBuilder()
                   .AddJsonFile(String.Format(_filenameBase, ""))
                   .Build();
            }
        }

        public T GetConfigVal<T>(string sectionName, string valueName)
        {
            return (T)_config!.GetRequiredSection(sectionName)!.GetValue(typeof(T), valueName);
        }
    }
}
