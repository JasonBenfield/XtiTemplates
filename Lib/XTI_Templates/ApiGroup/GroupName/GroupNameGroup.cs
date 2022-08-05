namespace XTI___APPNAME____APPTYPE__Api.__GROUPNAME__;

public sealed class __GROUPNAME__Group : AppApiGroupWrapper
{
    public __GROUPNAME__Group(AppApiGroup source, IServiceProvider sp)
        : base(source)
    {
        DoSomething = source.AddAction(nameof(DoSomething), () => sp.GetRequiredService<DoSomethingAction>());
    }

    public AppApiAction<EmptyRequest, EmptyActionResult> DoSomething { get; }
}