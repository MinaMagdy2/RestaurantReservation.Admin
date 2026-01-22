using RestaurantReservation.Admin.Service.Abstractions.Interfaces.Languages;

namespace RestaurantReservation.Admin.Service.ClientServices;

internal sealed class LanguageService(
    ILanguageApi languageApi,
    ISecureLanguageApi secureLanguageApi) : ILanguageService
{
    public Task<Result<Guid>> AddAsync(AddLanguageRequest model) =>
        secureLanguageApi.Add(model)
        .ReturnResult("Add Language", ErrorType.Problem);

    public Task<Result<IEnumerable<LanguageDto>>> GetAllAsync() =>
        languageApi.GetAll()
        .ReturnResult("Get All", ErrorType.NotFound);
    public Task<Result<LanguageDto>> GetByIdAsync(Guid id) =>
        languageApi.GetById(id)
        .ReturnResult("Get By Id", ErrorType.NotFound);

    public Task<Result<LanguageDto>> GetByNationalityAsync(string nationality) =>
        languageApi.GetByNationality(nationality)
        .ReturnResult("Get By Nationality", ErrorType.NotFound);

    public Task<Result<LanguageDto>> GetDefaultAsync() =>
        languageApi.GetDefault()
        .ReturnResult("Get Default", ErrorType.NotFound);


    public Task<Result> UpdateAsync(UpdateLanguageRequest model) =>
         secureLanguageApi.Update(model)
        .ReturnResult("Update Language", ErrorType.Problem);
    public Task<Result> AddNationalities(AddLanguageNationalitiesRequest model) =>
        secureLanguageApi.AddNationalities(model).ReturnResult("Add Nationalities", ErrorType.Problem);
}
