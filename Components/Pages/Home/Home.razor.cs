using Microsoft.AspNetCore.Components;
using VestigeApp.Models;
using VestigeApp.Services;

namespace VestigeApp.Components.Pages.Home;

public partial class Home
{
    [Inject] private TourService TourStore { get; set; } = default!;
    [Inject] private ReviewService ReviewStore { get; set; } = default!;

    private record Moment(string Src, string Caption, string Span);
    private record Step(string Title, string Text);

    private readonly Moment[] _moments =
    {
        new("moment-1.jpg", "Morning light on the old town", "md:col-span-2"),
        new("moment-2.jpg", "Details you would walk past", "md:row-span-2"),
        new("tour-landmarks.jpg", "Stories carved in stone", ""),
        new("tour-food.jpg", "Recipes with a past", ""),
    };

    private readonly Step[] _steps =
    {
        new("Meet", "Gather with your guide and a small group at an easy-to-find starting point."),
        new("Walk", "Move at a relaxed pace, stopping where the story is, not where the schedule says."),
        new("Taste & Talk", "Finish with something local and the chance to ask everything you wondered."),
    };

    private IReadOnlyList<Tour> Tours => TourStore.All;

    private IEnumerable<Review> FeaturedReviews => ReviewStore.All.Take(2);

    private int RoundedAverage => (int)Math.Round(ReviewStore.Average);

    /// <summary>Link to the booking page with the tour already selected.</summary>
    private static string BookUrl(Tour tour) => $"book?tour={Uri.EscapeDataString(tour.Name)}";
}
