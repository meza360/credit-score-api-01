using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Relational.SAT
{
    public class Imposition
    {
        public int Id { get; set; }
        public DateTime PaymentDate { get; set; }
        public double PaymentAmount { get; set; }
        public int ContributorId { get; set; }
        public Contributor Contributor { get; set; }
    }
}