using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XTI___APPNAME__WebAppApi;
using XTI_App.Api;

namespace __APPNAME__WebApp.Extensions;

public static class __APPNAME__Extensions
{
    public static void Add__APPNAME__WebAppServices(this IServiceCollection services)
    {
        services.AddScoped<AppApiFactory, __APPNAME__AppApiFactory>();
        services.AddScoped(sp => (__APPNAME__AppApi)sp.GetRequiredService<IAppApi>());
        services.Add__APPNAME__AppApiServices();
    }
}