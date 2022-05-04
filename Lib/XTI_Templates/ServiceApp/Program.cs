using Microsoft.Extensions.Hosting;
using XTI_Core;
using XTI_HubAppClient.ServiceApp.Extensions;
using XTI_Schedule;
using XTI___APPNAME__ServiceAppApi;

await XtiServiceAppHost.CreateDefault(__APPNAME__Info.AppKey, args)
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
                        agendaItem.Action(api.Home.DoSomething.Path)
                            .Interval(TimeSpan.FromMinutes(5))
                            .AddSchedule
                            (
                                Schedule.EveryDay().At(TimeRange.AllDay())
                            );
                    }
                );
            }
        );
    })
    .UseWindowsService()
    .Build()
    .RunAsync();