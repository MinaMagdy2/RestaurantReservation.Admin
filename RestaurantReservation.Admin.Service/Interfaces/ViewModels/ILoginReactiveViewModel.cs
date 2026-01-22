namespace RestaurantReservation.Admin.Service.Interfaces.ViewModels;

public interface ILoginReactiveViewModel : IReactiveObject
{
    string ErrorMessage { get; set; }
    bool HasError { get; }
    bool IsLoading { get; }
    ReactiveCommand<Unit, bool> LoginCommand { get; }
    string Password { get; set; }
    string UserName { get; set; }
}
