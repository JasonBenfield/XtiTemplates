using System.Text.Json;
using XTI_Core;

namespace __APPNAME____TESTTYPE__Tests;

internal static class TestExtensions
{
    public static void WriteToConsole(this object data) =>
        Console.WriteLine
        (
            XtiSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true })
        );
}
