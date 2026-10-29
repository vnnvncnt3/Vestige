using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace VestigeApp.Components.Shared;

public partial class StarRating
{
    [CascadingParameter] private EditContext? EditContext { get; set; }

    [Parameter] public int Value { get; set; }
    [Parameter] public EventCallback<int> ValueChanged { get; set; }
    [Parameter] public Expression<Func<int>>? ValueExpression { get; set; }
    [Parameter] public bool ReadOnly { get; set; }
    [Parameter] public bool Large { get; set; }
    [Parameter] public int Max { get; set; } = 5;
    [Parameter] public string SizeClass { get; set; } = "text-lg";
    [Parameter] public string? Class { get; set; }

    private bool IsInteractive => !ReadOnly && ValueChanged.HasDelegate;

    private async Task Select(int star)
    {
        Value = star;
        await ValueChanged.InvokeAsync(star);

        // Tell the form this field changed, so its validation message clears once a star is picked.
        if (EditContext is not null && ValueExpression is not null)
            EditContext.NotifyFieldChanged(FieldIdentifier.Create(ValueExpression));
    }
}
