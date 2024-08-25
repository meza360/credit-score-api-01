namespace Domain.Relational.Renap
{
    public class Citizen
    {
        public int Id { get; set; }
        public string CUI { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime? DateOfDecease { get; set; }
        public string Nationality { get; set; }
    }
}