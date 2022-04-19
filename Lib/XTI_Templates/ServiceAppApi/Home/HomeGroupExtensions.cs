using XTI___APPNAME__ServiceAppApi.Home;

namespace XTI___APPNAME__ServiceAppApi;

internal static class HomeGroupExtensions
{
    public static void AddHomeGroupServices(this IServiceCollection services)
    {
        services.AddScoped<DoSomethingAction>();
    }
}