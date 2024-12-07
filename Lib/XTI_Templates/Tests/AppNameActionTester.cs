using Microsoft.Extensions.DependencyInjection;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_App.Fakes;

namespace __APPNAME____APPTYPE____TESTTYPE__Tests;

public static class __APPNAME__ActionTester
{
    public static __APPNAME__ActionTester<TModel, TResult> Create<TModel, TResult>(IServiceProvider services, Func<__APPNAME__AppApi, AppApiAction<TModel, TResult>> getAction)
    {
        return new __APPNAME__ActionTester<TModel, TResult>(services, getAction);
    }
}

public interface I__APPNAME__ActionTester
{
    IServiceProvider Services { get; }
    __APPNAME__ActionTester<TOtherModel, TOtherResult> Create<TOtherModel, TOtherResult>(Func<__APPNAME__AppApi, AppApiAction<TOtherModel, TOtherResult>> getAction);
}

public sealed class __APPNAME__ActionTester<TModel, TResult> : I__APPNAME__ActionTester
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

    public void Login(params AppRoleName[]? roleNames) => Login(ModifierCategoryName.Default, ModifierKey.Default, roleNames);

    public void Login(ModifierCategoryName categoryName, ModifierKey modifier, params AppRoleName[]? roleNames)
    {
        var userContext = Services.GetRequiredService<FakeUserContext>();
        var userName = new AppUserName("loggedinUser");
        userContext.AddUser(userName);
        userContext.SetCurrentUser(userName);
        userContext.SetUserRoles(categoryName, modifier, roleNames ?? new AppRoleName[0]);
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
        var modifierKeyAccessor = Services.GetRequiredService<FakeModifierKeyAccessor>();
        var path = actionForSuperUser.Path.WithModifier(modKey);
        modifierKeyAccessor.SetValue(modKey);
        var currentUserName = Services.GetRequiredService<ICurrentUserName>();
        var currentUserAccess = new CurrentUserAccess(userContext, appContext, currentUserName);
        var apiUser = new AppApiUser(currentUserAccess, modifierKeyAccessor);
        var appApi = (__APPNAME__AppApi)appApiFactory.Create(apiUser);
        var action = getAction(appApi);
        var result = await action.Invoke(model);
        return result;
    }
}