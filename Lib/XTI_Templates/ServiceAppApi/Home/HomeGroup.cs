using XTI_Core;
using XTI_Schedule;

namespace XTI___APPNAME__ServiceAppApi.Home;

public sealed class HomeGroup : AppApiGroupWrapper
{
    public HomeGroup(AppApiGroup source, IServiceProvider sp)
        : base(source)
    {
        DoSomething = source.AddAction<EmptyRequest, EmptyActionResult>()
            .Named(nameof(DoSomething))
            .WithExecution<DoSomethingAction>()
            .RunUntilSuccess()
            .Interval(TimeSpan.FromMinutes(5))
            .AddSchedule(Schedule.EveryDay().At(TimeRange.AllDay()))
            .Build();
    }

    public AppApiAction<EmptyRequest, EmptyActionResult> DoSomething { get; }
}