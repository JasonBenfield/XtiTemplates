namespace XTI___APPNAME__WebAppApiActions.Home;

public sealed class IndexPage : AppAction<EmptyRequest, WebViewResult>
{
    private readonly WebViewResultFactory viewFactory;

    public IndexPage(WebViewResultFactory viewFactory)
    {
        this.viewFactory = viewFactory;
    }

    public Task<WebViewResult> Execute(EmptyRequest requestData, CancellationToken stoppingToken) =>
        Task.FromResult(viewFactory.Default("home", "Home"));
}