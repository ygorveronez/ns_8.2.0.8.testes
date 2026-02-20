using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class RateioDespesaVeiculo
    {
        #region Propriedades

        public int Codigo { get; set; }
        private DateTime DataInicial { get; set; }
        private DateTime DataFinal { get; set; }
        public string NumeroDocumento { get; set; }
        public string TipoDocumento { get; set; }
        public string GrupoDespesa { get; set; }
        public string TipoDespesa { get; set; }
        public decimal Valor { get; set; }
        public string Segmentos { get; set; }
        public string Veiculos { get; set; }
        public string CentrosResultados { get; set; }
        public string Pessoa { get; set; }
        private DateTime DataLancamento { get; set; }
        private OrigemRateioDespesaVeiculo OrigemRateio { get; set; }

        #endregion

        #region Propriedades com Regras

        public string DataInicialFormatada
        {
            get { return DataInicial != DateTime.MinValue ? DataInicial.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataFinalFormatada
        {
            get { return DataFinal != DateTime.MinValue ? DataFinal.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataLancamentoFormatada
        {
            get { return DataLancamento != DateTime.MinValue ? DataLancamento.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string OrigemRateioFormatada
        {
            get { return OrigemRateio.ObterDescricao(); }
        }

        #endregion
    }
}