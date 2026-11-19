using Microsoft.AspNetCore.Components;
using VestigeApp.Models;
using VestigeApp.Services;

namespace VestigeApp.Components.Pages.Reviews;

public partial class Reviews
{
    [Inject] private ReviewService ReviewStore { get; set; } = default!;
    [Inject] private TourService TourStore { get; set; } = default!;

    private List<Review> _reviews = new();
    private Review _draft = new();
    private bool _submitted;
    private bool _saving;

    // inline edit state
    private long? _editingId;
    private int _editRating;
    private string _editComment = "";
    private string? _editError;

    private IReadOnlyList<Tour> Tours => TourStore.All;

    // Computed from the list loaded once, so no extra queries
    private double Average => _reviews.Count == 0 ? 0 : _reviews.Average(r => r.Rating);
    private int RoundedAverage => (int)Math.Round(Average);
    private int CountFor(int star) => _reviews.Count(r => r.Rating == star);
    private int PercentFor(int star) => _reviews.Count == 0 ? 0 : CountFor(star) * 100 / _reviews.Count;

    protected override async Task OnInitializedAsync() => await Load();

    private async Task Load() => _reviews = await ReviewStore.GetAllAsync();

    // CREATE: only called when the DataAnnotations rules on Review all pass.
    private async Task Submit()
    {
        if (_saving) return;
        _saving = true;
        await ReviewStore.AddAsync(new Review
        {
            Author = _draft.Author.Trim(),
            Tour = _draft.Tour,
            Rating = _draft.Rating,
            Comment = _draft.Comment.Trim(),
        });
        _draft = new Review();
        _submitted = true;
        _saving = false;
        await Load();
    }

    private void HandleInvalid() => _submitted = false;

    // UPDATE
    private void StartEdit(Review r)
    {
        _editingId = r.Id;
        _editRating = r.Rating;
        _editComment = r.Comment;
        _editError = null;
    }

    private void CancelEdit() => _editingId = null;

    private async Task SaveEdit(Review r)
    {
        var comment = _editComment.Trim();
        if (_editRating < 1 || _editRating > 5) { _editError = "Please select a star rating."; return; }
        if (comment.Length < 10 || comment.Length > 500) { _editError = "Your comment must be between 10 and 500 characters."; return; }

        _saving = true;
        await ReviewStore.UpdateAsync(r.Id, _editRating, comment);
        _editingId = null;
        _saving = false;
        await Load();
    }

    // DELETE (soft)
    private async Task Remove(Review r)
    {
        _saving = true;
        await ReviewStore.SoftDeleteAsync(r.Id);
        _saving = false;
        await Load();
    }
}