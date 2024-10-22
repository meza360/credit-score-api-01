using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.NoSQL.SAT
{
    public class Contributor
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Cui { get; set; }
        public string Nit { get; set; }
        public string FullName { get; set; }
        public DateTime LastUpdate { get; set; }
        public string TaxScore { get; set; }
        public double AccumulatedDebt { get; set; }
        public List<ImpositionETL> ImpositionHistoricalRecord { get; set; }
        public List<StatementETL> StatementHistoricalRecord { get; set; }
    }
}