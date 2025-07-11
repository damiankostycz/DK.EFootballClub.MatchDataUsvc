using System.Net;
using System.Text.Json;
using DK.EFootballClub.MatchDataUsvc.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace DK.EFootballClub.MatchDataUsvc;

public class MatchDataHttpTrigger(ILoggerFactory loggerFactory)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<MatchDataHttpTrigger>();
    private readonly string? _dbConnectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");
    private readonly string? _dbName = Environment.GetEnvironmentVariable("DATABASE_NAME");

   [Function("GetAllMatches")]
    public async Task<HttpResponseData> GetAllMatches(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "matches")] HttpRequestData req)
    {
        var response = req.CreateResponse();
        try
        {
            var db = new MongoDbService(_dbConnectionString, _dbName);
            List<Match> matches  = await db.GetAllMatchesAsync();
            await response.WriteAsJsonAsync(matches);
            response.Headers.Add("Location", $"/api/match");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Matches");
            response.StatusCode = HttpStatusCode.InternalServerError;
            await response.WriteStringAsync("Internal server error");
            return response;
        }
    }

    [Function("CreateMatch")]
    public async Task<HttpResponseData> CreateMatch(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "matches")] HttpRequestData req)
    {
        var response = req.CreateResponse();

        try
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var match = JsonSerializer.Deserialize<Match>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (match == null)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync("Invalid Match data.");
                return response;
            }

            var db = new MongoDbService(_dbConnectionString, _dbName);
            Match? createdMatch = await db.CreateMatchAsync(match);

            response.StatusCode = HttpStatusCode.Created;
            response.Headers.Add("Location", $"/api/match/{createdMatch!.Id}");
            await response.WriteAsJsonAsync(createdMatch);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating match");
            response.StatusCode = HttpStatusCode.InternalServerError;
            await response.WriteStringAsync("Internal server error");
            return response;
        }
    }

    [Function("UpdateMatch")]
    public async Task<HttpResponseData> UpdateMatchr(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "matches/{id}")] HttpRequestData req,
        string id)
    {
        var response = req.CreateResponse();

        try
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var updatedMatchData = JsonSerializer.Deserialize<Match>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (updatedMatchData == null)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync("Invalid match data.");
                return response;
            }

            var db = new MongoDbService(_dbConnectionString, _dbName);
            var updatedMatch = await db.UpdateMatchAsync(id, updatedMatchData);

            if (updatedMatch == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                await response.WriteStringAsync($"Match with ID {id} not found.");
                return response;
            }

            response.StatusCode = HttpStatusCode.OK;
            response.Headers.Add("Location", $"/api/match/{updatedMatch.Id}");
            await response.WriteAsJsonAsync(updatedMatch);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating match with ID {id}");
            response.StatusCode = HttpStatusCode.InternalServerError;
            await response.WriteStringAsync("Internal server error");
            return response;
        }
    }

    [Function("DeleteMatch")]
    public async Task<HttpResponseData> DeleteMatch(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "matches/{id}")] HttpRequestData req,
        string id)
    {
        var response = req.CreateResponse();

        try
        {
            var db = new MongoDbService(_dbConnectionString, _dbName);
            var success = await db.DeleteMatchAsync(id);

            if (!success)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                await response.WriteStringAsync($"Match with ID {id} not found.");
                return response;
            }

            response.StatusCode = HttpStatusCode.NoContent;
            response.Headers.Add("Location", $"/api/match");

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting match with ID {id}");
            response.StatusCode = HttpStatusCode.InternalServerError;
            await response.WriteStringAsync("Internal server error");
            return response;
        }
    }
    
}