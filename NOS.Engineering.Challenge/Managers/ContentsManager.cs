using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using NOS.Engineering.Challenge.Database;
using NOS.Engineering.Challenge.Models;
using NOS.Engineering.Challenge.Services;
using Serilog;
using static NOS.Engineering.Challenge.Utils.Enums;

namespace NOS.Engineering.Challenge.Managers;

public class ContentsManager : IContentsManager
{
   
    private readonly IDatabase<Content?, ContentDto> _database;
    private readonly IMongoDatabase<Content?, ContentDto>? _mongoDbDatabase;
    private readonly AppEnviroment _enviroment;
    private readonly IRedisCacheService _redisCacheService;
    private readonly ILogger<ContentsManager> _logger;
    private readonly object _lock = new();

    public ContentsManager(IDatabase<Content?, ContentDto> database, IMongoDatabase<Content?, ContentDto> mongoDbDatabase, IRedisCacheService redisCacheService, ILogger<ContentsManager> logger)
    {
        _database = database;
        _mongoDbDatabase = mongoDbDatabase;
        _enviroment = GetAppEnviroment();
        _redisCacheService = redisCacheService;
        _logger = logger;
    }

    public Task<IEnumerable<Content?>> GetManyContents()
    {
        IEnumerable<Content?> contentMembers;
        if (_enviroment == AppEnviroment.Production)
        {
            var filters = FilterDefinition<Content>.Empty;
            contentMembers = _mongoDbDatabase!.ReadAll(filters!).ConfigureAwait(false).GetAwaiter().GetResult();
        }
        else
        {
            var key = "Contents";
            var cachedMember = _redisCacheService.GetCache<IEnumerable<Content?>>(key).ConfigureAwait(false).GetAwaiter().GetResult();
            if (cachedMember == default)
            {
                contentMembers = _database.ReadAll().ConfigureAwait(false).GetAwaiter().GetResult();
                _redisCacheService.SetCache(key, contentMembers);
            }
            else
            {
                contentMembers = cachedMember;
            }            
        }

        Log.Information("All Contents => {@result}", contentMembers);
        return Task.FromResult(contentMembers!)!;
    }

    public Task<IEnumerable<Content?>> GetManyContents(string Title, string[] Genres)
    {
        IEnumerable<Content?> contentMembers;
        if (_enviroment == AppEnviroment.Production)
        {
            var filters = Builders<Content>.Filter.Empty;

            //corrigir filtros
            if (string.IsNullOrEmpty(Title))
            {
                var nomeRegex = Builders<Content>.Filter.Regex(x => x.Title, "^" + Title + "$");
                filters |= nomeRegex;
            }           

            if (Genres?.Length > 0)
            {
                var generosFilter = Builders<Content>.Filter.AnyIn(x => x.GenreList, Genres);
                filters |= generosFilter;
            }

            contentMembers = _mongoDbDatabase!.ReadAll(filters!).ConfigureAwait(false).GetAwaiter().GetResult();
        }
        else
        {
            var key = "Contents";
            var cachedMember = _redisCacheService.GetCache<IEnumerable<Content?>>(key).ConfigureAwait(false).GetAwaiter().GetResult();
            if (cachedMember == default)
            {
                contentMembers = _database.ReadAll().ConfigureAwait(false).GetAwaiter().GetResult();
                _redisCacheService.SetCache(key, contentMembers);
            }
            else
            {
                contentMembers = cachedMember;
            }
        }

        Log.Information("All Contents => {@result}", contentMembers);
        return Task.FromResult(contentMembers!)!;
    }

    public Task<Content?> CreateContent(ContentDto content)
    {
        Content? ContentMember;
        if (_enviroment == AppEnviroment.Production)
        {
            ContentMember= _mongoDbDatabase!.Create(content).ConfigureAwait(false).GetAwaiter().GetResult();
        }
        else
        {
            ContentMember = _database.Create(content).ConfigureAwait(false).GetAwaiter().GetResult();
        }
        Log.Information("Content Created => {@result}", ContentMember);
        return Task.FromResult(ContentMember!)!;
    }

