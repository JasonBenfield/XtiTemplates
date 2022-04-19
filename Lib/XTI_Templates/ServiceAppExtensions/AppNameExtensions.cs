using Microsoft.Extensions.DependencyInjection;
using XTI_App.Api;
using XTI___APPNAME__ServiceAppApi;

namespace __APPNAME__ServiceApp.Extensions;

public static class __APPNAME__Extensions
{
    public static void Add__APPNAME__ServiceAppServices(this IServiceCollection services)
    {
        services.AddSingleton<IAppApiUser, AppApiSuperUser>();
        services.Add__APPNAME__AppApiServices();
        services.AddScoped<AppApiFactory, __APPNAME__AppApiFactory>();
        services.AddScoped(sp => (__APPNAME__AppApi)sp.GetRequiredService<IAppApi>());
    }
}