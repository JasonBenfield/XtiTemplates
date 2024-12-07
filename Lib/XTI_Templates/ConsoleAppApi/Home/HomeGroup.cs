using System;

namespace XTI___APPNAME__ConsoleAppApi.Home;

public sealed class HomeGroup : AppApiGroupWrapper
{
    public HomeGroup(AppApiGroup source, IServiceProvider sp)
        : base(source)
    {
        DoSomething = source.AddAction<EmptyRequest, EmptyActionResult>()
            .Named(nameof(DoSomething))
            .WithExecution<DoSomethingAction>()
            .Build();
    }

    public AppApiAction<EmptyRequest, EmptyActionResult> DoSomething { get; }
}