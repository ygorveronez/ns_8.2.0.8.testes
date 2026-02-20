using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Containers.HistoricoMovimentacaoContainers
{
    public class HistoricoMovimentacaoContainers
    {
        #region Propriedades

        public int Codigo { get; set; }
        public string CodigoContainer { get; set; }
        private StatusColetaContainer SituacaoContainer { get; set; }
        public string Carga { get; set; }
        public string Auditado { get; set; }
        public string Nome { get; set; }
        public double CPF_CNPJ { get; set; }
        public string Tipo { get; set; }
        public DateTime DataHistorico { get; set; }
        private DateTime DataFimHistorico { get; set; }
        private OrigemMovimentacaoContainer Origem { get; set; }
        private InformacaoOrigemMovimentacaoContainer InformacaoOrigem { get; set; }

        #endregion

        #region Propriedades com Regras

        public string SituacaoContainerDescricao
        {
            get { return SituacaoContainer.ObterDescricao(); }
        }

        public string OrigemDescricao
        {
            get { return Origem.ObterDescricao(); }
        }

        public string InformacaoOrigemDescricao
        {
            get { return InformacaoOrigem.ObterDescricao(); }
        }

        public string NomeCNPJ
        {
            get
            {
                string descricao = "";

                if (!string.IsNullOrWhiteSpace(this.Nome))
                    descricao += this.Nome;
                if (!string.IsNullOrWhiteSpace(this.Tipo))
                    descricao += " - " + this.CPF_CNPJ_Formatado;

                return descricao;
            }
        }

        private string CPF_CNPJ_Formatado
        {
            get
            {
                if (this.Tipo.Equals("E"))
                    return "00.000.000/0000-00";
                else
                    return this.Tipo.Equals("J") ? String.Format(@"{0:00\.000\.000\/0000\-00}", this.CPF_CNPJ) : String.Format(@"{0:000\.000\.000\-00}", this.CPF_CNPJ);               
            }
        }

        public string TempoHistorico
        {
            get
            {
                return this.DataFimHistorico != DateTime.MinValue ? FormatarTempo(this.DataFimHistorico - this.DataHistorico) : "";
            }
        }

        #endregion

        #region Metódos

        public string FormatarTempo(TimeSpan tempo)
        {
            string formato = String.Empty;
            if (tempo.Days > 0)
            {
                formato = $"{tempo.Days}d";
            }
            else if (tempo.Days < 0)
            {
                formato = $"{tempo.Days * -1}d";
            }
            formato += tempo.ToString(@"hh\:mm");
            return formato;
        }

        #endregion
    }
}
