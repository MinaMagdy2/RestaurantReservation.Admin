using Microsoft.AspNetCore.Components;
using MudBlazor;
using RestaurantReservation.Dto.Models.Restaurants;
using Syncfusion.Blazor.Grids;

namespace RestaurantReservation.Admin.Components.Views.Restaurants;

public partial class RestaurantTimingView
{
    private TimeSpan? _fromTime = new TimeSpan(00, 00, 00);
    private TimeSpan? _toTime = new TimeSpan(00, 00, 00);
    private int _maximumCapacity = 0;
    private Guid seatingId = Guid.Empty;
    private const string EditCommandId = "edit";
    private const string DeleteCommandId = "delete";
    private bool DisableEdit;
    private Dictionary<string, object> GridAttributes { get; set; } = [];
    private IEnumerable<SeatingDetailDto> _currentValue = [];
    [Parameter]
    public IEnumerable<SeatingDetailDto> Value { get; set; } = [];
    [Parameter]
    public EventCallback<IEnumerable<SeatingDetailDto>> ValueChanged { get; set; }

    private IEnumerable<SeatingDetailDto> CurrentValue
    {
        get => _currentValue;
        set
        {
            if (_currentValue != value)
            {
                _currentValue = value;
                ValueChanged.InvokeAsync(value); // Notify parent of the change
            }
        }
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
    private void Add()
    {
        DisableEdit = false;
        DisableGrid(true);
        seatingId = Guid.Empty;
    }
    protected override void OnInitialized()
    {
        _currentValue = Value;
        DisableEdit = true;
        GridAttributes.Add("disable", "no");
    }
    private TimeSpan GetTimeSpan(string timeString)
    {
        TimeSpan fromTime = TimeSpan.TryParseExact(timeString, @"hh\:mm", null, out var time)
        ? time
        : new TimeSpan(00, 00, 00);
        return fromTime;

    }
    private string TimSpanToString(TimeSpan? timeSpan)
    {
        string timeString = timeSpan?.ToString(@"hh\:mm") ?? string.Empty;
        return timeString;
    }
    private void ClearData()
    {
        _fromTime = new TimeSpan(00, 00, 00);
        _toTime = new TimeSpan(00, 00, 00);
        _maximumCapacity = 0;
    }
    private Task OnCommandClicked(CommandClickEventArgs<SeatingDetailDto> args)
    {
        var rowData = args.RowData;
        if (rowData != null)
        {
            var buttonId = args.CommandColumn.ID.ToLower();
            switch (buttonId)
            {
                case EditCommandId:
                    seatingId = rowData.SeatingId;
                    _maximumCapacity = rowData.MaximumCapacity;
                    _fromTime = GetTimeSpan(rowData.FromTime);
                    _toTime = GetTimeSpan(rowData.ToTime);
                    DisableEdit = false;
                    DisableGrid(true);
                    break;
                case DeleteCommandId:
                    CurrentValue = CurrentValue.Where(x => x.SeatingId != rowData.SeatingId);
                    break;
            }
        }
        return Task.CompletedTask;
    }
    private void Save()
    {
        List<SeatingDetailDto> data = new(CurrentValue);
        var existing = data.FirstOrDefault(x => x.SeatingId == seatingId);
        if (existing == null)
        {
            var seating = new SeatingDetailDto();
            seating.FromTime = TimSpanToString(_fromTime);
            seating.ToTime = TimSpanToString(_toTime);
            seating.MaximumCapacity = _maximumCapacity;
            seating.SeatingId = Guid.NewGuid();
            data.Add(seating);
        }
        else
        {
            existing.FromTime = TimSpanToString(_fromTime);
            existing.ToTime = TimSpanToString(_toTime);
            existing.MaximumCapacity = _maximumCapacity;
        }
        CurrentValue = data;
        ClearData();
        DisableEdit = true;
        DisableGrid(false);
        seatingId = Guid.Empty;
    }
    private void Cancel()
    {
        DisableEdit = true;
        DisableGrid(false);
        ClearData();
        seatingId = Guid.Empty;
    }
}