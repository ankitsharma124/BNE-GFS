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
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.HttpOverrides;

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

    // Data Accessser Service Add
    builder.Services.AddDataAccessServices(builder.Configuration);
    // Hangfuire Service Add
    builder.Services.AddHangfireServices(builder.Configuration);

    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = AppSetting.GetConnectStringRedis(builder.Configuration);
        options.InstanceName = "redis-1";
    });


    // CustomServices
    builder.Services.AddCustomServices();



    // Session(Cookie)
    builder.Services.AddSession(options =>
    {
        // �Z�b�V�����N�b�L�[�̖��O��ς���Ȃ�
        options.Cookie.Name = "session";
    });

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        IWebHostEnvironment webHostEnvironment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

        var test = Environment.GetEnvironmentVariables();
        Console.Write(test);

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
        app.UseMiddleware<HashAdminMiddleware>();
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseMiddleware<DebugMiddleware>();

        //app.UseAuthorization();

        // HangFire Dashbord

        app.UseHangfireServer();
        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = new[] { new HungfireAuthorizationFilter() }
        });



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
    logger.Error(exception, "��O�̂��߂Ƀv���O�������~���܂����B");
    throw;
}
finally
{
    // �A�v���P�[�V�������I������O�ɁA�����^�C�}�[/�X���b�h���t���b�V�����Ē�~����悤�ɂ��Ă�������
    // (Linux �ł̃Z�O�����e�[�V�����ᔽ��������Ă��������j
    LogManager.Shutdown();
}
