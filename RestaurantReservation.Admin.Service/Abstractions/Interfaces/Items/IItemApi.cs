using RestaurantReservation.Dto.Models.Items;
using RestaurantReservation.Dto.Requests.Items;

namespace RestaurantReservation.Admin.Service.Abstractions.Interfaces.Items;

public interface IItemApi
{
    [Get("/api/Items")]
    Task<ApiResponse<IEnumerable<ItemDto>>> GetAll();
    [Get("/api/Items/{id}")]
    Task<ApiResponse<ItemDto>> GetById(Guid id);
    [Get("/api/Items/RestaurantId/{id}")]
    Task<ApiResponse<IEnumerable<ItemDto>>> GetByRestaurantId(Guid id);
    [Get("/api/Items/basicData")]
    Task<ApiResponse<IEnumerable<ItemDto>>> GetAllBasic();
}

public interface ISecureItemApi
{
    [Post("/api/Items")]
    Task<ApiResponse<Guid>> AddItem([Body(buffered: true)] AddItemRequest model);
    [Put("/api/Items")]
    Task<IApiResponse> UpdateItem([Body(buffered: true)] UpdateItemRequest model);
    [Delete("/api/Items/Obsolete/{id}")]
    Task<IApiResponse> ObsoleteItem(Guid id);
    [Delete("/api/Items/{id}")]
    Task<IApiResponse> DeleteItem(Guid id);
}