using Microsoft.AspNetCore.Components;
using RestaurantReservation.Admin.Service.Interfaces.ViewModels;
using RestaurantReservation.Dto.Models.Items;
using System.Reactive.Threading.Tasks;

namespace RestaurantReservation.Admin.Components.Views.Items;

public partial class AddItemView
{
    [Inject]
    protected NavigationManager Navigation { get; set; } = null!;
    private string Header = string.Empty;
    [Parameter]
    public Guid Id { get; set; }
    [Inject]
    protected IAddItemViewModel AddItemViewModel
    {
        get => ViewModel!;
        set => ViewModel = value;
    }

    protected override void OnInitialized()
    {
        Header = Id == Guid.Empty ? "Add Item" : "Update Item";
        ViewModel!.ItemId = Id;

        if (Id == Guid.Empty)
        {
            
            ViewModel.Item = new ItemDto();
            ViewModel.SelectedRestaurantNames = new List<string>();
        }
    
    }

    private async Task SaveEntity()
    {
       
        var saved = await ViewModel!.SaveCommand.Execute().ToTask();
        if (saved)
            Navigation.NavigateTo("/Items");
    }

    private async Task ClearErrors()
    {
        await ViewModel!.ClearErrorMessageCommand.Execute().ToTask();
    }
    private void Cancel()
    {
        Navigation.NavigateTo("/Items");
        
    }
  
}