using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.Produtos
{
    public class ProdutoComEstoqueAgrupado
    {
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public UnidadeDeMedida UnidadeDeMedida { get; set; }
        public string CodigoProduto { get; set; }
        public string CodigoNCM { get; set; }
        public string CodigoBarrasEAN { get; set; }
        public string Status { get; set; }
        public decimal Estoque { get; set; }
        public string UnidadeMedidaFormatada
        {
            get { return UnidadeDeMedida.ObterSigla(); }
        }
    }
}
