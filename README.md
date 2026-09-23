# Web Timbraje

Aplicación ASP.NET Core MVC que consulta documentos comerciales en SQL Server y prepara su envío al servicio de FolderERP para el flujo de timbraje de documentos electrónicos.

## Tecnologías

C# · .NET 10 · ASP.NET Core MVC · Razor · SQL Server · Dapper · Microsoft.Data.SqlClient · HttpClient · JSON.

## Flujo y organización

1. `TimbrajeController` recibe los datos del documento y empresa.
2. `Servicios/` consulta documentos y datos del emisor mediante procedimientos almacenados.
3. El controlador utiliza la integración con FolderERP para procesar el envío.

El proyecto web está en `WebTimbraje/`; la solución es `WebTimbraje.slnx`.

## Compilar y ejecutar

Con el SDK de .NET 10 instalado, desde la raíz:

```powershell
dotnet restore WebTimbraje.slnx
dotnet build WebTimbraje.slnx
dotnet run --project WebTimbraje/WebTimbraje.csproj
```

Abre la URL de la terminal; la ruta inicial es `/Timbraje/Index`.

## Dependencias externas

La compilación no equivale a una integración operativa. Necesitas las bases SQL, procedimientos almacenados, datos del emisor y acceso autorizado a FolderERP. El repositorio no contiene un entorno de pruebas autónomo ni el esquema completo de las bases.

El repositorio de datos selecciona conexiones por empresa, con claves como `ConnectionStrings:TB`, `ConnectionStrings:ANDPAC` y `ConnectionStrings:SRV_VENTAS`. Configúralas mediante variables de entorno usando doble guion bajo, por ejemplo `ConnectionStrings__TB`.

Revisa el destino y la configuración de la integración antes de ejecutar envíos. Para una demostración utiliza un entorno de pruebas autorizado; no envíes documentos reales como prueba del portafolio.

## Configuración y alcance

Usa una base de desarrollo y credenciales propias. Configura secretos mediante variables de entorno o User Secrets; no los incluyas en commits. La compilación no comprueba la disponibilidad de bases de datos, SMTP o APIs externas.

## Autor

[Eduardo Araneda](https://github.com/eduardoaraneda) · [Portafolio](https://eduardoaraneda.github.io/Portafolio/)
