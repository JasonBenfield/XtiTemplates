using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using XTI_Processes;

namespace XTI_DotnetTool;

internal sealed class HostedService : IHostedService
{
    private readonly IServiceProvider sp;

    public HostedService(IServiceProvider sp)
    {
        this.sp = sp;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = sp.CreateScope();
        try
        {
            var options = scope.ServiceProvider.GetRequiredService<IOptions<ToolOptions>>().Value;
            if (string.IsNullOrWhiteSpace(options.AppName)) { throw new ArgumentException("App Name is required."); }
            if (string.IsNullOrWhiteSpace(options.AppType)) { throw new ArgumentException("App Type is required."); }
            var appType = new[] { "WebApp", "ServiceApp", "ConsoleApp" }.FirstOrDefault(str => str.Equals(options.AppType.Replace(" ", "")));
            if (appType == null) { throw new NotSupportedException($"App type '{options.AppType}' is not supported"); }
            var appName = options.AppName.Replace(" ", "");
            var fullAppName = $"{appName}{appType}";
            await DotnetInstallTemplate("WebAppApi");
            await DotnetInstallTemplate("WebAppControllers");
            await DotnetInstallTemplate("WebAppClient");
            await DotnetInstallTemplate("WebAppExtensions");
            await DotnetInstallTemplate("ApiGeneratorApp");
            await DotnetInstallTemplate("SetupApp");
            await DotnetInstallTemplate("WebApp");
            await DotnetInstallTemplate("WebAppSolution");
            var dir = string.IsNullOrWhiteSpace(options.Directory) ? Environment.CurrentDirectory : options.Directory;
            var slnDir = Path.Combine(dir, fullAppName);
            if (options.DeleteExisting)
            {
                if (Directory.Exists(slnDir)) { Directory.Delete(slnDir, true); }
            }
            await DotnetNewSln(slnDir);
            var appsDir = CreateDirIfNotExists(Path.Combine(slnDir, "Apps"));
            var internalDir = CreateDirIfNotExists(Path.Combine(slnDir, "Internal"));
            var libDir = CreateDirIfNotExists(Path.Combine(slnDir, "Lib"));
            if (appType == "WebApp")
            {
                await DotnetNewWebAppSolution(slnDir, appName);
                await DotnetNewApiGeneratorApp(appsDir, appName);
                await DotnetNewWebAppApi(internalDir, appName);
                await DotnetNewWebAppControllers(internalDir, appName);
                await DotnetNewWebAppClient(libDir, appName);
                await DotnetNewWebAppExtensions(internalDir, appName);
                await DotnetNewApiGeneratorApp(appsDir, appName);
                await DotnetNewSetupApp(appsDir, appName);
                await DotnetNewWebApp(appsDir, appName);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            Environment.ExitCode = -999;
        }
        var lifetime = scope.ServiceProvider.GetRequiredService<IHostApplicationLifetime>();
        lifetime.StopApplication();
    }

    private static async Task DotnetNewSln(string slnDir)
    {
        CreateDirIfNotExists(slnDir);
        Console.WriteLine($"running dotnet new sln '{slnDir}'");
        var newSlnResult = await new WinProcess("dotnet")
            .WriteOutputToConsole()
            .SetWorkingDirectory(slnDir)
            .AddArgument("new")
            .AddArgument("sln")
            .Run();
        newSlnResult.EnsureExitCodeIsZero();
    }

    private static string CreateDirIfNotExists(string dir)
    {
        if (!Directory.Exists(dir)) { Directory.CreateDirectory(dir); }
        return dir;
    }

    private static async Task DotnetInstallTemplate(string name)
    {
        var templateDir = Path.Combine("..", "..", "Lib", "XTI_Templates", name);
        Console.WriteLine($"Uninstalling template '{templateDir}'");
        await new WinProcess("dotnet")
            .WriteOutputToConsole()
            .AddArgument("new")
            .UseArgumentNameDelimiter("--")
            .AddArgument("uninstall", templateDir)
            .Run();
        Console.WriteLine($"Installing template '{templateDir}'");
        var result = await new WinProcess("dotnet")
            .WriteOutputToConsole()
            .AddArgument("new")
            .UseArgumentNameDelimiter("--")
            .AddArgument("install", templateDir)
            .Run();
        result.EnsureExitCodeIsZero();
    }

    private static async Task DotnetNewWebAppSolution(string slnDir, string appName)
    {
        var solutionFiles = new[] { "xti.private.ps1", "xti.ps1", "common.targets" };
        var psFileName = $"xti.ps1";
        if
        (
            Directory.GetFiles(slnDir)
                .Select(f => Path.GetFileName(f))
                .Intersect(solutionFiles, StringComparer.InvariantCultureIgnoreCase)
                .Any()
        )
        {
            Console.WriteLine($"Solution files have already been added");
        }
        else
        {
            var templateName = "xtiwebappsolution";
            Console.WriteLine($"running dotnet new '{templateName}' '{slnDir}'");
            var dotnetNewResult = await new WinProcess("dotnet")
                .WriteOutputToConsole()
                .SetWorkingDirectory(slnDir)
                .AddArgument("new")
                .AddArgument(templateName)
                .UseArgumentNameDelimiter("--")
                .AddArgument("AppName", appName)
                .Run();
            dotnetNewResult.EnsureExitCodeIsZero();
        }
    }

    private static Task DotnetNewWebAppApi(string internalDir, string appName) =>
        DotnetNewProject(Path.Combine(internalDir, $"XTI_{appName}AppApi"), "xtiwebappapi", appName);

    private static Task DotnetNewWebAppControllers(string internalDir, string appName) =>
        DotnetNewProject(Path.Combine(internalDir, $"{appName}WebApp.ApiControllers"), "xtiwebappcontrollers", appName);

    private static Task DotnetNewWebAppClient(string libDir, string appName) =>
        DotnetNewProject(Path.Combine(libDir, $"XTI_{appName}AppClient"), "xtiwebappclient", appName);

    private static Task DotnetNewWebAppExtensions(string internalDir, string appName) =>
        DotnetNewProject(Path.Combine(internalDir, $"{appName}WebApp.Extensions"), "xtiwebappextensions", appName);

    private static Task DotnetNewApiGeneratorApp(string appsDir, string appName) =>
        DotnetNewProject(Path.Combine(appsDir, $"{appName}ApiGeneratorApp"), "xtiapigeneratorapp", appName);

    private static Task DotnetNewSetupApp(string appsDir, string appName) =>
        DotnetNewProject(Path.Combine(appsDir, $"{appName}SetupApp"), "xtisetupapp", appName);

    private static Task DotnetNewWebApp(string appsDir, string appName) =>
        DotnetNewProject(Path.Combine(appsDir, $"{appName}WebApp"), "xtiwebapp", appName);

    private static async Task DotnetNewProject(string projectDir, string templateName, string appName)
    {
        CreateDirIfNotExists(projectDir);
        var projectFileName = $"{new DirectoryInfo(projectDir).Name}.csproj";
        if (File.Exists(Path.Combine(projectDir, projectFileName)))
        {
            Console.WriteLine($"Project '{projectFileName}' already exists");
        }
        else
        {
            Console.WriteLine($"running dotnet new '{templateName}' '{projectDir}'");
            var dotnetNewResult = await new WinProcess("dotnet")
                .WriteOutputToConsole()
                .SetWorkingDirectory(projectDir)
                .AddArgument("new")
                .AddArgument(templateName)
                .UseArgumentNameDelimiter("--")
                .AddArgument("AppName", appName)
                .Run();
            dotnetNewResult.EnsureExitCodeIsZero();
            var slnDir = new DirectoryInfo(projectDir).Parent?.Parent?.FullName ?? "";
            Console.WriteLine($"adding project '{projectDir}' to '{slnDir}'");
            var dotnetAddResult = await new WinProcess("dotnet")
                .WriteOutputToConsole()
                .SetWorkingDirectory(slnDir)
                .AddArgument("sln")
                .AddArgument("add")
                .AddArgument(new Quoted(projectDir))
                .Run();
            dotnetAddResult.EnsureExitCodeIsZero();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}