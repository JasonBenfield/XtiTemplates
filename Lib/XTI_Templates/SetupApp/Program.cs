using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using __APPNAME__SetupApp;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_AppSetupApp.Extensions;
using XTI___APPNAME____APPTYPE__Api;

await XtiSetupAppHost.CreateDefault(__APPNAME__AppKey.Value, args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddSingleton(_ => AppVersionKey.Current);
        services.AddScoped<AppApiFactory, __APPNAME__AppApiFactory>();
        services.AddScoped<IAppSetup, __APPNAME__AppSetup>();
    })
    .RunConsoleAsync();