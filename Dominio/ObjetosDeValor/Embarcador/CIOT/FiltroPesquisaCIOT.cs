using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT
{
    public class FiltroPesquisaCIOT
    {
        public string NumeroCarga { get; set; }
        public double? CPFCNPJTransportador { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT? Situacao { get; set; }
        public bool? SelecionarTodos { get; set; }
        public List<int> ListaCodigosCIOT { get; set; }
        public string NumeroPedidoEmbarcador { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutorizacaoPagamentoCIOTParcela? TipoAutorizacaoPagamentoCIOTParcela { get; set; }
        public int CodigoContratoTerceiro { get; set; }
    }
}
