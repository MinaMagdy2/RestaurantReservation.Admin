using Microsoft.AspNetCore.Components;
using RestaurantReservation.Admin.Service.Interfaces.ViewModels;
using System.Reactive.Threading.Tasks;

namespace RestaurantReservation.Admin.Components.Views.Languages;

public partial class EditLanguageView
{
    [Inject]
    protected NavigationManager Navigation { get; set; } = null!;
    private string Header = string.Empty;
    [Parameter]
    public Guid Id { get; set; }
    [Inject]
    protected IEditLanguageViewModel EditLanguageViewModel
    {
        get => ViewModel!;
        set => ViewModel = value;
    }

    protected override void OnInitialized()
    {
        Header = Id == Guid.Empty ? "Add Language" : "Update Language";
        ViewModel!.LanguageId = Id;
    }
    private async Task SaveEntity()
    {
        var saved = await ViewModel!.SaveLanguageCommand.Execute().ToTask();
        if (saved)
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