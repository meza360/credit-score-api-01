namespace Domain.Relational.BancoUnion
{
    public class Loan : Domain.Relational.Banco.Loan
    {
        public Domain.Relational.BancoUnion.Customer Customer { get; set; }
        public int CustomerId { get; set; }
        public List<Domain.Relational.BancoUnion.Installment>? Installments { get; set; }
    }
}