using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using RestaurantReservation.Dto.Models.Restaurants;
using Syncfusion.Blazor.Grids;

namespace RestaurantReservation.Admin.Components.Views.Restaurants;

public partial class RestaurantLanguageView
{
    [Parameter]
    public IEnumerable<RestaurantLanguageModel> Value { get; set; } = [];
    [Parameter]
    public EventCallback<IEnumerable<RestaurantLanguageModel>> ValueChanged { get; set; }

    private const string EditCommandId = "edit";
    private const string DeleteCommandId = "delete";
    private bool DisableEdit;
    private Dictionary<string, object> GridAttributes { get; set; } = [];
    private IEnumerable<RestaurantLanguageModel> _currentValue = [];
    private Guid languageId = Guid.Empty;
    private string languageName = string.Empty;
    private string languageValue = string.Empty;
    private IEnumerable<RestaurantLanguageModel> CurrentValue
    {
        get => _currentValue;
        set
        {
            if (_currentValue != value)
            {
                _currentValue = value;
                ValueChanged.InvokeAsync(value);
            }
        }
    }
    protected override void OnInitialized()
    {
        _currentValue = Value;
        DisableEdit = true;
        GridAttributes.Add("disable", "no");
    }
    private void DisableGrid(bool value)
    {
        if (value)
        {
            GridAttributes["disable"] = "yes";
        }
        else
        {
            GridAttributes["disable"] = "no";
        }
    }
    private void Save()
    {
        List<RestaurantLanguageModel> data = new(CurrentValue);
        var existing = data.FirstOrDefault(x => x.LanguageId == languageId);
        if (existing != null)
        {
            existing.Value = languageValue;
        }
        CurrentValue = data;
        DisableEdit = true;
        DisableGrid(false);
        ClearData();
    }
    private void ClearData()
    {
        languageId = Guid.Empty;
        languageValue = string.Empty;
        languageName = string.Empty;
    }
    private void Cancel()
    {
        DisableEdit = true;
        DisableGrid(false);
        ClearData();

    }
    private Task OnCommandClicked(CommandClickEventArgs<RestaurantLanguageModel> args)
    {
        var rowData = args.RowData;
        if (rowData != null)
        {
            var buttonId = args.CommandColumn.ID.ToLower();
            switch (buttonId)
            {
                case EditCommandId:
                    languageId = rowData.LanguageId;
                    languageValue = rowData.Value;
                    languageName = rowData.LanguageName;
                    DisableEdit = false;
                    DisableGrid(true);
                    break;
                case DeleteCommandId:
                    rowData.Value = string.Empty;
                    break;
            }
        }
        return Task.CompletedTask;
    }
}