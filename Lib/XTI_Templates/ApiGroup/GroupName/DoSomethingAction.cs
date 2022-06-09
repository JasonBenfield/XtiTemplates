namespace XTI___APPNAME____APPTYPE__Api.__GROUPNAME__;

public sealed class DoSomethingAction : AppAction<EmptyRequest, EmptyActionResult>
{
    public async Task<EmptyActionResult> Execute(EmptyRequest model, CancellationToken ct)
    {
        Console.WriteLine($"{DateTime.Now:M/dd/yy HH:mm:ss} Doing Something");
        return new EmptyActionResult();
    }
}