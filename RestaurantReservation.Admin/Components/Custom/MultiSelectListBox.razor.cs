using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Reflection;

namespace RestaurantReservation.Admin.Components.Custom;

/// <summary>
/// A reusable multi-select list box component that supports generic data sources and value types.
/// </summary>
/// <typeparam name="TItem">The type of items in the data source</typeparam>
/// <typeparam name="TValue">The type of the value field (string, Guid, int, etc.)</typeparam>
public partial class MultiSelectListBox<TItem, TValue> : ComponentBase, IDisposable
{
    /// <summary>
    /// The data source collection to display in the list box
    /// </summary>
    [Parameter]
    public IEnumerable<TItem>? DataSource { get; set; }

    /// <summary>
    /// The currently selected values (for @bind-Value syntax)
    /// </summary>
    [Parameter]
    public List<TValue>? Value { get; set; }

    /// <summary>
    /// Event callback fired when value changes (for @bind-Value syntax)
    /// </summary>
    [Parameter]
    public EventCallback<List<TValue>> ValueChanged { get; set; }

    /// <summary>
    /// The currently selected values (legacy parameter, use Value for @bind-Value)
    /// </summary>
    [Parameter]
    public List<TValue>? SelectedValues { get; set; }

    /// <summary>
    /// Event callback fired when selected values change (legacy parameter, use ValueChanged for @bind-Value)
    /// </summary>
    [Parameter]
    public EventCallback<List<TValue>> SelectedValuesChanged { get; set; }

    /// <summary>
    /// The name or expression of the property to use as the display text
    /// </summary>
    [Parameter]
    public string TextField { get; set; } = string.Empty;

    /// <summary>
    /// The name or expression of the property to use as the value
    /// </summary>
    [Parameter]
    public string ValueField { get; set; } = string.Empty;

    /// <summary>
    /// Optional label to display above the list box
    /// </summary>
    [Parameter]
    public string? Label { get; set; }

    /// <summary>
    /// The size (height) of the list box in rows
    /// </summary>
    [Parameter]
    public int Size { get; set; } = 5;

    /// <summary>
    /// Additional CSS classes to apply to the select element
    /// </summary>
    [Parameter]
    public string? CssClass { get; set; }

    /// <summary>
    /// Validation message to display below the list box
    /// </summary>
    [Parameter]
    public string? ValidationMessage { get; set; }

    /// <summary>
    /// Whether the component is disabled
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Whether to show search functionality
    /// </summary>
    [Parameter]
    public bool ShowSearch { get; set; }

    /// <summary>
    /// Placeholder text for the search input
    /// </summary>
    [Parameter]
    public string SearchPlaceholder { get; set; } = "Search...";

    /// <summary>
    /// Whether to show Select All and Select None buttons
    /// </summary>
    [Parameter]
    public bool ShowSelectButtons { get; set; } = true;

    /// <summary>
    /// Text for the Select All button
    /// </summary>
    [Parameter]
    public string SelectAllText { get; set; } = "Select All";

    /// <summary>
    /// Text for the Select None button
    /// </summary>
    [Parameter]
    public string SelectNoneText { get; set; } = "Select None";

    /// <summary>
    /// Maximum number of items that can be selected (0 = unlimited)
    /// </summary>
    [Parameter]
    public int MaxSelection { get; set; } = 0;

    /// <summary>
    /// Whether to show the item count (e.g., "5 of 10 selected")
    /// </summary>
    [Parameter]
    public bool ShowItemCount { get; set; } = false;

    /// <summary>
    /// Whether the component is in a loading state
    /// </summary>
    [Parameter]
    public bool IsLoading { get; set; } = false;

    /// <summary>
    /// Loading text to display when IsLoading is true
    /// </summary>
    [Parameter]
    public string LoadingText { get; set; } = "Loading...";

    /// <summary>
    /// Debounce delay in milliseconds for search input (0 = no debouncing)
    /// </summary>
    [Parameter]
    public int SearchDebounceMs { get; set; } = 300;

    /// <summary>
    /// Event callback fired when an item is selected
    /// </summary>
    [Parameter]
    public EventCallback<TValue> OnItemSelected { get; set; }

