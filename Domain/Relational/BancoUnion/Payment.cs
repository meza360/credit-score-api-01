namespace Domain.Relational.BancoUnion
{
    public class Payment : Domain.Relational.Banco.Payment
    {
        public Domain.Relational.BancoUnion.Installment? Installment { get; set; }
        public int? InstallmentId { get; set; }
        public Domain.Relational.BancoUnion.Statement? Statement { get; set; }
        public int? StatementId { get; set; }
    }
}