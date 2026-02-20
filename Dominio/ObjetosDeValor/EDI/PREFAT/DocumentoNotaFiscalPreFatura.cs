namespace Dominio.ObjetosDeValor.EDI.PREFAT
{
    public class DocumentoNotaFiscalPreFatura
    {
        public string TipoFrete { get; set; }
        public string ModalidadeFrete { get; set; }
        public int NumeroNotaFiscal { get; set; }
        public string SerieNotaFiscal { get; set; }
        public string DataEmissaoNotaFiscal { get; set; }
        public decimal ValorMercadoriaNotaFiscal { get; set; }
        public string NumeroRomaneio { get; set; }
        public string NumeroProtocolo { get; set; }
        public decimal PesoDensidadeCubagem { get; set; }
        public string CNPJDestinatario { get; set; }
        public string NomeDestinatario { get; set; }
        public string CidadeDestinatario { get; set; }
        public decimal PesoMercadoria { get; set; }
        public string ChaveNFe { get; set; }
        public string ProtocoloNFe { get; set; }
    }
}
