namespace Domain.Relational.EEGSA
{
    public class Payment
    {
        public int Id { get; set; }
        public double PaymentAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        //public List<Bill> Bill { get; set; }
        public int BillId { get; set; }
        public Bill Bill { get; set; }
    }
}