using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.WebService.CTe
{
    public class RequestCtePorCarga
    {
        public int  ProtocoloCarga { get; set; }
        public TipoDocumentoRetorno TipoDocumentoRetorno { get; set; }
        public  int Inicio { get; set; }
        public  int Limite { get; set; }
    }
}
