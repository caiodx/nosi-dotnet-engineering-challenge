using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOS.Engineering.Challenge.Database
{
    public  interface IMongoDatabase<TOut, in TIn>
    {
        Task<TOut?> Create(TIn item);
        Task<TOut?> Read(FilterDefinition<TOut> filters);
        Task<IEnumerable<TOut?>> ReadAll();
        Task<TOut?> Update(TIn item, FilterDefinition<TOut> filters);
        Task<bool> Delete(FilterDefinition<TOut> filters);
        Task<bool> Update(TOut item, FilterDefinition<TOut> filters);

    }
}
