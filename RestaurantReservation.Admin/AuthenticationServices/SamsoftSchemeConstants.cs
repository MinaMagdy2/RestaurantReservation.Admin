using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using RestaurantReservation.Admin.Service.Interfaces.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace RestaurantReservation.Admin.AuthenticationServices;

public static class SamsoftSchemeConstants
{
    public const string AuthScheme = "ss-auth";
    public const string AuthenticationScheme = "SamSoftAuthenticationScheme";
}
public class SamSoftAuthOptions : AuthenticationSchemeOptions
{
    // Add any custom options here if needed in the future
}
public class SamSoftAuthenticationHandler(
    IOptionsMonitor<SamSoftAuthOptions> options,
    ILoggerFactory logger, UrlEncoder encoder)
    : AuthenticationHandler<SamSoftAuthOptions>(options, logger: logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var auth = Context.User.Identity?.IsAuthenticated ?? false;
        if (!auth)
            return Task.FromResult(AuthenticateResult.Fail("Is Not Authenticated"));

        var ticket = new AuthenticationTicket(Context.User, SamsoftSchemeConstants.AuthenticationScheme);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
internal sealed class CustomJwtAuthenticationStateProvider(
    IUserStateService userStateData,
    ILogger<CustomJwtAuthenticationStateProvider> logger) : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        logger.LogInformation("Custom Jwt Authentication State Provider Started");
        var tokenString = await userStateData.GetAccessToken();
        var refreshToken = await userStateData.GetRefreshToken();
        if (string.IsNullOrEmpty(tokenString) || string.IsNullOrEmpty(refreshToken))
        {
            return NotAuthorized();
        }
        return Authorized(tokenString);
    }
    private AuthenticationState Authorized(string tokenString)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.ReadJwtToken(tokenString);
        var identity = new ClaimsIdentity(token.Claims, "Bearer", ClaimTypes.Name, ClaimTypes.Role);
        var user = new ClaimsPrincipal(identity);
        var authState = new AuthenticationState(user);
        NotifyAuthenticationStateChanged(Task.FromResult(authState));
        return authState;
    }

    private AuthenticationState NotAuthorized()
    {
        var logoutState = new AuthenticationState(new ClaimsPrincipal());
        NotifyAuthenticationStateChanged(Task.FromResult(logoutState));
        return logoutState;
    }
}
public class AuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
    public Task HandleAsync(RequestDelegate next, HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        return next(context);
    }
}
public static class AuthenticationServiceExtension
{
    public static IServiceCollection AddCustomAuthenticationServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = SamsoftSchemeConstants.AuthScheme;
            options.DefaultChallengeScheme = SamsoftSchemeConstants.AuthScheme;
        }).AddScheme<SamSoftAuthOptions, SamSoftAuthenticationHandler>(
            SamsoftSchemeConstants.AuthScheme,
            _ => { });
        services.AddAuthorization()
                //.AddScoped<IAppAuthenticate, AppAuthenticate>()
                .AddSingleton<IAuthorizationMiddlewareResultHandler, AuthorizationMiddlewareResultHandler>()
                .AddScoped<AuthenticationStateProvider, CustomJwtAuthenticationStateProvider>();

        //services.AddAuthorization(o =>
        //{
        //    o.AddPolicy("AdminPolicy", builder =>
        //    {
        //        var permissionClaims = cla.FindAll(CustomClaims.Permissions);
        //        builder.RequireClaim()
        //    });
        //});
        //services.AddScoped<IUserStateService, UserDataStateService>();
        //services
        //services;
        //services.AddScoped<LoginService>();


        return services;
    }
}