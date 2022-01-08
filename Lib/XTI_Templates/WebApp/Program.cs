using __APPNAME__WebApp.Extensions;
using XTI_Configuration.Extensions;
using XTI_WebApp.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.UseXtiConfiguration(builder.Environment, new string[0]);

builder.Services.AddResponseCaching();
builder.Services.ConfigureXtiCookieAndTokenAuthentication(builder.Environment, builder.Configuration);
builder.Services.Add__APPNAME__WebAppServices(builder.Environment, builder.Configuration);

var app = builder.Build();

if ( app.Environment.IsDevOrTest())
{
    app.UseDeveloperExceptionPage();
}
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseResponseCaching();
app.UseXti();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute
    (
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}"
    );
});
await app.RunAsync();