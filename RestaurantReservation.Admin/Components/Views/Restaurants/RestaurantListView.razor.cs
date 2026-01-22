using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using RestaurantReservation.Admin.Service.Interfaces.ViewModels;
using RestaurantReservation.Dto.Models.Restaurants;
using Syncfusion.Blazor.Grids;
using System.Reactive.Threading.Tasks;


namespace RestaurantReservation.Admin.Components.Views.Restaurants;

public partial class RestaurantListView
{
    [Inject] protected IDeletRestaurantViewModel DeletRestaurantViewModel { get; set; } = null!;

    private const string EditCommandId = "edit";
    [Inject]

    protected IRestaurantListViewModel viewModel
    {
        get => ViewModel!;
        set => ViewModel = value;
    }
    [Parameter]
    public EventCallback AddEvent { get; set; }
    [Parameter]
    public EventCallback<Guid> EditEvent { get; set; }
    private async Task AddNew()
    {
        if (AddEvent.HasDelegate)
            await AddEvent.InvokeAsync();
    }
    protected override async Task OnInitializedAsync()
    {
        await ViewModel!.InitializeCommand.Execute().ToTask();
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

        await DeletRestaurantViewModel.DeleteItemCommand.Execute(id).ToTask();
        await ViewModel!.InitializeCommand.Execute().ToTask();
    }
    private async Task ToggleObsolete(RestaurantDto item)
    {
        string message = item.IsObsolete
        ? "Do you want to reactivate this item?"
        : "Do you want to pause this item?";

        bool confirmed = await JS.InvokeAsync<bool>("confirm", message);
        if (!confirmed)
            return;

        await DeletRestaurantViewModel.ObsoleteItemCommand.Execute(item.Id).ToTask();
        await ViewModel!.InitializeCommand.Execute().ToTask();
    }
    private async Task OnCommandClicked(CommandClickEventArgs<RestaurantDto> args)
    {
        var rowData = args.RowData;
        if (rowData != null)
        {
            var buttonId = args.CommandColumn.ID.ToLower();
            switch (buttonId)
            {
                case EditCommandId:
                    if (EditEvent.HasDelegate)
                        await EditEvent.InvokeAsync(rowData.Id);
                    break;

            }
        }
    }
}