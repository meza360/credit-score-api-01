using Microsoft.EntityFrameworkCore;

namespace Persistence.Relational;

public class SatContext : DbContext
{
    public SatContext(DbContextOptions<SatContext> options) : base(options)
    {

    }
    public DbSet<Domain.Relational.SAT.Contributor> Contributors { get; set; }
    public DbSet<Domain.Relational.SAT.Regime> Regimes { get; set; }
    public DbSet<Domain.Relational.SAT.Statement> Statements { get; set; }
    public DbSet<Domain.Relational.SAT.Payment> Payments { get; set; }
    public DbSet<Domain.Relational.SAT.Imposition> Impositions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Domain.Relational.SAT.Contributor>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.FirstName).HasColumnName("first_name").IsRequired();
            entity.Property(e => e.LastName).HasColumnName("last_name").IsRequired();
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth").IsRequired();
            entity.Property(e => e.CUI).HasColumnName("cui").IsRequired();
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.NIT).HasColumnName("nit").IsRequired();
            entity.Property(e => e.Nationality).HasColumnName("nationality").IsRequired();
            entity.Property(e => e.RegimeId).HasColumnName("regime_id").IsRequired();
        });

        modelBuilder.Entity<Domain.Relational.SAT.Contributor>()
            .ToTable("tbl_contributor")
            .HasMany(c => c.Statements)
            .WithOne(s => s.Contributor)
            .HasForeignKey(s => s.ContributorId);

        modelBuilder.Entity<Domain.Relational.SAT.Contributor>()
            .ToTable("tbl_contributor")
            .HasOne(c => c.Regime).WithMany(c => c.Contributors).HasForeignKey(c => c.RegimeId);


        modelBuilder.Entity<Domain.Relational.SAT.Statement>(entity =>
        {
            entity.Property(e => e.StatementId).HasColumnName("statement_id");
            entity.Property(e => e.StatementId).ValueGeneratedOnAdd();
            entity.Property(e => e.ContributorId).HasColumnName("contributor_id").IsRequired();
            entity.Property(e => e.RegimeId).HasColumnName("regime_id").IsRequired();
            entity.Property(e => e.StatementAmount).HasColumnName("statement_amount").IsRequired();
            entity.Property(e => e.IssueDate).HasColumnName("issue_date").IsRequired();
            entity.Property(e => e.StatementType).HasColumnName("statement_type").IsRequired();
            entity.Property(e => e.StatementMonth).HasColumnName("statement_month").IsRequired();
            entity.Property(e => e.StatementYear).HasColumnName("statement_year").IsRequired();
            entity.Property(e => e.StatementOverdue).HasColumnName("statement_overdue").IsRequired();
        });

        modelBuilder.Entity<Domain.Relational.SAT.Statement>()
            .ToTable("tbl_statement")
            .HasOne(s => s.Contributor)
            .WithMany(c => c.Statements)
            .HasForeignKey(s => s.ContributorId);

        modelBuilder.Entity<Domain.Relational.SAT.Statement>()
            .ToTable("tbl_statement")
            .HasOne(s => s.Regime)
            .WithMany(r => r.Statements)
            .HasForeignKey(s => s.RegimeId);

        modelBuilder.Entity<Domain.Relational.SAT.Payment>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.PaymentAmount).HasColumnName("payment_amount").IsRequired();
            entity.Property(e => e.PaymentDate).HasColumnName("payment_date").IsRequired();
        });

        modelBuilder.Entity<Domain.Relational.SAT.Payment>()
            .ToTable("tbl_payment")
            .HasOne(p => p.Statement);
        //.WithOne(s => s.Payment);
        //.WithMany(s => s.Payments)
        //.HasForeignKey(p => p.StatementId);

        modelBuilder.Entity<Domain.Relational.SAT.Imposition>(imposition =>
        {
            imposition.ToTable("tbl_imposition");
            imposition.Property(e => e.Id).HasColumnName("id");
            imposition.Property(e => e.Id).ValueGeneratedOnAdd();
            imposition.Property(e => e.PaymentAmount).HasColumnName("payment_amount").IsRequired();
            imposition.Property(e => e.PaymentDate).HasColumnName("payment_date").IsRequired();
            imposition.Property(e => e.ContributorId).HasColumnName("contributor_id").IsRequired();
        });

        modelBuilder.Entity<Domain.Relational.SAT.Imposition>()
            .HasOne(i => i.Contributor)
            .WithMany(c => c.Impositions)
            .HasForeignKey(i => i.ContributorId);
    }
}