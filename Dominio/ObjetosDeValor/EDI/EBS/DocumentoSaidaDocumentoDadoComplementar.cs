namespace Dominio.ObjetosDeValor.EDI.EBS
{
    public class DocumentoSaidaDocumentoDadoComplementar
    {
        public decimal ValorMercadorias { get; set; }
        public decimal Desconto { get; set; }
        public decimal Frete { get; set; }
        public decimal Despesas { get; set; }
        public decimal Seguro { get; set; }
        public decimal PesoBruto { get; set; }
        public decimal PesoLiquido { get; set; }
        public string CPFCNPJTransportador { get; set; }
        public int MeioTransporte { get; set; }
        public string Placa { get; set; }
        public int Volumes { get; set; }
        public string Especie { get; set; }
        public int NumeroRE { get; set; }
        public int NumeroDespacho { get; set; }
        public int PaisDestino { get; set; }
        public int Moeda { get; set; }
        public string DataDespacho { get; set; }
        public decimal ValorDespacho { get; set; }
        public string CPFCNPJRemetente { get; set; }
        public string UFDestino { get; set; }
        public string IdentificacaoExteriorRemetente { get; set; }
        public string Redespacho { get; set; }
        public decimal INSSRetido { get; set; }
        public decimal FUNRURALRetido { get; set; }
        public string ChaveNFe { get; set; }
        public string ISSRetido { get; set; }
        public string ISSDevidoPrestacao { get; set; }
        public string UFPrestacao { get; set; }
        public string MunicipioPrestacao { get; set; }
        public string UFOrigem { get; set; }
        public int CodigoIBGEOrigem { get; set; }
        public string ICMSSTRetidoAntecipadamente { get; set; }
        public string IEDestinatario { get; set; }
        public string TipoAssinanteTelecom { get; set; }
        public string TipoUtilizacaoTelecom { get; set; }
        public string NumeroTerminalTelecom { get; set; }
        public string NumeroFaturaTelecom { get; set; }
        public string CPFCNPJConsignatario { get; set; }
        public string ChaveCTeReferencia { get; set; }
        public int Sequencia { get; set; }
    }
}
