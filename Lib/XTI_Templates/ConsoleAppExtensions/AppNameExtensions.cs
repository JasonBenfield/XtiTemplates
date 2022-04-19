using Microsoft.Extensions.DependencyInjection;
using XTI_App.Api;
using XTI___APPNAME__ConsoleAppApi;

namespace __APPNAME__ConsoleApp.Extensions;

public static class __APPNAME__Extensions
{
    public static void Add__APPNAME__ConsoleAppServices(this IServiceCollection services)
    {
        services.Add__APPNAME__AppApiServices();
        services.AddScoped<AppApiFactory, __APPNAME__AppApiFactory>();
        services.AddScoped(sp => (__APPNAME__AppApi)sp.GetRequiredService<IAppApi>());
    }
}