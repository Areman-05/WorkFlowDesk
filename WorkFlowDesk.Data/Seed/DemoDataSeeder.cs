using Microsoft.EntityFrameworkCore;
using WorkFlowDesk.Common.Services;
using WorkFlowDesk.Domain.Entities;

namespace WorkFlowDesk.Data.Seed;

/// <summary>Datos de demostración para primera ejecución (dashboard y listados poblados).</summary>
public static class DemoDataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Proyectos.AnyAsync())
            return;

        var admin = await context.Usuarios.FirstAsync(u => u.NombreUsuario == DatabaseSeeder.DefaultAdminUserName);
        var supervisorUser = await context.Usuarios.FirstAsync(u => u.NombreUsuario == DatabaseSeeder.DefaultSupervisorUserName);
        var empleadoUser = await context.Usuarios.FirstAsync(u => u.NombreUsuario == DatabaseSeeder.DefaultEmpleadoUserName);

        var clientes = await SeedClientesAsync(context);
        var empleados = await SeedEmpleadosAsync(context, admin, supervisorUser, empleadoUser);
        var proyectos = await SeedProyectosAsync(context, clientes, empleados);
        await SeedTareasAsync(context, proyectos, empleados);
        await context.SaveChangesAsync();
    }

    private static async Task<Dictionary<string, Cliente>> SeedClientesAsync(ApplicationDbContext context)
    {
        var ahora = DateTime.Now;
        var clientes = new[]
        {
            CrearCliente("Global Devs", "contacto@globaldevs.com", "Global Devs", ahora.AddMonths(-8)),
            CrearCliente("Nike Inc.", "projects@nike.com", "Nike Inc.", ahora.AddMonths(-6)),
            CrearCliente("BBVA", "digital@bbva.com", "BBVA", ahora.AddMonths(-5)),
            CrearCliente("TechCorp S.A.", "hola@techcorp.es", "TechCorp S.A.", ahora.AddDays(-5)),
            CrearCliente("MediaFlow", "info@mediaflow.es", "MediaFlow", ahora.AddMonths(-3)),
            CrearCliente("Retail Plus", "ops@retailplus.com", "Retail Plus", ahora.AddMonths(-2)),
            CrearCliente("InnovaLab", "hello@innovalab.io", "InnovaLab", ahora.AddMonths(-1)),
            CrearCliente("Studio Norte", "studio@norte.es", "Studio Norte", ahora.AddDays(-12))
        };

        await context.Clientes.AddRangeAsync(clientes);
        await context.SaveChangesAsync();
        return clientes.ToDictionary(c => c.Empresa, StringComparer.OrdinalIgnoreCase);
    }

    private static Cliente CrearCliente(string nombre, string email, string empresa, DateTime fechaCreacion) =>
        new()
        {
            Nombre = nombre,
            Email = email,
            Telefono = "+34 900 000 000",
            Direccion = "Madrid, España",
            Empresa = empresa,
            FechaCreacion = fechaCreacion,
            Activo = true
        };

    private static async Task<Dictionary<string, Empleado>> SeedEmpleadosAsync(
        ApplicationDbContext context,
        Usuario admin,
        Usuario supervisorUser,
        Usuario empleadoUser)
    {
        var ahora = DateTime.Now;
        var empleados = new List<Empleado>
        {
            new()
            {
                Nombre = "Administrador", Apellidos = "Sistema", Email = admin.Email,
                Telefono = "600 000 001", Departamento = "Dirección", Cargo = "Administrador",
                FechaContratacion = ahora.AddYears(-2), UsuarioId = admin.Id
            },
            new()
            {
                Nombre = "Supervisor", Apellidos = "Demo", Email = supervisorUser.Email,
                Telefono = "600 000 002", Departamento = "Operaciones", Cargo = "Supervisor de equipo",
                FechaContratacion = ahora.AddYears(-1), UsuarioId = supervisorUser.Id
            },
            new()
            {
                Nombre = "Empleado", Apellidos = "Demo", Email = empleadoUser.Email,
                Telefono = "600 000 003", Departamento = "Proyectos", Cargo = "Analista",
                FechaContratacion = ahora.AddMonths(-10), UsuarioId = empleadoUser.Id
            },
            new() { Nombre = "Carlos", Apellidos = "Ruiz", Email = "c.ruiz@workflowdesk.local", Telefono = "600 000 004", Departamento = "Diseño", Cargo = "Diseñador UX", FechaContratacion = ahora.AddMonths(-9) },
            new() { Nombre = "Marta", Apellidos = "López", Email = "m.lopez@workflowdesk.local", Telefono = "600 000 005", Departamento = "Marketing", Cargo = "SEO Specialist", FechaContratacion = ahora.AddMonths(-8), Estado = EstadoEmpleado.Vacaciones },
            new() { Nombre = "Laura", Apellidos = "García", Email = "l.garcia@workflowdesk.local", Telefono = "600 000 006", Departamento = "Desarrollo", Cargo = "Dev Frontend", FechaContratacion = ahora.AddMonths(-7) },
            new() { Nombre = "Pablo", Apellidos = "Martín", Email = "p.martin@workflowdesk.local", Telefono = "600 000 007", Departamento = "Desarrollo", Cargo = "Dev Backend", FechaContratacion = ahora.AddMonths(-6) },
            new() { Nombre = "Ana", Apellidos = "Serrano", Email = "a.serrano@workflowdesk.local", Telefono = "600 000 008", Departamento = "QA", Cargo = "Tester", FechaContratacion = ahora.AddMonths(-5) },
            new() { Nombre = "Diego", Apellidos = "Torres", Email = "d.torres@workflowdesk.local", Telefono = "600 000 009", Departamento = "Soporte", Cargo = "Técnico", FechaContratacion = ahora.AddMonths(-4) },
            new() { Nombre = "Elena", Apellidos = "Vega", Email = "e.vega@workflowdesk.local", Telefono = "600 000 010", Departamento = "RRHH", Cargo = "People Ops", FechaContratacion = ahora.AddMonths(-3) },
            new() { Nombre = "Jorge", Apellidos = "Núñez", Email = "j.nunez@workflowdesk.local", Telefono = "600 000 011", Departamento = "Ventas", Cargo = "Account Manager", FechaContratacion = ahora.AddMonths(-2), Estado = EstadoEmpleado.Inactivo },
            new() { Nombre = "Sofía", Apellidos = "Ramos", Email = "s.ramos@workflowdesk.local", Telefono = "600 000 012", Departamento = "Diseño", Cargo = "UI Designer", FechaContratacion = ahora.AddDays(-20) }
        };

        for (var i = 0; i < empleados.Count; i++)
            empleados[i].AvatarIndex = i % AvatarCatalog.Count;

        await context.Empleados.AddRangeAsync(empleados);
        await context.SaveChangesAsync();
        return empleados.ToDictionary(e => $"{e.Nombre} {e.Apellidos}", StringComparer.OrdinalIgnoreCase);
    }

    private static async Task<List<Proyecto>> SeedProyectosAsync(
        ApplicationDbContext context,
        Dictionary<string, Cliente> clientes,
        Dictionary<string, Empleado> empleados)
    {
        var supervisor = empleados["Supervisor Demo"];
        var adminEmp = empleados["Administrador Sistema"];
        var ahora = DateTime.Now;

        var proyectos = new List<Proyecto>
        {
            new() { Nombre = "WorkFlow CMS", Descripcion = "v2.0 Beta", Estado = EstadoProyecto.EnProgreso, FechaInicio = ahora.AddMonths(-4), ClienteId = clientes["Global Devs"].Id, ResponsableId = supervisor.Id },
            new() { Nombre = "Rebranding Nike", Descripcion = "Campaña global", Estado = EstadoProyecto.EnProgreso, FechaInicio = ahora.AddMonths(-3), ClienteId = clientes["Nike Inc."].Id, ResponsableId = supervisor.Id },
            new() { Nombre = "App Móvil BBVA", Descripcion = "Mobile Design", Estado = EstadoProyecto.EnProgreso, FechaInicio = ahora.AddMonths(-2), ClienteId = clientes["BBVA"].Id, ResponsableId = supervisor.Id },
            new() { Nombre = "Nike Campaign", Descripcion = "Social Media", Estado = EstadoProyecto.Planificacion, FechaInicio = ahora.AddMonths(-1), ClienteId = clientes["Nike Inc."].Id, ResponsableId = supervisor.Id },
            new() { Nombre = "Banking UI", Descripcion = "Mobile Design", Estado = EstadoProyecto.Completado, FechaInicio = ahora.AddMonths(-6), FechaFin = ahora.AddMonths(-1), ClienteId = clientes["BBVA"].Id, ResponsableId = supervisor.Id },
            new() { Nombre = "Rediseño Web", Descripcion = "Sitio corporativo", Estado = EstadoProyecto.EnProgreso, FechaInicio = ahora.AddMonths(-2), ClienteId = clientes["MediaFlow"].Id, ResponsableId = supervisor.Id },
            new() { Nombre = "Portal TechCorp", Descripcion = "Intranet", Estado = EstadoProyecto.EnProgreso, FechaInicio = ahora.AddDays(-45), ClienteId = clientes["TechCorp S.A."].Id, ResponsableId = supervisor.Id },
            new() { Nombre = "E-commerce Retail", Descripcion = "Tienda online", Estado = EstadoProyecto.EnPausa, FechaInicio = ahora.AddMonths(-5), ClienteId = clientes["Retail Plus"].Id, ResponsableId = supervisor.Id },
            new() { Nombre = "App InnovaLab", Descripcion = "MVP producto", Estado = EstadoProyecto.Planificacion, FechaInicio = ahora.AddDays(-14), ClienteId = clientes["InnovaLab"].Id, ResponsableId = supervisor.Id },
            new() { Nombre = "Auditoría SEO", Descripcion = "Marketing digital", Estado = EstadoProyecto.Completado, FechaInicio = ahora.AddMonths(-2), FechaFin = ahora.AddDays(-3), ClienteId = clientes["Studio Norte"].Id, ResponsableId = supervisor.Id },
            new() { Nombre = "Dashboard Analytics", Descripcion = "BI interno", Estado = EstadoProyecto.EnProgreso, FechaInicio = ahora.AddMonths(-1), ClienteId = clientes["Global Devs"].Id, ResponsableId = supervisor.Id },
            new() { Nombre = "Onboarding RRHH", Descripcion = "Proceso interno", Estado = EstadoProyecto.EnProgreso, FechaInicio = ahora.AddDays(-30), ResponsableId = adminEmp.Id }
        };

        await context.Proyectos.AddRangeAsync(proyectos);
        await context.SaveChangesAsync();
        return proyectos;
    }

    private static async Task SeedTareasAsync(
        ApplicationDbContext context,
        List<Proyecto> proyectos,
        Dictionary<string, Empleado> empleados)
    {
        var ahora = DateTime.Now;
        var inicioSemana = ahora.Date.AddDays(-(int)ahora.DayOfWeek + (ahora.DayOfWeek == DayOfWeek.Sunday ? -6 : 1));

        var carlos = empleados["Carlos Ruiz"];
        var marta = empleados["Marta López"];
        var empleadoDemo = empleados["Empleado Demo"];
        var laura = empleados["Laura García"];
        var pablo = empleados["Pablo Martín"];
        var ana = empleados["Ana Serrano"];
        var supervisor = empleados["Supervisor Demo"];

        var porNombre = proyectos.ToDictionary(p => p.Nombre);
        var tareas = new List<Tarea>();

        void Add(string titulo, string proyectoNombre, Empleado asignado, Empleado creador,
            EstadoTarea estado, PrioridadTarea prioridad, DateTime? vencimiento, DateTime creada)
        {
            tareas.Add(new Tarea
            {
                Titulo = titulo,
                Descripcion = titulo,
                ProyectoId = porNombre[proyectoNombre].Id,
                AsignadoId = asignado.Id,
                CreadorId = creador.Id,
                Estado = estado,
                Prioridad = prioridad,
                FechaVencimiento = vencimiento,
                FechaCreacion = creada,
                FechaInicio = estado is EstadoTarea.EnProgreso or EstadoTarea.Completada ? creada.AddDays(1) : null,
                FechaFin = estado == EstadoTarea.Completada ? creada.AddDays(5) : null
            });
        }

        Add("Wireframes pantalla principal", "Rebranding Nike", carlos, marta, EstadoTarea.EnProgreso, PrioridadTarea.Alta, ahora.AddDays(3), ahora.AddMinutes(-12));
        Add("Revisión assets marca", "Rebranding Nike", laura, carlos, EstadoTarea.Pendiente, PrioridadTarea.Media, ahora.AddDays(7), ahora.AddDays(-2));
        Add("API login móvil", "App Móvil BBVA", pablo, supervisor, EstadoTarea.EnProgreso, PrioridadTarea.Critica, ahora.AddDays(2), ahora.AddDays(-1));
        Add("Pruebas regresión UI", "App Móvil BBVA", ana, pablo, EstadoTarea.Pendiente, PrioridadTarea.Alta, null, ahora.AddDays(-4));
        Add("Migración módulos CMS", "WorkFlow CMS", empleadoDemo, laura, EstadoTarea.EnProgreso, PrioridadTarea.Media, inicioSemana.AddDays(4), ahora.AddDays(-3));
        Add("Documentación API v2", "WorkFlow CMS", pablo, empleadoDemo, EstadoTarea.Pendiente, PrioridadTarea.Baja, inicioSemana.AddDays(5), ahora.AddDays(-1));
        Add("Auditoría SEO técnica", "Auditoría SEO", marta, marta, EstadoTarea.Completada, PrioridadTarea.Alta, ahora.AddDays(-3), ahora.AddDays(-10));
        Add("Informe keywords", "Auditoría SEO", marta, supervisor, EstadoTarea.Completada, PrioridadTarea.Media, ahora.AddDays(-2), ahora.AddHours(-2));
        Add("Maquetación home", "Rediseño Web", carlos, carlos, EstadoTarea.Pendiente, PrioridadTarea.Media, inicioSemana.AddDays(2), ahora.AddHours(-12));
        Add("Integración formularios", "Rediseño Web", empleadoDemo, carlos, EstadoTarea.EnProgreso, PrioridadTarea.Alta, ahora.AddDays(1), ahora.AddDays(-5));

        for (var i = 1; i <= 25; i++)
        {
            var proyecto = proyectos[i % proyectos.Count];
            var asignado = i % 3 == 0 ? empleadoDemo : i % 2 == 0 ? laura : carlos;
            var estado = i % 5 == 0 ? EstadoTarea.Completada : i % 4 == 0 ? EstadoTarea.EnProgreso : EstadoTarea.Pendiente;
            var prioridad = i % 7 == 0 ? PrioridadTarea.Critica : i % 3 == 0 ? PrioridadTarea.Alta : PrioridadTarea.Media;
            DateTime? venc = i % 4 == 0 ? null : ahora.AddDays(i % 14);

            tareas.Add(new Tarea
            {
                Titulo = $"Tarea operativa #{i}",
                Descripcion = $"Tarea operativa #{i}",
                ProyectoId = proyecto.Id,
                AsignadoId = asignado.Id,
                CreadorId = supervisor.Id,
                Estado = estado,
                Prioridad = prioridad,
                FechaVencimiento = venc,
                FechaCreacion = ahora.AddDays(-i),
                FechaInicio = estado is EstadoTarea.EnProgreso or EstadoTarea.Completada ? ahora.AddDays(-i + 1) : null,
                FechaFin = estado == EstadoTarea.Completada ? ahora.AddDays(-i + 5) : null
            });
        }

        await context.Tareas.AddRangeAsync(tareas);
    }
}
