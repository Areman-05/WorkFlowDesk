# Changelog

Todos los cambios relevantes de WorkFlowDesk se documentan en este archivo.

## [1.1.0] - 2026-06-13

### Cambiado

- Base de datos migrada de SQL Server LocalDB a **SQLite embebido** (sin instalación externa).
- Backup/restore mediante copia del archivo `.db`.
- Consultas de listado con `AsNoTracking` y reportes con conteos SQL directos.

### Eliminado

- Dependencia de SQL Server LocalDB y paquete `Microsoft.Data.SqlClient`.
- Código muerto: repositorios genéricos, helpers y ViewModels base sin uso.

## [1.0.0] - 2026-06-08

### Añadido

- Aplicación WPF con arquitectura en capas (UI, ViewModel, Services, Data, Domain, Common)
- Login con roles: Administrador, Supervisor y Empleado
- CRUD de empleados, proyectos, tareas y clientes
- Dashboard con estadísticas en tiempo real
- Comentarios en tareas
- Reportes y exportación a CSV
- Control de acceso por rol en menú y acciones
- Paginación y estados vacíos en listados
- Backup y restauración del archivo SQLite (`.db`)
- Configuración persistida en `appconfig.json`
- Cambio de contraseña con hash PBKDF2
- Tests unitarios e integración con EF InMemory
- CI en GitHub Actions
- Atajos de teclado en navegación (Ctrl+1…7, Ctrl+L)
- Documentación: README, ARCHITECTURE, COMO_PROBAR, DESPLIEGUE

### Seguridad

- Contraseñas con PBKDF2 y migración automática desde SHA256 legacy
- Confirmación antes de operaciones destructivas (eliminar, inicializar BD)
