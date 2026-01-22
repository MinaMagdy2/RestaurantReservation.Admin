using RestaurantReservation.Admin.Service.Abstractions.Interfaces;

namespace RestaurantReservation.Admin.Service.ClientServices;

internal sealed class ComsysService(IComsysApi comsysApi) : IComsysService
{
    public Task<Result<GuestDataDto>> GetGuestByRoomAsync(string room) =>
        comsysApi.GetGuestByRoom(room).ReturnResult("Get Guest", ErrorType.NotFound);

    public Task<IEnumerable<MealPatternDto>> GetMealPatternsAsync() =>
        comsysApi.GetMealPatterns();

    public Task<IEnumerable<NationalityDto>> GetNationalitiesAsync() =>
        comsysApi.GetNationalities();

    public Task<IEnumerable<RoomTypeDto>> GetRoomTypesAsync() =>
        comsysApi.GetRoomTypes();

    public Task<IEnumerable<TravelAgentDto>> GetTravelAgentsAsync() =>
        comsysApi.GetTravelAgents();
}