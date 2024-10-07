using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Relational.BancoUnion
{
    public class Installment : Domain.Relational.Banco.Installment
    {
        public Domain.Relational.BancoUnion.Loan Loan { get; set; }
        public int LoanId { get; set; }
        public List<Domain.Relational.BancoUnion.Payment>? Payments { get; set; }
    }
}