using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOS.Engineering.Challenge.Database
{
    public  interface IMongoDbDatabase<TOut, in TIn>
    {
        IMongoDatabase? Database { get; }
        Task<TOut?> Create(TIn item);
    }
}
