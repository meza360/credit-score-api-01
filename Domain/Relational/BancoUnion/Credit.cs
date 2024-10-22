namespace Domain.Relational.BancoUnion
{
    public class Credit : Domain.Relational.Banco.Credit
    {
        public Domain.Relational.BancoUnion.Customer Customer { get; set; }
        public int CustomerId { get; set; }
        public List<Domain.Relational.BancoUnion.Statement> Statements { get; set; }
    }
}