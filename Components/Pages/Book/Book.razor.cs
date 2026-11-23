using Microsoft.AspNetCore.Components;
using VestigeApp.Models;
using VestigeApp.Services;

namespace VestigeApp.Components.Pages.Book;

public partial class Book
{
    [Inject] private TourService TourStore { get; set; } = default!;
    [Inject] private BookingService BookingStore { get; set; } = default!;

    /// <summary>Optional ?tour= value, so "Book this tour" on Home arrives with the tour selected.</summary>
    [Parameter, SupplyParameterFromQuery(Name = "tour")]
    public string? TourName { get; set; }

    private Booking _booking = new();
    private Booking? _confirmed;
    private string? _error;
    private bool _saving;

    private IReadOnlyList<Tour> Tours => TourStore.All;

    private Tour? SelectedTour => TourStore.Find(_booking.Tour);

    private int MaxGuests => SelectedTour?.MaxGuests ?? Booking.MaxGuestsOverall;

    // The date picker only offers days the rules in Booking allow.
    private string MinDate => DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
    private string MaxDate => DateTime.Today.AddDays(Booking.MaxDaysAhead).ToString("yyyy-MM-dd");

    protected override void OnInitialized()
    {
        if (TourStore.Find(TourName) is { } tour)
            _booking.Tour = tour.Name;

        ClampGuests();
    }

    private void ChangeGuests(int delta)
    {
        _booking.Guests = Math.Clamp(_booking.Guests + delta, 1, MaxGuests);
    }

    // A smaller tour may allow fewer guests than the current count.
    private void ClampGuests()
    {
        _booking.Guests = Math.Clamp(_booking.Guests, 1, MaxGuests);
    }

    // Only called when the DataAnnotations rules on Booking all pass.
    private async Task Submit()
    {
        if (_saving) return;
        _saving = true;

        _booking.Name = _booking.Name.Trim();
        _booking.Email = _booking.Email.Trim();

        var (ok, error) = await BookingStore.AddAsync(_booking);
        _error = error;
        if (ok) _confirmed = _booking;

        _saving = false;
    }

    private void ClearError() => _error = null;

    private void BookAnother()
    {
        _booking = new Booking();
        _confirmed = null;
        _error = null;
    }
}