using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Relational.Banco
{
    public class Loan
    {
        public int Id { get; set; }
        public double TotalValue { get; set; }
        public int InstallmentsQuantity { get; set; }
        public string? LoanType { get; set; }
        public DateTime IssueDate { get; set; }
    }
}