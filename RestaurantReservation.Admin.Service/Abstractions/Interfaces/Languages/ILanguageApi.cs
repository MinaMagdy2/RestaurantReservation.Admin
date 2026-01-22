namespace RestaurantReservation.Admin.Service.Abstractions.Interfaces.Languages;

public interface ILanguageApi
{
    [Get("/api/Languages")]
    Task<ApiResponse<IEnumerable<LanguageDto>>> GetAll();
    [Get("/api/Languages/{id}")]
    Task<ApiResponse<LanguageDto>> GetById(Guid id);
    [Get("/api/Languages/GetDefault")]
    Task<ApiResponse<LanguageDto>> GetDefault();
    [Get("/api/Languages/GetByNationality")]
    Task<ApiResponse<LanguageDto>> GetByNationality([Query] string code);
}
