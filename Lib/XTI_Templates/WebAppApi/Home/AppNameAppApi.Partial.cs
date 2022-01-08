using XTI___APPNAME__AppApi.Home;

namespace XTI___APPNAME__AppApi;

partial class __APPNAME__AppApi
{
    private HomeGroup? home;

    public HomeGroup Home { get => home ?? throw new ArgumentNullException(nameof(home)); }

    partial void createHomeGroup(IServiceProvider sp)
    {
        home = new HomeGroup
        (
            source.AddGroup(nameof(Home)),
            sp
        );
    }
}