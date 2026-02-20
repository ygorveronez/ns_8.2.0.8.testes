using System;

namespace Dominio.ObjetosDeValor.EDI.DOCCOB
{
    public class NotaFiscalCobrancaConhecimento
    {
        public string Serie { get; set; }
        public int Numero { get; set; }
        public DateTime DataEmissao { get; set; }
        public decimal Peso { get; set; }
        public decimal PesoDensidadeCubagem { get; set; }
        public decimal ValorMercadoria { get; set; }
        public string CNPJEmissorNotaFiscal { get; set; }
        public string NumeroRomaneio { get; set; }
        public string NumeroPedido { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Destinatario { get; set; }
        public string Chave { get; set; }
        public string Protocolo { get; set; }
        /// <summary>
        /// Número do protocolo do cliente
        /// </summary>
        public string NumeroProtocolo { get; set; }
        public string Filler { get; set; }
    }
}