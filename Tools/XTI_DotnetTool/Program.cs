using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XTI_Core;
using XTI_Core.Extensions;
using XTI_DotnetTool;
using XTI_Git;
using XTI_Git.Abstractions;
using XTI_Git.GitLib;
using XTI_Git.Secrets;
using XTI_GitHub;
using XTI_GitHub.Web;
using XTI_Secrets.Extensions;

await Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.UseXtiConfiguration(hostingContext.HostingEnvironment, "", "", args);
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.Configure<ToolOptions>(hostContext.Configuration);
        services.AddSingleton(_ => XtiEnvironment.Parse(hostContext.HostingEnvironment.EnvironmentName));
        services.AddSingleton<XtiFolder>();
        services.AddFileSecretCredentials();
        services.AddScoped<IGitHubCredentialsAccessor, SecretGitHubCredentialsAccessor>();
        services.AddScoped<IGitHubFactory, WebGitHubFactory>();
        services.AddScoped<GitLibCredentials>();
        services.AddScoped<IXtiGitFactory, GitLibFactory>();
        services.AddHostedService<HostedService>();
    })
    .RunConsoleAsync();