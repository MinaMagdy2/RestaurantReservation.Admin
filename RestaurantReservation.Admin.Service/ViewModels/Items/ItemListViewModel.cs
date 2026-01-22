using RestaurantReservation.Dto.Models.Items;

namespace RestaurantReservation.Admin.Service.ViewModels.Items;

internal sealed class ItemListViewModel : ReactiveObject, IItemListViewModel
{
    private readonly IItemService _ItemService;
    private readonly SourceList<ItemDto> itemList = new();
    private readonly ReadOnlyObservableCollection<ItemDto> _items;
    private string errorMessage = string.Empty;
    private readonly CompositeDisposable _disposable = [];
    private readonly ObservableAsPropertyHelper<bool> _isLoading;
    public ItemListViewModel(IItemService ItemService)
    {
        _ItemService = ItemService;
        InitializeCommand = ReactiveCommand.CreateFromTask(InitializeMethod);
        itemList.Connect()
            .RefCount()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _items)
            .DisposeMany()
            .Subscribe(_ => { }, RxApp.DefaultExceptionHandler.OnNext)
            .DisposeWith(_disposable);
        _isLoading = this.WhenAnyObservable(x => x.InitializeCommand.IsExecuting)
            .ToProperty(this, nameof(IsLoading));

    }

    public ReactiveCommand<Unit, Unit> InitializeCommand { get; }
    public bool IsLoading => _isLoading.Value;
    public ReadOnlyObservableCollection<ItemDto> Items => _items;

    public string ErrorMessage
    {
        get => errorMessage;
        set => this.RaiseAndSetIfChanged(ref errorMessage, value);
    }
    private async Task InitializeMethod()
    {
        itemList.Clear();
        var result = await _ItemService.GetBasicAsync();
        if (result.IsFailure)
            ErrorMessage = result.Error.Message;
        else
        {
            itemList.AddRange(result.Value);
        }
    }
}

