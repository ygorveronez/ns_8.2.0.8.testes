using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Pedidos
{
    public class PedidoOcorrencia
    {
        public int Codigo { get; set; }
        public string NumeroPedido { get; set; }
        public string TipoOcorrencia { get; set; }
        public DateTime DataOcorrencia { get; set; }
        public string Observacao { get; set; }
        private string RemetenteNome { get; set; }
        private double RemetenteCPFCNPJ { get; set; }
        private string DestinatarioNome { get; set; }
        private double DestinatarioCPFCNPJ { get; set; }
        public string NumeroCarga { get; set; }
        public string NotasFiscais { get; set; }
        private DateTime DataPedido { get; set; }
        private string RazaoFilial { get; set; }
        private string CNPJFilial { get; set; }
        private string RazaoTransportador { get; set; }
        private string CNPJTransportador { get; set; }
        public string Motoristas { get; set; }
        public string Veiculo { get; set; }
        public string NaturezaOP { get; set; }
        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega SituacaoEntrega { get; set; }
        public string DataOcorrenciaFormatada 
        { 
            get { return DataOcorrencia != DateTime.MinValue ? DataOcorrencia.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string Remetente
        { 
            get { return !string.IsNullOrWhiteSpace(RemetenteNome) ? $"{RemetenteNome} ({RemetenteCPFCNPJ.ToString().ObterCpfOuCnpjFormatado()})" : string.Empty; } 
        }

        public string Destinatario
        {
            get { return !string.IsNullOrWhiteSpace(DestinatarioNome) ? $"{DestinatarioNome} ({DestinatarioCPFCNPJ.ToString().ObterCpfOuCnpjFormatado()})" : string.Empty; }
        }

        public string DataPedidoFormatada 
        { 
            get { return DataPedido != DateTime.MinValue ? DataPedido.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string Filial
        {
            get { return !string.IsNullOrWhiteSpace(RazaoFilial) ? $"{RazaoFilial} ({CNPJFilial.ObterCnpjFormatado()})" : string.Empty; }
        }

        public string Transportador
        {
            get { return !string.IsNullOrWhiteSpace(RazaoTransportador) ? $"{RazaoTransportador} ({CNPJTransportador.ObterCnpjFormatado()})" : string.Empty; }
        }

        public string SituacaoEntregaDescricao 
        { 
            get { return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntregaHelper.ObterDescricao(SituacaoEntrega); }
        }

    }
}
