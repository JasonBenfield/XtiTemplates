using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using __APPNAME__SetupApp;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_AppSetupApp.Extensions;
using XTI_Configuration.Extensions;
using XTI___APPNAME__AppApi;

await Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.UseXtiConfiguration(hostingContext.HostingEnvironment, args);
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddAppSetupServices(hostContext.HostingEnvironment, hostContext.Configuration);
        services.AddSingleton(_ => __APPNAME__Info.AppKey);
        services.AddSingleton(_ => AppVersionKey.Current);
        services.AddScoped<AppApiFactory, __APPNAME__AppApiFactory>();
        services.AddScoped<IAppSetup, __APPNAME__AppSetup>();
    })
    .RunConsoleAsync();