using XTI___APPNAME__WebServiceApi.Home;

namespace XTI___APPNAME__WebServiceApi;

partial class __APPNAME__AppApi
{
    private HomeGroup? home;

    public HomeGroup Home { get => home ?? throw new ArgumentNullException(nameof(home)); }

    partial void createHomeGroup(IServiceProvider sp)
    {
        home = new HomeGroup
        (
            source.AddGroup(nameof(Home), ResourceAccess.AllowAuthenticated()),
            sp
        );
    }
}