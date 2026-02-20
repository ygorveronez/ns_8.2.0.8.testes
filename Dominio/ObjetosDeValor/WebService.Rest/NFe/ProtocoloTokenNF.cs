using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Rest.NFe
{
    public class ProtocoloTokenNF
    {

        public WebService.Carga.Protocolos Protocolo { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.NFe.TokenNF> TokensXMLNotasFiscais { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Carga.Averbacao Averbacao { get; set; }
        public Dominio.ObjetosDeValor.MDFe.ValePedagio ValePedagio { get; set; }
        public Dominio.ObjetosDeValor.MDFe.CIOT Ciot { get; set; }
        public Dominio.ObjetosDeValor.MDFe.InformacoesPagamentoPedido InformacoesPagamento { get; set; }
    }
}
