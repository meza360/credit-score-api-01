namespace Domain.Relational.EEGSA
{
    public class Contract
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public Customer Customer { get; set; }
        public int CustomerId { get; set; }
        public List<Bill>? Bills { get; set; }
    }
}