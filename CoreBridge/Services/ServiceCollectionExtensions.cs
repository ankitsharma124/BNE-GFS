using Autofac;
using CoreBridge.Models.Interfaces;
using CoreBridge.Models.Repositories;
using CoreBridge.Models;
using Hangfire;
using Microsoft.Extensions.DependencyInjection.Extensions;
using CoreBridge.Models.Context;
using Microsoft.Extensions.Options;
using Google.Cloud.EntityFrameworkCore.Spanner.Extensions;
using StackExchange.Redis;
using CloudStructures;
using CoreBridge.Services.Interfaces;

namespace CoreBridge.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataAccessServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Connect String
            string connectionStrings = configuration.GetConnectionString(AppSetting.ConnectionStringMySQL);
            string redis_connection = AppSetting.GetConnectStringRedis(configuration);

            // DBContextPool
            services.AddDbContextPool<CoreBridgeContext>(options =>
            {
                options.UseSpanner(AppSetting.GetConnectStringSpanner(configuration));
            }, poolSize: 50);

            // Repository
            services.TryAddScoped(typeof(ICoreBridgeReadOnlyRepository<>), typeof(CoreBridgeRepository<>));
            services.TryAddScoped(typeof(ICoreBridgeRepository<>), typeof(CoreBridgeRepository<>));


            // UnitOfWork
            services.TryAddScoped<IUnitOfWork, UnitOfWork>();

            // RedisConnection
            services.AddSingleton(new RedisConnection(
                new RedisConfig("RedisServer", redis_connection)));

            // AppSettings
            services.AddSingleton<IAppSetting, AppSetting>();

            return services;
        }

        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            services.TryAddScoped<ILoggerService, LoggerService>();
            services.TryAddScoped<IAdminUserService, AdminUserService>();
            services.TryAddScoped<IResponseService, ResponseService>();
            services.TryAddScoped<ITitleInfoService, TitleInfoService>();

            return services;
        }

        public static IServiceCollection AddHangfireServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Connect String
            string redis_connection = AppSetting.GetConnectStringRedis(configuration);

            // Job Scope Add
            //services.AddScoped<IAppStoreJob, AppStoreJob>();

            // Hangfire
            services.AddHangfire(configuration =>
            {
                // Using Redis
                configuration.UseRedisStorage(ConnectionMultiplexer.Connect(redis_connection));
            });

            // コンテナに型を登録、Activatorに設定
            var builder = new ContainerBuilder();
            //builder.RegisterType<IAppStoreJob>().As<IAppStoreJob>();

            GlobalConfiguration.Configuration.UseAutofacActivator(builder.Build());

            return services;
        }
    }
}
