using System.ComponentModel.DataAnnotations;

namespace VestigeApp.Models;

public class Review
{
    public Guid Id { get; init; } = Guid.NewGuid();

    [Required(ErrorMessage = "Please enter your name.")]
    [StringLength(60, ErrorMessage = "Your name must be 60 characters or fewer.")]
    public string Author { get; set; } = "";

    [Required(ErrorMessage = "Please choose a tour.")]
    public string Tour { get; set; } = "";

    [Range(1, 5, ErrorMessage = "Please select a star rating.")]
    public int Rating { get; set; }

    [Required(ErrorMessage = "Please write a comment.")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "Your comment must be between 10 and 500 characters.")]
    public string Comment { get; set; } = "";

    public DateTime CreatedAt { get; init; } = DateTime.Now;
}
