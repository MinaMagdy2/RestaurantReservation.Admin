namespace RestaurantReservation.Admin.Service.ViewModels.Languages;


internal sealed class AddLanguageNationalitiesViewModel : ReactiveObject, IAddLanguageNationalitiesViewModel
{
    private readonly ILanguageService _languageService;
    private readonly IComsysService _comsysService;
    private readonly SourceList<NationalityDto> nationalityList = new();
    private LanguageDto language = new();
    private string errorMessage = string.Empty;
    private string[] selectedNationalityCodes = [];
    private readonly ObservableAsPropertyHelper<bool> _isLoading;
    private readonly ObservableAsPropertyHelper<bool> _hasError;

    private readonly ReadOnlyObservableCollection<NationalityDto> nationalities;
    private readonly CompositeDisposable _disposable = [];

    public AddLanguageNationalitiesViewModel(ILanguageService languageService, IComsysService comsysService)
    {
        _comsysService = comsysService;
        _languageService = languageService;
        InitializeCommand = ReactiveCommand.CreateFromTask<Guid>(InitialMethod);
        ClearErrorMessageCommand = ReactiveCommand.CreateFromTask(ClearErrorMessage);
        SaveCommand = ReactiveCommand.CreateFromTask(Save);
        _isLoading = this.WhenAnyObservable(x => x.InitializeCommand.IsExecuting,
            x => x.SaveCommand.IsExecuting,
            (init, save) => init || save)
            .ToProperty(this, nameof(IsLoading));
        nationalityList.Connect().RefCount()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out nationalities)
            .DisposeMany()
            .Subscribe(_ => { }, RxApp.DefaultExceptionHandler.OnNext)
            .DisposeWith(_disposable);
        _hasError = this.WhenAnyValue(x => x.ErrorMessage)
          .Select(x => !string.IsNullOrEmpty(x))
          .ToProperty(this, nameof(HasError));
    }

    public ReactiveCommand<Guid, Unit> InitializeCommand { get; }
    public ReactiveCommand<Unit, Unit> ClearErrorMessageCommand { get; }
    public ReactiveCommand<Unit, bool> SaveCommand { get; }
    public LanguageDto Language
    {
        get => language;
        set => this.RaiseAndSetIfChanged(ref language, value);
    }
    public string ErrorMessage
    {
        get => errorMessage;
        set => this.RaiseAndSetIfChanged(ref errorMessage, value);
    }
    public string[] SelectedNationalityCodes
    {
        get => selectedNationalityCodes;
        set => this.RaiseAndSetIfChanged(ref selectedNationalityCodes, value);
    }
    public bool IsLoading => _isLoading.Value;
    public bool HasError => _hasError.Value;
    public ReadOnlyObservableCollection<NationalityDto> Nationalities => nationalities;
    private async Task InitialMethod(Guid id)
    {
        nationalityList.Clear();
        var nationalitiesTask = _comsysService.GetNationalitiesAsync();
        var languageTask = _languageService.GetByIdAsync(id);
        await Task.WhenAll(nationalitiesTask, languageTask);
        var languageResult = languageTask.Result;
        if (languageResult.IsFailure)
            ErrorMessage = languageResult.Error.Message;
        else
        {
            nationalityList.AddRange(nationalitiesTask.Result);
            Language = languageResult.Value;
            SelectedNationalityCodes = [.. Language.Nationalities];
        }
    }
    private Task ClearErrorMessage()
    {
        ErrorMessage = string.Empty;
        return Task.CompletedTask;
    }
    private async Task<bool> Save()
    {
        var request = new AddLanguageNationalitiesRequest(Language.Id, SelectedNationalityCodes);
        var result = await _languageService.AddNationalities(request);
        if (result.IsFailure)
            ErrorMessage = result.Error.Message;
        return result.IsSuccess;
    }
}