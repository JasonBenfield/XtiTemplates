using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XTI_HubAppClient.WebApp.Extensions;
using XTI___APPNAME__AppApi;
using __APPNAME__WebApp.ApiControllers;
using XTI_App.Api;

namespace __APPNAME__WebApp.Extensions;

public static class __APPNAME__Extensions
{
    public static void Add__APPNAME__AppServices(this IServiceCollection services, IHostEnvironment hostEnv, IConfiguration configuration)
    {
        services.AddWebAppServices(hostEnv, configuration);
        services.AddSingleton(_ => __APPNAME__Info.AppKey);
        services.AddScoped<AppApiFactory, __APPNAME__AppApiFactory>();
        services.AddScoped(sp => (__APPNAME__AppApi)sp.GetRequiredService<IAppApi>());
        services.Add__APPNAME__AppApiServices();
        services
            .AddMvc()
            .AddJsonOptions(options =>
            {
                options.SetDefaultJsonOptions();
            })
            .AddMvcOptions(options =>
            {
                options.SetDefaultMvcOptions();
            });
        services.AddControllersWithViews()
            .PartManager.ApplicationParts.Add
            (
                new AssemblyPart(typeof(HomeController).Assembly)
            );
    }
}