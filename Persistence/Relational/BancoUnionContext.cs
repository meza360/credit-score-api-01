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
        modelBuilder.Entity<Customer>().ToTable("Usuarios");
        //modelBuilder.Entity<Customer>().HasKey(u => u.Id);
        // modelBuilder.Entity<Customer>().Property(u => u.Id).ValueGeneratedOnAdd();
        //modelBuilder.Entity<Customer>().Property(u => u.Nome).HasMaxLength(100).IsRequired();
        //modelBuilder.Entity<Customer>().Property(u => u.Email).HasMaxLength(100).IsRequired();
        // modelBuilder.Entity<Customer>().Property(u => u.Senha).HasMaxLength(100).IsRequired();
        // modelBuilder.Entity<Customer>().Property(u => u.DataNascimento).IsRequired();
        // modelBuilder.Entity<Customer>().Property(u => u.CriadoEm).IsRequired();
        //modelBuilder.Entity<Customer>().Property(u => u.AtualizadoEm).IsRequired();
    }

    public DbSet<Customer> Customer { get; set; }
    public DbSet<Payment> Payments { get; set; }

}