namespace RestaurantReservation.Admin.Service.Interfaces.ViewModels;

public interface IRestaurantListViewModel : IReactiveObject
{
    string ErrorMessage { get; set; }
    bool HasError { get; }
    ReactiveCommand<Unit, Unit> InitializeCommand { get; }
    bool IsLoading { get; }
    ReadOnlyObservableCollection<RestaurantDto> Restaurants { get; }
}
