using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Pedidos
{
    public class PedidoDevolucao
    {
        public int Codigo { get; set; }
        public int NumeroNotaFiscal { get; set; }
        private DateTime DataEmissaoNotaFiscal { get; set; }
        public decimal ValorTotalNotaFiscal { get; set; }
        private string NomeDestinatario { get; set; }
        private double CNPJDestinatario { get; set; }
        private string CidadeDestino { get; set; }
        private string UFDestino { get; set; }
        private string RazaoTransportador { get; set; }
        private string CNPJTransportador { get; set; }
        public string TipoDevolucao { get; set; }
        public string Motivo { get; set; }
        public int ISISReturn { get; set; }
        private DateTime DataEntrega { get; set; }
        public int QuantidadeVolumes { get; set; }
        public int QuantidadePecas { get; set; }
        public int NFD { get; set; }
        public decimal ValorNFD { get; set; }

        public string DataEmissaoNotaFiscalFormatada
        {
            get { return DataEmissaoNotaFiscal != DateTime.MinValue ? DataEmissaoNotaFiscal.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataEntregaFormatada
        {
            get { return DataEntrega != DateTime.MinValue ? DataEntrega.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string Destinatario
        {
            get { return !string.IsNullOrWhiteSpace(NomeDestinatario) ? $"{NomeDestinatario} ({CNPJDestinatario.ToString().ObterCpfOuCnpjFormatado()})" : string.Empty; }
        }

        public string Destino
        {
            get { return !string.IsNullOrEmpty(CidadeDestino) ? $"{CidadeDestino} - {UFDestino}" : string.Empty; }
        }

        public string Transportador
        {
            get { return !string.IsNullOrWhiteSpace(RazaoTransportador) ? $"{RazaoTransportador} ({CNPJTransportador.ObterCpfOuCnpjFormatado()})" : string.Empty; }
        }
    }
}
