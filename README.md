# WorkFlowDesk

Aplicación de escritorio WPF para gestión de tareas, proyectos, empleados y clientes. Pensada como producto de portfolio: arquitectura en capas, MVVM, Entity Framework Core y SQL Server LocalDB.

## Características

- Login con roles (Administrador, Supervisor, Empleado)
- Dashboard con estadísticas en tiempo real
- CRUD de empleados, proyectos, tareas y clientes
- Comentarios en tareas
- Reportes y exportación a CSV
- Control de acceso por rol en menú y acciones
- Cerrar sesión, cambio de contraseña y reinicio de base de datos desde Configuración
- Backup y restauración real de SQL Server (`.bak`)
- Paginación y estados vacíos en listados
- Tests unitarios y CI en GitHub Actions

## Requisitos

- Windows 10/11
- Visual Studio 2022 (carga de trabajo **Desarrollo de escritorio .NET**)
- .NET 8 SDK
- SQL Server Express LocalDB

## Inicio rápido

1. Abre `WorkFlowDesk.sln` en Visual Studio.
2. Establece **WorkFlowDesk.UI** como proyecto de inicio.
3. Pulsa **F5**.
4. Inicia sesión con cualquiera de estos usuarios demo:

| Rol | Usuario | Contraseña |
|-----|---------|------------|
| Administrador | `admin` | `Admin123` |
| Supervisor | `supervisor` | `Supervisor123` |
| Empleado | `empleado` | `Empleado123` |

Guía detallada de pruebas y resolución de problemas: [COMO_PROBAR.md](COMO_PROBAR.md).

## Arquitectura

```
WorkFlowDesk.UI          → WPF, vistas XAML, navegación
WorkFlowDesk.ViewModel   → MVVM (CommunityToolkit.Mvvm)
WorkFlowDesk.Services    → Lógica de negocio e interfaces
WorkFlowDesk.Data        → EF Core, migraciones, seed
WorkFlowDesk.Domain      → Entidades y enums
WorkFlowDesk.Common      → Sesión, permisos, helpers, DTOs
```

## Base de datos

- Motor: SQL Server LocalDB
- Base de datos: `WorkFlowDeskDb`
- Cadena de conexión: `WorkFlowDesk.UI/appsettings.json`
- Esquema gestionado con **migraciones EF Core** (`WorkFlowDesk.Data/Migrations`)

Si migras desde una versión anterior que usaba `EnsureCreated`, entra en **Configuración → Inicializar base de datos** o borra la BD local y vuelve a ejecutar la app.

## Roles y permisos

| Sección        | Admin | Supervisor | Empleado |
|----------------|:-----:|:----------:|:--------:|
| Dashboard      | Sí    | Sí         | Sí       |
| Empleados      | Sí    | Sí         | No       |
| Proyectos      | Sí    | Sí         | No       |
| Tareas         | Sí    | Sí         | Solo lectura |
| Clientes       | Sí    | Sí         | No       |
| Reportes       | Sí    | Sí         | No       |
| Configuración  | Sí    | No         | No       |

## Exportaciones

Los listados y reportes generan CSV en la carpeta `Exports/` junto al ejecutable de la aplicación.

## Tests y CI

```bash
dotnet test WorkFlowDesk.Tests/WorkFlowDesk.Tests.csproj
```

El workflow `.github/workflows/build.yml` compila la solución y ejecuta los tests en cada push a `main`.

## Licencia

Proyecto de portfolio personal. Uso libre con atribución.
