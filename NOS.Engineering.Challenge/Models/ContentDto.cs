namespace NOS.Engineering.Challenge.Models;

public class ContentDto
{
    public ContentDto(
        string? title,
        string? subTitle,
        string? description,
        string? imageUrl,
        int? duration,
        DateTime? startTime,
        DateTime? endTime,
        IEnumerable<string> genreList)
    {
        Title = title;
        SubTitle = subTitle;
        Description = description;
        ImageUrl = imageUrl;
        Duration = duration;
        StartTime = startTime;
        EndTime = endTime;
        GenreList = genreList;
    }

    public string? Title { get; set; }
    public string? SubTitle { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int? Duration { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public IEnumerable<string> GenreList { get; set; }
    
}