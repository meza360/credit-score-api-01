using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.NoSQL.Private
{
    public class HistoricalRecord
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public bool WasDue { get; set; }
        public int DaysDue { get; set; }
    }
}