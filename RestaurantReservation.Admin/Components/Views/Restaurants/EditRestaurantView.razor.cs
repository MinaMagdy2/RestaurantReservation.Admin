using Microsoft.AspNetCore.Components;
using RestaurantReservation.Admin.Service.Interfaces.ViewModels;
using RestaurantReservation.Common.Enums;
using System.Reactive.Threading.Tasks;

namespace RestaurantReservation.Admin.Components.Views.Restaurants;

public partial class EditRestaurantView
{
    private bool acceptMeal = false;
    private readonly Meals[] excludeMeals = new[] { Meals.None };
    [Inject]
    protected NavigationManager Navigation { get; set; } = null!;
    [Inject]
    protected IEditRestaurantViewModel EditRestaurantViewModel
    {
        get => ViewModel!;
        set => ViewModel = value;
    }
    private string Header = string.Empty;
    [Parameter]
    public Guid Id { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await ViewModel!.InitializeCommand.Execute().ToTask();
        Header = Id == Guid.Empty ? "Add Restaurant" : "Update Restaurant";
        if (Id != Guid.Empty)
        {
            ViewModel!.RestaurantId = Id;
            await ViewModel!.GetRestaurantCommand.Execute(Id).ToTask();
        }
        await ViewModel!.CompleteDataCommand.Execute().ToTask();
        acceptMeal = ViewModel.Meals != Meals.None;
    }
    private async Task ClearErrors()
    {
        await ViewModel!.ClearErrorMessageCommand.Execute().ToTask();
    }
    private async Task Save()
    {
        var result = await ViewModel!.SaveCommand.Execute().ToTask();
        if (result)
            Navigation.NavigateTo("/Restaurants");
    }
    private void Cancel()
    {
        Navigation.NavigateTo("/Restaurants");
    }

}