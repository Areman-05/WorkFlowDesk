# Cómo probar WorkFlowDesk

Guía rápida para ejecutar la aplicación e iniciar sesión con los usuarios demo. Ver también [DESPLIEGUE.md](DESPLIEGUE.md) para usar solo la terminal.

## 1. Requisitos

- **Windows 10/11**
- **.NET 8 SDK** ([descarga](https://dotnet.microsoft.com/download/dotnet/8.0))
- **No requiere SQL Server ni LocalDB** — la BD SQLite se crea sola en `%LocalAppData%\WorkFlowDesk\`

Opcional: Visual Studio 2022 con carga de trabajo “Desarrollo de escritorio .NET”.

## 2. Ejecutar (terminal)

```powershell
cd ruta\al\repo\WorkFlowDesk
dotnet run --project WorkFlowDesk.UI\WorkFlowDesk.UI.csproj -c Release
```

## 3. Ejecutar (Visual Studio)

1. Abre **`WorkFlowDesk.sln`**.
2. Establece **WorkFlowDesk.UI** como proyecto de inicio.
3. Pulsa **F5** o **Ctrl+F5**.

## 4. Primera ejecución

Se crea la base de datos SQLite y el seed (roles + usuarios demo). Aparece la ventana de login.

- **Administrador:** usuario `admin` / contraseña `Admin123`
- **Supervisor:** usuario `supervisor` / contraseña `Supervisor123`
- **Empleado:** usuario `empleado` / contraseña `Empleado123`

Escribe las contraseñas exactamente así (respetando mayúsculas).

La primera vez que abras la app, la base de datos se crea en segundo plano. Si pulsas **Iniciar sesión** y sale "Usuario o contraseña incorrectos", espera 2–3 segundos y vuelve a pulsar **Iniciar sesión**.

Tras iniciar sesión se abrirá la ventana principal con el menú lateral (Dashboard, Empleados, Proyectos, Tareas, Clientes, Reportes, Configuración). Desde el menú puedes navegar a cada sección.

## 6. Si algo falla

Si tras seguir los pasos anteriores algo no funciona, revisa los puntos siguientes.

- **“Unable to locate a Local Database Runtime installation” / “The server was not found or was not accessible”**  
  La aplicación no encuentra **SQL Server LocalDB**. Hay que instalarlo:
  1. Cierra Visual Studio.
  2. Abre **Instalador de Visual Studio** (busca “Visual Studio Installer” en el menú Inicio).
  3. En la lista de instalaciones, pulsa **Modificar** en tu edición de Visual Studio 2022.
  4. Ve a la pestaña **Componentes individuales**.
  5. En el buscador escribe **LocalDB**.
  6. Marca **SQL Server Express 2019 LocalDB** (o la versión que aparezca).
  7. Pulsa **Modificar** y espera a que termine la instalación.
  8. Vuelve a abrir la solución y ejecuta la aplicación de nuevo (F5).

- **“No se puede conectar a la base de datos”**  
  Si ya tienes LocalDB instalado y sigue fallando, en el Instalador de Visual Studio → Modificar → **Componentes individuales** → busca **SQL Server Express LocalDB** y asegúrate de que esté marcado.

- **“Usuario o contraseña incorrectos”**  
  Comprueba que escribes exactamente: usuario `admin` y contraseña `Admin123` (A mayúscula, el resto minúsculas y el número 123).

- **Errores de compilación**  
  Clic derecho en la solución → **Restaurar paquetes NuGet**. Luego **Compilar → Recompilar solución**.

- **La base de datos está vacía o da error**  
  Entra en **Configuración** desde el menú, y usa el botón **Inicializar base de datos** para volver a crear tablas y datos de prueba (incluido el usuario `admin`).

- **Error al aplicar migraciones tras actualizar el código**  
  Si la BD se creó con una versión antigua (`EnsureCreated`), puede haber conflicto con las migraciones EF. Solución: en SQL Server Management Studio o `sqllocaldb`, elimina la base `WorkFlowDeskDb` y vuelve a ejecutar la app, o usa **Inicializar base de datos** en Configuración.

## 7. Cadena de conexión

La conexión está en **WorkFlowDesk.UI → appsettings.json**:

```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=WorkFlowDeskDb;Trusted_Connection=True;MultipleActiveResultSets=true"
```

Solo necesitas cambiarla si usas otra instancia de SQL Server o otra base de datos. La base de datos se crea en **LocalDB** con nombre `WorkFlowDeskDb`.

---

**Ver también:** Requisitos (sección 1) para instalar LocalDB si aún no lo tienes. Para más ayuda, revisa la sección 6 (Si algo falla).
