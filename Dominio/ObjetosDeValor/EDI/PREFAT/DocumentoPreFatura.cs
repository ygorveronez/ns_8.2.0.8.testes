using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.PREFAT
{
    public class DocumentoPreFatura
    {
        public string CNPJEmpresaEmissoraDocumento { get; set; }
        public string SerieDocumentoEmbarcador { get; set; }
        public string IdentificacaoDocumentoEmbarcador { get; set; }
        public string DataEmissaoDocumentoEmbarcador { get; set; }
        public string SerieConhecimentoTransportadora { get; set; }
        public string NumeroConhecimentoTransportadora { get; set; }
        public string DataEmissaoConhecimento { get; set; }
        public string CPFCNPJLocalOrigemTransporte { get; set; }
        public string CPFCNPJLocalDestinoTransporte { get; set; }
        public string IndicacaoCNPJCPFDestino { get; set; }
        public decimal ValorTotalFreteEmbarcador { get; set; }
        public decimal ValorTotalFreteConhecimento { get; set; }
        public string IndicadorTipoDiferencaValor { get; set; }
        public decimal ValorDiferencaFretes { get; set; }
        public string NumeroProtocolo { get; set; }
        public string Filler { get; set; }
        public List<DocumentoNotaFiscalPreFatura> NotasFiscais { get; set; }
        public ValorEmbarcador ValorCalculadoEmbarcador { get; set; }
    }
}
