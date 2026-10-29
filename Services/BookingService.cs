using VestigeApp.Models;

namespace VestigeApp.Services;

/// <summary>In-memory booking store (front end only) that enforces the capacity rules of each tour.</summary>
public class BookingService
{
    private readonly TourService _tours;
    private readonly List<Booking> _bookings = new();
    private readonly object _lock = new();

    public BookingService(TourService tours) => _tours = tours;

    /// <summary>Saves the booking, or returns false with the reason it cannot be accepted.</summary>
    public bool TryAdd(Booking booking, out string? error)
    {
        var tour = _tours.Find(booking.Tour);
        if (tour is null)
        {
            error = "That tour is not available.";
            return false;
        }

        if (booking.Guests > tour.MaxGuests)
        {
            error = $"{tour.Name} takes up to {tour.MaxGuests} guests.";
            return false;
        }

        lock (_lock)
        {
            var taken = _bookings.Where(b => b.Tour == tour.Name && b.Date == booking.Date).Sum(b => b.Guests);
            var left = tour.MaxGuests - taken;
            if (booking.Guests > left)
            {
                error = left <= 0
                    ? "That date is fully booked. Please choose another day."
                    : $"Only {left} spot{(left == 1 ? "" : "s")} left on that date.";
                return false;
            }

            _bookings.Add(booking);
        }

        error = null;
        return true;
    }
}
