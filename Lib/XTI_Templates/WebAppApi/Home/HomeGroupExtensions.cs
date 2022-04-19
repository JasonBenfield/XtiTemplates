using XTI___APPNAME__WebAppApi.Home;

namespace XTI___APPNAME__WebAppApi;

internal static class HomeGroupExtensions
{
    public static void AddHomeGroupServices(this IServiceCollection services)
    {
        services.AddScoped<IndexAction>();
    }
}