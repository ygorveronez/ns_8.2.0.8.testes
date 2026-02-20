using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class FiltroPesquisaRelatorioLiberacaoPagamentoProvedor
    {
        public DateTime DataCargaInicial { get; set; }
        public DateTime DataCargaFinal { get; set; }
        public string NumeroCarga { get; set; }
        public string NumeroOS { get; set; }
        public int CodigoTipoOperacao { get; set; }
        public double CodigoProvedor { get; set; }
        public int CodigoFilialEmissora { get; set; }
        public double CodigoTomador { get; set; }
        public SituacaoLiberacaoPagamentoProvedor SituacaoLiberacaoPagamentoProvedor { get; set; }
        public TipoDocumentoProvedor TipoDocumentoProvedor { get; set; }
        public OpcaoSimNaoPesquisa IndicacaoLiberacaoOK { get; set; }
        public EtapaLiberacaoPagamentoProvedor EtapaLiberacaoPagamentoProvedor { get; set; }
    }
}
