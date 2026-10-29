using Microsoft.AspNetCore.Components;

namespace VestigeApp.Components.Shared;

public partial class Photo
{
    [Parameter, EditorRequired] public string Src { get; set; } = "";
    [Parameter] public string Alt { get; set; } = "";
    [Parameter] public string Class { get; set; } = "relative overflow-hidden";
    [Parameter] public bool Zoom { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }

    private string BackgroundStyle =>
        $"background-image: url('images/{Src}'), linear-gradient(135deg, #9a3412 0%, #78350f 55%, #431407 100%);";
}
