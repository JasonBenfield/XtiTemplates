using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Dynamic;
using XTI_Core;
using XTI_Git;
using XTI_GitHub;
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
            if (options.Command.Equals("uninstall", StringComparison.InvariantCultureIgnoreCase))
            {
                await DotnetUninstallTemplatesLocally();
            }
            else if (options.Command.Equals("install", StringComparison.InvariantCultureIgnoreCase))
            {
                await DotnetInstallTemplatesLocally();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(options.RepoOwner)) { throw new ArgumentException("Repo Owner is required."); }
                if (string.IsNullOrWhiteSpace(options.RepoName)) { throw new ArgumentException("Repo Name is required."); }
                if (options.InstallTemplatesLocally)
                {
                    await DotnetInstallTemplatesLocally();
                }
                var srcDir = options.SrcDir;
                if (string.IsNullOrWhiteSpace(srcDir))
                {
                    var xtiFolder = scope.ServiceProvider.GetRequiredService<XtiFolder>();
                    srcDir = Path.Combine(xtiFolder.FolderPath(), "src");
                }
                var slnDir = Path.Combine(srcDir, options.RepoOwner, options.RepoName);
                if (options.DeleteExisting && Directory.Exists(slnDir))
                {
                    Directory.Delete(slnDir, true);
                }
                if (!Directory.Exists(slnDir))
                {
                    Directory.CreateDirectory(slnDir);
                }
                Environment.CurrentDirectory = slnDir;
                var appType = getAppType(options);
                var appName = options.AppName.Replace(" ", "");
                var fullAppName = $"{appName}{appType}";
                if (options.Command.Equals("apigroup", StringComparison.InvariantCultureIgnoreCase))
                {
                    await DotnetNewApiGroup(options, appName);
                }
                else if (options.Command.Equals("tests", StringComparison.InvariantCultureIgnoreCase))
                {
                    await DotnetTests(options);
                }
                else
                {
                    var gitHubFactory = sp.GetRequiredService<IGitHubFactory>();
                    var gitHubRepo = await gitHubFactory.CreateNewGitHubRepositoryIfNotExists(options.RepoOwner, options.RepoName);
                    if (!Directory.Exists(slnDir) || (!Directory.GetFiles(slnDir).Any() && !Directory.GetDirectories(slnDir).Any()))
                    {
                        var gitFactory = sp.GetRequiredService<IXtiGitFactory>();
                        var repoInfo = await gitHubRepo.RepositoryInformation();
                        await gitFactory.CloneRepository(repoInfo.CloneUrl, slnDir);
                        await DotnetNewSln();
                    }
                    if (appType != "")
                    {
                        CreateDirIfNotExists(getLibDir(options));
                        if (appType != "Package")
                        {
                            CreateDirIfNotExists(getAppsDir(options));
                            CreateDirIfNotExists(getInternalDir(options));
                        }
                    }
                    await DotnetNewXtiSolution(options.RepoOwner, options.RepoName);
                    if (appType == "WebApp")
                    {
                        if (string.IsNullOrWhiteSpace(options.AppName)) { throw new ArgumentException("App Name is required."); }
                        await AddWebAppProjects(options, appName);
                        await NpmInstall(options);
                    }
                    else if (appType == "WebPackage")
                    {
                        if (string.IsNullOrWhiteSpace(options.AppName)) { throw new ArgumentException("App Name is required."); }
                        await DotnetNewWebPackage(options, appName);
                        await NpmInstall(options);
                    }
                    else if (appType == "ServiceApp")
                    {
                        if (string.IsNullOrWhiteSpace(options.AppName)) { throw new ArgumentException("App Name is required."); }
                        await AddServiceAppProjects(options, appName);
                    }
                    else if (appType == "ConsoleApp")
                    {
                        if (string.IsNullOrWhiteSpace(options.AppName)) { throw new ArgumentException("App Name is required."); }
                        await AddConsoleAppProjects(options, appName);
                    }
                    else if (appType == "Package")
                    {
                        if (string.IsNullOrWhiteSpace(options.AppName)) { throw new ArgumentException("App Name is required."); }
                        await DotnetNewPackageExport(options, appName);
                    }
                }
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

    private static async Task DotnetNewApiGroup(ToolOptions options, string appName)
    {
        if (string.IsNullOrWhiteSpace(options.GroupName)) { throw new ArgumentException("Group Name is required."); }
        var templateName = "xtiapigroup";
        var appType = getAppType(options);
        var projectDir = Path.Combine(getInternalDir(options), $"XTI_{appName}{appType}Api");
        if (!Directory.Exists(projectDir))
        {
            throw new ArgumentException($"Project Directory '{projectDir}' does not exist.");
        }
        if (Directory.Exists(Path.Combine(projectDir, options.GroupName)))
        {
            throw new ArgumentException($"Api Group '{options.GroupName}' already exists.");
        }
        Console.WriteLine($"running dotnet new '{templateName}' '{projectDir}'");
        var dotnetNewResult = await new WinProcess("dotnet")
            .WriteOutputToConsole()
            .SetWorkingDirectory(projectDir)
            .AddArgument("new")
            .AddArgument(templateName)
            .UseArgumentNameDelimiter("--")
            .AddArgument("AppName", appName)
            .AddArgument("AppType", appType)
            .AddArgument("GroupName", options.GroupName)
            .Run();
        dotnetNewResult.EnsureExitCodeIsZero();
    }

    private static async Task NpmInstall(ToolOptions options)
    {
        var result = await new CmdProcess
        (
            new WinProcess("npm").AddArgument("install")
        )
        .WriteOutputToConsole()
        .SetWorkingDirectory(Path.Combine(getAppsDir(options), getFullAppName(options)))
        .Run();
        result.EnsureExitCodeIsZero();
    }

    private static string getFullAppName(ToolOptions options)
    {
        var appName = getAppName(options);
        var appType = getAppType(options);
        var fullAppName = appType == "Package" ? appName : $"{appName}{appType}";
        return fullAppName;
    }

    private static string getAppName(ToolOptions options) => options.AppName.Replace(" ", "");

    private static string getAppsDir(ToolOptions options)
    {
        return Path.Combine(Environment.CurrentDirectory, getFullAppName(options), "Apps");
    }

    private static string getTestsDir(ToolOptions options)
    {
        return Path.Combine(Environment.CurrentDirectory, getFullAppName(options), "Tests");
    }

    private static string getInternalDir(ToolOptions options)
    {
        return Path.Combine(Environment.CurrentDirectory, getFullAppName(options), "Internal");
    }

    private static string getLibDir(ToolOptions options)
    {
        return Path.Combine(Environment.CurrentDirectory, getFullAppName(options), "Lib");
    }

    private static string getAppType(ToolOptions options)
    {
        var appType = new[] { "", "WebApp", "ServiceApp", "ConsoleApp", "Package", "WebPackage" }.FirstOrDefault(str => str.Equals(options.AppType.Replace(" ", "")));
        if (appType == null) { throw new NotSupportedException($"App type '{options.AppType}' is not supported"); }
        return appType;
    }

    private static async Task AddConsoleAppProjects(ToolOptions options, string appName)
    {
        await DotnetNewConsoleApp(options, appName);
        await DotnetNewConsoleAppApi(options, appName);
        await DotnetNewSetupApp(options, appName);
    }

    private static async Task AddServiceAppProjects(ToolOptions options, string appName)
    {
        await DotnetNewServiceApp(options, appName);
        await DotnetNewServiceAppApi(options, appName);
        await DotnetNewSetupApp(options, appName);
    }

    private static async Task AddWebAppProjects(ToolOptions options, string appName)
    {
        await DotnetNewWebApp(options, appName);
        await DotnetNewWebAppApi(options, appName);
        await DotnetNewWebAppControllers(options, appName);
        await DotnetNewWebAppClient(options, appName);
        await DotnetNewApiGeneratorApp(options, appName);
        await DotnetNewSetupApp(options, appName);
    }

    private static readonly string[] LocalTemplateNames = new[]
    {
        "ApiGeneratorApp",
        "ApiGroup",
        "ConsoleApp",
        "ConsoleAppApi",
        "PackageExport",
        "ServiceApp",
        "ServiceAppApi",
        "SetupApp",
        "WebApp",
        "WebAppApi",
        "WebAppClient",
        "WebAppControllers",
        "WebPackage",
        "XtiSolution",
        "Tests"
    };

    private static async Task DotnetUninstallTemplatesLocally()
    {
        foreach (var templateName in LocalTemplateNames)
        {
            await DotnetUninstallTemplateLocally(templateName);
        }
    }

    private static async Task DotnetInstallTemplatesLocally()
    {
        foreach (var templateName in LocalTemplateNames)
        {
            await DotnetInstallTemplateLocally(templateName);
        }
    }

    private static async Task DotnetNewSln()
    {
        Console.WriteLine($"running dotnet new sln '{Environment.CurrentDirectory}'");
        var newSlnResult = await new WinProcess("dotnet")
            .WriteOutputToConsole()
            .SetWorkingDirectory(Environment.CurrentDirectory)
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

    private static async Task DotnetInstallTemplateLocally(string name)
    {
        await DotnetUninstallTemplateLocally(name);
        var templateDir = getLocalTemplateDir(name);
        var process = new WinProcess("dotnet")
            .WriteOutputToConsole()
            .AddArgument("new")
            .AddArgument("install", templateDir);
        Console.WriteLine(process.CommandText());
        var result = await process.Run();
        result.EnsureExitCodeIsZero();
    }

    private static async Task DotnetUninstallTemplateLocally(string name)
    {
        var templateDir = getLocalTemplateDir(name);
        Console.WriteLine($"Uninstalling template '{templateDir}'");
        await new WinProcess("dotnet")
            .WriteOutputToConsole()
            .AddArgument("new")
            .AddArgument("uninstall", templateDir)
            .Run();
        Console.WriteLine($"Installing template '{templateDir}'");
    }

    private static string getLocalTemplateDir(string name) =>
        Path.Combine("..", "..", "Lib", "XTI_Templates", name);

    private static async Task DotnetNewXtiSolution(string repoOwner, string repoName)
    {
        var solutionFiles = new[] { "xti.private.ps1", "xti.ps1" };
        if
        (
            Directory.GetFiles(Environment.CurrentDirectory)
                .Select(f => Path.GetFileName(f))
                .Intersect(solutionFiles, StringComparer.InvariantCultureIgnoreCase)
                .Any()
        )
        {
            Console.WriteLine($"Solution files have already been added");
        }
        else
        {
            Console.WriteLine($"running dotnet new xtisolution '{Environment.CurrentDirectory}'");
            var dotnetNewResult = await new WinProcess("dotnet")
                .WriteOutputToConsole()
                .SetWorkingDirectory(Environment.CurrentDirectory)
                .AddArgument("new")
                .AddArgument("xtisolution")
                .UseArgumentNameDelimiter("--")
                .AddArgument("RepoName", repoName)
                .AddArgument("RepoOwner", repoOwner)
                .Run();
            dotnetNewResult.EnsureExitCodeIsZero();
        }
    }

    private static Task DotnetNewWebAppApi(ToolOptions options, string appName) =>
        DotnetNewProject(Path.Combine(getInternalDir(options), $"XTI_{appName}WebAppApi"), "xtiwebappapi", appName);

    private static Task DotnetNewWebAppControllers(ToolOptions options, string appName) =>
        DotnetNewProject(Path.Combine(getInternalDir(options), $"{appName}WebApp.ApiControllers"), "xtiwebappcontrollers", appName);

    private static Task DotnetNewWebAppClient(ToolOptions options, string appName) =>
        DotnetNewProject(Path.Combine(getLibDir(options), $"XTI_{appName}AppClient"), "xtiwebappclient", appName);

    private static Task DotnetNewApiGeneratorApp(ToolOptions options, string appName) =>
        DotnetNewProject
        (
            Path.Combine(getAppsDir(options), $"{appName}ApiGeneratorApp"),
            "xtiapigeneratorapp",
            appName,
            new
            {
                AppType = getAppType(options),
                ClientType = getAppType(options) == "WebApp" ? "App" : "Service"
            }
        );

    private static Task DotnetNewSetupApp(ToolOptions options, string appName) =>
        DotnetNewProject
        (
            Path.Combine(getAppsDir(options), $"{appName}SetupApp"),
            "xtisetupapp",
            appName,
            new
            {
                AppType = getAppType(options)
            }
        );

    private static Task DotnetNewWebApp(ToolOptions options, string appName) =>
        DotnetNewProject
        (
            Path.Combine(getAppsDir(options), $"{appName}WebApp"),
            "xtiwebapp",
            appName,
            new
            {
                RepoOwner = options.RepoOwner.ToLower(),
                RepoName = options.RepoName,
                Domain = options.Domain,
                AppNameLower = appName.ToLower()
            }
        );

    private static Task DotnetNewWebPackage(ToolOptions options, string appName) =>
        DotnetNewProject
        (
            Path.Combine(getAppsDir(options), $"{appName}WebPackage"),
            "xtiwebpackage",
            appName,
            new
            {
                RepoOwner = options.RepoOwner.ToLower(),
                RepoName = options.RepoName,
                Domain = options.Domain,
                AppNameLower = appName.ToLower()
            }
        );

    private static Task DotnetTests(ToolOptions options)
    {
        var appName = getAppName(options);
        var path = Path.Combine(getTestsDir(options), $"{appName}{options.TestType}Test5s");
        return DotnetNewProject
        (
            path,
            "xtitests",
            appName,
            new
            {
                AppType = getAppType(options),
                options.TestType
            }
        );
    }

    private static Task DotnetNewServiceAppApi(ToolOptions options, string appName) =>
        DotnetNewProject(Path.Combine(getInternalDir(options), $"XTI_{appName}ServiceAppApi"), "xtiserviceappapi", appName);

    private static Task DotnetNewServiceApp(ToolOptions options, string appName) =>
        DotnetNewProject(Path.Combine(getAppsDir(options), $"{appName}ServiceApp"), "xtiserviceapp", appName);

    private static Task DotnetNewConsoleAppApi(ToolOptions options, string appName) =>
        DotnetNewProject(Path.Combine(getInternalDir(options), $"XTI_{appName}ConsoleAppApi"), "xticonsoleappapi", appName);

    private static Task DotnetNewConsoleApp(ToolOptions options, string appName) =>
        DotnetNewProject(Path.Combine(getAppsDir(options), $"{appName}ConsoleApp"), "xticonsoleapp", appName);

    private static Task DotnetNewPackageExport(ToolOptions options, string appName) =>
        DotnetNewProject(Path.Combine(getLibDir(options), appName), "xtipackageexport", appName);

    private static async Task DotnetNewProject(string projectDir, string templateName, string appName, object? config = null)
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
            var dotnetNewProcess = new WinProcess("dotnet")
                .WriteOutputToConsole()
                .SetWorkingDirectory(projectDir)
                .AddArgument("new")
                .AddArgument(templateName)
                .UseArgumentNameDelimiter("--")
                .AddArgument("AppName", appName);
            if (config != null)
            {
                foreach (var prop in config.GetType().GetProperties())
                {
                    var propValue = (string?)prop.GetValue(config);
                    if (!string.IsNullOrWhiteSpace(propValue))
                    {
                        dotnetNewProcess.AddArgument(prop.Name, propValue);
                    }
                }
            }
            var dotnetNewResult = await dotnetNewProcess.Run();
            dotnetNewResult.EnsureExitCodeIsZero();
            var slnDir = Environment.CurrentDirectory;
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