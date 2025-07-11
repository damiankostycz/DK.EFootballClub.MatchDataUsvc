using MongoDB.Bson.Serialization.Attributes;

namespace DK.EFootballClub.MatchDataUsvc.Models;

public class Event
{
    [BsonElement("minute")]
    public int Minute { get; set; }

    [BsonElement("type")]
    public string Type { get; set; } = string.Empty;

    [BsonElement("player")]
    public string Player { get; set; } = string.Empty;

    [BsonElement("team")]
    public string Team { get; set; } = string.Empty;
    
}