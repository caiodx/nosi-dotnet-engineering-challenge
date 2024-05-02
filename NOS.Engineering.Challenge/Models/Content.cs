using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace NOS.Engineering.Challenge.Models;

public class Content : ModelBase
{    
    [BsonElement("title"), BsonRepresentation(BsonType.String)]
    public string? Title { get; }
    [BsonElement("sub_title"), BsonRepresentation(BsonType.String)]
    public string? SubTitle { get; }
    [BsonElement("description"), BsonRepresentation(BsonType.String)]
    public string? Description { get; }
    [BsonElement("image_url"), BsonRepresentation(BsonType.String)]
    public string? ImageUrl { get; }
    [BsonElement("duration"), BsonRepresentation(BsonType.Int32)]
    public int? Duration { get; }
    [BsonElement("start_time"), BsonRepresentation(BsonType.DateTime)]
    public DateTime? StartTime { get; }
    [BsonElement("end_time"), BsonRepresentation(BsonType.DateTime)]
    public DateTime? EndTime { get; }
    [BsonElement("genre_list")]
    public IEnumerable<string> GenreList { get; set; }


    public Content(Guid id, string? title, string? subTitle, string? description, string? imageUrl, int? duration, DateTime? startTime, DateTime? endTime, IEnumerable<string> genreList)
    {
        Id = id;
        Title = title;
        SubTitle = subTitle;
        Description = description;
        ImageUrl = imageUrl;
        Duration = duration;
        StartTime = startTime;
        EndTime = endTime;
        GenreList = genreList;
    }

    public void ClearGenreList()
    {
        GenreList = new List<string>().ToArray();
    }
}