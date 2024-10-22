using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Relational.Banco
{
    public class Installment
    {
        public int Id { get; set; }
        public DateTime DueDate { get; set; }
        public bool? InstallmentOverdue { get; set; }
        public int? DaysOverdue { get; set; }
        public double Amount { get; set; }
    }
}