using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_App.Fakes;
using XTI___APPNAME____APPTYPE__Api;

namespace __APPNAME____TESTTYPE__Tests;

internal static class __APPNAME__ActionTester
{
    public static __APPNAME__ActionTester<TModel, TResult> Create<TModel, TResult>(IServiceProvider services, Func<__APPNAME__AppApi, AppApiAction<TModel, TResult>> getAction)
    {
        return new __APPNAME__ActionTester<TModel, TResult>(services, getAction);
    }
}

internal interface I__APPNAME__ActionTester
{
    IServiceProvider Services { get; }
    __APPNAME__ActionTester<TOtherModel, TOtherResult> Create<TOtherModel, TOtherResult>(Func<__APPNAME__AppApi, AppApiAction<TOtherModel, TOtherResult>> getAction);
}

internal sealed class __APPNAME__ActionTester<TModel, TResult> : I__APPNAME__ActionTester
{
    private readonly Func<__APPNAME__AppApi, AppApiAction<TModel, TResult>> getAction;

    public __APPNAME__ActionTester
    (
        IServiceProvider services,
        Func<__APPNAME__AppApi, AppApiAction<TModel, TResult>> getAction
    )
    {
        Services = services;
        this.getAction = getAction;
    }

    public __APPNAME__ActionTester<TOtherModel, TOtherResult> Create<TOtherModel, TOtherResult>
    (
        Func<__APPNAME__AppApi, AppApiAction<TOtherModel, TOtherResult>> getAction
    )
    {
        return __APPNAME__ActionTester.Create(Services, getAction);
    }

    public IServiceProvider Services { get; }

    public void Logout()
    {
        var currentUserName = Services.GetRequiredService<FakeCurrentUserName>();
        currentUserName.SetUserName(AppUserName.Anon);
    }

    public void LoginAsAdmin()
    {
        var currentUserName = Services.GetRequiredService<FakeCurrentUserName>();
        currentUserName.SetUserName(new AppUserName("admin.user"));
    }

    public void Login(params AppRoleName[]? roleNames) => Login(ModifierKey.Default, roleNames);

    public void Login(ModifierKey modifier, params AppRoleName[]? roleNames)
    {
        var userContext = Services.GetRequiredService<FakeUserContext>();
        var userName = new AppUserName("loggedinUser");
        userContext.AddUser(userName);
        userContext.SetCurrentUser(userName);
        userContext.SetUserRoles(modifier, roleNames ?? new AppRoleName[0]);
    }

    public Task<TResult> Execute(TModel model) =>
        Execute(model, ModifierKey.Default);

    public async Task<TResult> Execute(TModel model, ModifierKey modKey)
    {
        var appContext = Services.GetRequiredService<IAppContext>();
        var appApiFactory = Services.GetRequiredService<AppApiFactory>();
        var apiForSuperUser = (__APPNAME__AppApi)appApiFactory.CreateForSuperUser();
        var actionForSuperUser = getAction(apiForSuperUser);
        var modKeyPath = modKey.Equals(ModifierKey.Default) ? "" : $"/{modKey.Value}";
        var appKey = Services.GetRequiredService<AppKey>();
        var userContext = Services.GetRequiredService<ISourceUserContext>();
        var pathAccessor = Services.GetRequiredService<FakeXtiPathAccessor>();
        var path = actionForSuperUser.Path.WithModifier(modKey ?? ModifierKey.Default);
        pathAccessor.SetPath(path);
        var currentUserName = Services.GetRequiredService<ICurrentUserName>();
        var currentUserAccess = new CurrentUserAccess(userContext, appContext, currentUserName);
        var apiUser = new AppApiUser(currentUserAccess, pathAccessor);
        var appApi = (__APPNAME__AppApi)appApiFactory.Create(apiUser);
        var action = getAction(appApi);
        var result = await action.Invoke(model);
        return result;
    }
}