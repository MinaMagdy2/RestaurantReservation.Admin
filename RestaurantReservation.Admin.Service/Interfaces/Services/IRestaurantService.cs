namespace RestaurantReservation.Admin.Service.Interfaces.Services;

public interface IRestaurantService
{
    Task<Result<IEnumerable<RestaurantDto>>> GetAllAsync();
    Task<Result<RestaurantDto>> GetByIdAsync(Guid id);
    Task<Result<Guid>> AddAsync(AddRestaurantRequest model);
    Task<Result> UpdateAsync(UpdateRestaurantRequest model);
    Task<Result> DeleteAsync(Guid id);
    Task<Result> ObsoleteRestaurant(Guid id);
    Task<Result<IEnumerable<RestaurantDto>>> GetRestaurantsByConditions(GetRestaurantsByConditionsRequest model);
}
