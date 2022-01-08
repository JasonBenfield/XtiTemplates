using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XTI_Configuration.Extensions;
using XTI_DotnetTool;

await Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.UseXtiConfiguration(hostingContext.HostingEnvironment, args);
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.Configure<ToolOptions>(hostContext.Configuration);
        services.AddHostedService<HostedService>();
    })
    .RunConsoleAsync();