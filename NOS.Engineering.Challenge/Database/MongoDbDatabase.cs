using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOS.Engineering.Challenge.Database
{
    public class MongoDbDatabase<TOut, TIn> : IMongoDbDatabase<TOut, TIn>
    {
        private readonly IConfiguration _configuration;
        private readonly IMongoDatabase _database;
        private readonly IMapper<TOut?, TIn> _mapper;
        private IMongoCollection<TOut> _colletion;
        public MongoDbDatabase(IConfiguration configuration, IMapper<TOut?, TIn> mapper)
        {
            _configuration = configuration;
            var connectionString = _configuration.GetConnectionString("DbConnectionMongo");
            var mongoUrl = MongoUrl.Create(connectionString);
            var mongoClient = new MongoClient(mongoUrl);
            _database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
            _mapper = mapper;
            _colletion = _database.GetCollection<TOut>(typeof(TOut).Name);
        }

        public async Task<TOut?> Create(TIn item)
        {
            var id = Guid.NewGuid();
            var createdItem = _mapper.Map(id, item);
            //There is no need to worry about id clashes for this exercise.
            await _colletion.InsertOneAsync(createdItem);
            return createdItem;
        }

        public IMongoDatabase? Database => _database;
    }
}
