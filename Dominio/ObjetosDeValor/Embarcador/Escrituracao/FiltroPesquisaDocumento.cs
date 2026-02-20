using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Escrituracao
{
    public class FiltroPesquisaDocumento
    {
        public int CodigoPagamento { get; set; }
        public int ModeloDocumentoFiscal { get; set; }
        public int TipoOperacaoDocumento { get; set; }
        public DateTime DataFim { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataInicialEmissao { get; set; }
        public DateTime DataFinalEmissao { get; set; }
        public int CodigoCarga { get; set; }
        public int CodigoOcorrencia { get; set; }
        public bool PagamentoLiberado { get; set; }
        public bool PagamentoFinalizados { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento SituacaoPagamentoDocumento { get; set; }
        public Enumeradores.TipoDocumentoGerado? TipoDocumentoGerado { get; set; }
        public Enumeradores.SituacaoDocumentoPagamento? SituacaoDocumentoPagamento { get; set; }
        public bool ConsultarPorDataEmissao { get; set; }
        public DateTime DataContabilizacaoComplemento { get; set; }
        public bool SomenteComDocumentosDesbloqueados { get; set; }
        public bool NaoGerarAutomaticamenteLotesCancelados { get; set; }
        public List<int> CodigosTransportador { get; set; }
        public List<int> CodigosFilial { get; set; }
        public List<double> CodigosTomador { get; set; }
        public List<int> CodigosTiposOperacao { get; set; }
        public bool ConsiderarHorasRetroativas { get; set; }
        public bool SomenteComProvisaoGerada { get; set; }

    }
}
