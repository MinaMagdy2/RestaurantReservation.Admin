using RestaurantReservation.Dto.Requests.Languages;

namespace RestaurantReservation.Admin.Service.Abstractions.Interfaces.Languages;

public interface ISecureLanguageApi
{
    [Post("/api/Languages")]
    Task<ApiResponse<Guid>> Add([Body(buffered: true)] AddLanguageRequest model);
    [Put("/api/Languages")]
    Task<IApiResponse> Update([Body(buffered: true)] UpdateLanguageRequest model);
    [Put("/api/Languages/UpdateNationalities")]
    Task<IApiResponse> AddNationalities([Body(buffered: true)] AddLanguageNationalitiesRequest model);
}