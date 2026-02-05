using FrutigerWebApp;
using Ganss.Xss;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.SignalR;
using NWebsec.AspNetCore.Middleware.Middleware;
using Microsoft.AspNetCore.HttpOverrides;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IHtmlSanitizer, HtmlSanitizer>();
builder.Services.AddSignalR();
builder.Services.AddDataProtection();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.Configure<QREncryptor>(
    builder.Configuration.GetSection("QRAuth")
);
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});
var app = builder.Build();
app.UseForwardedHeaders();
app.MapHub<ChatHub>("/GetChats");
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseCsp(csp =>
{
    csp.DefaultSources(s => s.None());
    csp.ScriptSources(s => s.Self());
    csp.StyleSources(s => s.Self().CustomSources(
        "https://fonts.googleapis.com",
        "https://cdn.jsdelivr.net"
        ));
    csp.ImageSources(s => s.Self().CustomSources("data:"));
    csp.FontSources(s => s.Self().CustomSources(
        "https://fonts.gstatic.com",
        "https://cdn.jsdelivr.net"
        ));
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

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Intro}/{id?}");



app.Run();
