namespace Domain.Relational.BancoUnion
{
    public class Customer : Domain.Relational.Banco.Customer
    {
        public List<Domain.Relational.BancoUnion.Loan>? Loans { get; set; }
        public List<Domain.Relational.BancoUnion.Credit>? Credits { get; set; }
    }
}
