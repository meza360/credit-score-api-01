namespace Domain.Relational.SAT
{
    public class Contributor
    {
        public int Id { get; set; }
        public string Cui { get; set; }
        public string Nit { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Nationality { get; set; } = "Guatemalteco/a";
        public List<Statement> Statements { get; set; }
        public Regime Regime { get; set; }
        public int RegimeId { get; set; }
        public List<Imposition> Impositions { get; set; }
    }
}

