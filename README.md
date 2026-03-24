# WebTimbraje

Proyecto ASP.NET (Razor Pages / MVC) para integración con FolderERP para timbraje de documentos electrónicos.

Descripción
- Aplicación web en .NET 10 que consulta documentos desde un repositorio y envía el JSON al servicio de FolderERP para generar/firmar documentos electrónicos.

Requisitos
- .NET 10 SDK
- Visual Studio 2022/2026 o VS Code
- Conexión a la base de datos configurada en `appsettings.*.json` (se usa Dapper y Microsoft.Data.SqlClient)

Instalación y ejecución
1. Clona el repositorio:
   ```powershell
   git clone <repo-url>
   cd WebTimbraje
   ```
2. Restaurar paquetes y compilar:
   ```powershell
   dotnet restore
   dotnet build
   ```
3. Ejecutar en modo desarrollo:
   ```powershell
   dotnet run --project WebTimbraje
   ```
   La aplicación estará disponible en https://localhost:5001 (o el puerto que indique la salida).

Configuración
- `WebTimbraje.csproj` targeting `net10.0`.
- No incluir archivos de compilación en el control de versiones: se añadió `.gitignore` para ignorar `bin/` y `obj/`.
- Ajusta `appsettings.Development.json` con la cadena de conexión y otros valores locales. Este archivo está en `.gitignore` por seguridad.

Uso
- Interfaz principal: `Views/Timbraje/Index.cshtml`.
- Endpoint para timbrar (POST): `/Timbraje/Timbrar` con parámetros: `tipo`, `numero`, `empresa`.
- Lógica principal del envío a FolderERP en `TimbrajeController.TimbraDocumento`.

Buenas prácticas
- No subir secretos ni archivos de configuración locales (`appsettings.Development.json` está ignorado).
- Reutilizar `HttpClient` (ya se inyecta en el controlador).

Contribuir
- Abrir un issue para bugs o mejoras.
- Enviar pull requests contra la rama `main`.

Licencia
- Añadir aquí la licencia del proyecto (por ejemplo MIT) si procede.

Contacto
- Mantén los datos de contacto y credenciales fuera del repositorio.
