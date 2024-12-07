using System;

namespace XTI___APPNAME____APPTYPE__Api.__GROUPNAME__;

public sealed class __GROUPNAME__Group : AppApiGroupWrapper
{
    public __GROUPNAME__Group(AppApiGroup source, IServiceProvider sp)
        : base(source)
    {
        DoSomething = source.AddAction<EmptyRequest, EmptyActionResult>()
            .Named(nameof(DoSomething))
            .WithExecution<DoSomethingAction>()
            .Build();
    }

    public AppApiAction<EmptyRequest, EmptyActionResult> DoSomething { get; }
}