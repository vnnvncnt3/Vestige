using Supabase;
using VestigeApp.Models;
using static Supabase.Postgrest.Constants;

namespace VestigeApp.Services;

/// <summary>Booking store backed by the Supabase "bookings" table. Enforces each tour's capacity.</summary>
public class BookingService
{
    private readonly Client _db;
    private readonly TourService _tours;

    public BookingService(Client db, TourService tours)
    {
        _db = db;
        _tours = tours;
    }

    /// <summary>Guests already booked on a tour and date (one query, filtered in the database).</summary>
    private async Task<int> SeatsTakenAsync(string tour, string date, long? exceptId = null)
    {
        var res = await _db.From<BookingRow>()
            .Where(b => b.Tour == tour && b.CancelledAt == null)
            .Filter("tour_date", Operator.Equals, date)
            .Get();

        return res.Models.Where(b => b.Id != exceptId).Sum(b => b.Guests);
    }

    private static string SeatsMessage(int left) => left <= 0
        ? "That date is fully booked. Please choose another day."
        : $"Only {left} spot{(left == 1 ? "" : "s")} left on that date.";

    /// <summary>CREATE: saves the booking, or returns false with the reason it cannot be accepted.</summary>
    public async Task<(bool Ok, string? Error)> AddAsync(Booking booking)
    {
        var tour = _tours.Find(booking.Tour);
        if (tour is null)
            return (false, "That tour is not available.");

        if (booking.Guests > tour.MaxGuests)
            return (false, $"{tour.Name} takes up to {tour.MaxGuests} guests.");

        var date = booking.Date!.Value.ToString("yyyy-MM-dd");
        var left = tour.MaxGuests - await SeatsTakenAsync(tour.Name, date);
        if (booking.Guests > left)
            return (false, SeatsMessage(left));

        await _db.From<BookingRow>().Insert(new BookingRow
        {
            Reference = booking.Reference,
            Name = booking.Name,
            Email = booking.Email,
            Tour = booking.Tour,
            TourDate = booking.Date!.Value.ToDateTime(TimeOnly.MinValue),
            Guests = booking.Guests,
        });
        return (true, null);
    }

    /// <summary>READ: one active (not cancelled) booking by its reference code, or null.</summary>
    public async Task<BookingRow?> GetByReferenceAsync(string reference)
    {
        var res = await _db.From<BookingRow>()
            .Where(b => b.Reference == reference && b.CancelledAt == null)
            .Get();
        return res.Models.FirstOrDefault();
    }

    /// <summary>UPDATE: changes the guest count, re-checking the tour's capacity (ignoring this booking itself).</summary>
    public async Task<(bool Ok, string? Error)> UpdateGuestsAsync(BookingRow booking, int guests)
    {
        var tour = _tours.Find(booking.Tour);
        if (tour is null)
            return (false, "That tour is not available.");

        if (guests < 1 || guests > tour.MaxGuests)
            return (false, $"{tour.Name} takes up to {tour.MaxGuests} guests.");

        var date = booking.TourDate.ToString("yyyy-MM-dd");
        var left = tour.MaxGuests - await SeatsTakenAsync(tour.Name, date, booking.Id);
        if (guests > left)
            return (false, SeatsMessage(left));

        await _db.From<BookingRow>()
            .Where(b => b.Id == booking.Id)
            .Set(b => b.Guests, guests)
            .Update();
        return (true, null);
    }

    /// <summary>DELETE (soft): only fills in cancelled_at, the row stays in the table.</summary>
    public async Task CancelAsync(long id)
        => await _db.From<BookingRow>()
            .Where(b => b.Id == id)
            .Set(b => b.CancelledAt, DateTime.UtcNow)
            .Update();
}