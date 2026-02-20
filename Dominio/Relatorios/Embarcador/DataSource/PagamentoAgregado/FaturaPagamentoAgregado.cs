using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado
{
    public class FaturaPagamentoAgregado
    {
        #region Propriedades

        public int Numero { get; set; }
        public string NumeroFatura { get; set; }
        private DateTime DataPagamento { get; set; }
        private DateTime DataGeracao { get; set; }

        public string NomeTomadorFatura { get; set; }
        public double CNPJTomadorFatura { get; set; }

        public string NomeCliente { get; set; }
        public double CNPJCliente { get; set; }
        public string EnderecoCliente { get; set; }
        public string BairroCliente { get; set; }
        public string CidadeCliente { get; set; }
        public string EstadoCliente { get; set; }
        public string CEPCliente { get; set; }
        public string CodigoProvedorCliente { get; set; }

        public decimal Valor { get; set; }
        public string Observacao { get; set; }
        public string AgenciaCliente { get; set; }
        public string DigitoAgenciaCliente { get; set; }
        public string ContaCliente { get; set; }
        public string BancoCliente { get; set; }
        public Mes CompetenciaMes { get; set; }
        public Quinzena CompetenciaQuinzena { get; set; }
        public string DescricaoCompetencia { get; set; }

        #endregion

        #region Propriedades com Regras

        public string DataPagamentoFormatada
        {
            get { return DataPagamento != DateTime.MinValue ? DataPagamento.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataGeracaoFormatada
        {
            get { return DataGeracao != DateTime.MinValue ? DataGeracao.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string CompetenciaMesFormatado
        {
            get { return CompetenciaMes.ObterDescricao(); }
        }

        public string CompetenciaQuinzenaFormatada
        {
            get { return CompetenciaQuinzena.ObterDescricao(); }
        }

        public string CompetenciaAno
        {
            get { return DataGeracao != DateTime.MinValue ? DataGeracao.ToString("yyyy") : string.Empty; }
        }

        #endregion
    }
}
