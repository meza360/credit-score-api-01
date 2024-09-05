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
        modelBuilder.Entity<Domain.Relational.BancoUnion.Customer>().ToTable("tbl_customer");
        modelBuilder.Entity<Customer>().HasKey(u => u.Id);
        modelBuilder.Entity<Customer>().Property(u => u.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<Customer>().Property(u => u.Id).HasColumnName("id");
        modelBuilder.Entity<Customer>().Property(u => u.CUI).HasColumnName("cui").HasMaxLength(13).IsRequired();
        modelBuilder.Entity<Customer>().Property(u => u.FirstName).HasColumnName("first_name").HasMaxLength(100).IsRequired();
        modelBuilder.Entity<Customer>().Property(u => u.LastName).HasColumnName("last_name").HasMaxLength(100).IsRequired();
        modelBuilder.Entity<Customer>().Property(u => u.DateOfBirth).HasColumnName("date_of_birth").IsRequired();
    }

    public DbSet<Domain.Relational.BancoUnion.Customer> Customers { get; set; }
    //public DbSet<Payment> Payments { get; set; }

}