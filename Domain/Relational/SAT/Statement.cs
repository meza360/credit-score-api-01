namespace Domain.Relational.SAT
{
    public class Statement
    {
        public int StatementId { get; set; }
        public string StatementType { get; set; }
        public DateTime IssueDate { get; set; }
        public int StatementMonth { get; set; }
        public int StatementYear { get; set; }
        public float StatementAmount { get; set; }
        public bool StatementOverdue { get; set; }
        public Contributor Contributor { get; set; }
        public int ContributorId { get; set; }
        public Regime Regime { get; set; }
        public int RegimeId { get; set; }
        //public Payment Payment { get; set; }
    }
}