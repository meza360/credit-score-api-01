namespace Domain.Relational.EEGSA
{
    public class Bill
    {
        public int Id { get; set; }
        public string BillType { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public bool BillOverdue { get; set; }
        public int DaysOverdue { get; set; }
        public Contract Contract { get; set; }
        public int ContractId { get; set; }
        public double BillAmount { get; set; }
        public List<Payment>? Payments { get; set; }
        //public int Payment { get; set; }
    }
}