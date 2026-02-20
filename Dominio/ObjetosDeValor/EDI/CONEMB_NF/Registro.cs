using System;

namespace Dominio.ObjetosDeValor.EDI.CONEMB_NF
{
    public class Registro
    {
        public string CNPJTransportador { get; set; }
        public int Numero { get; set; }
        public int Serie { get; set; }
        public string Chave { get; set; }
        public DateTime DataEmissao { get; set; }
        public decimal ValorServicoPrestado { get; set; }
        public decimal ValorTotalMercadoria { get; set; }
        public decimal ValorBaseEfeitoSeguro { get; set; }
        public decimal PesoBruto { get; set; }
        public decimal ValorImposto { get; set; }
        public int CFOP { get; set; }
        public string NumeroNegociacao { get; set; }
        public int NumeroDocumentoOriginal { get; set; }
        public int SerieDocumentoOriginal { get; set; }
        public string ChaveDocumentoOriginal { get; set; }
        public string IndicadorRedespacho { get; set; }
        public string CNPJPrimeiroOuSegundoTransportadorRedespacho { get; set; }
        public int NumeroNotaFiscal { get; set; }
        public int SerieNotaFiscal { get; set; }
        public DateTime DataEmissaoNotaFiscal { get; set; }
        public string CNPJRemetente { get; set; }
        public string CNPJDestinatario { get; set; }
        public string NumeroCarga { get; set; }
        public decimal ValorNotaFiscal { get; set; }
        public string DataChegadaMercadoriaCliente { get; set; }
        public string HoraChegadaMercadoriaCliente { get; set; }
        public string DataInicioDescarregamento { get; set; }
        public string HoraInicioDescarregamento { get; set; }
        public string DataFimDescarregamento { get; set; }
        public string HoraFimDescarregamento { get; set; }
        public string IndicadorCobrancaTaxaDescarga { get; set; }
        public string IndicadorAtrasoDevolucao { get; set; }
        public string MotivoAtrasoDevolucao { get; set; }
        public decimal ValorPedagio { get; set; }
        public string ModeloDocumento { get; set; }
        public decimal BaseCalculoICMS { get; set; }
        public string PreCTeNumeroPedido { get; set; }
        public int NumeroSequencia { get; set; }
    }
}
