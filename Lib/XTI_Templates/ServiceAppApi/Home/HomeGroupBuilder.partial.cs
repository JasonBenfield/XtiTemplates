namespace XTI___APPNAME__ServiceAppApi.Home;

partial class HomeGroupBuilder
{
    partial void Configure()
    {
        DoSomething
            .RunContinuously()
            .Interval(TimeSpan.FromSeconds(5))
            .AddSchedule
            (
                Schedule.EveryDay().At(TimeRange.AllDay())
            );
    }
}
