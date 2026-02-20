using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Rest.Unilever.NFe
{
    public class ProtocoloCargaListaChave
    {
        public WebService.Carga.Protocolos Protocolo { get; set; }
        public List<Embarcador.NFe.NotaFiscalChave> ListaNotasFiscaisChaves { get; set; }
           
    }
}
