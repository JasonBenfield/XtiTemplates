using XTI___APPNAME__WebServiceApi.Home;

namespace XTI___APPNAME__WebServiceApi;

internal static class HomeGroupExtensions
{
    public static void AddHomeGroupServices(this IServiceCollection services)
    {
        services.AddScoped<IndexAction>();
    }
}