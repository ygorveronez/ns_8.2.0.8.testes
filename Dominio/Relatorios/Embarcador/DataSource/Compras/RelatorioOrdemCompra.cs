using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Compras
{
    public class RelatorioOrdemCompra
    {
        #region Propriedades

        public int Numero { get; set; }

        public string Fornecedor { get; set; }

        private DateTime DataGeracaoInicio { get; set; }
        private DateTime DataGeracaoFim { get; set; }

        private DateTime DataPrevisaoInicio { get; set; }
        private DateTime DataPrevisaoFim { get; set; }

        public string Transportador { get; set; }

        public SituacaoOrdemCompra Situacao { get; set; }

        public string Produto { get; set; }

        public string Operador { get; set; }

        public decimal Quantidade { get; set; }

        public decimal ValorUnitario { get; set; }

        public decimal ValorTotal { get; set; }

        public string Motivo { get; set; }

        public string Veiculo { get; set; }

        public string CondicaoPagamento { get; set; }
        public string Aprovador { get; set; }

        #endregion

        #region Propriedades com Regras

        public string SituacaoDescricao
        {
            get { return Situacao.ObterDescricao(); }
        }

        public string DataGeracaoInicioFormatada
        {
            get { return DataGeracaoInicio != DateTime.MinValue ? DataGeracaoInicio.ToString("dd/MM/yyyy") : string.Empty; }
        }
        public string DataGeracaoFimFormatada
        {
            get { return DataGeracaoFim != DateTime.MinValue ? DataGeracaoFim.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataPrevisaoInicioFormatada
        {
            get { return DataPrevisaoInicio != DateTime.MinValue ? DataPrevisaoInicio.ToString("dd/MM/yyyy") : string.Empty; }
        }
        public string DataPrevisaoFimFormatada
        {
            get { return DataPrevisaoFim != DateTime.MinValue ? DataPrevisaoFim.ToString("dd/MM/yyyy") : string.Empty; }
        }

        #endregion
    }
}
