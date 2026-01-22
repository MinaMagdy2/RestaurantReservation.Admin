using DynamicData;
using ReactiveUI;
using RestaurantReservation.Dto.Models.Items;
using RestaurantReservation.Dto.Requests.Items;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text.Json;

internal sealed class AddEditItemViewModel : ReactiveObject, IAddItemViewModel
{
    private readonly IItemService _itemService;
    private readonly IRestaurantService _restaurantService;
    private readonly CompositeDisposable _disposable = new();
    private readonly ObservableAsPropertyHelper<bool> _isLoading;
    private readonly ObservableAsPropertyHelper<bool> _hasError;
    private string errorMessage = string.Empty;
    private Guid itemId = Guid.Empty;

    private readonly SourceList<RestaurantDto> restaurantList = new();
    private readonly ReadOnlyObservableCollection<RestaurantDto> restaurants;

    private ItemDto item = new();

    [Reactive]
    public List<string> SelectedRestaurantNames { get; set; } = new();

    public ReadOnlyObservableCollection<RestaurantDto> Restaurants => restaurants;

    public AddEditItemViewModel(IItemService itemService, IRestaurantService restaurantService)
    {
        _itemService = itemService;
        _restaurantService = restaurantService;

        GetItemCommand = ReactiveCommand.CreateFromTask<Guid>(GetEntity);
        SaveCommand = ReactiveCommand.CreateFromTask(Save);
        ClearErrorMessageCommand = ReactiveCommand.CreateFromTask(ClearErrorMessage);

        this.WhenAnyValue(x => x.ItemId)
            .Where(x => x != Guid.Empty)
            .InvokeCommand(GetItemCommand);

        _isLoading = this.WhenAnyObservable(
            x => x.GetItemCommand.IsExecuting,
            x => x.SaveCommand.IsExecuting,
            (getData, saveData) => getData || saveData)
            .ToProperty(this, nameof(IsLoading));

        _hasError = this.WhenAnyValue(x => x.ErrorMessage)
            .Select(x => !string.IsNullOrEmpty(x))
            .ToProperty(this, nameof(HasError));

        restaurantList.Connect()
            .RefCount()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out restaurants)
            .DisposeMany()
            .Subscribe()
            .DisposeWith(_disposable);

        LoadRestaurants();
    }

    #region Commands
    public ReactiveCommand<Guid, Unit> GetItemCommand { get; }
    public ReactiveCommand<Unit, bool> SaveCommand { get; }
    public ReactiveCommand<Unit, Unit> ClearErrorMessageCommand { get; }
    #endregion

    #region Properties
    public bool HasError => _hasError.Value;
    public Guid ItemId
    {
        get => itemId;
        set => this.RaiseAndSetIfChanged(ref itemId, value);
    }
    public ItemDto Item
    {
        get => item;
        set => this.RaiseAndSetIfChanged(ref item, value);
    }
    public string ErrorMessage
    {
        get => errorMessage;
        set => this.RaiseAndSetIfChanged(ref errorMessage, value);
    }
    public bool IsLoading => _isLoading.Value;
    #endregion

    #region Private Methods
    private async Task LoadRestaurants()
    {
        var result = await _restaurantService.GetAllAsync();
        if (result.IsSuccess)
        {
            restaurantList.Clear();
            restaurantList.AddRange(result.Value);
        }
    }


    private async Task GetEntity(Guid id)
    {
        var entityResult = await _itemService.GetByIdAsync(id);
        if (entityResult.IsFailure)
            ErrorMessage = entityResult.Error.Message;
        else
        {
            Item = entityResult.Value;

            
            SelectedRestaurantNames = Item.Restaurants.Select(r => r.RestaurantName).ToList();
            this.RaisePropertyChanged(nameof(SelectedRestaurantNames));
        }
    }


    private Task ClearErrorMessage()
    {
        ErrorMessage = string.Empty;
        return Task.CompletedTask;
    }

    private async Task<bool> Save()
    {
        var restaurantIds = Restaurants
           .Where(r => SelectedRestaurantNames.Contains(r.RestaurantName))
           .Select(r => r.Id)
           .ToList();
        if (ItemId == Guid.Empty)
        {
           
            var addRequest = new AddItemRequest(
                Item.ItemName,
                Item.Description,
                Item.Price,
                Item.ImageUrl,
                restaurantIds
            );
            Console.WriteLine(JsonSerializer.Serialize(addRequest));

            var addResult = await _itemService.AddAsync(addRequest);
            if (addResult.IsFailure)
            {
                ErrorMessage = addResult.Error.Message;
                return false;
            }

            return true;
        }
        else
        {
            var updateRequest = new UpdateItemRequest(
                ItemId,
                Item.ItemName,
                Item.Description,
                Item.Price,
                Item.ImageUrl,
                restaurantIds
            );

            var updateResult = await _itemService.UpdateAsync(updateRequest);
            if (updateResult.IsFailure)
            {
                ErrorMessage = updateResult.Error.Message;
                return false;
            }

            return true;
        }
    }
    #endregion
}
