using XTI_Core;

namespace XTI___APPNAME__WebAppApi;

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
                sp,
                __APPNAME__Info.AppKey,
                user,
                ResourceAccess.AllowAuthenticated()
                    .WithAllowed(AppRoleName.Admin),
                XtiSerializer.Serialize(new __APPNAME__AppOptions())
            ),
            sp
        )
    {
        createHomeGroup(sp);
    }

    partial void createHomeGroup(IServiceProvider sp);
}