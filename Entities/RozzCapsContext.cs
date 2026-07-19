using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RozzCaps.Entidades;

public partial class RozzCapsContext : DbContext
{
    public RozzCapsContext()
    {
    }

    public RozzCapsContext(DbContextOptions<RozzCapsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Categoria> Categorias { get; set; }

    public virtual DbSet<Colore> Colores { get; set; }

    public virtual DbSet<Gorra> Gorras { get; set; }

    public virtual DbSet<GorraImagene> GorraImagenes { get; set; }

    public virtual DbSet<GorraVariacione> GorraVariaciones { get; set; }

    public virtual DbSet<Venta> Ventas { get; set; }

    public virtual DbSet<VentaDetalle> VentaDetalles { get; set; }

    public virtual DbSet<VentaEnvio> VentaEnvios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Colore>(entity =>
        {
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.CodigoHex)
                .HasMaxLength(7)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Gorra>(entity =>
        {
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Descripcion).IsUnicode(false);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Precio).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Categoria).WithMany(p => p.Gorras)
                .HasForeignKey(d => d.CategoriaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Gorras_Categorias");
        });

        modelBuilder.Entity<GorraImagene>(entity =>
        {
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.UrlImagen)
                .HasMaxLength(2083)
                .IsUnicode(false);

            entity.HasOne(d => d.GorraVariacion).WithMany(p => p.GorraImagenes)
                .HasForeignKey(d => d.GorraVariacionId)
                .HasConstraintName("FK_GorraImagenes_GorraVariaciones");
        });

        modelBuilder.Entity<GorraVariacione>(entity =>
        {
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Talla)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Color).WithMany(p => p.GorraVariaciones)
                .HasForeignKey(d => d.ColorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GorraVariaciones_Colores");

            entity.HasOne(d => d.Gorra).WithMany(p => p.GorraVariaciones)
                .HasForeignKey(d => d.GorraId)
                .HasConstraintName("FK_GorraVariaciones_Gorras");
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.Property(e => e.CodigoOrden).HasMaxLength(50);
            entity.Property(e => e.CostoEnvio).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Estado)
                .HasMaxLength(30)
                .HasDefaultValue("Pendiente");
            entity.Property(e => e.Fecha).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.PasarelaPagoId).HasMaxLength(100);
            entity.Property(e => e.Total).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<VentaDetalle>(entity =>
        {
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.GorraVariacion).WithMany(p => p.VentaDetalles)
                .HasForeignKey(d => d.GorraVariacionId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Venta).WithMany(p => p.VentaDetalles).HasForeignKey(d => d.VentaId);
        });

        modelBuilder.Entity<VentaEnvio>(entity =>
        {
            entity.Property(e => e.Barrio).HasMaxLength(100);
            entity.Property(e => e.Ciudad).HasMaxLength(100);
            entity.Property(e => e.ClienteEmail).HasMaxLength(150);
            entity.Property(e => e.ClienteNombre).HasMaxLength(150);
            entity.Property(e => e.ClienteTelefono).HasMaxLength(20);
            entity.Property(e => e.CodigoPostal).HasMaxLength(20);
            entity.Property(e => e.Departamento).HasMaxLength(100);
            entity.Property(e => e.Direccion).HasMaxLength(250);

            entity.HasOne(d => d.Venta).WithMany(p => p.VentaEnvios).HasForeignKey(d => d.VentaId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
