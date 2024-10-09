using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.NoSQL.Report
{
    public class HistoryObject
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public double Total { get; set; }
    }
}