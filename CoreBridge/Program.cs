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

ThreadPool.SetMinThreads(200, 200);

var logger = LogManager.Setup().LoadConfigurationFromFile(String.Concat(Directory.GetCurrentDirectory(), "/nlog.config")).GetCurrentClassLogger();
var builder = WebApplication.CreateBuilder(args);

try
{
    // Add services to the container.
    //builder.Services.AddControllersWithViews();
    builder.Services.AddMvc().AddMvcOptions(option =>
    {
        option.OutputFormatters.Clear();
        option.OutputFormatters.Add(new MessagePackOutputFormatter(ContractlessStandardResolver.Options));
        option.InputFormatters.Clear();
        option.InputFormatters.Add(new MessagePackInputFormatter(ContractlessStandardResolver.Options));
    });
    builder.Services.AddRazorPages();

    // Data Accessser Service Add
    builder.Services.AddDataAccessServices(builder.Configuration);
    // Hangfuire Service Add
    builder.Services.AddHangfireServices(builder.Configuration);
    // CustomServices
    builder.Services.AddCustomServices();



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
        //app.UseAuthorization();

        // HangFire Dashbord

        app.UseHangfireServer();
        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = new[] { new HungfireAuthorizationFilter() }
        });

        //Middleware
        app.UseMiddleware<ExceptionMiddleware>();

        // Api Routing Add
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
