using RestaurantReservation.Dto.Models.Items;

namespace RestaurantReservation.Admin.Service.Interfaces.ViewModels;

public interface IItemListViewModel : IReactiveObject
{
    string ErrorMessage { get; set; }
    ReactiveCommand<Unit, Unit> InitializeCommand { get; }
    bool IsLoading { get; }
    ReadOnlyObservableCollection<ItemDto> Items { get; }
}