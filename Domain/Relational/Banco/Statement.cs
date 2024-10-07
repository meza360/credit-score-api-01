using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Relational.Banco
{
    public class Statement
    {
        public int Id { get; set; }
        public DateTime DueDate { get; set; }
        public bool StatementOverdue { get; set; }
        public int DaysOverdue { get; set; }
        public double StatementAmount { get; set; }
        public int StatementMonth { get; set; }
        public int StatementYear { get; set; }
    }
}