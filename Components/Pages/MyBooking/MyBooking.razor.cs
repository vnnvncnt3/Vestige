using Microsoft.AspNetCore.Components;
using VestigeApp.Models;
using VestigeApp.Services;

namespace VestigeApp.Components.Pages.MyBooking;

public partial class MyBooking
{
    [Inject] private BookingService BookingStore { get; set; } = default!;
    [Inject] private TourService TourStore { get; set; } = default!;

    /// <summary>Optional ?ref= value, so the confirmation panel can link straight here.</summary>
    [Parameter, SupplyParameterFromQuery(Name = "ref")]
    public string? Ref { get; set; }

    private string _lookup = "";
    private BookingRow? _booking;
    private int _guests;
    private bool _notFound;
    private bool _saving;
    private bool _confirmCancel;
    private bool _cancelled;
    private string? _message;
    private string? _error;

    private int MaxGuests => TourStore.Find(_booking?.Tour)?.MaxGuests ?? Booking.MaxGuestsOverall;

    protected override async Task OnInitializedAsync()
    {
        if (!string.IsNullOrWhiteSpace(Ref))
        {
            _lookup = Ref;
            await Find();
        }
    }

    // READ
    private async Task Find()
    {
        _message = _error = null;
        _confirmCancel = _cancelled = false;

        var code = _lookup.Trim().ToUpperInvariant();
        _booking = code.Length == 0 ? null : await BookingStore.GetByReferenceAsync(code);
        _notFound = _booking is null;
        _guests = _booking?.Guests ?? 0;
    }

    private void ChangeGuests(int delta) => _guests = Math.Clamp(_guests + delta, 1, MaxGuests);

    // UPDATE
    private async Task Save()
    {
        if (_booking is null || _saving) return;
        _saving = true;
        _message = _error = null;

        var (ok, error) = await BookingStore.UpdateGuestsAsync(_booking, _guests);
        if (ok)
        {
            _booking.Guests = _guests;
            _message = "Your booking has been updated.";
        }
        else
        {
            _error = error;
            _guests = _booking.Guests;
        }
        _saving = false;
    }

    // DELETE (soft): asks twice, so one stray click cannot cancel a booking
    private void AskCancel() => _confirmCancel = true;
    private void KeepBooking() => _confirmCancel = false;

    private async Task ConfirmCancel()
    {
        if (_booking is null || _saving) return;
        _saving = true;
        await BookingStore.CancelAsync(_booking.Id);
        _booking = null;
        _confirmCancel = false;
        _cancelled = true;
        _saving = false;
    }
}