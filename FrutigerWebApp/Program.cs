using Ganss.Xss;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Extensions;
using NWebsec.AspNetCore.Middleware.Middleware;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IHtmlSanitizer, HtmlSanitizer>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

builder.Services.AddDataProtection();
builder.Services.AddDistributedMemoryCache();


app.UseCsp(csp =>
{
    csp.DefaultSources(s => s.None());
    csp.ScriptSources(s => s.Self());
    csp.StyleSources(s => s.Self());
    csp.ImageSources(s => s.Self().CustomSources("data:"));
    csp.FontSources(s => s.Self());
    csp.ConnectSources(s => s.Self());
    csp.FrameAncestors(s => s.None());
    csp.ObjectSources(s => s.None());
    csp.BaseUris(s => s.Self());
    csp.FormActions(s => s.Self());
    csp.ManifestSources(s => s.Self());
    csp.FrameSources(s => s.Self());
    csp.WorkerSources(s => s.Self());
    csp.MediaSources(s => s.Self());
    csp.ChildSources(s => s.Self());
    csp.UpgradeInsecureRequests();
    csp.BlockAllMixedContent();
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Intro}/{id?}");



app.Run();
