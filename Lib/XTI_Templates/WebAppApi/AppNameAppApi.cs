namespace XTI___APPNAME__AppApi;

public sealed partial class __APPNAME__AppApi : WebAppApiWrapper
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
            ),
            sp
        )
    {
        createHomeGroup(sp);
    }

    partial void createHomeGroup(IServiceProvider sp);
}