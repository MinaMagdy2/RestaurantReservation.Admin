using RestaurantReservation.Dto.Models.Comsys;

namespace RestaurantReservation.Admin.Service.Abstractions.Interfaces;

public interface ILoginApi
{
    [Post("/api/Authentication/login")]
    Task<ApiResponse<TokenResponse>> Login([Body(buffered: true)] LoginRequest model);
    [Post("/api/Authentication/RefreshToken")]
    Task<ApiResponse<TokenResponse>> RefreshToken([Body(buffered: true)] RefreshTokenRequest request);
}
public interface IComsysApi
{
    [Get("/api/system/Comsys/{room}")]
    Task<ApiResponse<GuestDataDto>> GetGuestByRoom(string room);
    [Get("/api/system/nationalities")]
    Task<IEnumerable<NationalityDto>> GetNationalities();
    [Get("/api/system/roomTypes")]
    Task<IEnumerable<RoomTypeDto>> GetRoomTypes();
    [Get("/api/system/TravelAgents")]
    Task<IEnumerable<TravelAgentDto>> GetTravelAgents();
    [Get("/api/system/MealPatterns")]
    Task<IEnumerable<MealPatternDto>> GetMealPatterns();
}