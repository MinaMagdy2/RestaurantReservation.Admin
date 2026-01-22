using RestaurantReservation.Admin.Service.Abstractions.Interfaces.Restaurants;

namespace RestaurantReservation.Admin.Service.ClientServices;

internal sealed class RestaurantService(
    ISecureRestaurantApi secureRestaurantApi,
    IRestaurantApi restaurantApi) : IRestaurantService
{
    public Task<Result<Guid>> AddAsync(AddRestaurantRequest model) =>
        secureRestaurantApi.AddRestaurant(model)
        .ReturnResult("Add Restaurant", ErrorType.Problem);
    public Task<Result> DeleteAsync(Guid id) =>
        secureRestaurantApi.DeleteRestaurant(id)
        .ReturnResult("Delete Restaurant", ErrorType.Problem);
    public Task<Result<IEnumerable<RestaurantDto>>> GetAllAsync() =>
        restaurantApi.GetAll()
        .ReturnResult("Get All Restaurant", ErrorType.Problem);
    public Task<Result<RestaurantDto>> GetByIdAsync(Guid id) =>
        restaurantApi.GetById(id)
        .ReturnResult("Get All Restaurant", ErrorType.Problem);
    public Task<Result<IEnumerable<RestaurantDto>>> GetRestaurantsByConditions(
        GetRestaurantsByConditionsRequest model) =>
        restaurantApi.GetRestaurantsByConditions(model)
        .ReturnResult("Get Restaurants By Conditions", ErrorType.Problem);
    public Task<Result> ObsoleteRestaurant(Guid id) =>
        secureRestaurantApi.ObsoleteRestaurant(id)
        .ReturnResult("Obsolete Restaurant", ErrorType.Problem);
    public Task<Result> UpdateAsync(UpdateRestaurantRequest model) =>
        secureRestaurantApi.UpdateRestaurant(model)
        .ReturnResult("Update Restaurant", ErrorType.Problem);
}
