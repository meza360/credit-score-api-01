using Microsoft.EntityFrameworkCore;
using Domain.Relational.Renap;

namespace Persistence.Relational;

public class RenapContext : DbContext
{
    public RenapContext(DbContextOptions<RenapContext> options) : base(options)
    {
    }

    public DbSet<Citizen> Citizens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Citizen>(entity =>
        {
            entity.ToTable("tbl_citizen");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CUI).HasColumnName("cui").IsRequired();
            entity.Property(e => e.FirstName).HasColumnName("first_name").IsRequired();
            entity.Property(e => e.LastName).HasColumnName("last_name").IsRequired();
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth").IsRequired();
            entity.Property(e => e.DateOfDecease).HasColumnName("date_of_decease");
            entity.Property(e => e.Nationality).HasColumnName("nationality").IsRequired();
        });
    }
}