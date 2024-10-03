namespace Domain.Relational.SAT
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public DateTime PaymentDate { get; set; }
        public double PaymentAmount { get; set; }
        public Statement Statement { get; set; }
        public int StatementId { get; set; }
    }
}