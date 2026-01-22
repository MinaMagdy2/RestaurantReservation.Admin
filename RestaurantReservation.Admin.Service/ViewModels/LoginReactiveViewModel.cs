namespace RestaurantReservation.Admin.Service.ViewModels;

internal class LoginReactiveViewModel : ReactiveObject, ILoginReactiveViewModel
{
    private readonly ObservableAsPropertyHelper<bool> _hasError;
    private readonly ObservableAsPropertyHelper<bool> _isLoading;
    private readonly ILoginService _loginService;
    private readonly IUserStateService _userStateService;
    private string userName = string.Empty;
    private string password = string.Empty;
    private string errorMessage = string.Empty;

    public LoginReactiveViewModel(ILoginService loginService, IUserStateService userStateService)
    {
        _loginService = loginService;
        _userStateService = userStateService;
        var canExecuteLogin = this.WhenAnyValue(x => x.UserName,
            x => x.Password,
                (u, p) => !string.IsNullOrWhiteSpace(u) || !string.IsNullOrWhiteSpace(p));

        LoginCommand = ReactiveCommand.CreateFromTask(Login, canExecuteLogin);
        _isLoading = this.WhenAnyObservable(x => x.LoginCommand.IsExecuting)
            .ToProperty(this, x => x.IsLoading, initialValue: false);
        _hasError = this.WhenAnyValue(x => x.ErrorMessage)
            .Select(x => !string.IsNullOrEmpty(x))
            .ToProperty(this, x => x.HasError);
    }
    public ReactiveCommand<Unit, bool> LoginCommand { get; }

    public string UserName
    {
        get => userName;
        set => this.RaiseAndSetIfChanged(ref userName, value);
    }
    public string Password
    {
        get => password;
        set => this.RaiseAndSetIfChanged(ref password, value);
    }
    public string ErrorMessage
    {
        get => errorMessage;
        set => this.RaiseAndSetIfChanged(ref errorMessage, value);
    }
    public bool HasError => _hasError.Value;
    public bool IsLoading => _isLoading.Value;
    private async Task<bool> Login()
    {
        var result = await _loginService.Login(UserName, Password);
        if (result.IsFailure)
        {
            ErrorMessage = result.Error.Message;
            ClearData();
            return false;
        }
        var tokenResult = result.Value;
        await _userStateService.SetAccessToken(tokenResult.Token, tokenResult.Expiry, tokenResult.RefreshToken, tokenResult.TenantId);
        ClearData();
        return true;
    }
    private void ClearData()
    {
        UserName = string.Empty;
        Password = string.Empty;
    }
}
