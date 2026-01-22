namespace RestaurantReservation.Admin.Service.Interfaces.Services;

public interface ILoginService
{
    Task<Result<TokenResponse>> Login(string username, string password);
    Task<Result<TokenResponse>> RefreshToken(string token);
}
