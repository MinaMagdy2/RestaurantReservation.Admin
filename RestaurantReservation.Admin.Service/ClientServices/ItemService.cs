using RestaurantReservation.Admin.Service.Abstractions.Interfaces.Items;
using RestaurantReservation.Dto.Models.Items;
using RestaurantReservation.Dto.Requests.Items;

namespace RestaurantReservation.Admin.Service.ClientServices;

internal sealed class ItemService(
    ISecureItemApi secureItemApi,
    IItemApi ItemApi) : IItemService
{
    public Task<Result<Guid>> AddAsync(AddItemRequest model) =>
        secureItemApi.AddItem(model)
        .ReturnResult("Add Item", ErrorType.Problem);
    public Task<Result> DeleteAsync(Guid id) =>
        secureItemApi.DeleteItem(id)
        .ReturnResult("Delete Item", ErrorType.Problem);
    public Task<Result<IEnumerable<ItemDto>>> GetAllAsync() =>
        ItemApi.GetAll()
        .ReturnResult("Get All Item", ErrorType.Problem);
    public Task<Result<ItemDto>> GetByIdAsync(Guid id) =>
        ItemApi.GetById(id)
        .ReturnResult("Get All Item", ErrorType.Problem);
    public Task<Result<IEnumerable<ItemDto>>> GetBasicAsync() =>
        ItemApi.GetAllBasic()
        .ReturnResult("Get Items Basic", ErrorType.Problem);
    public Task<Result> ObsoleteItem(Guid id) =>
        secureItemApi.ObsoleteItem(id)
        .ReturnResult("Obsolete Item", ErrorType.Problem);
    public Task<Result> UpdateAsync(UpdateItemRequest model) =>
        secureItemApi.UpdateItem(model)
        .ReturnResult("Update Item", ErrorType.Problem);
}