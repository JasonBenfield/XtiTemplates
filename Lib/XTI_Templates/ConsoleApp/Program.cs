using Microsoft.Extensions.Hosting;
using XTI_HubAppClient.ConsoleApp.Extensions;
using XTI___APPNAME__ConsoleAppApi;

await XtiConsoleAppHost.CreateDefault(__APPNAME__Info.AppKey, args)
    .ConfigureServices((hostContext, services) =>
    {
        services.Add__APPNAME__AppApiServices();
        services.AddScoped<AppApiFactory, __APPNAME__AppApiFactory>();
        services.AddScoped(sp => (__APPNAME__AppApi)sp.GetRequiredService<IAppApi>());
        services.AddAppAgenda
        (
            (sp, agenda) =>
            {
                agenda.AddScheduled<__APPNAME__AppApi>
                (
                    (api, agendaItem) =>
                    {
                        agendaItem.AddImmediate<TestAppApi>(api => api.Home.DoSomething);
                    }
                );
            }
        );
    })
    .RunConsoleAsync();