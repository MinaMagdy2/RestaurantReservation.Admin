using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Buttons;

namespace RestaurantReservation.Admin.Components.Custom
{
    public partial class EnumRadioButton<TItem>
    {
        [Parameter]
        public string Caption { get; set; } = string.Empty;
        [Parameter]
        [EditorRequired]
        public string Name { get; set; }
        [Parameter]
        public TItem Value { get; set; } = default!;

        [Parameter]
        public EventCallback<TItem> ValueChanged { get; set; }
        [Parameter]
        public TItem[] Exclude { get; set; } = [];
        [Parameter]
        public bool Disabled { get; set; }

        private List<TItem> enumTypes = [];
        protected override void OnInitialized()
        {
            foreach (var item in Enum.GetValues(typeof(TItem)))
                enumTypes.Add((TItem)item);
            if (Exclude != null && Exclude.Length > 0)
                enumTypes = [.. enumTypes.Except(Exclude)];
        }

        private async Task OnValueChange(ChangeArgs<TItem> args)
        {
            if (Disabled) return;
            
            if (ValueChanged.HasDelegate)
                await ValueChanged.InvokeAsync(args.Value);
        }
    }
}