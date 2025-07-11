using DK.EFootballClub.MatchDataUsvc.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DK.EFootballClub.MatchDataUsvc;

public class MongoDbService
{    private readonly IMongoCollection<Match> _matches;

    public MongoDbService(string? connectionString, string? databaseName)
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _matches = database.GetCollection<Match>("Matches");
    }

    public async Task<List<Match>> GetAllMatchesAsync()
    {
        return await _matches.Find(_ => true).ToListAsync();
    }

    private async Task<Match?> GetMatchByIdAsync(string id)
    {
        var filter = Builders<Match>.Filter.Eq("_id", ObjectId.Parse(id));
        return await _matches.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<Match?> CreateMatchAsync(Match match)
    {
        await _matches.InsertOneAsync(match);
        return match;
    }

    public async Task<Match?> UpdateMatchAsync(string id, Match updatedMatch)
    {
        var filter = Builders<Match>.Filter.Eq("_id", ObjectId.Parse(id));
        var result = await _matches.ReplaceOneAsync(filter, updatedMatch);

        if (result.ModifiedCount > 0)
        {
            return await GetMatchByIdAsync(id);
        }

        return null;
    }

    public async Task<bool> DeleteMatchAsync(string id)
    {
        var filter = Builders<Match>.Filter.Eq("_id", ObjectId.Parse(id));
        var result = await _matches.DeleteOneAsync(filter);
        return result.DeletedCount > 0;
    }
}