    public Task<Content?> GetContent(Guid id)
    {
        Content? ContentMember;

        if (_enviroment == AppEnviroment.Production)
        {
            var filters = Builders<Content>.Filter.Eq(x => x.Id, id);
            ContentMember = _mongoDbDatabase!.Read(filters!).ConfigureAwait(false).GetAwaiter().GetResult();
        }
        else
        {
            var key = $"Content-{id}";
            var cachedMember = _redisCacheService.GetCache<Content?>(key).ConfigureAwait(false).GetAwaiter().GetResult();

            if (cachedMember == default)
            {
                ContentMember = _database.Read(id).ConfigureAwait(false).GetAwaiter().GetResult();
                _redisCacheService.SetCache(key, ContentMember);

            }
            else
            {
                ContentMember = cachedMember;
            }
        }

        Log.Information("Content Get => {@result}", ContentMember);
        return Task.FromResult(ContentMember!)!;
    }

    public Task<Content?> UpdateContent(Guid id, ContentDto content)
    {
        Content? ContentMember;

        if (_enviroment == AppEnviroment.Production)
        {
            var filters = Builders<Content>.Filter.Eq(x => x.Id, id);
            ContentMember =  _mongoDbDatabase!.Update(content, filters!).ConfigureAwait(false).GetAwaiter().GetResult();
        }
        else
        {
            var key = $"Content-{id}";
            ContentMember = _database.Update(id, content).ConfigureAwait(false).GetAwaiter().GetResult();
            var cachedMember = _redisCacheService.GetCache<Content?>(key).ConfigureAwait(false).GetAwaiter().GetResult();

            if (cachedMember != default && ContentMember != default)
            {
                _redisCacheService.SetCache(key, ContentMember);
            }

            key = "Contents";
            IEnumerable<Content?> contentAllMembers;
            lock (_lock)
            {
                contentAllMembers = _redisCacheService.GetCache<IEnumerable<Content?>>(key).ConfigureAwait(false).GetAwaiter().GetResult();
                if (contentAllMembers != default)
                {
                    contentAllMembers = contentAllMembers.Where(x => x!.Id != id);
                    _redisCacheService.SetCache(key, contentAllMembers);
                }

            }

        }

        Log.Information("Content Updated => {@result}", ContentMember);
        return Task.FromResult(ContentMember!)!;
    }

    public Task<Guid> DeleteContent(Guid id)
    {
        Guid DeletedMember;
        if (_enviroment == AppEnviroment.Production)
        {
            var filters = Builders<Content>.Filter.Eq(x => x.Id, id);
            if (_mongoDbDatabase!.Delete(filters!).GetAwaiter().GetResult())
                DeletedMember = id;
            else
                DeletedMember = Guid.Empty;
        }
        else
        {
            var deletedId = _database.Delete(id).GetAwaiter().GetResult();
            if (deletedId != Guid.Empty)
            {
                var key = $"Content-{id}";
                _redisCacheService.DeleteCache(key);
                key = "Contents";
                IEnumerable<Content?> contentAllMembers;
                lock (_lock)
                {
                    contentAllMembers = _redisCacheService.GetCache<IEnumerable<Content?>>(key).ConfigureAwait(false).GetAwaiter().GetResult();
                    if (contentAllMembers != default)
                    {
                        contentAllMembers = contentAllMembers.Where(x => x!.Id != deletedId);
                        _redisCacheService.SetCache(key, contentAllMembers);
                    }                    
                }
               
            }

            DeletedMember = deletedId;
        }

        Log.Information("Content Deleted => {@result}", DeletedMember);
        return Task.FromResult(DeletedMember!)!;
    }

    public async Task<Content?> AddGenres(Guid id, IEnumerable<string> genre)
    {
        Content? ContentMember;

        var filters = Builders<Content>.Filter.Eq(x => x.Id, id);
        ContentMember = await _mongoDbDatabase!.Read(filters!);

        if (ContentMember != null)
        {
            var newList = new List<string>(genre.Concat(ContentMember.GenreList).Distinct());
            ContentMember.GenreList = new List<string>(newList);
            await _mongoDbDatabase.Update(ContentMember, filters!);
        }

        Log.Information("Content Genres Added => {@result}", ContentMember);
        return ContentMember;
    }

    public async Task<Content?> RemoveGenres(Guid id, IEnumerable<string> genres)
    {
        Content? ContentMember;
        var filters = Builders<Content>.Filter.Eq(x => x.Id, id);
        ContentMember = await _mongoDbDatabase!.Read(filters!);

        if (ContentMember != null)
        {
            List<string> newList = ContentMember.GenreList.ToList();
            foreach (var genre in genres)
            {
                newList.Remove(genre);
            }
            ContentMember.GenreList = new List<string>(newList);
            await _mongoDbDatabase.Update(ContentMember, filters!);
        }

        Log.Information("Content Genres Removed => {@result}", ContentMember);
        return ContentMember;
    }


}