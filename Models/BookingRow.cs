using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace VestigeApp.Models;

/// <summary>One row of the Supabase "bookings" table. Only real columns live here.</summary>
[Table("bookings")]
public class BookingRow : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("reference")]
    public string Reference { get; set; } = "";

    [Column("name")]
    public string Name { get; set; } = "";

    [Column("email")]
    public string Email { get; set; } = "";

    [Column("tour")]
    public string Tour { get; set; } = "";

    [Column("tour_date")]
    public DateTime TourDate { get; set; }

    [Column("guests")]
    public int Guests { get; set; }

    [Column("cancelled_at")]
    public DateTime? CancelledAt { get; set; }

    [Column("created_at", ignoreOnInsert: true, ignoreOnUpdate: true)]
    public DateTime CreatedAt { get; set; }
}