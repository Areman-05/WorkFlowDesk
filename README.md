# WorkFlowDesk

**Gestor offline de clientes, proyectos y tareas para equipos pequeños:** roles, dashboard en vivo, Kanban, calendario, automatizaciones reales y datos 100 % locales en SQLite.

WorkFlowDesk es una aplicación de escritorio WPF pensada para quienes quieren operativa de verdad sin depender de la nube: dar de alta clientes, encadenar proyectos, repartir tareas, exportar reportes CSV y respaldar todo con un solo archivo `.db`. No es un mockup de CRUD: une login con roles, splash de arranque, sesión persistente, búsqueda global (Ctrl+K), historial de actividad, subtareas, dependencias, registro de tiempo, adjuntos y sincronización entre equipos vía carpeta compartida en un mismo flujo, con interfaz Stitch (claro/oscuro) y notificaciones in-app y de escritorio.

## La idea en una frase

Llevar el pulso de tu operativa en un solo sitio: clientes → proyectos → tareas → reportes — sin suscripción, sin servidor y con permisos claros por rol.

## Para quién es

- **Freelancer o consultora pequeña** que necesita clientes, proyectos y tareas en una sola app de Windows.
- **Equipo de 2–5 personas** que comparte un PC o sincroniza datos por carpeta de red.
- **Administrador de operaciones** que quiere empleados, roles, reportes y backup/restauración sin instalar SQL Server.
- **Desarrollador que arma portfolio**: arquitectura en capas, MVVM, EF Core, tests y CI en GitHub Actions.
- **Quien cuida la privacidad**: datos en `%LocalAppData%\WorkFlowDesk\`, sin telemetría obligatoria ni cuenta cloud.

## Qué hace la plataforma (en lenguaje humano)

### Inicio — Autenticación y arranque

Splash al entrar (login, registro o restauración de sesión si llevas menos de 2 días sin abrir la app). Login con usuarios demo, registro de nuevas cuentas y **PIN secundario opcional** (2FA local) configurable desde el perfil.

### Dashboard

Panel principal con estadísticas de empleados, proyectos, tareas y clientes; proyectos activos, actividad reciente y accesos rápidos a Tareas y Optimización. El contenido se adapta al rol (el empleado ve un subconjunto acotado).

### Empleados

Alta, edición y listado con paginación, búsqueda y exportación CSV. Avatares DiceBear, estados (activo, vacaciones, baja…) y vínculo opcional con usuario de la app.

### Proyectos

Cartera de proyectos con filtros, progreso calculado desde las tareas, selección de fila con **tarjeta de detalle** (cliente, fechas, estado, descripción) y enlace directo a las tareas del proyecto. Doble clic para editar.

### Tareas — Operativa diaria

Tres vistas: **Lista**, **Kanban** (columnas por estado) y **Calendario** (por fecha de vencimiento). Filtros por estado, comentarios, subtareas, dependencias entre tareas, registro de tiempo, adjuntos en disco e historial de actividad auditable.

### Clientes

CRM ligero: datos de contacto, estado activo/inactivo, exportación CSV y borrado lógico.

### Reportes

Estadísticas y gráficos de empleados, proyectos, tareas y clientes; filtros de productividad (7 / 30 días) y exportación de resúmenes a CSV.

### Optimización — Automatizaciones

Diseñador visual de flujos y reglas activables. Las automatizaciones **se ejecutan en la app** (notificaciones, escalado de retrasos, avisos de proyecto completado, webhook Slack si lo configuras). Las “recomendaciones” son plantillas que puedes aplicar al lienzo, no IA externa.

### Configuración

Ruta de la base SQLite, backup y restauración del `.db`, **sincronización LAN** (exportar/importar paquetes JSON en carpeta compartida), URL de webhook Slack, cambio de contraseña e inicialización de base de datos.

### Perfil

Avatar, tema claro/oscuro, idioma preferido, notificaciones de escritorio, PIN de 2FA y exportación de datos del perfil.

### Barra global

Búsqueda unificada con **Ctrl+K** (tareas, proyectos, clientes, empleados), campana de notificaciones, atajos de teclado por sección y cierre de sesión.

## Por qué WorkFlowDesk y no “otra herramienta de tareas”

- **Un solo viaje operativo**: de “nuevo cliente” a “proyecto en curso”, “tarea en Kanban” y “CSV en reportes” sin cambiar de app.
- **Offline de verdad**: SQLite embebido; no hace falta LocalDB ni SQL Server.
- **Roles con sentido**: admin, supervisor y empleado ven menús y acciones acordes a su responsabilidad.
- **Vistas que se usan cada día**: lista, Kanban y calendario, no solo una tabla plana.
- **Automatización honesta**: reglas que corren en tu máquina; sin prometer integraciones enterprise que no existen.
- **Experiencia cuidada**: splash, sesión recordada, diseño Stitch, estados vacíos y paginación en listados.
- **Código defendible**: capas MVVM, inyección de dependencias, migraciones EF Core y batería de tests automatizados.

## Cómo probarlo

### Ejecutar la aplicación (recomendado)

**Requisitos:** Windows 10/11 y [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

Desde la raíz del repositorio:

```powershell
dotnet run --project WorkFlowDesk.UI\WorkFlowDesk.UI.csproj
```

Compilación en Release (opcional):

```powershell
dotnet run --project WorkFlowDesk.UI\WorkFlowDesk.UI.csproj -c Release
```

### Usuarios demo

| Rol            | Usuario      | Contraseña       |
|----------------|--------------|------------------|
| Administrador  | `admin`      | `Admin123`       |
| Supervisor     | `supervisor` | `Supervisor123`  |
| Empleado       | `empleado`   | `Empleado123`    |

### Tests automatizados

```powershell
dotnet test WorkFlowDesk.sln
```

El workflow `.github/workflows/build.yml` compila la solución y ejecuta los tests en cada push a `main`.

### Datos y persistencia

- Base de datos: `%LocalAppData%\WorkFlowDesk\workflowdesk.db`
- Adjuntos: `%LocalAppData%\WorkFlowDesk\attachments\`
- Exportaciones CSV: carpeta `Exports/` junto al ejecutable
- Backups: carpeta `Backups/` (también configurable desde Configuración)

Si la base queda corrupta: **Configuración → Inicializar base de datos**, o borra `%LocalAppData%\WorkFlowDesk\` y vuelve a abrir la app (se regenera el seed demo).

## Estructura del repositorio (resumida)

| Área | Contenido |
|------|-----------|
| `WorkFlowDesk.UI` | WPF, vistas XAML, controles, navegación, splash, diálogos |
| `WorkFlowDesk.ViewModel` | MVVM (CommunityToolkit.Mvvm), comandos y estado de pantalla |
| `WorkFlowDesk.Services` | Lógica de negocio, exportación, backup, búsqueda, sync, automatizaciones |
| `WorkFlowDesk.Data` | EF Core, `ApplicationDbContext`, migraciones y seed |
| `WorkFlowDesk.Domain` | Entidades, enums y relaciones del dominio |
| `WorkFlowDesk.Common` | Sesión, permisos, preferencias, helpers compartidos |
| `WorkFlowDesk.Tests` | Tests unitarios (xUnit, EF Core InMemory) |
| `.github/workflows` | CI: build + test |

## Detalle técnico (opcional)

| Área | Tecnología |
|------|------------|
| UI | WPF (.NET 8), XAML, design system Stitch |
| Patrón | MVVM, DI (`Microsoft.Extensions.DependencyInjection`) |
| Datos | SQLite + Entity Framework Core 8 (migraciones) |
| Auth | Hash de contraseña, roles, PIN secundario local |
| Sync | JSON en carpeta compartida (multi-equipo sin servidor) |
| Tests | xUnit, ~52 casos |
| CI | GitHub Actions |

Arquitectura detallada (capas, arranque, servicios, persistencia): [ARCHITECTURE.md](ARCHITECTURE.md).

## Roles y permisos

| Sección       | Admin | Supervisor | Empleado |
|---------------|:-----:|:----------:|:--------:|
| Dashboard     | Sí    | Sí         | Sí       |
| Empleados     | Sí    | Sí         | No       |
| Proyectos     | Sí    | Sí         | No       |
| Tareas        | Sí    | Sí         | Sí (solo lectura) |
| Clientes      | Sí    | Sí         | No       |
| Reportes      | Sí    | Sí         | No       |
| Optimización  | Sí    | Sí         | No       |
| Configuración | Sí    | No         | No       |
| Perfil        | Sí    | Sí         | Sí       |

## Licencia

Proyecto de portfolio personal. Uso libre con atribución.

---

**WorkFlowDesk:** del cliente a la tarea entregada, con roles, Kanban y datos bajo tu control.

*Gestor offline de clientes, proyectos y tareas para equipos pequeños.*
