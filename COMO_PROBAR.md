# Cómo probar WorkFlowDesk en Visual Studio 2022

## 1. Requisitos

- **Visual Studio 2022** (cualquier edición: Community, Professional o Enterprise).
- **.NET 8 SDK** (incluido con VS 2022 si instalaste la carga de trabajo “Desarrollo de escritorio .NET”).
- **SQL Server LocalDB** (viene con Visual Studio al instalar “Desarrollo de almacenamiento y procesamiento de datos” o “Desarrollo de escritorio .NET”).

## 2. Abrir la solución

1. Abre Visual Studio 2022.
2. **Archivo → Abrir → Proyecto o solución**.
3. Ve a la carpeta del repositorio y selecciona **`WorkFlowDesk.sln`**.
4. Acepta y espera a que se restauren los paquetes NuGet.

## 3. Proyecto de inicio

1. En el **Explorador de soluciones**, clic derecho en el proyecto **WorkFlowDesk.UI**.
2. Elige **Establecer como proyecto de inicio**.
3. Comprueba que en la barra de herramientas aparezca **WorkFlowDesk.UI** como proyecto de inicio.

## 4. Ejecutar la aplicación

1. Pulsa **F5** (con depuración) o **Ctrl+F5** (sin depuración).
2. La primera vez se creará la base de datos en LocalDB y se ejecutará el seed (roles + usuario administrador).

## 5. Iniciar sesión

- **Usuario:** `admin`  
- **Contraseña:** `Admin123`

La primera vez que abras la app, la base de datos se crea en segundo plano. Si pulsas **Iniciar sesión** y sale "Usuario o contraseña incorrectos", espera 2–3 segundos y vuelve a pulsar **Iniciar sesión**.

Tras iniciar sesión se abrirá la ventana principal con el menú lateral (Dashboard, Empleados, Proyectos, Tareas, Clientes, Reportes, Configuración).

## 6. Si algo falla

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

## 7. Cadena de conexión

La conexión está en **WorkFlowDesk.UI → appsettings.json**:

```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=WorkFlowDeskDb;Trusted_Connection=True;MultipleActiveResultSets=true"
```

Solo necesitas cambiarla si usas otra instancia de SQL Server o otra base de datos.
