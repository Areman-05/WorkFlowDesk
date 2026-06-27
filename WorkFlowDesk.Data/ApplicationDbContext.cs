using Microsoft.EntityFrameworkCore;
using WorkFlowDesk.Domain.Entities;

namespace WorkFlowDesk.Data;

/// <summary>Contexto de Entity Framework para la base de datos de la aplicación.</summary>
public class ApplicationDbContext : DbContext
{
    /// <summary>Construye el contexto con las opciones de conexión inyectadas.</summary>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Rol> Roles { get; set; }
    public DbSet<Empleado> Empleados { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Proyecto> Proyectos { get; set; }
    public DbSet<Tarea> Tareas { get; set; }
    public DbSet<ComentarioTarea> ComentariosTareas { get; set; }
    public DbSet<SubTarea> Subtareas { get; set; }
    public DbSet<TareaDependencia> TareaDependencias { get; set; }
    public DbSet<RegistroTiempo> RegistrosTiempo { get; set; }
    public DbSet<TareaAdjunto> TareaAdjuntos { get; set; }
    public DbSet<RegistroActividad> RegistrosActividad { get; set; }

    /// <summary>Configura el modelo de entidades (claves, índices, relaciones).</summary>
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

        // Configuración de Empleado
        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Apellidos).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Telefono).HasMaxLength(20);
            entity.Property(e => e.Departamento).HasMaxLength(100);
            entity.Property(e => e.Cargo).HasMaxLength(100);
            entity.Property(e => e.AvatarIndex).HasDefaultValue(0);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasOne(e => e.Usuario)
                  .WithMany()
                  .HasForeignKey(e => e.UsuarioId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuración de Cliente
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Telefono).HasMaxLength(20);
            entity.Property(e => e.Direccion).HasMaxLength(500);
            entity.Property(e => e.Empresa).HasMaxLength(200);
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Configuración de Proyecto
        modelBuilder.Entity<Proyecto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Descripcion).HasMaxLength(1000);
            entity.HasOne(e => e.Cliente)
                  .WithMany(c => c.Proyectos)
                  .HasForeignKey(e => e.ClienteId)
                  .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.Responsable)
                  .WithMany()
                  .HasForeignKey(e => e.ResponsableId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuración de Tarea
        modelBuilder.Entity<Tarea>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Titulo).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Descripcion).HasMaxLength(2000);
            entity.HasOne(e => e.Asignado)
                  .WithMany()
                  .HasForeignKey(e => e.AsignadoId)
                  .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.Creador)
                  .WithMany()
                  .HasForeignKey(e => e.CreadorId)
                  .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.Proyecto)
                  .WithMany(p => p.Tareas)
                  .HasForeignKey(e => e.ProyectoId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<SubTarea>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Titulo).IsRequired().HasMaxLength(300);
            entity.HasOne(e => e.Tarea)
                  .WithMany(t => t.Subtareas)
                  .HasForeignKey(e => e.TareaId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TareaDependencia>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TareaId, e.DependeDeTareaId }).IsUnique();
            entity.HasOne(e => e.Tarea)
                  .WithMany(t => t.Dependencias)
                  .HasForeignKey(e => e.TareaId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.DependeDe)
                  .WithMany(t => t.BloqueaA)
                  .HasForeignKey(e => e.DependeDeTareaId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RegistroTiempo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nota).HasMaxLength(500);
            entity.HasOne(e => e.Tarea)
                  .WithMany(t => t.RegistrosTiempo)
                  .HasForeignKey(e => e.TareaId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Empleado)
                  .WithMany()
                  .HasForeignKey(e => e.EmpleadoId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<TareaAdjunto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NombreArchivo).IsRequired().HasMaxLength(260);
            entity.Property(e => e.RutaRelativa).IsRequired().HasMaxLength(500);
            entity.HasOne(e => e.Tarea)
                  .WithMany(t => t.Adjuntos)
                  .HasForeignKey(e => e.TareaId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.SubidoPor)
                  .WithMany()
                  .HasForeignKey(e => e.SubidoPorEmpleadoId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<RegistroActividad>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Entidad).IsRequired().HasMaxLength(80);
            entity.Property(e => e.Accion).IsRequired().HasMaxLength(80);
            entity.Property(e => e.Detalle).HasMaxLength(2000);
            entity.HasIndex(e => e.FechaUtc);
            entity.HasOne(e => e.Usuario)
                  .WithMany()
                  .HasForeignKey(e => e.UsuarioId)
                  .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.Empleado)
                  .WithMany()
                  .HasForeignKey(e => e.EmpleadoId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuración de ComentarioTarea
        modelBuilder.Entity<ComentarioTarea>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Contenido).IsRequired().HasMaxLength(2000);
            entity.HasOne(e => e.Tarea)
                  .WithMany(t => t.Comentarios)
                  .HasForeignKey(e => e.TareaId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Empleado)
                  .WithMany()
                  .HasForeignKey(e => e.EmpleadoId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
