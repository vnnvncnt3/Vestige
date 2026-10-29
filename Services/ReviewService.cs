using VestigeApp.Models;

namespace VestigeApp.Services;

/// <summary>In-memory review store (front end only). The three seed reviews are demo data.</summary>
public class ReviewService
{
    private readonly List<Review> _reviews;
    private readonly object _lock = new();

    public ReviewService(TourService tours)
    {
        _reviews = new()
        {
            new() { Author = "Maria Santos", Tour = tours.All[0].Name, Rating = 5, CreatedAt = DateTime.Now.AddDays(-9),
                    Comment = "Our guide knew every corner and never rushed us. The stop at the old market was the highlight of our trip." },
            new() { Author = "Daniel Okafor", Tour = tours.All[1].Name, Rating = 5, CreatedAt = DateTime.Now.AddDays(-6),
                    Comment = "I have walked past these buildings for years and learned more in three hours than in all that time." },
            new() { Author = "Aiko Tanaka", Tour = tours.All[2].Name, Rating = 4, CreatedAt = DateTime.Now.AddDays(-2),
                    Comment = "Came hungry and left with a list of dishes to try at home. The family-run stalls were a treat." },
        };
    }

    public IReadOnlyList<Review> All
    {
        get { lock (_lock) return _reviews.OrderByDescending(r => r.CreatedAt).ToList(); }
    }

    public double Average
    {
        get { lock (_lock) return _reviews.Count == 0 ? 0 : _reviews.Average(r => r.Rating); }
    }

    /// <summary>Number of reviews for each star value, 1 to 5.</summary>
    public Dictionary<int, int> Distribution
    {
        get
        {
            lock (_lock)
                return Enumerable.Range(1, 5).ToDictionary(star => star, star => _reviews.Count(r => r.Rating == star));
        }
    }

    public void Add(Review review)
    {
        lock (_lock) _reviews.Add(review);
    }
}
