using Microsoft.AspNetCore.Components;

namespace VestigeApp.Components.Shared;

public partial class PageHeader
{
    [Parameter, EditorRequired] public string Title { get; set; } = "";
    [Parameter] public string Eyebrow { get; set; } = "";
    [Parameter] public string? Lead { get; set; }
    [Parameter] public string Image { get; set; } = "hero.jpg";
    [Parameter] public string Alt { get; set; } = "";
}
