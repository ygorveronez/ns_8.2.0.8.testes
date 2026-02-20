using System;

namespace Dominio.Relatorios.Embarcador.DataSource.CTe
{
    public class ComissaoVendedorCTe
    {
        public int Codigo { get; set; }
        private DateTime DataEmissao { get; set; }
        private string NomeRemetente { get; set; }
        private string CNPJRemetente { get; set; }
        private string NomeDestinatario { get; set; }
        private string CNPJDestinatario { get; set; }
        private string NomeTomador { get; set; }
        private string CNPJTomador { get; set; }
        public string GrupoPessoa { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorAReceber { get; set; }
        public decimal PercentualComissao { get; set; }
        public string Vendedor { get; set; }
        public decimal ValorComissao { get; set; }

        public string DataEmissaoFormatada 
        {
            get { return DataEmissao != DateTime.MinValue ? DataEmissao.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string Remetente 
        {
            get { return !string.IsNullOrWhiteSpace(NomeRemetente) ? $"{NomeRemetente} ({CNPJRemetente.ObterCpfOuCnpjFormatado()})" : string.Empty; }
        }

        public string Destinatario
        {
            get { return !string.IsNullOrWhiteSpace(NomeDestinatario) ? $"{NomeDestinatario} ({CNPJDestinatario.ObterCpfOuCnpjFormatado()})" : string.Empty; }
        }

        public string Tomador
        {
            get { return !string.IsNullOrWhiteSpace(NomeTomador) ? $"{NomeTomador} ({CNPJTomador.ObterCpfOuCnpjFormatado()})" : string.Empty; }
        }
    }
}
