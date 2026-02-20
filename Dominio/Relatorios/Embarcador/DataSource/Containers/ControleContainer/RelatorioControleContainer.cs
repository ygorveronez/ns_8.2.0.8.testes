using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Containers.ControleContainer
{
    public class RelatorioControleContainer
    {
        #region Propriedades

        public int Codigo { get; set; }
        public string CodigoCargaEmbarcador { get; set; }
        public string NumeroCargaAgrupada { get; set; }
        public string NumeroContainer { get; set; }
        public string TipoContainer { get; set; }
        private StatusColetaContainer Status { get; set; }
        private DateTime DataMovimentacao { get; set; }
        private DateTime DataColeta { get; set; }
        public DateTime DataPorto { get; set; }
        public int FreeTime { get; set; }
        public DateTime DataEmbarqueNavio { get; set; }
        public string JustificativaDescritiva { get; set; }
        public string Justificativa { get; set; }
        public int DiasEmPosse { get; set; }
        private string NumeroEXP { get; set; }
        private string NumeroEXPAgrupado { get; set; }
        private string NumeroPedido { get; set; }
        private string NumeroPedidoAgrupado { get; set; }
        private string FilialCargaAtual { get; set; }
        private string CNPJFilialCargaAtual { get; set; }
        private string CNPJFilialCargaOrigem { get; set; }
        private string FilialCargaOrigem { get; set; }
        private double LocalColeta { get; set; }
        private string ClienteLocalColeta { get; set; }
        private double LocalAtual { get; set; }
        private string ClienteLocalAtual { get; set; }
        private string NumeroBooking { get; set; }
        private string NumeroBookingAgrupada { get; set; }
        private decimal ValorDiaria { get; set; }
        public string AreaEsperaVazio { get; set; }



        #endregion Propriedades

        #region Propriedades com Regras


        public string ExcedeuFreeTime
        {
            get { return DiasExcesso > 0 ? "Sim" : "Não"; }
        }
        private int DiasExcesso
        {
            get{ return DiasEmPosse - FreeTime; }
        }

        public string ValorDevido
        {
            get {  return (DiasExcesso * ValorDiaria) > 0 ? (DiasExcesso * ValorDiaria).ToString("N2") : "0,00";  }
        }

        public string Filial
        {
            get { return !string.IsNullOrWhiteSpace(CNPJFilialCargaAtual) ? (!string.IsNullOrEmpty(CNPJFilialCargaOrigem) ? FilialCargaAtual + " (" + CNPJFilialCargaAtual + ")" + " - " + FilialCargaOrigem + " (" + CNPJFilialCargaOrigem + ")" : FilialCargaAtual + " (" + CNPJFilialCargaAtual + ")") : string.Empty; }
        }

        public string NumeroBookingValido
        {
            get { return !string.IsNullOrEmpty(NumeroBooking) ? NumeroBooking : NumeroBookingAgrupada; }
        }

        public string Pedido
        {
            get { return !string.IsNullOrEmpty(NumeroPedido) ? NumeroPedido : NumeroPedidoAgrupado;  }
        }

        public string NumeroEXPValido
        {
            get { return !string.IsNullOrEmpty(NumeroEXP) ? NumeroEXP : NumeroEXPAgrupado; }
        }

        public string DataColetaFormatada
        {
            get { return DataColeta != DateTime.MinValue ? DataColeta.ToString("dd/MM/yyyy") : string.Empty;  }
        }

        public string SituacaoContainer
        {
            get { return Status.ObterDescricao(); }
        }

        public string DataMovimentacaoFormatada
        {
            get { return DataMovimentacao != DateTime.MinValue ? DataMovimentacao.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string AreaColeta
        {
            get { return LocalColeta > 0 ? ClienteLocalColeta + "(" + LocalColeta + ")" : string.Empty; }
        }

        public string AreaAtual
        {
            get { return LocalAtual > 0 ? ClienteLocalAtual + "(" + LocalAtual + ")" : string.Empty; }
        }

        public string DataPortoFormatada
        {
            get { return DataPorto != DateTime.MinValue ? DataPorto.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataEmbarqueNavioFormatada
        {
            get { return DataEmbarqueNavio != DateTime.MinValue ? DataEmbarqueNavio.ToString("dd/MM/yyyy") : string.Empty; }
        }

        #endregion Propriedades com Regras
    }
}
