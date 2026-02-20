using System.Collections.Generic;
using System.Collections.Specialized;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.RepomFrete
{
    public class retPadrao
    {
        public Response Response { get; set; }
        public List<Errors> Errors { get; set; } = new List<Errors>(); 
        public string Message { get; set; }
    }

    public class Response
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
    }

    public class Errors
    {
        public int ErrorCode { get; set; }
        public string Message { get; set; }
    }
}