using System.ComponentModel.DataAnnotations;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace VestigeApp.Models;

/// <summary>A guest's reservation request for one tour on one date.</summary>
[Table("bookings")]
public class Booking : BaseModel, IValidatableObject
{
    /// <summary>Tours can be booked from tomorrow up to this many days ahead.</summary>
    public const int MaxDaysAhead = 180;

    /// <summary>Hard upper limit for any tour; each tour sets its own (lower) limit.</summary>
    public const int MaxGuestsOverall = 12;

    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("reference")]
    public string Reference { get; set; } = $"VST-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";

    [Column("name")]
    [Required(ErrorMessage = "Please enter your name.")]
    [StringLength(60, ErrorMessage = "Your name must be 60 characters or fewer.")]
    public string Name { get; set; } = "";

    [Column("email")]
    [Required(ErrorMessage = "Please enter your email.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    public string Email { get; set; } = "";

    [Column("tour")]
    [Required(ErrorMessage = "Please choose a tour.")]
    public string Tour { get; set; } = "";

    // Used by the form (date picker). Not stored directly, see TourDate below.
    [Required(ErrorMessage = "Please pick a date.")]
    public DateOnly? Date { get; set; }

    // What the database stores in the "tour_date" (date) column.
    [Column("tour_date")]
    public DateTime TourDate
    {
        get => Date?.ToDateTime(TimeOnly.MinValue) ?? default;
        set => Date = DateOnly.FromDateTime(value);
    }

    [Column("guests")]
    [Range(1, MaxGuestsOverall, ErrorMessage = "Guests must be between 1 and 12.")]
    public int Guests { get; set; } = 2;

    [Column("cancelled_at")]
    public DateTime? CancelledAt { get; set; }

    [Column("created_at", ignoreOnInsert: true, ignoreOnUpdate: true)]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Date is not { } date) yield break;

        var today = DateOnly.FromDateTime(DateTime.Today);
        if (date <= today)
            yield return new ValidationResult("Tours must be booked at least one day ahead.", new[] { nameof(Date) });
        else if (date > today.AddDays(MaxDaysAhead))
            yield return new ValidationResult("Bookings open up to six months ahead.", new[] { nameof(Date) });
    }
}