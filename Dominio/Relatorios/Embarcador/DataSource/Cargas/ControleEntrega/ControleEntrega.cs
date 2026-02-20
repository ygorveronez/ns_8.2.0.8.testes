using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega
{
    public class ControleEntrega
    {
        public int Codigo { get; set; }
        public int NumeroCTe { get; set; }
        public string NumeroCarga { get; set; }
        private DateTime DataEmissaoCTe { get; set; }
        private string NomeRemetente { get; set; }
        private string CNPJRemetente { get; set; }
        private string CidadeRemetente { get; set; }
        private string UFRemetente { get; set; }
        private string CidadeOrigem { get; set; }
        private string UFOrigem { get; set; }
        private string NomeDestinatario { get; set; }
        private string CNPJDestinatario { get; set; }
        private string CidadeDestinatario { get; set; }
        private string UFDestinatario { get; set; }
        private string CidadeDestino { get; set; }
        private string UFDestino { get; set; }
        public string Notas { get; set; }
        public string Veiculos { get; set; }
        public string Motoristas { get; set; }
        public int NumeroPallets { get; set; }
        private DateTime DataAgendamento { get; set; }
        public string GrupoPessoaCarga { get; set; }
        public string Observacao { get; set; }
        private DateTime DataOcorrencia { get; set; }
        public string DescricaoOcorrencia { get; set; }
        public string DescricaoCentroResultado { get; set; }
        public string NumeroPedidoEmbarcador { get; set; }
        public DateTime DataPrevisaoColeta { get; set; }
        public DateTime DataPrevisaoEntrega { get; set; }
        public string SenhaAgendamento { get; set; }
        public string ObservacaoOcorrencia { get; set; }
        public string DataEmissaoCTeFormatada
        {
            get { return DataEmissaoCTe != DateTime.MinValue ? DataEmissaoCTe.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string Remetente
        {
            get { return $"{NomeRemetente} ({CNPJRemetente.ObterCpfOuCnpjFormatado()})"; }
        }

        public string Destinatario
        {
            get { return $"{NomeDestinatario} ({CNPJDestinatario.ObterCpfOuCnpjFormatado()})"; }
        }

        public string LocalidadeRemetente
        {
            get { return !string.IsNullOrWhiteSpace(CidadeRemetente) ? $"{CidadeRemetente} - {UFRemetente}" : string.Empty; }
        }

        public string LocalidadeDestinatario
        {
            get { return !string.IsNullOrWhiteSpace(CidadeDestinatario) ? $"{CidadeDestinatario} - {UFDestinatario}" : string.Empty; }
        }

        public string Origem
        {
            get { return !string.IsNullOrWhiteSpace(CidadeOrigem) ? $"{CidadeOrigem} - {UFOrigem}" : string.Empty; }
        }

        public string Destino
        {
            get { return !string.IsNullOrWhiteSpace(CidadeDestino) ? $"{CidadeDestino} - {UFDestino}" : string.Empty; }
        }

        public string DataAgendamentoFormatada
        {
            get { return DataAgendamento != DateTime.MinValue ? DataAgendamento.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataOcorrenciaFormatada
        {
            get { return DataOcorrencia != DateTime.MinValue ? DataOcorrencia.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataPrevisaoColetaFormatada
        {
            get { return DataPrevisaoColeta != DateTime.MinValue ? DataPrevisaoColeta.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataPrevisaoEntregaFormatada
        {
            get { return DataPrevisaoEntrega != DateTime.MinValue ? DataPrevisaoEntrega.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }
    }
}
