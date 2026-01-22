namespace RestaurantReservation.Admin.Service.ViewModels.Languages;

internal sealed class LanguageListViewModel : ReactiveObject, ILanguageListViewModel
{
    private readonly ILanguageService _languageService;
    private readonly SourceList<LanguageDto> languageList = new();
    private ReadOnlyObservableCollection<LanguageDto> languages;
    private string errorMessage = string.Empty;
    private readonly CompositeDisposable _disposable = [];
    private readonly ObservableAsPropertyHelper<bool> _isLoading;
    public LanguageListViewModel(ILanguageService languageService)
    {
        _languageService = languageService;
        InitializeCommand = ReactiveCommand.CreateFromTask(InitializeMethod);
        languageList.Connect()
            .RefCount()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out languages)
            .DisposeMany()
            .Subscribe(_ => { }, RxApp.DefaultExceptionHandler.OnNext)
            .DisposeWith(_disposable);
        _isLoading = this.WhenAnyObservable(x => x.InitializeCommand.IsExecuting)
            .ToProperty(this, nameof(IsLoading));

    }

    public ReactiveCommand<Unit, Unit> InitializeCommand { get; }
    public bool IsLoading => _isLoading.Value;
    public ReadOnlyObservableCollection<LanguageDto> Languages
    {
        get => languages;
        set => this.RaiseAndSetIfChanged(ref languages, value);
    }
    public string ErrorMessage
    {
        get => errorMessage;
        set => this.RaiseAndSetIfChanged(ref errorMessage, value);
    }
    private async Task InitializeMethod()
    {
        languageList.Clear();
        var result = await _languageService.GetAllAsync();
        if (result.IsFailure)
            ErrorMessage = result.Error.Message;
        else
        {
            languageList.AddRange(result.Value);
        }
    }
}