    /// <summary>
    /// Event callback fired when an item is deselected
    /// </summary>
    [Parameter]
    public EventCallback<TValue> OnItemDeselected { get; set; }

    /// <summary>
    /// Event callback fired when search text changes
    /// </summary>
    [Parameter]
    public EventCallback<string> OnSearchChanged { get; set; }

    /// <summary>
    /// Event callback fired when selection limit is reached
    /// </summary>
    [Parameter]
    public EventCallback OnSelectionLimitReached { get; set; }

    /// <summary>
    /// Unique identifier for the component (for accessibility)
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Additional attributes to apply to the select element
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private PropertyInfo? _textProperty;
    private PropertyInfo? _valueProperty;
    private string _searchText = string.Empty;
    private string _displaySearchText = string.Empty;
    private Timer? _debounceTimer;
    private string _componentId = string.Empty;
    private int _focusedIndex = -1;

    /// <summary>
    /// Checks if a type is a numeric type
    /// </summary>
    private static bool IsNumericType(Type type)
    {
        return type == typeof(byte) ||
               type == typeof(sbyte) ||
               type == typeof(short) ||
               type == typeof(ushort) ||
               type == typeof(int) ||
               type == typeof(uint) ||
               type == typeof(long) ||
               type == typeof(ulong) ||
               type == typeof(float) ||
               type == typeof(double) ||
               type == typeof(decimal);
    }

    /// <summary>
    /// Gets the current selected values list (prefers Value over SelectedValues for @bind-Value support)
    /// </summary>
    private List<TValue>? CurrentSelectedValues => Value ?? SelectedValues;

    /// <summary>
    /// Gets filtered items based on search text (uses display search text for debouncing)
    /// </summary>
    private IEnumerable<TItem> FilteredItems
    {
        get
        {
            if (DataSource == null)
                return [];

            if (string.IsNullOrWhiteSpace(_displaySearchText))
                return DataSource;

            var searchLower = _displaySearchText.ToLowerInvariant();
            return DataSource.Where(item =>
            {
                var text = GetText(item).ToLowerInvariant();
                return text.Contains(searchLower, StringComparison.OrdinalIgnoreCase);
            });
        }
    }

    /// <summary>
    /// Gets the count of selected items
    /// </summary>
    private int SelectedCount => CurrentSelectedValues?.Count ?? 0;

    /// <summary>
    /// Gets the total count of items (filtered if search is active)
    /// </summary>
    private int TotalCount => FilteredItems.Count();

    /// <summary>
    /// Gets the count of visible/filtered items
    /// </summary>
    private int VisibleCount => DataSource?.Count() ?? 0;

