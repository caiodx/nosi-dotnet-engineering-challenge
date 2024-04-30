using MongoDB.Driver;
using NOS.Engineering.Challenge.Database;
using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.Managers;

public class ContentsManager : IContentsManager
{
    private readonly IDatabase<Content?, ContentDto> _database;
    private readonly IMongoDbDatabase<Content?, ContentDto>? _mongoDbDatabase;

    public ContentsManager(IDatabase<Content?, ContentDto> database, IMongoDbDatabase<Content?, ContentDto> mongoDbDatabase )
    {
        _database = database;
        _mongoDbDatabase = mongoDbDatabase;
    }

    public Task<IEnumerable<Content?>> GetManyContents()
    {
        return _database.ReadAll();
    }

    public Task<Content?> CreateContent(ContentDto content)
    {
        return _mongoDbDatabase.Create(content);
        //return _database.Create(content);
    }

    public Task<Content?> GetContent(Guid id)
    {
        return _database.Read(id);
    }

    public Task<Content?> UpdateContent(Guid id, ContentDto content)
    {
        return _database.Update(id, content);
    }

    public Task<Guid> DeleteContent(Guid id)
    {
        return _database.Delete(id);
    }

    public async Task<Content?> AddGenres(Guid id, IEnumerable<string> genre)
    {
        //var filters = Builders<Content>.Filter.Eq(x => x.Id, id);
        //var content = await _contents.Find(filters).FirstOrDefaultAsync();

        //if (content != null)
        //{
        //    content.GenreList.Concat(genre.Distinct().ToArray());
        //    var genreListDistinct= content.GenreList.Distinct().ToArray();
        //    content.ClearGenreList();
        //    content.GenreList.Concat(genreListDistinct);
        //    await _contents.ReplaceOneAsync(filters, content);
        //}

        //return content;

        return default(Content);
    }


}