using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.NoSQL.Bank
{
    public class BancoUnionCustomerETL
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }
        public string FullName { get; set; }
        public string Cui { get; set; }
        public DateTime LastUpdate { get; set; }
        public string? BankScore { get; set; }
        public double AccumulatedDebt { get; set; }
        public List<CreditHistoricalRecordETL>? CreditHistoricalRecord { get; set; }
        public List<LoanHistoricalRecordETL>? LoanHistoricalRecord { get; set; }
    }
}