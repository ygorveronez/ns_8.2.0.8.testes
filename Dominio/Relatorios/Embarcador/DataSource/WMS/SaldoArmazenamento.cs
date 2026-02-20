using System;

namespace Dominio.Relatorios.Embarcador.DataSource.WMS
{
    public sealed class SaldoArmazenamento
    {
        #region Propriedades

        public string ProdutoEmbarcador { get; set; }
        public string Deposito { get; set; }
        public string Bloco { get; set; }
        public string Posicao { get; set; }
        public string Rua { get; set; }
        public string Abreviacao { get; set; }
        public string CodigoBarras { get; set; }
        public string Descricao { get; set; }
        private DateTime DataVencimento { get; set; }
        public string TipoRecebimento { get; set; }
        public decimal QuantidadeLote { get; set; }
        public decimal QuantidadeAtual { get; set; }
        public string UnidadeMedida { get; set; }

        #endregion

        #region  Propriedades com Regras

        public string DataVencimentoFormatada
        {
            get { return DataVencimento != DateTime.MinValue ? DataVencimento.ToString("dd/MM/yyyy") : ""; }
        }

        #endregion
    }
}
