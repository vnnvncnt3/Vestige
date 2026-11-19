using System.ComponentModel.DataAnnotations;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace VestigeApp.Models;

[Table("reviews")]
public class Review : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("author")]
    [Required(ErrorMessage = "Please enter your name.")]
    [StringLength(60, ErrorMessage = "Your name must be 60 characters or fewer.")]
    public string Author { get; set; } = "";

    [Column("tour")]
    [Required(ErrorMessage = "Please choose a tour.")]
    public string Tour { get; set; } = "";

    [Column("rating")]
    [Range(1, 5, ErrorMessage = "Please select a star rating.")]
    public int Rating { get; set; }

    [Column("comment")]
    [Required(ErrorMessage = "Please write a comment.")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "Your comment must be between 10 and 500 characters.")]
    public string Comment { get; set; } = "";

    [Column("created_at", ignoreOnInsert: true, ignoreOnUpdate: true)]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }
}