using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_App.Extensions;
using XTI_App.Fakes;
using XTI_Core;
using XTI_Core.Extensions;
using XTI_Core.Fakes;
using XTI_HubAppClient.Extensions;
using XTI_WebApp.Fakes;
using XTI_Secrets.Extensions;

namespace __APPNAME____APPTYPE____TESTTYPE__Tests;

internal sealed class __APPNAME__TestHost
{
    public Task<IServiceProvider> Setup(string envName, Action<IServiceCollection>? configure = null)
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", envName);
        var xtiEnv = XtiEnvironment.Parse(envName);
        var appKey = __APPNAME__Info.AppKey;
        var builder = new XtiHostBuilder(xtiEnv, appKey.Name.DisplayText, appKey.Type.DisplayText, []);
        builder.Services.AddFakesForXtiWebApp();
        builder.Services.AddSingleton<IHostEnvironment>
        (
            _ => new FakeHostEnvironment { EnvironmentName = envName }
        );
        builder.Services.AddSingleton<XtiFolder>();
        builder.Services.AddSingleton(sp => sp.GetRequiredService<XtiFolder>().AppDataFolder(appKey));
        builder.Services.AddSingleton(_ => appKey);
        builder.Services.AddSingleton(_ => AppVersionKey.Current);
        builder.Services.Add__APPNAME__AppApiServices();
        builder.Services.AddScoped<__APPNAME__AppApiFactory>();
        builder.Services.AddScoped<AppApiFactory>(sp => sp.GetRequiredService<__APPNAME__AppApiFactory>());
        builder.Services.AddScoped(sp => sp.GetRequiredService<AppApiFactory>().Create(sp.GetRequiredService<IAppApiUser>()));
        builder.Services.AddScoped(sp => (__APPNAME__AppApi)sp.GetRequiredService<IAppApi>());
        builder.Services.AddScoped<IAppContext>(sp => sp.GetRequiredService<FakeAppContext>());
        builder.Services.AddScoped<ICurrentUserName>(sp => sp.GetRequiredService<FakeCurrentUserName>());
        builder.Services.AddScoped<IUserContext>(sp => sp.GetRequiredService<FakeUserContext>());
        builder.Services.AddConfigurationOptions<DefaultAppOptions>();
        builder.Services.AddSingleton(sp => sp.GetRequiredService<DefaultAppOptions>().HubClient);
        builder.Services.AddSingleton(sp => sp.GetRequiredService<DefaultAppOptions>().XtiToken);
        builder.Services.AddSingleton(sp => sp.GetRequiredService<DefaultAppOptions>().DB);
        builder.Services.AddSingleton(sp => sp.GetRequiredService<DefaultAppOptions>().TempLog);
        builder.Services.AddScoped<IAppClientSessionKey, DefaultAppClientSessionKey>();
        builder.Services.AddScoped<IAppClientRequestKey, DefaultAppClientRequestKey>();
        builder.Services.AddScoped<AppClientOptions>();
        builder.Services.AddHttpClient();
        builder.Services.AddFileSecretCredentials(xtiEnv);
        builder.Services.AddHubClientServices();
        builder.Services.AddSystemUserXtiToken();
        builder.Services.AddConfigurationXtiToken();
        builder.Services.AddXtiTokenAccessorFactory((sp, accessor) =>
        {
            accessor.AddToken(() => sp.GetRequiredService<SystemUserXtiToken>());
            accessor.AddToken(() => sp.GetRequiredService<ConfigurationXtiToken>());
            accessor.UseDefaultToken<ConfigurationXtiToken>();
        });
        if (configure != null)
        {
            configure(builder.Services);
        }
        var sp = builder.Build().Scope();
        var apiFactory = sp.GetRequiredService<AppApiFactory>();
        var template = apiFactory.CreateTemplate();
        var appContext = sp.GetRequiredService<FakeAppContext>();
        var app = appContext.RegisterApp(template.ToModel());
        appContext.SetCurrentApp(app);
        var userContext = (FakeUserContext)sp.GetRequiredService<ISourceUserContext>();
        var userName = new AppUserName("admin.user");
        userContext.AddUser(userName);
        userContext.SetCurrentUser(userName);
        userContext.SetUserRoles(AppRoleName.Admin);
        return Task.FromResult(sp);
    }
}