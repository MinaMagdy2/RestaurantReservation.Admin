using RestaurantReservation.Dto.Models.Items;
using RestaurantReservation.Dto.Requests.Items;

namespace RestaurantReservation.Admin.Service.Interfaces.Services;

public interface IItemService
{
    Task<Result<IEnumerable<ItemDto>>> GetAllAsync();
    Task<Result<ItemDto>> GetByIdAsync(Guid id);
    Task<Result<Guid>> AddAsync(AddItemRequest model);
    Task<Result> UpdateAsync(UpdateItemRequest model);
    Task<Result> DeleteAsync(Guid id);
    Task<Result> ObsoleteItem(Guid id);
    Task<Result<IEnumerable<ItemDto>>> GetBasicAsync();
}