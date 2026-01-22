namespace RestaurantReservation.Admin.Service.Interfaces.ViewModels;

public interface IEditLanguageViewModel : IReactiveObject
{
    string ErrorMessage { get; set; }
    bool HasError { get; }
    bool IsLoading { get; }
    LanguageDto Language { get; set; }
    Guid LanguageId { get; set; }
    ReactiveCommand<Unit, bool> SaveLanguageCommand { get; }
    ReactiveCommand<Unit, Unit> ClearErrorMessageCommand { get; }
}