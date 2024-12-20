using XTI_HubAppClient.WebApp.Extensions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using __APPNAME__WebApp.ApiControllers;
using XTI_Core;
using XTI___APPNAME__WebAppApi;
using XTI_App.Api;

var builder = XtiWebAppHost.CreateDefault(__APPNAME__AppKey.Value, args);
var xtiEnv = XtiEnvironment.Parse(builder.Environment.EnvironmentName);
builder.Services.ConfigureXtiCookieAndTokenAuthentication(xtiEnv, builder.Configuration);
builder.Services.AddScoped<AppApiFactory, __APPNAME__AppApiFactory>();
builder.Services.AddScoped(sp => (__APPNAME__AppApi)sp.GetRequiredService<IAppApi>());
builder.Services.Add__APPNAME__AppApiServices();
builder.Services
    .AddMvc()
    .AddJsonOptions(options =>
    {
        options.SetDefaultJsonOptions();
    })
    .AddMvcOptions(options =>
    {
        options.SetDefaultMvcOptions();
    });
builder.Services.AddControllersWithViews()
    .PartManager.ApplicationParts.Add
    (
        new AssemblyPart(typeof(HomeController).Assembly)
    );

var app = builder.Build();
app.UseXtiDefaults();
await app.RunAsync();