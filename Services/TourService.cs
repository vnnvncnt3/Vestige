using VestigeApp.Models;

namespace VestigeApp.Services;

/// <summary>Single source of truth for the tours shown on Home and chosen in Reviews and Book.</summary>
public class TourService
{
    private readonly List<Tour> _tours = new()
    {
        new("Old Town Walking Tour", "Streets & Squares",
            "A gentle loop through the oldest lanes, markets and plazas of the historic centre.",
            "tour-old-town.jpg", "2.5 hours", 12,
            new[] { "Local guide throughout", "Rest stops along the route", "Small group" }),
        new("Landmarks & Stories", "Architecture",
            "Churches, forts and civic halls, and the people and events that shaped them.",
            "tour-landmarks.jpg", "3 hours", 10,
            new[] { "Entry to featured landmarks", "Story-led commentary", "Photo stops" }),
        new("Heritage Food Trail", "Culinary",
            "Taste local dishes at family-run stalls and learn how each recipe came to town.",
            "tour-food.jpg", "3 hours", 8,
            new[] { "Tastings at several stops", "Meet the cooks", "Vegetarian options on request" }),
    };

    public IReadOnlyList<Tour> All => _tours;

    public Tour? Find(string? name) => _tours.FirstOrDefault(t => t.Name == name);
}
