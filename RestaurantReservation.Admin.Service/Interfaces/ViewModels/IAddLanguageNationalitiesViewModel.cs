namespace RestaurantReservation.Admin.Service.Interfaces.ViewModels;

public interface IAddLanguageNationalitiesViewModel : IReactiveObject
{
    ReactiveCommand<Unit, Unit> ClearErrorMessageCommand { get; }
    string ErrorMessage { get; set; }
    bool HasError { get; }
    ReactiveCommand<Guid, Unit> InitializeCommand { get; }
    bool IsLoading { get; }
    string[] SelectedNationalityCodes { get; set; }
    LanguageDto Language { get; set; }
    ReadOnlyObservableCollection<NationalityDto> Nationalities { get; }
    ReactiveCommand<Unit, bool> SaveCommand { get; }
}
