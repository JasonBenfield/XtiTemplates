using Microsoft.Extensions.Hosting;
using __APPNAME__ConsoleApp.Extensions;
using XTI_HubAppClient.ConsoleApp.Extensions;
using XTI___APPNAME__ConsoleAppApi;

await XtiConsoleAppHost.CreateDefault(__APPNAME__Info.AppKey, args)
    .ConfigureServices((hostContext, services) =>
    {
        services.Add__APPNAME__ConsoleAppServices();
        services.AddAppAgenda
        (
            (sp, agenda) =>
            {
                agenda.AddImmediate<TestAppApi>(api => api.Home.DoSomething);
            }
        );
    })
    .RunConsoleAsync();