using System.Reflection;
using CarAuto.GrpcClientWrapper;
using CarAuto.UI.Data;
using CarAuto.UI.BusinessLogic.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using MudBlazor.Services;
using MudBlazor;
using MudBlazor.Extensions;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

foreach (var jsonFilename in Directory.EnumerateFiles("Config", "*.json", SearchOption.AllDirectories))
{
    config.AddJsonFile(jsonFilename);
}

config.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddValidatorsFromAssembly(Assembly.GetEntryAssembly());
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient().AddHeaderPropagation(e => e.Headers.Add("Authorization"));
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 10000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<IGrpcClientWrapper, GrpcClientWrapper>();
builder.Services.AddSingleton<WeatherForecastService>();
// use this to add MudServices and the MudBlazor.Extensions
builder.Services.AddMudServicesWithExtensions(c => c.WithoutAutomaticCssLoading());

// or this to add only the MudBlazor.Extensions but please ensure that this is added after mud servicdes are added. That means after `AddMudServices`
builder.Services.AddMudExtensions();

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = _ => false;
    options.MinimumSameSitePolicy = SameSiteMode.None;
    options.HttpOnly = HttpOnlyPolicy.None;
});

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie(e =>
    {
        e.Cookie.SameSite = SameSiteMode.None;
        e.Cookie.HttpOnly = false;
    })
    .AddOpenIdConnect(options =>
    {
        options.Authority = builder.Configuration.GetValue<string>("Oidc:Authority");
        options.ClientId = builder.Configuration.GetValue<string>("Oidc:ClientId");
        options.ClientSecret = builder.Configuration.GetValue<string>("Oidc:ClientSecret");
        options.RequireHttpsMetadata = builder.Configuration.GetValue<bool>("Oidc:RequireHttpsMetadata"); // disable only in dev env
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.GetClaimsFromUserInfoEndpoint = true;
        options.SaveTokens = true;
        options.MapInboundClaims = true;
        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");
        options.Scope.Add("roles");
        options.Events = new OpenIdConnectEvents
        {
            OnUserInformationReceived = context =>
            {
                context.MapKeyCloakRolesToRoleClaims();
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(o => o.AddPolicy("IsUser", (b) =>
{
    b.RequireRole("user");
}));

builder.Services.AddAuthorization(o => o.AddPolicy("IsAdmin", (b) =>
{
    b.RequireRole("admin");
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseHeaderPropagation();

app.UseRouting();
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();
app.UseMudExtensions();
app.UseEndpoints(endpoints =>
{
    endpoints.MapBlazorHub();
    endpoints.MapFallbackToPage("/_Host");
});

app.Run();
