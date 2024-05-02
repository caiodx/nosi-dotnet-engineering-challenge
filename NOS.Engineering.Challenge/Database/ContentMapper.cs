using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.Database;

public class ContentMapper : IMapper<Content, ContentDto>
{
    public Content Map(Guid id, ContentDto item)
    {
        return new Content(
            id,
            item.Title ,
            item.SubTitle ,
            item.Description ,
            item.ImageUrl ,
            item.Duration ,
            item.StartTime ,
            item.EndTime ,
            item.GenreList);
    }

    public Content Patch(Content oldItem, ContentDto newItem)
    {
        return new Content(
                oldItem.Id,
                newItem.Title ?? oldItem.Title,
                newItem.SubTitle ?? oldItem.SubTitle,
                newItem.Description ?? oldItem.Description,
                newItem.ImageUrl ?? oldItem.ImageUrl,
                newItem.Duration ?? oldItem.Duration,
                newItem.StartTime ?? oldItem.StartTime,
                newItem.EndTime ?? oldItem.EndTime,
                !newItem.GenreList.Any() ? oldItem.GenreList : newItem.GenreList);
    }
    
}