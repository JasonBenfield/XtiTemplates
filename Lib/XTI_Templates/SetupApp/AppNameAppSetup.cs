using XTI_App.Abstractions;

namespace __APPNAME__SetupApp;

internal sealed class __APPNAME__AppSetup : IAppSetup
{
    public Task Run(AppVersionKey versionKey)
    {
        return Task.CompletedTask;
    }
}
