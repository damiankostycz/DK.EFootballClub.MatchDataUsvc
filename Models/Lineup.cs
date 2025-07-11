using MongoDB.Bson.Serialization.Attributes;

namespace DK.EFootballClub.MatchDataUsvc.Models;

public class Lineup
{
    
    [BsonElement("formation")]
    public required string Formation { get; set; }

    [BsonElement("goalkeeper")]
    public int Goalkeeper { get; set; }

    [BsonElement("defenders")]
    public required List<int> Defenders { get; set; }

    [BsonElement("midfielders")]
    public required List<int> Midfielders { get; set; }

    [BsonElement("strikers")]
    public required List<int> Strikers { get; set; }

    [BsonElement("substitutes")]
    public required List<int> Substitutes { get; set; }

}