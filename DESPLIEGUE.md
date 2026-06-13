# Despliegue y ejecución sin Visual Studio

Esta guía permite compilar, probar y ejecutar WorkFlowDesk usando solo la línea de comandos.

## Requisitos

1. **Windows 10/11**
2. **[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)** — descarga e instala el SDK (no solo el runtime)

No necesitas instalar SQL Server, LocalDB ni Visual Studio. La base de datos SQLite se crea automáticamente en:

`%LocalAppData%\WorkFlowDesk\workflowdesk.db`

Comprueba que `dotnet` está en el PATH:

```powershell
dotnet --version
```

Debe mostrar `8.0.x` o superior.

## Compilar la solución

Desde la raíz del repositorio:

```powershell
cd C:\Users\pab48\Documents\GitHub\WorkFlowDesk
dotnet restore WorkFlowDesk.sln
dotnet build WorkFlowDesk.sln -c Release
```

## Ejecutar tests

```powershell
dotnet test WorkFlowDesk.Tests\WorkFlowDesk.Tests.csproj -c Release --verbosity normal
```

## Ejecutar la aplicación

```powershell
dotnet run --project WorkFlowDesk.UI\WorkFlowDesk.UI.csproj -c Release
```

La primera ejecución crea la base de datos SQLite y los usuarios demo.

### Usuarios demo

| Rol | Usuario | Contraseña |
|-----|---------|------------|
| Administrador | `admin` | `Admin123` |
| Supervisor | `supervisor` | `Supervisor123` |
| Empleado | `empleado` | `Empleado123` |

## Publicar ejecutable

```powershell
dotnet publish WorkFlowDesk.UI\WorkFlowDesk.UI.csproj -c Release -r win-x64 --self-contained false -o .\publish
.\publish\WorkFlowDesk.UI.exe
```

## Solución de problemas

### Error al inicializar la base de datos

Borra la carpeta de datos y reinicia la app:

```powershell
Remove-Item -Recurse -Force "$env:LOCALAPPDATA\WorkFlowDesk" -ErrorAction SilentlyContinue
```

O usa **Configuración → Inicializar base de datos** (como `admin`).

### `dotnet` no reconocido

Cierra y vuelve a abrir la terminal tras instalar el SDK, o añade al PATH:

`C:\Program Files\dotnet\`

### Migraciones

El esquema se aplica automáticamente al arrancar.
