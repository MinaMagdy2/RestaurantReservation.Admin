using RestaurantReservation.Admin.Service.Abstractions.Interfaces;

namespace RestaurantReservation.Admin.Service.ClientServices;

internal sealed class LoginService(ILoginApi loginApi) : ILoginService
{
    public async Task<Result<TokenResponse>> Login(string username, string password)
    {
        var model = new LoginRequest(username, password);
        var result = await loginApi.Login(model)
            .ReturnResult("User Name Or Password Error", ErrorType.NotFound);
        return result;
    }

    public async Task<Result<TokenResponse>> RefreshToken(string token)
    {
        var request = new RefreshTokenRequest(token);
        var result = await loginApi.RefreshToken(request)
            .ReturnResult("RefreshTokenError", ErrorType.Failure);
        return result;
    }
}
