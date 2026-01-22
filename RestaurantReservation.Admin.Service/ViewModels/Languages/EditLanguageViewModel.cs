namespace RestaurantReservation.Admin.Service.ViewModels.Languages;

internal sealed class EditLanguageViewModel : ReactiveObject, IEditLanguageViewModel
{
    private readonly ILanguageService _languageService;
    private Guid languageId = Guid.Empty;
    private LanguageDto language = new();
    private string errorMessage = string.Empty;
    private readonly ObservableAsPropertyHelper<bool> _isLoading;
    private readonly ObservableAsPropertyHelper<bool> _hasError;
    public EditLanguageViewModel(ILanguageService languageService)
    {
        _languageService = languageService;
        GetLanguage = ReactiveCommand.CreateFromTask<Guid>(GetEntity);
        SaveLanguageCommand = ReactiveCommand.CreateFromTask(Save);
        ClearErrorMessageCommand = ReactiveCommand.CreateFromTask(ClearErrorMessage);
        this.WhenAnyValue(x => x.LanguageId)
            .Where(x => x != Guid.Empty)
            .InvokeCommand(GetLanguage);
        _isLoading = this.WhenAnyObservable(
            x => x.GetLanguage.IsExecuting,
            x => x.SaveLanguageCommand.IsExecuting,
            (getData, saveData) => getData || saveData)
            .ToProperty(this, nameof(IsLoading));
        _hasError = this.WhenAnyValue(x => x.ErrorMessage)
            .Select(x => !string.IsNullOrEmpty(x))
            .ToProperty(this, x => x.HasError);
    }
    private ReactiveCommand<Guid, Unit> GetLanguage { get; }
    public ReactiveCommand<Unit, bool> SaveLanguageCommand { get; }
    public ReactiveCommand<Unit, Unit> ClearErrorMessageCommand { get; }
    public LanguageDto Language
    {
        get => language;
        set => this.RaiseAndSetIfChanged(ref language, value);
    }
    public bool IsLoading => _isLoading.Value;
    public bool HasError => _hasError.Value;
    public Guid LanguageId
    {
        get => languageId;
        set => this.RaiseAndSetIfChanged(ref languageId, value);
    }
    public string ErrorMessage
    {
        get => errorMessage;
        set => this.RaiseAndSetIfChanged(ref errorMessage, value);
    }
    private async Task GetEntity(Guid id)
    {
        var entityResult = await _languageService.GetByIdAsync(id);
        if (entityResult.IsFailure)
            ErrorMessage = entityResult.Error.Message;
        Language = entityResult.Value;
    }
    private Task ClearErrorMessage()
    {
        ErrorMessage = string.Empty;
        return Task.CompletedTask;
    }
    // TODO: Need Refactor
    private async Task<bool> Save()
    {
        if (languageId == Guid.Empty)
        {
            var added = new AddLanguageRequest(
                Language.LanguageName,
                Language.LanguageDescription,
                Language.LanguageSymbol);
            var addResult = await _languageService.AddAsync(added);
            if (addResult.IsFailure)
            {
                ErrorMessage = addResult.Error.Message;
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            var updated = new UpdateLanguageRequest(LanguageId, Language.LanguageName,
                Language.LanguageDescription,
                Language.LanguageSymbol);
            var addResult = await _languageService.UpdateAsync(updated);
            if (addResult.IsFailure)
            {
                ErrorMessage = addResult.Error.Message;
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}