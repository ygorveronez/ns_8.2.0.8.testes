using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz
{
    public class ErrorResponse
    {
        public string result { get; set; }
        public List<Error> errors { get; set; }
        public string status { get; set; }
    }
}
