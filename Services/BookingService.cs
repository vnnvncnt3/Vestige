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

    /// <summary>Saves the booking, or returns false with the reason it cannot be accepted.</summary>
    public async Task<(bool Ok, string? Error)> AddAsync(Booking booking)
    {
        var tour = _tours.Find(booking.Tour);
        if (tour is null)
            return (false, "That tour is not available.");

        if (booking.Guests > tour.MaxGuests)
            return (false, $"{tour.Name} takes up to {tour.MaxGuests} guests.");

        // One query: only the active bookings for this tour on this date (filtering happens in the database).
        var date = booking.Date!.Value.ToString("yyyy-MM-dd");
        var res = await _db.From<Booking>()
            .Where(b => b.Tour == tour.Name && b.CancelledAt == null)
            .Filter("tour_date", Operator.Equals, date)
            .Get();

        var taken = res.Models.Sum(b => b.Guests);
        var left = tour.MaxGuests - taken;
        if (booking.Guests > left)
        {
            return (false, left <= 0
                ? "That date is fully booked. Please choose another day."
                : $"Only {left} spot{(left == 1 ? "" : "s")} left on that date.");
        }

        await _db.From<Booking>().Insert(booking);
        return (true, null);
    }
}