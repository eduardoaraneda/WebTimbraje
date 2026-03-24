using WebTimbraje.Entidades;

namespace WebTimbraje.Seguridad
{
    public class TimbradoResult
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public JsonDoctoResponse Data { get; set; }

        public string PdfUrl { get; set; }
        public bool YaTimbrado { get; set; }
    }
}
