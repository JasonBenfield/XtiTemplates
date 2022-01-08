namespace XTI___APPNAME__AppApi;

public sealed class __APPNAME__AppApiFactory : AppApiFactory
{
    private readonly IServiceProvider sp;

    public __APPNAME__AppApiFactory(IServiceProvider sp)
    {
        this.sp = sp;
    }

    public new __APPNAME__AppApi Create(IAppApiUser user) => (__APPNAME__AppApi)base.Create(user);
    public new __APPNAME__AppApi CreateForSuperUser() => (__APPNAME__AppApi)base.CreateForSuperUser();

    protected override IAppApi _Create(IAppApiUser user) => new __APPNAME__AppApi(user, sp);
}