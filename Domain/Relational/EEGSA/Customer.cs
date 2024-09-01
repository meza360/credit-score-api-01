namespace Domain.Relational.EEGSA
{
    public class Customer
    {
        public int Id { get; set; }
        public string CUI { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public List<Contract> Contracts { get; set; }
    }
}