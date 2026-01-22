namespace RestaurantReservation.Admin.Service.Interfaces.Services;

public interface IComsysService
{
    Task<Result<GuestDataDto>> GetGuestByRoomAsync(string room);
    Task<IEnumerable<NationalityDto>> GetNationalitiesAsync();
    Task<IEnumerable<RoomTypeDto>> GetRoomTypesAsync();
    Task<IEnumerable<TravelAgentDto>> GetTravelAgentsAsync();
    Task<IEnumerable<MealPatternDto>> GetMealPatternsAsync();
}