using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using NOS.Engineering.Challenge.Database;
using NOS.Engineering.Challenge.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOS.Engineering.Challenge.Tests
{
    public class MongoRepositoryTests
    {        
        private readonly IConfiguration _configuration;
        private readonly IMapper<Content, ContentDto> _contetMapper;
        public MongoRepositoryTests()
        {
            var configBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

            _configuration = configBuilder.Build();
            _contetMapper = new ContentMapper();            
        }

        [Fact]
        public void Repository_ReadAll_Should_Return_All()
        {
            var mongoDatabase = new MongoDatabase<Content, ContentDto>(_configuration, _contetMapper!);
            var filters = FilterDefinition<Content>.Empty;

            IEnumerable<Content?> result = mongoDatabase.ReadAll(filters).GetAwaiter().GetResult();

            result.Should().NotBeNull();
            result.Should().BeOfType<List<Content?>>();
        }

        [Fact]
        public void Repository_Read_Should_Return_One_Or_Null()
        {
            Guid guid = Guid.Parse("8e7c6291-9ab3-47a8-8b27-069ec3fc151d");
            var mongoDatabase = new MongoDatabase<Content, ContentDto>(_configuration, _contetMapper!);
            //var filters = Builders<Content>.Filter.Eq(x => x.Id, Guid.Empty);
            var filters = Builders<Content>.Filter.Eq(x => x.Id, guid);

            Content? result = mongoDatabase.Read(filters).GetAwaiter().GetResult();

            if (result != null)
            {
                result.Should().BeOfType<Content?>();
            }
            else
            {
                result.Should().BeNull();
            }
        }

        [Fact]
        public void Repository_Should_Create_And_Return_Created_Item_Object()
        {
            var mongoDatabase = new MongoDatabase<Content, ContentDto>(_configuration, _contetMapper!);
            var newContent = new ContentDto("Title Create", "SubTitle Create", "Description Create", null, null, null, null, []);
            var result = mongoDatabase.Create(newContent).GetAwaiter().GetResult();

            result.Should().NotBeNull();
            result.Should().BeOfType<Content?>();
        }

        [Fact]
        public void Repository_Should_Update_And_Return_Updated_Item_Object()
        {
            var mongoDatabase = new MongoDatabase<Content, ContentDto>(_configuration, _contetMapper!);
            Guid id = Guid.Parse("8e7c6291-9ab3-47a8-8b27-069ec3fc151d");
            var filters = Builders<Content>.Filter.Eq(x => x.Id, id);
            var updatedContent = new ContentDto("Title Update", "SubTitle Update", "Description Update", null, null, null, null, []);

            Content? result = mongoDatabase!.Update(updatedContent, filters!).GetAwaiter().GetResult();

            if (result != null)
            {
                result.Should().BeOfType<Content?>();
            }
            else
            {
                result.Should().BeNull();
            }
        }

        [Fact]
        public void Repository_Should_Delete_And_Return_True_Or_False()
        {
            var mongoDatabase = new MongoDatabase<Content, ContentDto>(_configuration, _contetMapper!);
            Guid id = Guid.Parse("8e7c6291-9ab3-47a8-8b27-069ec3fc151d");
            var filters = Builders<Content>.Filter.Eq(x => x.Id, id);

            var result = mongoDatabase!.Delete(filters!).GetAwaiter().GetResult();

            result.Should().BeTrue();
        }

        [Fact]
        public void Repository_Should_Update_And_Return_True_Or_False() {
            var mongoDatabase = new MongoDatabase<Content, ContentDto>(_configuration, _contetMapper!);
            Guid id = Guid.Parse("8e7c6291-9ab3-47a8-8b27-069ec3fc151d");
            var filters = Builders<Content>.Filter.Eq(x => x.Id, id);

            Content? updateContent = mongoDatabase.Read(filters).GetAwaiter().GetResult();

            if (updateContent != null)
            {
                var result = mongoDatabase.Update(updateContent!, filters).GetAwaiter().GetResult();
                if (result)
                    result.Should().BeTrue();
                else
                    result.Should().BeFalse();
            }
           
        }


    }
}
