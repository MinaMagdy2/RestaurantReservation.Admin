namespace RestaurantReservation.Admin.Service.Interfaces.ViewModels;

public interface ILanguageListViewModel : IReactiveObject
{
    string ErrorMessage { get; set; }
    ReactiveCommand<Unit, Unit> InitializeCommand { get; }
    bool IsLoading { get; }
    ReadOnlyObservableCollection<LanguageDto> Languages { get; set; }
}
