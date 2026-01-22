namespace RestaurantReservation.Admin.Service.ViewModels.Restaurants;


internal class RestaurantListViewModel : ReactiveObject, IRestaurantListViewModel
{
    private readonly IRestaurantService _restaurantService;
    private readonly SourceList<RestaurantDto> restaurantList = new();
    private readonly ReadOnlyObservableCollection<RestaurantDto> restaurants;
    private string errorMessage = string.Empty;
    private readonly CompositeDisposable _disposable = [];
    private readonly ObservableAsPropertyHelper<bool> _isLoading;
    private readonly ObservableAsPropertyHelper<bool> _hasError;

    public RestaurantListViewModel(IRestaurantService restaurantService)
    {
        _restaurantService = restaurantService;
        InitializeCommand = ReactiveCommand.CreateFromTask(InitializeMethod);
        restaurantList.Connect()
            .RefCount()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out restaurants)
            .DisposeMany()
            .Subscribe(_ => { }, RxApp.DefaultExceptionHandler.OnNext)
            .DisposeWith(_disposable);
        _isLoading = this.WhenAnyObservable(x => x.InitializeCommand.IsExecuting).ToProperty(this, nameof(IsLoading));
        _hasError = this.WhenAnyValue(x => x.ErrorMessage, msg => !string.IsNullOrEmpty(msg)).ToProperty(this, nameof(HasError));
    }
    public ReactiveCommand<Unit, Unit> InitializeCommand { get; }
    public bool IsLoading => _isLoading.Value;
    public bool HasError => _hasError.Value;
    public ReadOnlyObservableCollection<RestaurantDto> Restaurants => restaurants;
    public string ErrorMessage
    {
        get => errorMessage;
        set => this.RaiseAndSetIfChanged(ref errorMessage, value);
    }
    private async Task InitializeMethod()
    {
        restaurantList.Clear();
        var result = await _restaurantService.GetAllAsync();
        if (result.IsFailure)
            ErrorMessage = result.Error.Message;
        else
        {
            restaurantList.AddRange(result.Value);
        }
    }
}