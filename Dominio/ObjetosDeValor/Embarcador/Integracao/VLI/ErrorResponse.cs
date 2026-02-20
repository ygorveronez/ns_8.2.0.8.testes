using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.VLI
{
    public class ErrorResponse
    {
        public string result { get; set; }
        public List<Error> errors { get; set; }
        public string status { get; set; }
    }

    public class Error
    {
        public string type { get; set; }
        public string message { get; set; }
    }
}