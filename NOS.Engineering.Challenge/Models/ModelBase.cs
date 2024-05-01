using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOS.Engineering.Challenge.Models
{
    public class ModelBase
    {
        [BsonId]
        [BsonElement("_id")]
        public Guid Id { get; set; }
    }
}
