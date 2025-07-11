using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DK.EFootballClub.MatchDataUsvc.Models;

public class Match
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("events")]
    public List<Event>? Events { get; set; }

    [BsonElement("date")]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime Date { get; set; }

    [BsonElement("home_team")]
    public required string HomeTeam { get; set; }

    [BsonElement("away_team")]
    public required string AwayTeam { get; set; }

    [BsonElement("lineup_team1")]
    public required Lineup LineupTeam1 { get; set; }

    [BsonElement("lineup_team2")]
    public required Lineup LineupTeam2 { get; set; }

    [BsonElement("result")]
    public required string Result { get; set; }
}
