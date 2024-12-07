using XTI_Core;

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
                sp,
                __APPNAME__Info.AppKey,
                user,
                ResourceAccess.AllowAuthenticated()
                    .WithAllowed(AppRoleName.Admin),
                XtiSerializer.Serialize(new __APPNAME__AppOptions())
            )
        )
    {
        createHomeGroup(sp);
    }

    partial void createHomeGroup(IServiceProvider sp);
}