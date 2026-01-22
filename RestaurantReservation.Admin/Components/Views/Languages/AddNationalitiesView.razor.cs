using Microsoft.AspNetCore.Components;
using RestaurantReservation.Admin.Service.Interfaces.ViewModels;
using System.Reactive.Threading.Tasks;

namespace RestaurantReservation.Admin.Components.Views.Languages;

public partial class AddNationalitiesView
{
    [Inject]
    protected IAddLanguageNationalitiesViewModel AddLanguageNationalitiesViewModel
    {
        get => ViewModel!;
        set => ViewModel = value;
    }
    [Parameter]
    public Guid Id { get; set; }
    [Inject]
    protected NavigationManager Navigation { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        await ViewModel!.InitializeCommand.Execute(Id).ToTask();
    }
    private async Task Save()
    {
        var result = await ViewModel!.SaveCommand.Execute().ToTask();
        if (result)
            Navigation.NavigateTo("/Languages");
    }
    private async Task ClearErrors()
    {
        await ViewModel!.ClearErrorMessageCommand.Execute().ToTask();
    }
    private void Cancel()
    {
        Navigation.NavigateTo("/Languages");
    }
}