using Microsoft.AspNetCore.Components;
using VestigeApp.Models;
using VestigeApp.Services;

namespace VestigeApp.Components.Pages.Reviews;

public partial class Reviews
{
    [Inject] private ReviewService ReviewStore { get; set; } = default!;
    [Inject] private TourService TourStore { get; set; } = default!;

    private Review _draft = new();
    private bool _submitted;

    private IReadOnlyList<Tour> Tours => TourStore.All;

    private int RoundedAverage => (int)Math.Round(ReviewStore.Average);

    private int CountFor(int star) => ReviewStore.Distribution[star];

    private int PercentFor(int star)
    {
        var total = ReviewStore.All.Count;
        return total == 0 ? 0 : CountFor(star) * 100 / total;
    }

    // Only called when the DataAnnotations rules on Review all pass.
    private void Submit()
    {
        ReviewStore.Add(new Review
        {
            Author = _draft.Author.Trim(),
            Tour = _draft.Tour,
            Rating = _draft.Rating,
            Comment = _draft.Comment.Trim(),
        });

        _draft = new Review();
        _submitted = true;
    }

    private void HandleInvalid() => _submitted = false;
}
