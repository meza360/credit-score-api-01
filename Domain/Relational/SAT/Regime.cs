namespace Domain.Relational.SAT
{
    public class Regime
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Statement> Statements { get; set; }
        public List<Contributor> Contributors { get; set; }
    }
}