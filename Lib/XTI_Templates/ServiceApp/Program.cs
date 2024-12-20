using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XTI_App.Api;
using XTI_HubAppClient.ServiceApp.Extensions;
using XTI___APPNAME__ServiceAppApi;

var hostBuilder = XtiServiceAppHost.CreateDefault(__APPNAME__AppKey.Value, args)
    .ConfigureServices((hostContext, services) =>
    {
        services.Add__APPNAME__AppApiServices();
        services.AddScoped<AppApiFactory, __APPNAME__AppApiFactory>();
        services.AddScoped(sp => (__APPNAME__AppApi)sp.GetRequiredService<IAppApi>());
    });
if (args.Length <= 0 || !args[0].Equals("RunAsConsole", StringComparison.OrdinalIgnoreCase))
{
    hostBuilder.UseWindowsService();
}
var host = hostBuilder.Build();
await host.RunAsync();