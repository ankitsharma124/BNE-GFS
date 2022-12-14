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

ThreadPool.SetMinThreads(200, 200);

var logger = LogManager.Setup().LoadConfigurationFromFile(String.Concat(Directory.GetCurrentDirectory(), "/nlog.config")).GetCurrentClassLogger();
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("CoreBridgeContextConnection") ?? throw new InvalidOperationException("Connection string 'CoreBridgeContextConnection' not found.");

builder.Services.AddDbContext<CoreBridgeContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<CoreBridgeContext>();
app.UseAuthentication();;


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

    builder.Services.AddAuthorization(options =>
    {
    });


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
