using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Rest.NFe
{
    public class ProtocoloListaNotasFiscais
    {
        public WebService.Carga.Protocolos Protocolo { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> ListaNotasFiscais { get; set; }
    }
}
