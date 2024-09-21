using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.NoSQL.SAT
{
    public class ImpositionETL
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public DateTime PaymentDate { get; set; }
        public double PaymentAmount { get; set; }
        public int ContributorId { get; set; }
        public Contributor Contributor { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
    }
}