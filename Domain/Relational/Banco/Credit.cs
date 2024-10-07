using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Relational.Banco
{
    public class Credit
    {
        public int Id { get; set; }
        public double MaxAllowed { get; set; }
    }
}