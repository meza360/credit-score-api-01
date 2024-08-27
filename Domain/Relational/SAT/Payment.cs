namespace Domain.Relational.SAT
{
    public class Payment
    {
        public int Id { get; set; }
        public DateTime PaymentDate { get; set; }
        public double PaymentAmount { get; set; }
        public Statement Statement { get; set; }
    }
}