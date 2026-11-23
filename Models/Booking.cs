using System.ComponentModel.DataAnnotations;

namespace VestigeApp.Models;

/// <summary>A guest's reservation request for one tour on one date (form model).</summary>
public class Booking : IValidatableObject
{
    /// <summary>Tours can be booked from tomorrow up to this many days ahead.</summary>
    public const int MaxDaysAhead = 180;

    /// <summary>Hard upper limit for any tour; each tour sets its own (lower) limit.</summary>
    public const int MaxGuestsOverall = 12;

    public string Reference { get; set; } = $"VST-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";

    [Required(ErrorMessage = "Please enter your name.")]
    [StringLength(60, ErrorMessage = "Your name must be 60 characters or fewer.")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Please enter your email.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Please choose a tour.")]
    public string Tour { get; set; } = "";

    [Required(ErrorMessage = "Please pick a date.")]
    public DateOnly? Date { get; set; }

    [Range(1, MaxGuestsOverall, ErrorMessage = "Guests must be between 1 and 12.")]
    public int Guests { get; set; } = 2;

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