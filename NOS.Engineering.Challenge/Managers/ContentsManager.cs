using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using NOS.Engineering.Challenge.Database;
using NOS.Engineering.Challenge.Models;
using NOS.Engineering.Challenge.Services;

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
    private readonly IRedisCacheService _redisCacheService;

    public ContentsManager(IDatabase<Content?, ContentDto> database, IMongoDatabase<Content?, ContentDto> mongoDbDatabase, IRedisCacheService redisCacheService)
    {
        _database = database;
        _mongoDbDatabase = mongoDbDatabase;
        _enviroment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production" ? Enviroment.Production : Enviroment.Development;
        _redisCacheService = redisCacheService;
    }

    public Task<IEnumerable<Content?>> GetManyContents()
    {
        if (_enviroment == Enviroment.Production)
        {
            return _mongoDbDatabase!.ReadAll();
        }
        else
        {
            var key = "Contents";
            var cachedMember = _redisCacheService.GetCache<IEnumerable<Content?>>(key).ConfigureAwait(false).GetAwaiter().GetResult();
            if (cachedMember == default)
            {
                var newData = _database.ReadAll().ConfigureAwait(false).GetAwaiter().GetResult();
                _redisCacheService.SetCache(key, newData);
                return Task.FromResult(newData);
            }
            else
            {
                return Task.FromResult(cachedMember);
            }            
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
            var key = $"Content-{id}";
            var cachedMember = _redisCacheService.GetCache<Content?>(key).ConfigureAwait(false).GetAwaiter().GetResult();

            if (cachedMember == default)
            {
                var newData = _database.Read(id).ConfigureAwait(false).GetAwaiter().GetResult();
                _redisCacheService.SetCache(key, newData);
                return Task.FromResult(newData);
            }
            else
            {
                return Task.FromResult(cachedMember!)!;
            }
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
            var key = $"Content-{id}";
            var updatedMember = _database.Update(id, content).ConfigureAwait(false).GetAwaiter().GetResult();
            var cachedMember = _redisCacheService.GetCache<Content?>(key).ConfigureAwait(false).GetAwaiter().GetResult();

            if (cachedMember != default && updatedMember != default)
            {
                _redisCacheService.SetCache(key, updatedMember);
            }

            return Task.FromResult(updatedMember);
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
                return Task.FromResult(Guid.Empty);
        }
        else
        {
            var deletedId = _database.Delete(id).GetAwaiter().GetResult();
            if (deletedId != Guid.Empty)
            {
                var key = $"Content-{id}";
                _redisCacheService.DeleteCache(key);
            }
            
            return Task.FromResult(deletedId);
        }
       
    }

    public async Task<Content?> AddGenres(Guid id, IEnumerable<string> genre)
    {
        var filters = Builders<Content>.Filter.Eq(x => x.Id, id);
        var savedContent = await _mongoDbDatabase!.Read(filters!);

        if (savedContent != null)
        {
            var newList = new List<string>(genre.Concat(savedContent.GenreList).Distinct());
            savedContent.GenreList = new List<string>(newList);
            await _mongoDbDatabase.Update(savedContent, filters!);
        }

        return savedContent;
    }

    public async Task<Content?> DeleteGenres(Guid id, IEnumerable<string> genres)
    {
        var filters = Builders<Content>.Filter.Eq(x => x.Id, id);
        var savedContent = await _mongoDbDatabase!.Read(filters!);

        if (savedContent != null)
        {
            List<string> newList = savedContent.GenreList.ToList();
            foreach (var genre in genres)
            {
                newList.Remove(genre);
            }
            savedContent.GenreList = new List<string>(newList);
            await _mongoDbDatabase.Update(savedContent, filters!);
        }

        return savedContent;
    }


}