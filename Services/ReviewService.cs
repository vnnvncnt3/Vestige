using Supabase;
using VestigeApp.Models;
using static Supabase.Postgrest.Constants;

namespace VestigeApp.Services;

/// <summary>Review store backed by the Supabase "reviews" table.</summary>
public class ReviewService
{
    private readonly Client _db;

    public ReviewService(Client db) => _db = db;

    /// <summary>Newest first, skips soft-deleted rows, first 50 only (filtering happens in the database).</summary>
    public async Task<List<Review>> GetAllAsync()
    {
        var res = await _db.From<Review>()
            .Where(r => r.DeletedAt == null)
            .Order(r => r.CreatedAt, Ordering.Descending)
            .Range(0, 49)
            .Get();
        return res.Models;
    }

    public async Task AddAsync(Review review)
        => await _db.From<Review>().Insert(review);

    public async Task UpdateAsync(long id, int rating, string comment)
        => await _db.From<Review>()
            .Where(r => r.Id == id)
            .Set(r => r.Rating, rating)
            .Set(r => r.Comment, comment)
            .Update();

    /// <summary>Soft delete: only fills in deleted_at, the row stays in the table.</summary>
    public async Task SoftDeleteAsync(long id)
        => await _db.From<Review>()
            .Where(r => r.Id == id)
            .Set(r => r.DeletedAt!, DateTime.UtcNow)
            .Update();
}