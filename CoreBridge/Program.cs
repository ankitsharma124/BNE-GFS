using CoreBridge.Filters;
using CoreBridge.Models.Context;
using Microsoft.EntityFrameworkCore;
using CoreBridge.Services;
using Hangfire;
using NLog;
using MessagePack.Resolvers;
using Microsoft.AspNetCore.Mvc;
using MessagePack.AspNetCoreMvcFormatter;
using CoreBridge.Models.Middleware;
using CoreBridge.Models;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.DotNet.Scaffolding.Shared.ProjectModel;
using NLog.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using CoreBridge.Utility;
using Microsoft.Extensions.Options;

ThreadPool.SetMinThreads(200, 200);

var logger = LogManager.Setup().LoadConfigurationFromFile(String.Concat(Directory.GetCurrentDirectory(), "/nlog.config")).GetCurrentClassLogger();
var builder = WebApplication.CreateBuilder(args);

try
{
    // Add services to the container.
    //builder.Services.AddControllersWithViews();

    builder.Services.AddMvc().AddMvcOptions(option =>
    {
        //option.OutputFormatters.Clear(); 
        option.OutputFormatters.Add(new MessagePackOutputFormatter(ContractlessStandardResolver.Options));
        SystemTextJsonInputFormatter jsonFormatter = (SystemTextJsonInputFormatter)option.InputFormatters.First();
        var inputFormatter = new MessagePackInputFormatter(ContractlessStandardResolver.Options);
        inputFormatter.SupportedMediaTypes.Add(new Microsoft.Net.Http.Headers.MediaTypeHeaderValue("application/x-messagepack"));
        option.InputFormatters.Add(inputFormatter);


    });
    builder.Services.AddRazorPages();

    //for RemoteIP
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders =
            ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    });

    // NLog: Setup NLog for Dependency injection
    builder.Host.ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    }).UseNLog();



    // Data Accessser Service Add
    builder.Services.AddDataAccessServices(builder.Configuration);
    // Hangfuire Service Add
    builder.Services.AddHangfireServices(builder.Configuration);

    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = AppSetting.GetConnectStringRedis(builder.Configuration);
        options.InstanceName = "redis";
    });


    // CustomServices
    builder.Services.AddCustomServices();


    // IdentityRole
    builder.Services.AddIdentity<IdentityUser, IdentityRole>(
        options => options.SignIn.RequireConfirmedAccount = false)
        .AddEntityFrameworkStores<CoreBridgeContext>()
        .AddDefaultTokenProviders();

    builder.Services.AddSingleton<IEmailSender, EmailSender>();
    builder.Services.AddScoped<IUserClaimsPrincipalFactory<IdentityUser>, AdditionalUserClaimsPrincipalFactory>();


    builder.Services.AddAuthorization(options =>
    {
    });


    // Session(Cookie)
    builder.Services.AddSession(options =>
    {
        // セッションクッキーの名前を変えるなら
        options.Cookie.Name = "session";
    });

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        IWebHostEnvironment webHostEnvironment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            CoreBridgeContext context = scope.ServiceProvider.GetRequiredService<CoreBridgeContext>();
            logger.Info($"Applying migrations for context {context}");
            context.Database.Migrate(); // Apply migrations
            logger.Info("Migrations done.");

            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        //app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseSession();

        //custom middleware
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseMiddleware<SessionStatusAdminMiddleware>();

        app.UseAuthentication();
        app.UseAuthorization();

        //app.UseMiddleware<HashAdminMiddleware>();
        //app.UseMiddleware<DebugMiddleware>();

        //app.UseAuthorization();

        // HangFire Dashbord

        app.UseHangfireServer();
        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = new[] { new HungfireAuthorizationFilter() }
        });



        // Api Routing Add
        //app.MapControllerRoute(
        //   name: "AppManagementUser",
        //   pattern: "{area:exists}/{controller=Home}/{id?}/{action=Index}");
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");


    }

    // TODO Job Scheduler
    // RecurringJob.AddOrUpdate<IAppStoreJob>("AppStoreJob", (x) => x.ExecuteAsync(), Cron.Daily);

    app.Run();
}
catch (Exception exception)
{
    logger.Error(exception, "例外のためにプログラムを停止しました。");
    throw;
}
finally
{
    // アプリケーションを終了する前に、内部タイマー/スレッドをフラッシュして停止するようにしてください
    // (Linux でのセグメンテーション違反を回避してください）
    LogManager.Shutdown();
}
