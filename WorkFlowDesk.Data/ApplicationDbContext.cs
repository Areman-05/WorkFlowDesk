using Microsoft.EntityFrameworkCore;
using WorkFlowDesk.Domain.Entities;

namespace WorkFlowDesk.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Rol> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NombreUsuario).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(256);
            entity.Property(e => e.NombreCompleto).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.NombreUsuario).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasOne(e => e.Rol)
                  .WithMany(r => r.Usuarios)
                  .HasForeignKey(e => e.RolId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuración de Rol
        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Descripcion).HasMaxLength(500);
            entity.HasIndex(e => e.TipoRol).IsUnique();
        });
    }
}
