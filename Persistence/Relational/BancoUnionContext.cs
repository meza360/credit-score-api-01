using Domain.Relational.BancoUnion;
using Infra;
using Microsoft.EntityFrameworkCore;
namespace Persistence.Relational;

public class BancoUnionContext : DbContext
{
    public BancoUnionContext(DbContextOptions<BancoUnionContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Domain.Relational.BancoUnion.Customer>(customer =>
        {
            customer.ToTable("tbl_customer");
            customer.HasKey(u => u.Id);
            customer.Property(u => u.Id).ValueGeneratedOnAdd();
            customer.Property(u => u.Id).HasColumnName("id");
            customer.Property(u => u.Cui).HasColumnName("cui").HasMaxLength(13).IsRequired();
            customer.Property(u => u.FirstName).HasColumnName("first_name").HasMaxLength(100).IsRequired();
            customer.Property(u => u.LastName).HasColumnName("last_name").HasMaxLength(100).IsRequired();
            customer.Property(u => u.DateOfBirth).HasColumnName("date_of_birth").IsRequired();
        });

        modelBuilder.Entity<Domain.Relational.BancoUnion.Customer>()
        .HasMany(customer => customer.Loans)
        .WithOne(loan => loan.Customer)
        .HasForeignKey(loan => loan.CustomerId);

        modelBuilder.Entity<Domain.Relational.BancoUnion.Customer>()
        .HasMany(customer => customer.Credits)
        .WithOne(credit => credit.Customer)
        .HasForeignKey(credit => credit.CustomerId);

        modelBuilder.Entity<Domain.Relational.BancoUnion.Payment>(payment =>
        {
            payment.ToTable("tbl_payment");
            payment.HasKey(p => p.Id);
            payment.Property(p => p.Id).ValueGeneratedOnAdd();
            payment.Property(p => p.Id).HasColumnName("id");
            payment.Property(p => p.PaymentAmount).HasColumnName("payment_amount").IsRequired();
            payment.Property(p => p.PaymentDate).HasColumnName("payment_date").IsRequired();
            payment.Property(p => p.InstallmentId).HasColumnName("installment_id");
            payment.Property(p => p.StatementId).HasColumnName("statement_id");
        });

        modelBuilder.Entity<Domain.Relational.BancoUnion.Payment>()
        .HasOne(payment => payment.Installment)
        .WithMany(installment => installment.Payments)
        .HasForeignKey(payment => payment.InstallmentId);

        modelBuilder.Entity<Domain.Relational.BancoUnion.Payment>()
        .HasOne(payment => payment.Statement)
        .WithMany(statement => statement.Payments)
        .HasForeignKey(payment => payment.StatementId);

        modelBuilder.Entity<Domain.Relational.BancoUnion.Installment>(installment =>
        {
            installment.ToTable("tbl_installment");
            installment.HasKey(i => i.Id);
            installment.Property(i => i.Id).ValueGeneratedOnAdd();
            installment.Property(i => i.Id).HasColumnName("id");
            installment.Property(i => i.DueDate).HasColumnName("due_date").IsRequired();
            installment.Property(i => i.InstallmentOverdue).HasColumnName("installment_overdue");
            installment.Property(i => i.DaysOverdue).HasColumnName("days_overdue");
            installment.Property(i => i.Amount).HasColumnName("amount").IsRequired();
            installment.Property(i => i.LoanId).HasColumnName("loan_id").IsRequired();
        });

        modelBuilder.Entity<Domain.Relational.BancoUnion.Loan>(loan =>
        {
            loan.ToTable("tbl_loan");
            loan.HasKey(l => l.Id);
            loan.Property(l => l.Id).ValueGeneratedOnAdd();
            loan.Property(l => l.Id).HasColumnName("id");
            loan.Property(l => l.InstallmentsQuantity).HasColumnName("installments").IsRequired();
            loan.Property(l => l.LoanType).HasColumnName("loan_type").IsRequired();
            loan.Property(l => l.IssueDate).HasColumnName("issue_date").IsRequired();
            loan.Property(l => l.TotalValue).HasColumnName("total_value").IsRequired();
            loan.Property(l => l.CustomerId).HasColumnName("customer_id").IsRequired();
        });

        modelBuilder.Entity<Domain.Relational.BancoUnion.Loan>()
        .HasMany(loan => loan.Installments)
        .WithOne(installment => installment.Loan)
        .HasForeignKey(installment => installment.LoanId);

        modelBuilder.Entity<Domain.Relational.BancoUnion.Statement>(statement =>
        {
            statement.ToTable("tbl_statement");
            statement.HasKey(s => s.Id);
            statement.Property(s => s.Id).ValueGeneratedOnAdd();
            statement.Property(s => s.Id).HasColumnName("id");
            statement.Property(s => s.DueDate).HasColumnName("due_date").IsRequired();
            statement.Property(s => s.StatementOverdue).HasColumnName("statement_overdue");
            statement.Property(s => s.DaysOverdue).HasColumnName("days_overdue");
            statement.Property(s => s.StatementAmount).HasColumnName("statement_amount").IsRequired();
            statement.Property(s => s.StatementMonth).HasColumnName("statement_month").IsRequired();
            statement.Property(s => s.StatementYear).HasColumnName("statement_year").IsRequired();
            statement.Property(s => s.CreditId).HasColumnName("credit_id").IsRequired();
        });

        modelBuilder.Entity<Domain.Relational.BancoUnion.Credit>(credit =>
        {
            credit.ToTable("tbl_credit");
            credit.HasKey(c => c.Id);
            credit.Property(c => c.Id).ValueGeneratedOnAdd();
            credit.Property(c => c.Id).HasColumnName("id");
            credit.Property(c => c.MaxAllowed).HasColumnName("max_allowed").IsRequired();
            credit.Property(c => c.CustomerId).HasColumnName("customer_id").IsRequired();
        });

        modelBuilder.Entity<Domain.Relational.BancoUnion.Credit>()
        .HasMany(credit => credit.Statements)
        .WithOne(statement => statement.Credit)
        .HasForeignKey(statement => statement.CreditId);
    }

    public DbSet<Domain.Relational.BancoUnion.Customer> Customers { get; set; }
    public DbSet<Domain.Relational.BancoUnion.Loan> Loans { get; set; }
    public DbSet<Domain.Relational.BancoUnion.Installment> Installments { get; set; }
    public DbSet<Domain.Relational.BancoUnion.Payment> Payments { get; set; }
    public DbSet<Domain.Relational.BancoUnion.Statement> Statements { get; set; }
    public DbSet<Domain.Relational.BancoUnion.Credit> Credits { get; set; }

}