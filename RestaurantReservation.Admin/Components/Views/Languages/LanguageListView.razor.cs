using Microsoft.AspNetCore.Components;
using RestaurantReservation.Admin.Service.Interfaces.Services;
using RestaurantReservation.Admin.Service.Interfaces.ViewModels;
using RestaurantReservation.Dto.Models;
using Syncfusion.Blazor.Grids;
using System.Reactive.Threading.Tasks;

namespace RestaurantReservation.Admin.Components.Views.Languages;

public partial class LanguageListView
{
    private const string EditCommandId = "edit";
    private const string AddNationalitiesCommandId = "add-nationalities";

    [Inject]
    protected IUserStateService UserStateService { get; set; } = null!;
    [Inject]
    protected ILanguageListViewModel LanguageListViewModel
    {
        get => ViewModel!;
        set => ViewModel = value;
    }
    [Parameter]
    public EventCallback AddEvent { get; set; }
    [Parameter]
    public EventCallback<Guid> EditEvent { get; set; }
    [Parameter]
    public EventCallback<Guid> AddNationalitiesEvent { get; set; }
    private async Task AddNew()
    {
        if (AddEvent.HasDelegate)
            await AddEvent.InvokeAsync();
    }
    protected override async Task OnInitializedAsync()
    {
        await ViewModel!.InitializeCommand.Execute().ToTask();
    }
    private async Task OnCommandClicked(CommandClickEventArgs<LanguageDto> args)
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
                case AddNationalitiesCommandId:
                    if (AddNationalitiesEvent.HasDelegate)
                        await AddNationalitiesEvent.InvokeAsync(rowData.Id);
                    break;
            }
        }
    }
}