    /// <summary>
    /// Checks if a value is in the selected values list
    /// </summary>
    private bool IsValueSelected(TValue value)
    {
        var selectedValues = CurrentSelectedValues;
        if (selectedValues == null || selectedValues.Count == 0)
            return false;

        var comparer = EqualityComparer<TValue>.Default;
        return selectedValues.Any(v => comparer.Equals(v, value));
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _componentId = string.IsNullOrEmpty(Id) ? $"multiselect-{Guid.NewGuid():N}" : Id;
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        // Validate required properties
        if (DataSource != null && !string.IsNullOrEmpty(ValueField))
        {
            var firstItem = DataSource.FirstOrDefault();
            if (firstItem != null)
            {
                var property = typeof(TItem).GetProperty(ValueField, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property == null)
                {
                    throw new InvalidOperationException($"Property '{ValueField}' not found on type '{typeof(TItem).Name}'. Please ensure the ValueField property exists.");
                }
            }
        }

        if (DataSource != null && !string.IsNullOrEmpty(TextField))
        {
            var firstItem = DataSource.FirstOrDefault();
            if (firstItem != null)
            {
                var property = typeof(TItem).GetProperty(TextField, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property == null)
                {
                    throw new InvalidOperationException($"Property '{TextField}' not found on type '{typeof(TItem).Name}'. Please ensure the TextField property exists.");
                }
            }
        }

        // Cache property info for performance
        if (!string.IsNullOrEmpty(TextField) && typeof(TItem) != null)
        {
            _textProperty = typeof(TItem).GetProperty(TextField, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        }

        if (!string.IsNullOrEmpty(ValueField) && typeof(TItem) != null)
        {
            _valueProperty = typeof(TItem).GetProperty(ValueField, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        }
    }

    protected override bool ShouldRender()
    {
        // Optimize rendering - only render if something meaningful changed
        return true; // Blazor handles this well, but we can add more logic here if needed
    }

    public void Dispose()
    {
        _debounceTimer?.Dispose();
    }

    /// <summary>
    /// Gets the display text for an item
    /// </summary>
    private string GetText(TItem item)
    {
        if (item is null)
            return string.Empty;

        if (string.IsNullOrEmpty(TextField))
        {
            // If no TextField specified, use ToString()
            return item.ToString() ?? string.Empty;
        }

        if (_textProperty != null)
        {
            var value = _textProperty.GetValue(item);
            return value?.ToString() ?? string.Empty;
        }

        return item.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Gets the value for an item
    /// </summary>
    private TValue GetValue(TItem item)
    {
        if (item is null)
            return default!;

        if (string.IsNullOrEmpty(ValueField))
        {
            // If no ValueField specified and TItem is TValue, return the item itself
            if (item is TValue directValue)
            {
                return directValue;
            }
            throw new InvalidOperationException($"ValueField must be specified when TItem ({typeof(TItem).Name}) is not the same as TValue ({typeof(TValue).Name})");
        }

        if (_valueProperty != null)
        {
            var value = _valueProperty.GetValue(item);
            if (value == null)
                return default!;

            // If the value is already the correct type, return it directly
            if (value is TValue typedValue)
            {
                return typedValue;
            }

            // Try to convert if possible
            var targetType = typeof(TValue);
            var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;
            var sourceType = value.GetType();
            var sourceUnderlyingType = Nullable.GetUnderlyingType(sourceType) ?? sourceType;

            // Handle numeric conversions explicitly
            if (IsNumericType(underlyingType) && IsNumericType(sourceUnderlyingType))
            {
                try
                {
                    return (TValue)Convert.ChangeType(value, underlyingType);
                }
                catch
                {
                    // If conversion fails, try direct cast
                }
            }

            // Try default conversion
            try
            {
                return (TValue)Convert.ChangeType(value, underlyingType);
            }
            catch
            {
                // If all conversions fail, throw
                throw new InvalidOperationException($"Unable to convert value from {sourceType.Name} to {typeof(TValue).Name}");
            }
        }

        throw new InvalidOperationException($"Unable to extract value from item using property '{ValueField}'");
    }

    /// <summary>
    /// Converts a value to string for use in HTML option value attribute
    /// </summary>
    private static string ConvertValueToString(TValue value)
    {
        if (value is null)
            return string.Empty;

        // Handle Guid specially to ensure proper formatting
        if (value is Guid guid)
        {
            return guid.ToString();
        }

        return value.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Converts a string back to TValue
    /// </summary>
    private static TValue ConvertStringToValue(string stringValue)
    {
        if (string.IsNullOrEmpty(stringValue))
            return default!;

        var targetType = typeof(TValue);
        var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        // Handle Guid specially
        if (underlyingType == typeof(Guid))
        {
            if (Guid.TryParse(stringValue, out var guid))
            {
                return (TValue)(object)guid;
            }
        }
        // Handle int
        else if (underlyingType == typeof(int))
        {
            if (int.TryParse(stringValue, out var intValue))
            {
                return (TValue)(object)intValue;
            }
        }
        // Handle long
        else if (underlyingType == typeof(long))
        {
            if (long.TryParse(stringValue, out var longValue))
            {
                return (TValue)(object)longValue;
            }
        }
        // Handle double
        else if (underlyingType == typeof(double))
        {
            if (double.TryParse(stringValue, out var doubleValue))
            {
                return (TValue)(object)doubleValue;
            }
        }
        // Handle decimal
        else if (underlyingType == typeof(decimal))
        {
            if (decimal.TryParse(stringValue, out var decimalValue))
            {
                return (TValue)(object)decimalValue;
            }
        }
        // Handle bool
        else if (underlyingType == typeof(bool))
        {
            if (bool.TryParse(stringValue, out var boolValue))
            {
                return (TValue)(object)boolValue;
            }
        }
        // For string, just return as-is
        else if (underlyingType == typeof(string))
        {
            return (TValue)(object)stringValue;
        }
        // Try default conversion
        else
        {
            try
            {
                return (TValue)Convert.ChangeType(stringValue, underlyingType);
            }
            catch
            {
                // If conversion fails, return default
            }
        }

        return default!;
    }

    /// <summary>
    /// Handles search text change with debouncing
    /// </summary>
    private void HandleSearchChanged(ChangeEventArgs e)
    {
        _searchText = e.Value?.ToString() ?? string.Empty;

        if (SearchDebounceMs > 0)
        {
            // Debounce the search
            _debounceTimer?.Dispose();
            _debounceTimer = new Timer(_ =>
            {
                InvokeAsync(() =>
                {
                    _displaySearchText = _searchText;
                    StateHasChanged();
                    OnSearchChanged.InvokeAsync(_displaySearchText);
                });
            }, null, SearchDebounceMs, Timeout.Infinite);
        }
        else
        {
            // No debouncing - update immediately
            _displaySearchText = _searchText;
            OnSearchChanged.InvokeAsync(_displaySearchText);
        }
    }

    /// <summary>
    /// Clears the search text
    /// </summary>
    private void ClearSearch()
    {
        _debounceTimer?.Dispose();
        _searchText = string.Empty;
        _displaySearchText = string.Empty;
        StateHasChanged();
        OnSearchChanged.InvokeAsync(string.Empty);
    }

    private bool _isHandlingCheckboxChange = false;

    /// <summary>
    /// Handles item click (when clicking on the item div area, not the checkbox or label)
    /// </summary>
    private async Task HandleItemClick(TValue itemValue)
    {
        if (Disabled || _isHandlingCheckboxChange)
            return;

        await ToggleItemSelection(itemValue);
    }

    /// <summary>
    /// Toggles item selection with validation and event callbacks
    /// </summary>
    private async Task ToggleItemSelection(TValue itemValue)
    {
        var currentValues = CurrentSelectedValues ?? [];
        var comparer = EqualityComparer<TValue>.Default;
        var isCurrentlySelected = currentValues.Any(v => comparer.Equals(v, itemValue));

        var updatedValues = new List<TValue>(currentValues);

        if (isCurrentlySelected)
        {
            // Remove from selection
            updatedValues.RemoveAll(v => comparer.Equals(v, itemValue));
            await OnItemDeselected.InvokeAsync(itemValue);
        }
        else
        {
            // Check max selection limit
            if (MaxSelection > 0 && updatedValues.Count >= MaxSelection)
            {
                await OnSelectionLimitReached.InvokeAsync();
                return;
            }

            // Add to selection
            updatedValues.Add(itemValue);
            await OnItemSelected.InvokeAsync(itemValue);
        }

        await UpdateSelectedValues(updatedValues);
    }

    /// <summary>
    /// Updates the selected values and invokes the appropriate callbacks
    /// </summary>
    private async Task UpdateSelectedValues(List<TValue> updatedValues)
    {
        // Priority: ValueChanged (for @bind-Value) > SelectedValuesChanged (legacy)
        // When using @bind-Value, ValueChanged is always set by Blazor
        if (ValueChanged.HasDelegate)
        {
            // Using @bind-Value syntax - always update Value and invoke callback
            Value = updatedValues;
            await ValueChanged.InvokeAsync(updatedValues);
        }
        else if (SelectedValuesChanged.HasDelegate)
        {
            // Using legacy SelectedValues syntax
            SelectedValues = updatedValues;
            await SelectedValuesChanged.InvokeAsync(updatedValues);
        }
        else
        {
            // Fallback: Update the parameter that was provided
            // This handles cases where neither callback is explicitly set
            if (Value != null)
            {
                Value = updatedValues;
            }
            else if (SelectedValues != null)
            {
                SelectedValues = updatedValues;
            }
            else
            {
                // If both are null, initialize Value (for @bind-Value support)
                Value = updatedValues;
            }
        }
    }

    /// <summary>
    /// Handles Select All button click
    /// </summary>
    private async Task HandleSelectAll()
    {
        if (Disabled || DataSource == null)
            return;

        // Get all values from filtered items (or all items if no search)
        var allValues = FilteredItems.Select(GetValue).ToList();

        if (allValues.Count == 0)
            return;

        // Get current selected values
        var currentValues = CurrentSelectedValues ?? [];
        var comparer = EqualityComparer<TValue>.Default;

        // Combine current selections with all filtered values, removing duplicates
        var updatedValues = new List<TValue>(currentValues);
        foreach (var value in allValues)
        {
            if (!updatedValues.Any(v => comparer.Equals(v, value)))
            {
                // Check max selection limit
                if (MaxSelection > 0 && updatedValues.Count >= MaxSelection)
                {
                    await OnSelectionLimitReached.InvokeAsync();
                    break;
                }
                updatedValues.Add(value);
                await OnItemSelected.InvokeAsync(value);
            }
        }

        await UpdateSelectedValues(updatedValues);
    }

    /// <summary>
    /// Handles Select None button click
    /// </summary>
    private async Task HandleSelectNone()
    {
        if (Disabled)
            return;

        // Get all values from filtered items (or all items if no search)
        var filteredValues = DataSource != null ? FilteredItems.Select(GetValue).ToList() : [];

        if (filteredValues.Count == 0)
        {
            // If no filtered items, clear all selections
            await UpdateSelectedValues([]);
            return;
        }

        // Get current selected values
        var currentValues = CurrentSelectedValues ?? [];
        var comparer = EqualityComparer<TValue>.Default;

        // Remove only the filtered items from selection
        var updatedValues = currentValues
            .Where(v => !filteredValues.Any(fv => comparer.Equals(fv, v)))
            .ToList();

        await UpdateSelectedValues(updatedValues);
    }

    /// <summary>
    /// Handles checkbox change event (triggered when clicking checkbox or label)
    /// </summary>
    private async Task HandleCheckboxChanged(ChangeEventArgs e, TValue itemValue)
    {
        if (Disabled)
            return;

        _isHandlingCheckboxChange = true;
        try
        {
            var isChecked = e.Value is bool checkedValue && checkedValue;
            var currentValues = CurrentSelectedValues ?? [];
            var comparer = EqualityComparer<TValue>.Default;
            var isCurrentlySelected = currentValues.Any(v => comparer.Equals(v, itemValue));

            // Only toggle if state actually changed
            if (isChecked == isCurrentlySelected)
                return;

            if (isChecked)
            {
                // Check max selection limit
                if (MaxSelection > 0 && currentValues.Count >= MaxSelection)
                {
                    await OnSelectionLimitReached.InvokeAsync();
                    return;
                }
                await OnItemSelected.InvokeAsync(itemValue);
            }
            else
            {
                await OnItemDeselected.InvokeAsync(itemValue);
            }

            await ToggleItemSelection(itemValue);
        }
        finally
        {
            _isHandlingCheckboxChange = false;
        }
    }

    /// <summary>
    /// Handles keyboard navigation
    /// </summary>
    private async Task HandleKeyDown(KeyboardEventArgs e, TValue itemValue, int index)
    {
        if (Disabled)
            return;

        switch (e.Key)
        {
            case " " or "Enter":
                await ToggleItemSelection(itemValue);
                break;
            case "ArrowDown":
                FocusNextItem(index);
                break;
            case "ArrowUp":
                FocusPreviousItem(index);
                break;
            case "Home":
                FocusFirstItem();
                break;
            case "End":
                FocusLastItem();
                break;
        }
    }

    private void FocusNextItem(int currentIndex)
    {
        var items = FilteredItems.ToList();
        if (currentIndex < items.Count - 1)
        {
            _focusedIndex = currentIndex + 1;
            StateHasChanged();
        }
    }

    private void FocusPreviousItem(int currentIndex)
    {
        if (currentIndex > 0)
        {
            _focusedIndex = currentIndex - 1;
            StateHasChanged();
        }
    }

    private void FocusFirstItem()
    {
        _focusedIndex = 0;
        StateHasChanged();
    }

    private void FocusLastItem()
    {
        var items = FilteredItems.ToList();
        _focusedIndex = items.Count > 0 ? items.Count - 1 : -1;
        StateHasChanged();
    }
}

