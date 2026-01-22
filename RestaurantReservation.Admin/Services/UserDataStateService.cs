using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using RestaurantReservation.Admin.Service.Interfaces.Services;
using RestaurantReservation.Dto;
using System.IdentityModel.Tokens.Jwt;

namespace RestaurantReservation.Admin.Services;
internal sealed class UserDataStateService(
    ProtectedSessionStorage sessionStorage,
    ProtectedLocalStorage localStorage,
    IJSRuntime jsRuntime,
    IHttpContextAccessor httpContextAccessor)
    : IUserStateService
{
    private const string AccessToken = "access_token";
    private const string RefreshToken = "refresh_token";
    private const string ExpirationOn = "expiration_on";
    private const string TenantId = "tenant_id";
    private const string HttpContextTenantIdKey = "UserState_TenantId";
    private const string HttpContextAccessTokenKey = "UserState_AccessToken";
    private const string HttpContextRefreshTokenKey = "UserState_RefreshToken";
    private readonly ProtectedSessionStorage _sessionStorage = sessionStorage;
    private readonly ProtectedLocalStorage _localStorage = localStorage;
    private readonly IJSRuntime _jsRuntime = jsRuntime;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    // In-memory cache for server-side access (scoped lifetime)
    private Guid? _cachedTenantId;
    private string? _cachedAccessToken;
    private string? _cachedRefreshToken;

    public async Task Clear()
    {
        await _sessionStorage.DeleteAsync(AccessToken);
        await _sessionStorage.DeleteAsync(RefreshToken);
        await _sessionStorage.DeleteAsync(ExpirationOn);
        await _localStorage.DeleteAsync(TenantId); // Tenant ID in local storage

        // Clear in-memory cache
        _cachedTenantId = null;
        _cachedAccessToken = null;
        _cachedRefreshToken = null;

        // Clear HttpContext.Items
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            httpContext.Items.Remove(HttpContextTenantIdKey);
            httpContext.Items.Remove(HttpContextAccessTokenKey);
            httpContext.Items.Remove(HttpContextRefreshTokenKey);
        }
    }

    private async Task<bool> JsAvailableAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("eval", "true");
            return true;
        }
        catch
        {
            return false; // still prerendering or server-side context
        }
    }

    public async Task<string> GetAccessToken(CancellationToken cancellationToken = default)
    {
        // Try in-memory cache first (works server-side)
        if (_cachedAccessToken != null)
            return _cachedAccessToken;

        // Always try to load from ProtectedSessionStorage first (when JS is available)
        // This is the source of truth and ensures cache is always fresh
        try
        {
            if (await JsAvailableAsync())
            {
                var token = await GetSessionValueOrEmpty(AccessToken);
                if (!string.IsNullOrEmpty(token))
                {
                    _cachedAccessToken = token; // Cache it for future server-side access
                    // Also store in HttpContext.Items as fallback
                    var httpContext = _httpContextAccessor.HttpContext;
                    if (httpContext != null)
                    {
                        httpContext.Items[HttpContextAccessTokenKey] = token;
                    }
                    return token;
                }
            }
        }
        catch
        {
            // If JS is not available, try HttpContext.Items as fallback
        }

        // Fallback: Try HttpContext.Items (for HTTP handlers when JS is not available)
        var fallbackContext = _httpContextAccessor.HttpContext;
        if (fallbackContext?.Items.TryGetValue(HttpContextAccessTokenKey, out var httpToken) == true && httpToken is string cachedToken)
        {
            _cachedAccessToken = cachedToken; // Cache it for future use
            return cachedToken;
        }

        return string.Empty;
    }

    public async Task<string> GetRefreshToken(CancellationToken cancellationToken = default)
    {
        // Try in-memory cache first (works server-side)
        if (_cachedRefreshToken != null)
            return _cachedRefreshToken;

        // Always try to load from ProtectedSessionStorage first (when JS is available)
        // This is the source of truth and ensures cache is always fresh
        try
        {
            if (await JsAvailableAsync())
            {
                var refreshToken = await GetSessionValueOrEmpty(RefreshToken);
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    _cachedRefreshToken = refreshToken; // Cache it for future server-side access
                    // Also store in HttpContext.Items as fallback
                    var httpContext = _httpContextAccessor.HttpContext;
                    if (httpContext != null)
                    {
                        httpContext.Items[HttpContextRefreshTokenKey] = refreshToken;
                    }
                    return refreshToken;
                }
            }
        }
        catch
        {
            // If JS is not available, try HttpContext.Items as fallback
        }

        // Fallback: Try HttpContext.Items (for HTTP handlers when JS is not available)
        var fallbackContext = _httpContextAccessor.HttpContext;
        if (fallbackContext?.Items.TryGetValue(HttpContextRefreshTokenKey, out var httpRefreshToken) == true && httpRefreshToken is string cachedRefreshToken)
        {
            _cachedRefreshToken = cachedRefreshToken; // Cache it for future use
            return cachedRefreshToken;
        }

        return string.Empty;
    }

    public async Task<Guid> GetTenantId(CancellationToken cancellationToken = default)
    {
        // Try in-memory cache first (works server-side, e.g., from HTTP handlers)
        if (_cachedTenantId.HasValue)
            return _cachedTenantId.Value;

        // Try to load from ProtectedLocalStorage first (persists across sessions)
        // This ensures tenant ID survives page refreshes and session expiration
        try
        {
            if (await JsAvailableAsync())
            {
                var result = await _localStorage.GetAsync<string>(TenantId);
                if (result.Success && Guid.TryParse(result.Value, out var id))
                {
                    // Cache it for future server-side access (for HTTP handlers)
                    _cachedTenantId = id;
                    // Also store in HttpContext.Items as fallback for HTTP handlers
                    var httpContext = _httpContextAccessor.HttpContext;
                    if (httpContext != null)
                    {
                        httpContext.Items[HttpContextTenantIdKey] = id;
                    }
                    return id;
                }
            }
        }
        catch
        {
            // If JS is not available, try other fallbacks
        }

        // Fallback 1: Try to extract tenant ID from JWT token if available
        try
        {
            var token = await GetAccessToken(cancellationToken);
            if (!string.IsNullOrEmpty(token))
            {
                var tenantIdFromToken = ExtractTenantIdFromToken(token);
                if (tenantIdFromToken != Guid.Empty)
                {
                    _cachedTenantId = tenantIdFromToken;
                    // Save it to local storage for future use
                    if (await JsAvailableAsync())
                    {
                        try
                        {
                            await _localStorage.SetAsync(TenantId, tenantIdFromToken.ToString());
                        }
                        catch
                        {
                            // Ignore storage errors
                        }
                    }
                    return tenantIdFromToken;
                }
            }
        }
        catch
        {
            // If token parsing fails, continue to next fallback
        }

        // Fallback 2: Try HttpContext.Items (for HTTP handlers when JS is not available)
        var fallbackContext = _httpContextAccessor.HttpContext;
        if (fallbackContext?.Items.TryGetValue(HttpContextTenantIdKey, out var httpTenantId) == true)
        {
            if (httpTenantId is Guid tenantId)
            {
                _cachedTenantId = tenantId; // Cache it for future use
                return tenantId;
            }
            if (httpTenantId is string tenantIdString && Guid.TryParse(tenantIdString, out var parsedId))
            {
                _cachedTenantId = parsedId; // Cache it for future use
                return parsedId;
            }
        }

        return Guid.Empty;
    }

    private static Guid ExtractTenantIdFromToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            if (tokenHandler.CanReadToken(token))
            {
                var jwtToken = tokenHandler.ReadJwtToken(token);
                // Try common claim names for tenant ID
                var tenantIdClaim = jwtToken.Claims.FirstOrDefault(c =>
                    c.Type == "tenant_id" ||
                    c.Type == "TenantId" ||
                    c.Type == CustomClaims.TenantId ||
                    c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/tenantid");

                if (tenantIdClaim != null && Guid.TryParse(tenantIdClaim.Value, out var tenantId))
                {
                    return tenantId;
                }
            }
        }
        catch
        {
            // If token parsing fails, return empty
        }
        return Guid.Empty;
    }

    public async Task SetAccessToken(string token, DateTime expirationOn, string refreshToken, Guid tenantId)
    {
        await Clear();

        // Store tokens in session storage (expires with session for security)
        await _sessionStorage.SetAsync(AccessToken, token);
        await _sessionStorage.SetAsync(RefreshToken, refreshToken);
        await _sessionStorage.SetAsync(ExpirationOn, expirationOn.ToString("O"));

        // Store tenant ID in local storage (persists across sessions/page refreshes)
        await _localStorage.SetAsync(TenantId, tenantId.ToString());

        // Also cache in memory (for server-side access, e.g., HTTP handlers)
        _cachedTenantId = tenantId;
        _cachedAccessToken = token;
        _cachedRefreshToken = refreshToken;

        // Store in HttpContext.Items for HTTP handlers (works across different scopes)
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            httpContext.Items[HttpContextTenantIdKey] = tenantId;
            httpContext.Items[HttpContextAccessTokenKey] = token;
            httpContext.Items[HttpContextRefreshTokenKey] = refreshToken;
        }
    }

    private async Task<string> GetSessionValueOrEmpty(string key)
    {
        if (!await JsAvailableAsync())
            return string.Empty;

        var result = await _sessionStorage.GetAsync<string>(key);
        return result.Success ? result.Value ?? string.Empty : string.Empty;
    }
}
