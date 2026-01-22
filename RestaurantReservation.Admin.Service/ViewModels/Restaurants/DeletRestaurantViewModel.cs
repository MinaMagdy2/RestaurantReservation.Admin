
namespace RestaurantReservation.Admin.Service.ViewModels.Restaurants
{
    public class DeletRestaurantViewModel : ReactiveObject, IDeletRestaurantViewModel
    {
        private readonly IRestaurantService _restaurantService;
        private readonly ObservableAsPropertyHelper<bool> _isLoading;
        private string _errorMessage = string.Empty;

        public DeletRestaurantViewModel(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;

            // Commands
            ObsoleteItemCommand = ReactiveCommand.CreateFromTask<Guid, bool>(ObsoleteItemAsync);
            DeleteItemCommand = ReactiveCommand.CreateFromTask<Guid, bool>(DeleteItemAsync);

            // Combine loading states
            _isLoading = this.WhenAnyObservable(
                x => x.ObsoleteItemCommand.IsExecuting,
                x => x.DeleteItemCommand.IsExecuting,
                (obs, del) => obs || del
            ).ToProperty(this, nameof(IsLoading));
        }

        // Commands
        public ReactiveCommand<Guid, bool> ObsoleteItemCommand { get; }
        public ReactiveCommand<Guid, bool> DeleteItemCommand { get; }

        // Properties
        public bool IsLoading => _isLoading.Value;
        public string ErrorMessage
        {
            get => _errorMessage;
            private set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }

        public async Task ClearErrorMessage()
        {
            ErrorMessage = string.Empty;
            await Task.CompletedTask;
        }

        // Private methods
        private async Task<bool> ObsoleteItemAsync(Guid id)
        {
            var result = await _restaurantService.ObsoleteRestaurant(id); //  
            if (result.IsFailure)
            {
                
                ErrorMessage = result.Error.Message;
                return false;
            }
            return true;
        }

        private async Task<bool> DeleteItemAsync(Guid id)
        {
            var result = await _restaurantService.DeleteAsync(id); // backend يحذف العنصر
            if (result.IsFailure)
            {
                ErrorMessage = result.Error.Message;
                return false;
            }
            return true;
        }
    }
}
