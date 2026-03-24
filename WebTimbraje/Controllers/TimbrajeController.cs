using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using WebTimbraje.Entidades;
using WebTimbraje.Seguridad;
using WebTimbraje.Servicios;
using static System.Net.WebRequestMethods;

namespace WebTimbraje.Controllers
{
    public class TimbrajeController : Controller
    {
        private readonly IRepositorioTimbraje repositorioTimbraje;
        private readonly HttpClient _http ;

        public TimbrajeController(IRepositorioTimbraje repositorioTimbraje, HttpClient http)
        {
            this.repositorioTimbraje = repositorioTimbraje;
            _http = http;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Timbrar(string tipo, int numero, string empresa)
        {
            var (status, datos) = await repositorioTimbraje.TraeDocumento(tipo, numero, empresa);

            if (status != 1)
            {
                return Json(new { success = false, message = "Error al consultar documento" });
            }

            if (datos == null || datos.Id_Documento == null || !Guid.TryParse(datos.Id_Documento.ToString(), out Guid guid))
            {
                return Json(new { success = false, message = "No se pudo obtener un Id_Documento válido" });
            }

            var (vstatus, vdatos) = await repositorioTimbraje.TraeVentaFolder(tipo, guid, empresa);

            if (vstatus != 1)
            {
                return Json(new { success = false, message = "Error al consultar venta folder" });
            }

            var row = (IDictionary<string, object>)vdatos;
            string? jsonEnvio = row["JSonEnvioSII"]?.ToString();

            if (string.IsNullOrWhiteSpace(jsonEnvio))
            {
                return Json(new { success = false, message = "No se encontró el JSON de envío" });
            }

            var (estatus, edatos) = await repositorioTimbraje.ObtieneEmisor(empresa);

            if (estatus != 1 || edatos == null || edatos.IdEmisor == null)
            {
                return Json(new { success = false, message = "No se pudo obtener el emisor" });
            }

            int idEmisor = edatos.IdEmisor;

            var resultado = await TimbraDocumento(idEmisor, jsonEnvio);

            if (resultado == null)
            {
                return Json(new { success = false, message = "No hubo respuesta del servicio de timbraje" });
            }

            if (resultado.Success == false)
            {
                string mensajeError = "Error al timbrar documento";

                if (!string.IsNullOrWhiteSpace(resultado.Error) && resultado.Error.Contains("{"))
                {
                    try
                    {
                        var jsonError = resultado.Error.Substring(resultado.Error.IndexOf("{"));

                        using var doc = JsonDocument.Parse(jsonError);

                        if (doc.RootElement.TryGetProperty("Mensaje", out JsonElement mensajeProp))
                        {
                            mensajeError = mensajeProp.GetString()?.Trim() ?? mensajeError;
                        }
                    }
                    catch
                    {
                        mensajeError = resultado.Error;
                    }
                }
                else if (!string.IsNullOrWhiteSpace(resultado.Error))
                {
                    mensajeError = resultado.Error;
                }

                return Json(new
                {
                    success = false,
                    message = mensajeError,
                    resultado
                });
            }

            return Json(new
            {
                success = true,
                resultado
            });
        }
        public async Task<TimbradoResult> TimbraDocumento(int empresaId, string jsonBody, CancellationToken ct = default)
        {
            try
            {
                using var req = new HttpRequestMessage(
                    HttpMethod.Post,
                    "https://api.foldererp.com/api/BoletaElectronica/Save");

                // Headers EXACTOS requeridos por FolderERP
                req.Headers.Add("Empresa_ID", empresaId.ToString());
                req.Headers.Add("Usuario_ID", "theline@thelinegroup.cl");
                req.Headers.Add("Tocken", "32dee7daf6764634810f73bd595fae6b");
                req.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

                req.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                using var resp = await _http.SendAsync(req, ct);
                var respText = await resp.Content.ReadAsStringAsync(ct);

                // ❌ Error HTTP
                if (!resp.IsSuccessStatusCode)
                {
                    return new TimbradoResult
                    {
                        Success = false,
                        Error = $"FolderERP {(int)resp.StatusCode}: {respText}"
                    };
                }

                // ✅ Deserializar respuesta
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var data = System.Text.Json.JsonSerializer.Deserialize<JsonDoctoResponse>(respText, options);

                if (data == null)
                {
                    return new TimbradoResult
                    {
                        Success = false,
                        Error = "Respuesta JSON inválida"
                    };
                }

                // 🟡 Detectar documento ya timbrado
                bool yaTimbrado =
                    !string.IsNullOrWhiteSpace(data.Mensaje) &&
                    data.Mensaje.Contains("ya existe", StringComparison.OrdinalIgnoreCase);

                // 🟢 Éxito real:
                // - Tiene Folio
                // - Tiene URL
                bool success =
                    data.FolioDTE > 0 &&
                    !string.IsNullOrWhiteSpace(data.UrlDTE);

                return new TimbradoResult
                {
                    Success = success,
                    YaTimbrado = yaTimbrado,
                    PdfUrl = data.DTEpdf,
                    Data = data,
                    Error = success ? null : data.Mensaje
                };
            }
            catch (Exception ex)
            {
                return new TimbradoResult
                {
                    Success = false,
                    Error = ex.Message
                };
            }
        }
    }
}
