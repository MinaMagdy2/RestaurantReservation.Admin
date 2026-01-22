using RestaurantReservation.Dto.Models.Restaurants;
using RestaurantReservation.Dto.Requests.Restaurants;

namespace RestaurantReservation.Admin.Service.Abstractions.Interfaces.Restaurants;

public interface ISecureRestaurantApi
{
    [Post("/api/Restaurants")]
    Task<ApiResponse<Guid>> AddRestaurant([Body(buffered: true)] AddRestaurantRequest model);
    [Put("/api/Restaurants")]
    Task<IApiResponse> UpdateRestaurant([Body(buffered: true)] UpdateRestaurantRequest model);
    [Delete("/api/Restaurants/{id}")]
    Task<IApiResponse> DeleteRestaurant(Guid id);
    [Delete("/api/Restaurants/Obsolete/{id}")]
    Task<IApiResponse> ObsoleteRestaurant(Guid id);
}
public interface IRestaurantApi
{
    [Get("/api/Restaurants")]
    Task<ApiResponse<IEnumerable<RestaurantDto>>> GetAll();
    [Get("/api/Restaurants/{id}")]
    Task<ApiResponse<RestaurantDto>> GetById(Guid id);
    [Post("/api/Restaurants/GetRestaurantsByConditions")]
    Task<ApiResponse<IEnumerable<RestaurantDto>>> GetRestaurantsByConditions([Body(buffered: true)] GetRestaurantsByConditionsRequest model);
}