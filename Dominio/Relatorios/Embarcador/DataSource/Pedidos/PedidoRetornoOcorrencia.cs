using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Pedidos
{
    public class PedidoRetornoOcorrencia
    {
        public int Codigo { get; set; }
        public string NumeroPedido { get; set; }
        public string TipoOcorrencia { get; set; }
        public string GrupoOcorrencia { get; set; }
        private DateTime DataOcorrencia { get; set; }
        public string NumeroCarga { get; set; }
        public string NotaFiscal { get; set; }
        private string RazaoFilial { get; set; }
        private string CNPJFilial { get; set; }
        private string RazaoTransportador { get; set; }
        private string CNPJTransportador { get; set; }
        public string CidadeDestino { get; set; }
        public string UFDestino { get; set; }
        public string TipoCarga { get; set; }
        public decimal ValorNF { get; set; }

        public string DataOcorrenciaFormatada
        { 
            get { return DataOcorrencia != DateTime.MinValue ? DataOcorrencia.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string Filial
        {
            get { return !string.IsNullOrWhiteSpace(RazaoFilial) ? $"{RazaoFilial} ({CNPJFilial.ObterCnpjFormatado()})" : string.Empty; }
        }

        public string Transportador
        {
            get { return !string.IsNullOrWhiteSpace(RazaoTransportador) ? $"{RazaoTransportador} ({CNPJTransportador.ObterCnpjFormatado()})" : string.Empty; }
        }

    }
}
