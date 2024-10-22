using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.NoSQL.Report
{
    public class EntityReport
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string FullName { get; set; }
        public string Cui { get; set; }
        public string Nit { get; set; }
        public string PrivateOverallScore { get; set; }
        public string BankOverallScore { get; set; }
        public string TaxOverallScore { get; set; }
        public double MonthlyOutcome { get; set; }
        public double AccumulatedDebt { get; set; }
        public List<TaxOverall>? TaxOverallHistory { get; set; }
        public List<PurchaseOverall>? PurchaseOverallHistory { get; set; }
        public List<BankPaymentOverall>? BankPaymentOverallHistory { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Nationality { get; set; }
    }
}