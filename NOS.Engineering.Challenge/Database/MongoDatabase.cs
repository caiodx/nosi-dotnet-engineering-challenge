using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using NOS.Engineering.Challenge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOS.Engineering.Challenge.Database
{
    public class MongoDatabase<TOut, TIn> : IMongoDatabase<TOut, TIn>
    {
        private readonly IConfiguration _configuration;
        private readonly IMongoDatabase _database;
        private readonly IMapper<TOut?, TIn> _mapper;
        private IMongoCollection<TOut> _colletion;
        public MongoDatabase(IConfiguration configuration, IMapper<TOut?, TIn> mapper)
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

        public async Task<TOut?> Read(FilterDefinition<TOut> filters) { 
            return await _colletion.Find(filters).FirstOrDefaultAsync();             
        }

        public async Task<IEnumerable<TOut?>> ReadAll()
        {
            return await _colletion.Find(FilterDefinition<TOut>.Empty).ToListAsync();
        }

        public async Task<TOut?> Update(TIn item, FilterDefinition<TOut> filters)
        {
            var dbItem = await _colletion.Find(filters).FirstOrDefaultAsync();
            if (dbItem == null)
            {
                return default; 
            }

            var updatedItem = _mapper.Patch(dbItem, item);
            await _colletion.ReplaceOneAsync(filters, updatedItem!);            

            return updatedItem;
        }

        public async Task<bool> Delete(FilterDefinition<TOut> filters)
        {
            var dbItem = await _colletion.Find(filters).FirstOrDefaultAsync();
            if (dbItem == null)
            {
                return false;
            }

           var deleteResult = await _colletion.DeleteOneAsync(filters);
           return deleteResult.DeletedCount > 0;
        }

        public async Task<bool> Update(TOut item, FilterDefinition<TOut> filters)
        {
            var updateResult = await _colletion.ReplaceOneAsync(filters, item!);
            return updateResult.ModifiedCount > 0;
        }
    }
}
