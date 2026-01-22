

using RestaurantReservation.Dto.Models.Items;

namespace RestaurantReservation.Admin.Service.Interfaces.ViewModels;

public interface IAddItemViewModel : IReactiveObject
{
    // Properties
    string ErrorMessage { get; set; }
    bool HasError { get; }
    bool IsLoading { get; }
    ItemDto Item { get; set; }
    Guid ItemId { get; set; }
    ReactiveCommand<Unit, bool> SaveCommand { get; }
    ReactiveCommand<Unit, Unit> ClearErrorMessageCommand { get; }
    ReadOnlyObservableCollection<RestaurantDto> Restaurants { get; }

    // جديد: المطاعم المختارة
    List<string> SelectedRestaurantNames { get; set; }


}
