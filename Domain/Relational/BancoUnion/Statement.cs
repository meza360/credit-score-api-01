using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Relational.BancoUnion
{
    public class Statement : Domain.Relational.Banco.Statement
    {
        public Domain.Relational.BancoUnion.Credit Credit { get; set; }
        public int CreditId { get; set; }
        public List<Domain.Relational.BancoUnion.Payment>? Payments { get; set; }
    }
}