using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.PREFAT
{
    public class PreFatura
    {
        public string IdentificacaoPreFatura { get; set; }
        public string DataEmissaoPreFatura { get; set; }
        public string DataPagamentoPreFatura { get; set; }
        public string CNPJCliente { get; set; }
        public string NomeCliente { get; set; }
        public string CodigoDeposito { get; set; }
        public string CodigoTransportadora { get; set; }
        public string CNPJTransportadora { get; set; }
        public string TipoFrete { get; set; }
        public string ModalidadeFrete { get; set; }
        public string NumeroPreFatura { get; set; }
        public string DataInicialPreFatura { get; set; }
        public string DataFinalPreFatura { get; set; }
        public decimal ValorBloqueioAcumulado { get; set; }
        public decimal ValorDesbloqueio { get; set; }
        public string CNPJResponsavelFrete { get; set; }
        public int QuantidadeDocumentosPreFatura { get; set; }
        public decimal ValorTotalPreFatura { get; set; }
        public string AcaoDocumento { get; set; }
        public string Filler { get; set; }
        public List<DocumentoPreFatura> Documentos { get; set; }
    }
}
