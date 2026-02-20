using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class FiltroPesquisaPagamentoProvedor
    {
        public double CodigoProvedor { get; set; }
        public int CodigoCarga { get; set; }
        public List<SituacaoLiberacaoPagamentoProvedor> SituacaoLiberacaoPagamentoProvedor { get; set; }
        public List<EtapaLiberacaoPagamentoProvedor> EtapaLiberacaoPagamentoProvedor { get; set; }
    }
}