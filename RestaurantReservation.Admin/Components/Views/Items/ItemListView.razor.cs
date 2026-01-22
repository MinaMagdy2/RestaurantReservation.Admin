using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using RestaurantReservation.Admin.Service.Interfaces.Services;
using RestaurantReservation.Admin.Service.Interfaces.ViewModels;
using RestaurantReservation.Dto.Models.Items;
using Syncfusion.Blazor.Grids;
using System.Reactive.Threading.Tasks;

namespace RestaurantReservation.Admin.Components.Views.Items;

public partial class ItemListView
{
    
    [Inject] protected IDeletItemViewModel DeletItemViewModel { get; set; } = null!;
    [Inject] protected IUserStateService UserStateService { get; set; } = null!;

    [Inject]
    protected IItemListViewModel IitemListViewModel
    {
        get => ViewModel!;
        set => ViewModel = value;
    }

    [Parameter] public EventCallback AddEvent { get; set; }
    [Parameter] public EventCallback<Guid> EditEvent { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await ViewModel!.InitializeCommand.Execute().ToTask();
    }

    private async Task AddNew()
    {
        if (AddEvent.HasDelegate)
            await AddEvent.InvokeAsync();
    }

    private async Task EditItem(Guid id)
    {
        if (EditEvent.HasDelegate)
            await EditEvent.InvokeAsync(id);
    }

    private async Task DeleteItem(Guid id)
    {
        bool confirmed = await JS.InvokeAsync<bool>("confirm", "Are you sure you want to delete this item?");
        if (!confirmed)
            return;

        await DeletItemViewModel.DeleteItemCommand.Execute(id).ToTask();
        await ViewModel!.InitializeCommand.Execute().ToTask();
    }

    private async Task ToggleObsolete(ItemDto item)
    {
        string message = item.IsObsolete
        ? "Do you want to reactivate this item?"
        : "Do you want to pause this item?";

        bool confirmed = await JS.InvokeAsync<bool>("confirm", message);
        if (!confirmed)
            return;

        await DeletItemViewModel.ObsoleteItemCommand.Execute(item.Id).ToTask();
        await ViewModel!.InitializeCommand.Execute().ToTask();
    }



}
