using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.NoSQL.Private
{
    public class Customer
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string FullName { get; set; }
        public string Cui { get; set; }
        public DateTime LastUpdate { get; set; }
        public string? PrivateScore { get; set; }
        public double AccumulatedDebt { get; set; }
        public List<HistoricalRecord>? HistoricalRecord { get; set; }
    }
}