namespace RestaurantReservation.Admin.Service.Interfaces.Services;

public interface IUserStateService
{
    Task Clear();
    Task<string> GetAccessToken(CancellationToken cancellationToken = default);
    Task<string> GetRefreshToken(CancellationToken cancellationToken = default);
    Task SetAccessToken(string token, DateTime expirationOn, string refreshToken, Guid tenantId);
    Task<Guid> GetTenantId(CancellationToken cancellationToken = default);
}
