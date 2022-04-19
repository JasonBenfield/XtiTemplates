using XTI___APPNAME__ConsoleAppApi.Home;

namespace XTI___APPNAME__ConsoleAppApi;

internal static class HomeGroupExtensions
{
    public static void AddHomeGroupServices(this IServiceCollection services)
    {
        services.AddScoped<DoSomethingAction>();
    }
}