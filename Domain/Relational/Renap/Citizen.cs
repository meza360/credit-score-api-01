namespace Domain.Relational.Renap
{
    public class Citizen
    {
        public int Id { get; set; }
        public string CUI { get; set; } = new Random().NextInt64(1000000000000, 9999999999999).ToString();
        public string FirstName { get; set; } = "John";
        public string LastName { get; set; } = "Doe";
        public DateTime DateOfBirth { get; set; } = DateTime.Now.AddYears(-10);
        public DateTime? DateOfDecease { get; set; }
        public string Nationality { get; set; } = "Guatemala";
    }
}