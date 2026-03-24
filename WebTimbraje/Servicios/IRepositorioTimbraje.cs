using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using WebTimbraje.Entidades;

namespace WebTimbraje.Servicios
{
    public interface IRepositorioTimbraje
    {
        Task<(int status, dynamic Datos)> TraeVentaFolder(string Tipo_Docto, Guid Id_Documento, string Empresa);
        Task<(int status, DocumentoDto Datos)> TraeDocumento(string Tipo_Docto, int Id_Documento,string Empresa);
        Task<(int status, NumeroEmisor Datos)> ObtieneEmisor(string Empresa);
    }

    public class RepositorioTimbraje : IRepositorioTimbraje
    {
        private readonly IConfiguration _configuration;

        public RepositorioTimbraje(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<(int status, dynamic Datos)> TraeVentaFolder(string Tipo_Docto, Guid Id_Documento, string Empresa)
        {
            if (Empresa == "Tecnobuy")
            {
                Empresa = "TB";
            }
            else if (Empresa == "ANDPAC")
            {
                Empresa = "ANDPAC";
            }
            else
            {
                Empresa = "SRV_VENTAS";
            }

            var connectionString = _configuration.GetConnectionString(Empresa);

            using var connection = new SqlConnection(connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("@Tipo_Docto", Tipo_Docto, DbType.String);
            parameters.Add("@Id_Documento", Id_Documento, DbType.Guid);
            parameters.Add("@Status", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var result = await connection.QueryFirstOrDefaultAsync(
                "Ges_Ele_XmlEnvioSII_Folder",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            int status = parameters.Get<int>("@Status");

            return (status, result);
        }

        public async Task<(int status, DocumentoDto Datos)> TraeDocumento(string Tipo_Docto, int Id_Documento, string Empresa)
        {
            string EMP;
            if (Empresa == "Tecnobuy")
            {
                EMP = "TB";
            }
            else if (Empresa == "ANDPAC")
            {
                EMP = "ANDPAC";
            }
            else
            {
                EMP = "SRV_VENTAS";
            }

            if (Empresa == "ANDPAC")
            {
                Empresa = "SALTY CO.";
            } 

            var connectionString = _configuration.GetConnectionString(EMP);

            using var connection = new SqlConnection(connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@Tipo_Docto", Tipo_Docto, DbType.String);
            parameters.Add("@Num_Documento", Id_Documento, DbType.Int32);
            parameters.Add("@Empresa", Empresa, DbType.String);
            parameters.Add("@Status", dbType: DbType.Int32, direction: ParameterDirection.Output);
            var result = await connection.QueryFirstOrDefaultAsync<DocumentoDto>(
                "Ges_Ele_TraeIdDocumento",
                parameters,
                commandType: CommandType.StoredProcedure
            );
            int status = parameters.Get<int>("@Status");
            return (status, result);
        }
        public async Task<(int status, NumeroEmisor Datos)> ObtieneEmisor(string Empresa)
        {
            string EMP;
            if (Empresa == "Tecnobuy")
            {
                EMP = "TB";
            }
            else if (Empresa == "ANDPAC")
            {
                EMP = "ANDPAC";
            }
            else
            {
                EMP = "SRV_VENTAS";
            }

            if (Empresa == "ANDPAC")
            {
                Empresa = "SALTY CO.";
            }

            var connectionString = _configuration.GetConnectionString(EMP);
            using var connection = new SqlConnection(connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@Empresa", Empresa, DbType.String);
            parameters.Add("@Status", dbType: DbType.Int32, direction: ParameterDirection.Output);
            var result = await connection.QueryFirstOrDefaultAsync<NumeroEmisor>(
                "Ges_Ele_ObtieneEmisor",
                parameters,
                commandType: CommandType.StoredProcedure
            );
            int status = parameters.Get<int>("@Status");
            return (status, result);
        }
    }
}