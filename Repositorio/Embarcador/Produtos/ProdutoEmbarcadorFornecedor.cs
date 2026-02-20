using System.Linq;

namespace Repositorio.Embarcador.Produtos
{
    public class ProdutoEmbarcadorFornecedor : RepositorioBase<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFornecedor>
    {
        #region Constructores

        public ProdutoEmbarcadorFornecedor(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFornecedor BuscarPorCodigoInternoEFornecedor(string codigoInterno, double cnpjFornecedor)
        {
            var consultaProdutoEmbarcadorFornecedor = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFornecedor>()
                .Where(o => o.CodigoInterno == codigoInterno && o.Fornecedor.CPF_CNPJ == cnpjFornecedor);

            return consultaProdutoEmbarcadorFornecedor.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFornecedor BuscarPorProdutoCodigoInternoEFornecedor(int codigoProduto, string codigoInterno, double cnpjFornecedor, int codigoFilial)
		{
            var consultaProdutoEmbarcadorFornecedor = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFornecedor>()
                .Where(o => o.ProdutoEmbarcador.Codigo == codigoProduto && o.CodigoInterno == codigoInterno && o.Fornecedor.CPF_CNPJ == cnpjFornecedor && o.Filial.Codigo == codigoFilial);

            return consultaProdutoEmbarcadorFornecedor.FirstOrDefault();
        }

        #endregion Métodos Públicos
    }
}
