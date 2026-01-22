using MudBlazor.Services;
using RestaurantReservation.Admin.AuthenticationServices;
using RestaurantReservation.Admin.Components;
using RestaurantReservation.Admin.Service;
using RestaurantReservation.Admin.Service.Interfaces.Services;
using RestaurantReservation.Admin.Services;
using Syncfusion.Blazor;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddCustomAuthenticationServices();
builder.Services.AddMudServices();
builder.Services.AddSyncfusionBlazor();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddScoped<IUserStateService, UserDataStateService>();
builder.Services.AddSignalR(e =>
{
    e.MaximumReceiveMessageSize = 102400000;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JGaF5cXGpCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWH1ccnRdQ2ZZWUN+XENWYEs=");

app.UseHttpsRedirection();
app.MapStaticAssets();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.UseStatusCodePagesWithRedirects("/notFound");
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
await app.RunAsync();
