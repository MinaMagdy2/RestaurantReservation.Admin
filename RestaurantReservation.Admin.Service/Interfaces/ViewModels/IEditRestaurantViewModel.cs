namespace RestaurantReservation.Admin.Service.Interfaces.ViewModels;

public interface IEditRestaurantViewModel : IReactiveObject
{
    bool AdultsOnly { get; set; }
    bool CanReserveWithAnotherRestaurantOnSameDay { get; set; }
    bool ChildrenOnly { get; set; }
    ReactiveCommand<Unit, Unit> ClearErrorMessageCommand { get; }
    ReactiveCommand<Unit, Unit> CompleteDataCommand { get; }
    ReactiveCommand<Guid, Unit> GetRestaurantCommand { get; }
    IEnumerable<RestaurantLanguageModel> Descriptions { get; set; }
    bool DisplayMessageWhenSelect { get; set; }
    string ErrorMessage { get; }
    bool HasError { get; }
    string ImageUrl { get; set; }
    ReactiveCommand<Unit, Unit> InitializeCommand { get; }
    bool IsLoading { get; }
    ReadOnlyObservableCollection<LanguageDto> Languages { get; }
    ReadOnlyObservableCollection<MealPatternDto> MealPatterns { get; }
    Meals Meals { get; set; }
    IEnumerable<RestaurantLanguageModel> Messages { get; set; }
    bool MustReserveDayBefore { get; set; }
    bool MustSelectItemsBeforeSave { get; set; }
    ReadOnlyObservableCollection<NationalityDto> Nationalities { get; }
    bool ReserveOncePerStay { get; set; }
    Guid RestaurantId { get; set; }
    string RestaurantName { get; set; }
    ReadOnlyObservableCollection<RoomTypeDto> RoomTypes { get; }
    ReactiveCommand<Unit, bool> SaveCommand { get; }
    IEnumerable<SeatingDetailDto> Seats { get; set; }
    List<string> SelectedMealPatternCodes { get; set; }
    List<string> SelectedNationalityCodes { get; set; }
    List<string> SelectedRoomTypeCodes { get; set; }
    List<string> SelectedTravelAgentCodes { get; set; }
    ReadOnlyObservableCollection<TravelAgentDto> TravelAgents { get; }
    bool UseMealPatternCondition { get; set; }
    bool UseNationalityCondition { get; set; }
    bool UseTravelAgentCondition { get; set; }
    bool UseRoomTypeCondition { get; set; }
}
