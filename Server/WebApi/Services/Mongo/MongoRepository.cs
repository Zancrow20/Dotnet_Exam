using Domain.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace WebApi.Services.Mongo;

public interface IRatingRepository
{
    Task<IEnumerable<Rating>> GetAllRatingsAsync(CancellationToken cancellationToken);
    Task<Rating?> GetRatingByIdAsync(string userId, CancellationToken cancellationToken);
    Task UpdateRatingAsync(string userId, int delta, CancellationToken cancellationToken);
    
    Task<string> InsertRatingAsync(Rating rating, CancellationToken cancellationToken);
}

public class RatingRepository : IRatingRepository
{
    private readonly IMongoCollection<Rating> _rating;

    public RatingRepository(IMongoCollection<Rating> rating)
    {
        _rating = rating;
    }
    
    public async Task<IEnumerable<Rating>> GetAllRatingsAsync(CancellationToken cancellationToken)
    {
        return await _rating.Find(x => true)
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<Rating?> GetRatingByIdAsync(string userId, CancellationToken cancellationToken)
    {
        var filter = Builders<Rating>.Filter.Eq(r => r.UserId, userId);
        return await (await _rating.FindAsync(filter, cancellationToken: cancellationToken))
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }

    public async Task UpdateRatingAsync(string userId, int delta, CancellationToken cancellationToken)
    {
        var filter = Builders<Rating>.Filter.Eq(r => r.UserId, userId);
        var rating = await GetRatingByIdAsync(userId, cancellationToken: cancellationToken);
        var newRating = new Rating() { Id = rating.Id, UserId = rating!.UserId, Score = rating.Score + delta };
        await _rating.ReplaceOneAsync(filter, newRating, cancellationToken: cancellationToken);

    }

    public async Task<string> InsertRatingAsync(Rating rating, CancellationToken cancellationToken)
    {
        rating.Id = ObjectId.GenerateNewId().ToString();
        await _rating.InsertOneAsync(rating, cancellationToken: cancellationToken);
        return rating.UserId;
    }
}