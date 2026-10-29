using Microsoft.AspNetCore.Components.Routing;

namespace VestigeApp.Components.Layout;

public partial class MainLayout
{
    private bool _open;

    private readonly (string Label, string Href)[] _links =
    {
        ("Home", ""),
        ("Our Story", "our-story"),
        ("Reviews", "reviews"),
    };

    private static NavLinkMatch MatchFor(string href) => href == "" ? NavLinkMatch.All : NavLinkMatch.Prefix;

    private void Toggle() => _open = !_open;

    private void Close() => _open = false;
}
