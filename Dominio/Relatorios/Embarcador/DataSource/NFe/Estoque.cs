using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.NFe
{
    public class Estoque
    {
        #region Propriedades

        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public string CodigoProduto { get; set; }
        public string CodigoNCM { get; set; }
        public string CodigoCEST { get; set; }
        public string DescricaoStatus { get; set; }
        private CategoriaProduto Categoria { get; set; }
        public decimal UltimoCusto { get; set; }
        public decimal CustoMedio { get; set; }
        public decimal CustoMedioEstoqueAtual { get; set; }
        public decimal ValorVenda { get; set; }
        public decimal QuantidadeEstoque { get; set; }
        public decimal ValorEstoque { get; set; }
        public string Empresa { get; set; }
        public string GrupoProduto { get; set; }
        public string Marca { get; set; }
        public string LocalArmazenamento { get; set; }
        public decimal PesoBruto { get; set; }
        public decimal PesoLiquido { get; set; }
        public decimal Entradas { get; set; }
        public decimal Saidas { get; set; }
        public decimal ValorEstoqueAcumulado { get; set; }
        public decimal PesoBrutoAcumulado { get; set; }
        public decimal PesoLiquidoAcumulado { get; set; }
        private UnidadeDeMedida UnidadeMedida { get; set; }
        public string LocalArmazenamentoEstoque { get; set; }
        public decimal EstoqueDisponivel { get; set; }
        private decimal EstoqueReservado { get; set; }
        public decimal EstoqueAtual { get; set; }
        #endregion

        #region Propriedades com Regras

        public string DescricaoCategoria
        {
            get { return CategoriaProdutoHelper.ObterDescricao(Categoria); }
        }

        public string DescricaoUnidadeMedida
        {
            get { return UnidadeDeMedidaHelper.ObterDescricao(UnidadeMedida); }
        }

        public string QuantidadeEstoqueFormatado
        {
            get { return QuantidadeEstoque.ToString("n4"); }
        }

        public string EstoqueReservadoFormatado
        {
            get { return EstoqueReservado.ToString("n4"); }
        }

        #endregion
    }
}
