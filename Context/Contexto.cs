using DLeon_Asociados_Web.Models;
using Microsoft.EntityFrameworkCore;

namespace DLeon_Asociados_Web.Context;

public class Contexto : DbContext
{
	public Contexto(DbContextOptions<Contexto> options)
		: base(options)
	{
	}

	public DbSet<Vehiculos> Vehiculos { get; set; }
	public DbSet<Seguros> Seguros { get; set; }

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

		modelBuilder.Entity<Seguros>(entity =>
		{
			entity.HasKey(s => s.Id);
			entity.Property(s => s.Tipo).HasMaxLength(80).IsRequired();
			entity.Property(s => s.ClienteNombre).HasMaxLength(80).HasDefaultValue(string.Empty).IsRequired();
			entity.Property(s => s.ClienteApellido).HasMaxLength(80).HasDefaultValue(string.Empty).IsRequired();
			entity.Property(s => s.ClienteDocumento).HasMaxLength(40).HasDefaultValue(string.Empty).IsRequired();
			entity.Property(s => s.ClienteTelefono).HasMaxLength(30).HasDefaultValue(string.Empty).IsRequired();
			entity.Property(s => s.ClienteCorreo).HasMaxLength(150).HasDefaultValue(string.Empty).IsRequired();
			entity.Property(s => s.ClienteDireccion).HasMaxLength(250).HasDefaultValue(string.Empty).IsRequired();
			entity.Property(s => s.FechaInicio).HasColumnType("date");
			entity.Property(s => s.FechaExpiracion).HasColumnType("date");
			entity.HasIndex(s => s.VehiculoId);
			entity.HasIndex(s => s.FechaExpiracion);
			entity.HasOne(s => s.Vehiculo)
				.WithMany(v => v.Seguros)
				.HasForeignKey(s => s.VehiculoId)
				.OnDelete(DeleteBehavior.Restrict);
		});
	}
}
