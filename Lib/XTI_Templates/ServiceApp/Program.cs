using Microsoft.Extensions.Hosting;
using __APPNAME__ServiceApp.Extensions;
using XTI_Core;
using XTI_HubAppClient.ServiceApp.Extensions;
using XTI_Schedule;
using XTI___APPNAME__ServiceAppApi;

await XtiServiceAppHost.CreateDefault(__APPNAME__Info.AppKey, args)
    .ConfigureServices((hostContext, services) =>
    {
        services.Add__APPNAME__ServiceAppServices();
        services.AddAppAgenda
        (
            (sp, agenda) =>
            {
                agenda.AddScheduled<__APPNAME__AppApi>
                (
                    (api, agenda) =>
                    {
                        agenda.Action(api.Home.DoSomething.Path)
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
    .RunConsoleAsync();