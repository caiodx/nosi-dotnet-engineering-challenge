using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using NOS.Engineering.Challenge.Database;
using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.Managers;

public class ContentsManager : IContentsManager
{
    private enum Enviroment
    {
        Development,
        Production
    }
    private readonly IDatabase<Content?, ContentDto> _database;
    private readonly IMongoDatabase<Content?, ContentDto>? _mongoDbDatabase;
    private readonly Enviroment _enviroment;

    public ContentsManager(IDatabase<Content?, ContentDto> database, IMongoDatabase<Content?, ContentDto> mongoDbDatabase)
    {
        _database = database;
        _mongoDbDatabase = mongoDbDatabase;
        _enviroment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production" ? Enviroment.Production : Enviroment.Development;
    }

    public Task<IEnumerable<Content?>> GetManyContents()
    {
        if (_enviroment == Enviroment.Production)
        {
            return _mongoDbDatabase!.ReadAll();
        }
        else
        {
            return _database.ReadAll();
        }
       
    }

    public Task<Content?> CreateContent(ContentDto content)
    {
        if (_enviroment == Enviroment.Production)
        {
            return _mongoDbDatabase!.Create(content);
        }
        else
        {
            return _database.Create(content);
        }
    }

    public Task<Content?> GetContent(Guid id)
    {
        if (_enviroment == Enviroment.Production)
        {
            var filters = Builders<Content>.Filter.Eq(x => x.Id, id);
            return _mongoDbDatabase!.Read(filters!);
        }
        else
        {
            return _database.Read(id);
        }
        
    }

    public Task<Content?> UpdateContent(Guid id, ContentDto content)
    {
        if (_enviroment == Enviroment.Production)
        {
            var filters = Builders<Content>.Filter.Eq(x => x.Id, id);
            return _mongoDbDatabase!.Update(content, filters!);
        }
        else
        {
            return _database.Update(id, content);
        }
        
    }

    public Task<Guid> DeleteContent(Guid id)
    {
        if (_enviroment == Enviroment.Production)
        {
            var filters = Builders<Content>.Filter.Eq(x => x.Id, id);
            if (_mongoDbDatabase!.Delete(filters!).Result)
                return Task.FromResult(id);
            else 
                return Task.FromResult(Guid.Empty) ;
        }
        else
        {
            return _database.Delete(id);
        }
       
    }

    public async Task<Content?> AddGenres(Guid id, IEnumerable<string> genre)
    {
        var filters = Builders<Content>.Filter.Eq(x => x.Id, id);
        var savedContent = await _mongoDbDatabase!.Read(filters!);

        if (savedContent != null)
        {
            var newList = new List<string>(genre.Concat(savedContent.GenreList).Distinct());
            //savedContent.ClearGenreList();
            savedContent.GenreList = new List<string>(newList);
            await _mongoDbDatabase.Update(savedContent, filters!);
        }

        return savedContent;
    }


}