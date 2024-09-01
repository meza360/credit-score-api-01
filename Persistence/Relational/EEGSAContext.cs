using Domain.Relational.EEGSA;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Relational
{
    public class EEGSAContext : DbContext
    {
        public EEGSAContext(DbContextOptions<EEGSAContext> options) : base(options)
        {
        }
        public DbSet<Domain.Relational.EEGSA.Customer> Customers { get; set; }
        public DbSet<Domain.Relational.EEGSA.Contract> Contracts { get; set; }
        public DbSet<Domain.Relational.EEGSA.Bill> Bills { get; set; }
        public DbSet<Domain.Relational.EEGSA.Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Domain.Relational.EEGSA.Customer>(entity =>
            {
                entity.ToTable("tbl_customer");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.CUI).HasColumnName("cui").IsRequired();
                entity.Property(e => e.FirstName).HasColumnName("first_name").IsRequired();
                entity.Property(e => e.LastName).HasColumnName("last_name").IsRequired();
                entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth").IsRequired();
            });

            modelBuilder.Entity<Domain.Relational.EEGSA.Customer>()
                .HasMany(customer => customer.Contracts)
                .WithOne(contract => contract.Customer)
                .HasForeignKey(contract => contract.CustomerId);

            modelBuilder.Entity<Domain.Relational.EEGSA.Contract>(entity =>
            {
                entity.ToTable("tbl_contract");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.IsActive).HasColumnName("is_active").IsRequired();
                entity.Property(e => e.CustomerId).HasColumnName("customer_id").IsRequired();
                //entity.HasOne(e => e.Customer).WithMany().HasForeignKey(e => e.CustomerId);
            });

            modelBuilder.Entity<Domain.Relational.EEGSA.Contract>();
            //.HasOne(contract => contract.Customer)
            //.OwnsMany(contract => contract.Bills)
            //.WithMany(customer => customer.Contracts)
            //.HasForeignKey(contract => contract.CustomerId);

            modelBuilder.Entity<Domain.Relational.EEGSA.Contract>()
                .HasMany(bill => bill.Bills)
                .WithOne(contract => contract.Contract)
                .HasForeignKey(bill => bill.ContractId);

            modelBuilder.Entity<Domain.Relational.EEGSA.Bill>(entity =>
            {
                entity.ToTable("tbl_bill");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.BillAmount).HasColumnName("bill_amount").IsRequired();
                entity.Property(e => e.BillType).HasColumnName("bill_type").IsRequired();
                entity.Property(e => e.IssueDate).HasColumnName("issue_date").IsRequired();
                entity.Property(e => e.DueDate).HasColumnName("due_date").IsRequired();
                entity.Property(e => e.BillOverdue).HasColumnName("bill_overdue");
                entity.Property(e => e.DaysOverdue).HasColumnName("days_overdue");
                entity.Property(e => e.ContractId).HasColumnName("contract_id").IsRequired();
                //entity.HasOne(e => e.Contract).WithMany().HasForeignKey(e => e.ContractId);
            });

            modelBuilder.Entity<Domain.Relational.EEGSA.Payment>(entity =>
            {
                entity.ToTable("tbl_payment");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.PaymentAmount).HasColumnName("payment_amount").IsRequired();
                entity.Property(e => e.PaymentDate).HasColumnName("payment_date").IsRequired();
                entity.Property(e => e.BillId).HasColumnName("bill_id").IsRequired();
                entity.HasOne(e => e.Bill).WithMany().HasForeignKey(e => e.BillId);
            });

            modelBuilder.Entity<Domain.Relational.EEGSA.Bill>()
                .HasMany(bill => bill.Payments)
                .WithOne(payment => payment.Bill)
                .HasForeignKey(payment => payment.BillId);
        }
    }
}