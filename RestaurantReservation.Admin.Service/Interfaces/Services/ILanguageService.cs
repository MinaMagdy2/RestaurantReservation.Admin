namespace RestaurantReservation.Admin.Service.Interfaces.Services;

public interface ILanguageService
{
    Task<Result<IEnumerable<LanguageDto>>> GetAllAsync();
    Task<Result<LanguageDto>> GetByIdAsync(Guid id);
    Task<Result<LanguageDto>> GetDefaultAsync();
    Task<Result<LanguageDto>> GetByNationalityAsync(string nationality);
    Task<Result<Guid>> AddAsync(AddLanguageRequest model);
    Task<Result> UpdateAsync(UpdateLanguageRequest model);
    Task<Result> AddNationalities(AddLanguageNationalitiesRequest model);
}
