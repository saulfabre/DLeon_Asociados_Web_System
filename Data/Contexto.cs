using DLeon_Asociados_Web.Models;
using Microsoft.EntityFrameworkCore;

namespace DLeon_Asociados_Web.Data;

public class Contexto : DbContext
{
    public Contexto(DbContextOptions<Contexto> options)
        : base(options)
    {
    }

    public DbSet<Vehiculos> Vehiculos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Vehiculos>(entity =>
        {
            entity.HasKey(v => v.Id);
            entity.Property(v => v.Marca).HasMaxLength(80).IsRequired();
            entity.Property(v => v.Modelo).HasMaxLength(80).IsRequired();
            entity.Property(v => v.Tipo).HasMaxLength(80).IsRequired();
            entity.Property(v => v.Nombre).HasMaxLength(200).IsRequired();
            entity.Property(v => v.Precio).HasColumnType("decimal(18,2)");
            entity.HasIndex(v => new { v.Marca, v.Modelo });
            entity.HasIndex(v => v.Estado);
        });
    }
}
