namespace VestigeApp.Components.Pages.OurStory;

public partial class OurStory
{
    private record Item(string Title, string Text);

    private readonly Item[] _chapters =
    {
        new("A few friends and a map", "The first walks were free, informal and always ran late because someone had another story to tell."),
        new("Finding our routes", "We learned which streets reward slow walking, and which stories deserve a chair and a cold drink."),
        new("Opening the door wider", "Today the tours are organised, small and open to anyone curious about the old city."),
    };

    private readonly Item[] _values =
    {
        new("Respect the place", "We tread lightly, support neighbourhood businesses and honour the communities who live here."),
        new("Tell it accurately", "Stories are researched and sourced. When history is uncertain, we say so."),
        new("Welcome everyone", "Routes and pacing are designed so more people can join in."),
    };
}
