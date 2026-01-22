using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantReservation.Admin.Service.Interfaces.ViewModels;

public interface  IDeletItemViewModel
{
    ReactiveCommand<Guid, bool> ObsoleteItemCommand { get; }
    ReactiveCommand<Guid, bool> DeleteItemCommand { get; }
    bool IsLoading { get; }
    string ErrorMessage { get; }
    Task ClearErrorMessage();

}
