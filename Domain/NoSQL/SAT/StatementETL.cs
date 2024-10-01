using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.NoSQL.SAT
{
    public class StatementETL
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public bool? WasDue { get; set; }
        public int? DaysOverdue { get; set; }
        public double StatementAmount { get; set; }
    }
}