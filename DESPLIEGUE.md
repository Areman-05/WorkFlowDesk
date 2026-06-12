# Despliegue y ejecución sin Visual Studio

Esta guía permite compilar, probar y ejecutar WorkFlowDesk usando solo la línea de comandos.

## Requisitos

1. **Windows 10/11**
2. **[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)** — descarga e instala el SDK (no solo el runtime)
3. **SQL Server Express LocalDB** — suele venir con Visual Studio Build Tools o se instala aparte:
   - [SQL Server Express](https://www.microsoft.com/sql-server/sql-server-downloads) (incluye LocalDB)

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

La primera ejecución crea la base de datos `WorkFlowDeskDb` en LocalDB y los usuarios demo.

### Usuarios demo

| Rol | Usuario | Contraseña |
|-----|---------|------------|
| Administrador | `admin` | `Admin123` |
| Supervisor | `supervisor` | `Supervisor123` |
| Empleado | `empleado` | `Empleado123` |

## Publicar ejecutable

Genera una carpeta autocontenida lista para distribuir:

```powershell
dotnet publish WorkFlowDesk.UI\WorkFlowDesk.UI.csproj -c Release -r win-x64 --self-contained false -o .\publish
```

Ejecuta desde la carpeta publicada:

```powershell
.\publish\WorkFlowDesk.UI.exe
```

## Solución de problemas

### Error de conexión a LocalDB

Comprueba que LocalDB está instalado:

```powershell
sqllocaldb info
sqllocaldb start MSSQLLocalDB
```

Si la BD quedó en un estado inconsistente, borra `WorkFlowDeskDb` o usa **Configuración → Inicializar base de datos** (como `admin`).

### `dotnet` no reconocido

Cierra y vuelve a abrir la terminal tras instalar el SDK, o añade al PATH:

`C:\Program Files\dotnet\`

### Migraciones

El esquema se aplica automáticamente al arrancar. Si hay conflictos con una BD antigua, elimina la base local y reinicia la app.
