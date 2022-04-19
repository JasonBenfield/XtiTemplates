using __APPNAME__WebApp.Extensions;
using XTI_HubAppClient.WebApp.Extensions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using __APPNAME__WebApp.ApiControllers;
using XTI_Core;
using XTI___APPNAME__WebAppApi;

var builder = XtiWebAppHost.CreateDefault(__APPNAME__Info.AppKey, args);
var xtiEnv = XtiEnvironment.Parse(builder.Environment.EnvironmentName);
builder.Services.ConfigureXtiCookieAndTokenAuthentication(xtiEnv, builder.Configuration);
builder.Services.Add__APPNAME__WebAppServices();
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