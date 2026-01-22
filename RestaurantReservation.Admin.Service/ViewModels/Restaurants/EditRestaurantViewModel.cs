namespace RestaurantReservation.Admin.Service.ViewModels.Restaurants;

internal partial class EditRestaurantViewModel : ReactiveObject, IEditRestaurantViewModel
{
    private readonly IComsysService _comsysService;
    private readonly CompositeDisposable _disposable = [];
    private readonly ObservableAsPropertyHelper<bool> _hasError;
    private readonly ObservableAsPropertyHelper<bool> _isLoading;
    private readonly ILanguageService _languageService;
    private readonly IRestaurantService _restaurantService;
    private readonly SourceList<LanguageDto> languageList = new();
    private readonly ReadOnlyObservableCollection<LanguageDto> languages;
    private readonly SourceList<MealPatternDto> mealPatternList = new();
    private readonly ReadOnlyObservableCollection<MealPatternDto> mealPatterns;
    private readonly ReadOnlyObservableCollection<NationalityDto> nationalities;
    private readonly SourceList<NationalityDto> nationalityList = new();
    private readonly SourceList<RoomTypeDto> roomTypeList = new();
    private readonly ReadOnlyObservableCollection<RoomTypeDto> roomTypes;
    private readonly SourceList<TravelAgentDto> travelAgentList = new();
    private readonly ReadOnlyObservableCollection<TravelAgentDto> travelAgents;
    public EditRestaurantViewModel(IComsysService comsysService,
        IRestaurantService restaurantService,
        ILanguageService languageService)
    {
        _comsysService = comsysService;
        _languageService = languageService;
        _restaurantService = restaurantService;
        InitializeCommand = ReactiveCommand.CreateFromTask(Initialize);
        ClearErrorMessageCommand = ReactiveCommand.CreateFromTask(ClearErrorMessage);
        SaveCommand = ReactiveCommand.CreateFromTask(Save);
        GetRestaurantCommand = ReactiveCommand.CreateFromTask<Guid>(GetRestaurant);
        CompleteDataCommand = ReactiveCommand.CreateFromTask(CompleteData);
        _isLoading = this.WhenAnyObservable(
            x => x.InitializeCommand.IsExecuting,
            x => x.SaveCommand.IsExecuting,
            x=> x.GetRestaurantCommand.IsExecuting,
            x=> x.CompleteDataCommand.IsExecuting,
            (init, save, getData, complete) => init || save || getData || complete)
            .ToProperty(this, nameof(IsLoading));
        _hasError = this.WhenAnyValue(x => x.ErrorMessage)
            .Select(x => !string.IsNullOrEmpty(x))
            .ToProperty(this, nameof(HasError));

        nationalityList.Connect().RefCount()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out nationalities)
            .DisposeMany()
            .Subscribe(_ => { }, RxApp.DefaultExceptionHandler.OnNext)
            .DisposeWith(_disposable);
        languageList.Connect()
            .RefCount()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out languages)
            .DisposeMany()
            .Subscribe(_ => { }, RxApp.DefaultExceptionHandler.OnNext)
            .DisposeWith(_disposable);
        mealPatternList.Connect()
            .RefCount()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out mealPatterns)
            .DisposeMany()
            .Subscribe(_ => { }, RxApp.DefaultExceptionHandler.OnNext)
            .DisposeWith(_disposable);
        travelAgentList.Connect()
            .RefCount()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out travelAgents)
            .DisposeMany()
            .Subscribe(_ => { }, RxApp.DefaultExceptionHandler.OnNext)
            .DisposeWith(_disposable);
        roomTypeList.Connect()
            .RefCount()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out roomTypes)
            .DisposeMany()
            .Subscribe(_ => { }, RxApp.DefaultExceptionHandler.OnNext)
            .DisposeWith(_disposable);

    }
    #region Commands
    public ReactiveCommand<Unit, Unit> ClearErrorMessageCommand { get; }
    public ReactiveCommand<Unit, Unit> InitializeCommand { get; }
    public ReactiveCommand<Unit, bool> SaveCommand { get; }
    public ReactiveCommand<Guid, Unit> GetRestaurantCommand { get; }
    public ReactiveCommand<Unit, Unit> CompleteDataCommand { get; }
    #endregion
    #region Properties
    [Reactive]
    public bool AdultsOnly { get; set; }

    [Reactive]
    public bool CanReserveWithAnotherRestaurantOnSameDay { get; set; }

    [Reactive]
    public bool ChildrenOnly { get; set; }

    [Reactive]
    public IEnumerable<RestaurantLanguageModel> Descriptions { get; set; } = [];

    [Reactive]
    public bool DisplayMessageWhenSelect { get; set; }

    [Reactive]
    public string ErrorMessage { get; private set; } = string.Empty;

    public bool HasError => _hasError.Value;

    [Reactive]
    public string ImageUrl { get; set; } = string.Empty;


    public bool IsLoading => _isLoading.Value;

    public ReadOnlyObservableCollection<LanguageDto> Languages => languages;

    public ReadOnlyObservableCollection<MealPatternDto> MealPatterns => mealPatterns;

    [Reactive]
    public Meals Meals { get; set; } = Meals.None;

    [Reactive]
    public IEnumerable<RestaurantLanguageModel> Messages { get; set; } = [];

    [Reactive]
    public bool MustReserveDayBefore { get; set; }

    [Reactive]
    public bool MustSelectItemsBeforeSave { get; set; }

    public ReadOnlyObservableCollection<NationalityDto> Nationalities => nationalities;

    [Reactive]
    public bool ReserveOncePerStay { get; set; }

    [Reactive]
    public Guid RestaurantId { get; set; }

    [Reactive]
    public string RestaurantName { get; set; } = string.Empty;

    public ReadOnlyObservableCollection<RoomTypeDto> RoomTypes => roomTypes;

    [Reactive]
    public IEnumerable<SeatingDetailDto> Seats { get; set; } = [];

    [Reactive]
    public List<string> SelectedMealPatternCodes { get; set; } = [];

    [Reactive]
    public List<string> SelectedNationalityCodes { get; set; } = [];

    [Reactive]
    public List<string> SelectedRoomTypeCodes { get; set; } = [];

    [Reactive]
    public List<string> SelectedTravelAgentCodes { get; set; } = [];

    public ReadOnlyObservableCollection<TravelAgentDto> TravelAgents => travelAgents;

    [Reactive]
    public bool UseMealPatternCondition { get; set; }

    [Reactive]
    public bool UseNationalityCondition { get; set; }

    [Reactive]
    public bool UseTravelAgentCondition { get; set; }
    [Reactive]
    public bool UseRoomTypeCondition { get; set; }

    [Reactive]
    private RestaurantDto SelectedRestaurant { get; set; } = default!;
    #endregion
    #region Private Methods
    private Task ClearErrorMessage()
    {
        ErrorMessage = string.Empty;
        return Task.CompletedTask;
    }
    private async Task GetRestaurant(Guid id)
    {
        var result = await _restaurantService.GetByIdAsync(id);
        if (result.IsSuccess)
        {
            SelectedRestaurant = result.Value;
            RestaurantId = SelectedRestaurant.Id;
            RestaurantName = SelectedRestaurant.RestaurantName;
            Meals = SelectedRestaurant.Meals;
            MustReserveDayBefore = SelectedRestaurant.MustReserveDayBefore;
            DisplayMessageWhenSelect = SelectedRestaurant.DisplayMessageWhenSelect;
            MustSelectItemsBeforeSave = SelectedRestaurant.MustSelectItemsBeforeSave;
            ReserveOncePerStay = SelectedRestaurant.ReserveOncePerStay;
            CanReserveWithAnotherRestaurantOnSameDay = SelectedRestaurant.CanReserveWithAnotherRestaurantOnSameDay;
            AdultsOnly = SelectedRestaurant.AdultsOnly;
            ChildrenOnly = SelectedRestaurant.ChildrenOnly;
            ImageUrl = SelectedRestaurant.ImageUrl;
            SelectedNationalityCodes = [.. SelectedRestaurant.Conditions
                .Where(x => x.Condition == ConditionType.Nationality)
                .Select(x => x.ConditionValue)];


            SelectedMealPatternCodes = [.. SelectedRestaurant.Conditions
                .Where(x => x.Condition == ConditionType.MealPattern)
                .Select(x => x.ConditionValue)];
            SelectedRoomTypeCodes = [.. SelectedRestaurant.Conditions
                .Where(x => x.Condition == ConditionType.RoomType)
                .Select(x => x.ConditionValue)];
            SelectedTravelAgentCodes = [.. SelectedRestaurant.Conditions
                .Where(x => x.Condition == ConditionType.TravelAgent)
                .Select(x => x.ConditionValue)];
            UseMealPatternCondition = SelectedRestaurant.Conditions
                .Any(x => x.Condition == ConditionType.MealPattern);
            UseNationalityCondition = SelectedRestaurant.Conditions
                .Any(x => x.Condition == ConditionType.Nationality);
            UseTravelAgentCondition = SelectedRestaurant.Conditions
                .Any(x => x.Condition == ConditionType.TravelAgent);
            UseRoomTypeCondition = SelectedRestaurant.Conditions
                .Any(x => x.Condition == ConditionType.RoomType);
            Messages = [.. SelectedRestaurant.Messages.Select(x=> new RestaurantLanguageModel
            {
                LanguageId = x.LanguageId,
                LanguageName = Languages.FirstOrDefault(l => l.Id == x.LanguageId)?.LanguageName ?? string.Empty,
                Value = x.Value
            })];
            Descriptions = [.. SelectedRestaurant.Descriptions.Select(x=> new RestaurantLanguageModel
            {
                LanguageId = x.LanguageId,
                LanguageName = Languages.FirstOrDefault(l => l.Id == x.LanguageId)?.LanguageName ?? string.Empty,
                Value = x.Value
            })];
            Seats = SelectedRestaurant.SeatingDetails;
        }
        else
        {
            ErrorMessage = result.Error.Message;
        }
    }
    private async Task Initialize()
    {
        SelectedRestaurant = null;

        RestaurantId = Guid.Empty;
        RestaurantName = string.Empty;
        ImageUrl = string.Empty;

        Meals = Meals.None;

        MustReserveDayBefore = false;
        MustSelectItemsBeforeSave = false;
        ReserveOncePerStay = false;
        CanReserveWithAnotherRestaurantOnSameDay = false;

        AdultsOnly = false;
        ChildrenOnly = false;
        DisplayMessageWhenSelect = false;

        SelectedMealPatternCodes = [];
        SelectedNationalityCodes = [];
        SelectedRoomTypeCodes = [];
        SelectedTravelAgentCodes = [];

        UseMealPatternCondition = false;
        UseNationalityCondition = false;
        UseRoomTypeCondition = false;
        UseTravelAgentCondition = false;

        Messages = [];
        Descriptions = [];
        Seats = [];

        ErrorMessage = string.Empty;
        // =======================================

        languageList.Clear();
        nationalityList.Clear();
        mealPatternList.Clear();
        travelAgentList.Clear();
        roomTypeList.Clear();

        var languageTask = _languageService.GetAllAsync();
        var nationalitiesTask = _comsysService.GetNationalitiesAsync();
        var mealPatternsTask = _comsysService.GetMealPatternsAsync();
        var travelAgentsTask = _comsysService.GetTravelAgentsAsync();
        var roomTypesTask = _comsysService.GetRoomTypesAsync();

        await Task.WhenAll(languageTask, nationalitiesTask, mealPatternsTask, travelAgentsTask, roomTypesTask);

        if (languageTask.Result.IsSuccess)
            languageList.AddRange(languageTask.Result.Value);

        nationalityList.AddRange(nationalitiesTask.Result);
        mealPatternList.AddRange(mealPatternsTask.Result);
        travelAgentList.AddRange(travelAgentsTask.Result);
        roomTypeList.AddRange(roomTypesTask.Result);
    }
    private async Task<bool> Save()
    {
        var nationalityConditions = SelectedNationalityCodes.Select(
            nat => new ConditionRequest(ConditionType.Nationality, nat));
        var mealPatternCondition = SelectedMealPatternCodes.Select(
            meal => new ConditionRequest(ConditionType.MealPattern, meal));
        var travelAgentCondition = SelectedTravelAgentCodes.Select(
            travel => new ConditionRequest(ConditionType.TravelAgent, travel));
        var roomTypeCondition = SelectedRoomTypeCodes.Select(
            room => new ConditionRequest(ConditionType.RoomType, room));
        List<ConditionRequest> conditions =
        [
            .. nationalityConditions,
            .. roomTypeCondition,
            .. mealPatternCondition,
            .. travelAgentCondition,
        ];
        var messageRequest = Messages
            .Where(x => !string.IsNullOrEmpty(x.Value))
            .Select(x => new MessageRequest(x.LanguageId, x.Value));
        var seatRequest = Seats.Select(x => new SeatRequest(x.SeatingId, x.FromTime, x.ToTime, x.MaximumCapacity));
        var descriptionRequest = Descriptions
            .Where(x => !string.IsNullOrEmpty(x.Value))
            .Select(x => new DescriptionRequest(x.LanguageId, x.Value));
        if (SelectedRestaurant == null)
        {
            var addRequest = new AddRestaurantRequest(RestaurantName, Meals,
                MustReserveDayBefore, MustSelectItemsBeforeSave, ImageUrl, AdultsOnly, ChildrenOnly,
                CanReserveWithAnotherRestaurantOnSameDay, ReserveOncePerStay,
                messageRequest, seatRequest, conditions, descriptionRequest);
            var response = await _restaurantService.AddAsync(addRequest);
            if (response.IsFailure)
                ErrorMessage = response.Error.Message;
            return response.IsSuccess;
        }
        else // Update
        {
            foreach (var item in conditions)
            {
                var existing = SelectedRestaurant.Conditions.FirstOrDefault(x => x.ConditionValue == item.ConditionValue && x.Condition == item.Condition);
                if (existing != null)
                {
                    item.SetConditionId(existing.ConditionId);
                }
            }
            var updateRequest = new UpdateRestaurantRequest(RestaurantId, RestaurantName, Meals,
                MustReserveDayBefore, MustSelectItemsBeforeSave, ImageUrl, AdultsOnly, ChildrenOnly,
                CanReserveWithAnotherRestaurantOnSameDay, ReserveOncePerStay,
                messageRequest, seatRequest, conditions, descriptionRequest);
            var response = await _restaurantService.UpdateAsync(updateRequest);
            if (response.IsFailure)
                ErrorMessage = response.Error.Message;
            return response.IsSuccess;
        }
    }
    
    private Task CompleteData()
    {
        
        

        if (!Messages.Any())
        {
            var messages = Languages.Select(lang => new RestaurantLanguageModel
            {
                LanguageId = lang.Id,
                LanguageName = lang.LanguageName,
                Value = string.Empty
            });
            Messages = messages;
        }
        else
        {
            var localMessages = Messages.ToList();
            var existingLanguageIds = Messages.Select(x => x.LanguageId);
            var missingMessages = Languages.Where(x => !existingLanguageIds.Contains(x.Id));
            var messages = missingMessages.Select(lang => new RestaurantLanguageModel
            {
                LanguageId = lang.Id,
                LanguageName = lang.LanguageName,
                Value = string.Empty
            });
            localMessages.AddRange(messages);
            Messages = localMessages;
        }

        if (!Descriptions.Any())
        {
            var descriptions = Languages.Select(lang => new RestaurantLanguageModel
            {
                LanguageId = lang.Id,
                LanguageName = lang.LanguageName,
                Value = string.Empty
            });
            Descriptions = descriptions;
        }
        else
        {
            var localDescription = Descriptions.ToList();
            var existingLanguageIds = Descriptions.Select(x => x.LanguageId);
            var missingMessages = Languages.Where(x => !existingLanguageIds.Contains(x.Id));
            var descriptions = missingMessages.Select(lang => new RestaurantLanguageModel
            {
                LanguageId = lang.Id,
                LanguageName = lang.LanguageName,
                Value = string.Empty
            });
            localDescription.AddRange(descriptions);
            Descriptions = localDescription;
        }
        return Task.CompletedTask;
    }
    #endregion

}
