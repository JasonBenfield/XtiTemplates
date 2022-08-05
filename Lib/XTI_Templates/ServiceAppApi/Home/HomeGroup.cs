namespace XTI___APPNAME__ServiceAppApi.Home;

public sealed class HomeGroup : AppApiGroupWrapper
{
    public HomeGroup(AppApiGroup source, IServiceProvider sp)
        : base(source)
    {
        DoSomething = source.AddAction(nameof(DoSomething), () => sp.GetRequiredService<DoSomethingAction>());
    }

    public AppApiAction<EmptyRequest, EmptyActionResult> DoSomething { get; }
}