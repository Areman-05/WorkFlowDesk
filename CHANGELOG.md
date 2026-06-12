# Changelog

Todos los cambios relevantes de WorkFlowDesk se documentan en este archivo.

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
- Backup y restauración SQL Server (`.bak`)
- Configuración persistida en `appconfig.json`
- Cambio de contraseña con hash PBKDF2
- Tests unitarios e integración con EF InMemory
- CI en GitHub Actions
- Atajos de teclado en navegación (Ctrl+1…7, Ctrl+L)
- Documentación: README, ARCHITECTURE, COMO_PROBAR, DESPLIEGUE

### Seguridad

- Contraseñas con PBKDF2 y migración automática desde SHA256 legacy
- Confirmación antes de operaciones destructivas (eliminar, inicializar BD)
