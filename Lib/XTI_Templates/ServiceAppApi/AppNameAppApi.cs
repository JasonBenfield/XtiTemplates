namespace XTI___APPNAME__ServiceAppApi;

public sealed partial class __APPNAME__AppApi : AppApiWrapper
{
    public __APPNAME__AppApi
    (
        IAppApiUser user,
        IServiceProvider sp
    )
        : base
        (
            new AppApi
            (
                __APPNAME__Info.AppKey,
                user,
                ResourceAccess.AllowAuthenticated()
                    .WithAllowed(AppRoleName.Admin)
            )
        )
    {
        createHomeGroup(sp);
    }

    partial void createHomeGroup(IServiceProvider sp);
}