using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XTI_App.Api;
using XTI_HubAppClient.ConsoleApp.Extensions;
using XTI___APPNAME__ConsoleAppApi;

await XtiConsoleAppHost.CreateDefault(__APPNAME__AppKey.Value, args)
    .ConfigureServices((hostContext, services) =>
    {
        services.Add__APPNAME__AppApiServices();
        services.AddScoped<AppApiFactory, __APPNAME__AppApiFactory>();
        services.AddScoped(sp => (__APPNAME__AppApi)sp.GetRequiredService<IAppApi>());
        services.AddAppAgenda
        (
            (sp, agenda) =>
            {
                agenda.AddImmediate<__APPNAME__AppApi>(api => api.Home.DoSomething);
            }
        );
    })
    .RunConsoleAsync();