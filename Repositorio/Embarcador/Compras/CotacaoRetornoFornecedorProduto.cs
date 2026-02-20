using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Compras
{
    public class CotacaoRetornoFornecedorProduto : RepositorioBase<Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto>
    {
        public CotacaoRetornoFornecedorProduto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto> BuscarRetornosGanhadores(int codigoCotacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto>();
            var result = from obj in query where (obj.CotacaoFornecedor.Cotacao.Codigo == codigoCotacao || obj.CotacaoProduto.Cotacao.Codigo == codigoCotacao) && obj.GerarOrdemCompra == true select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto> BuscarRetornosGanhadores(int codigoCotacao, double cnpjFornecedor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto>();
            var result = from obj in query where (obj.CotacaoFornecedor.Cotacao.Codigo == codigoCotacao || obj.CotacaoProduto.Cotacao.Codigo == codigoCotacao) && obj.GerarOrdemCompra == true && obj.CotacaoFornecedor.Fornecedor.CPF_CNPJ == cnpjFornecedor select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto> BuscarRetornosPerdedores(int codigoCotacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto>();
            var result = from obj in query where (obj.CotacaoFornecedor.Cotacao.Codigo == codigoCotacao || obj.CotacaoProduto.Cotacao.Codigo == codigoCotacao) && !obj.GerarOrdemCompra select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto> BuscarPorCodigosMantidos(List<int> codigosMantidos, int codigoCotacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto>();
            var result = from obj in query where !codigosMantidos.Contains(obj.Codigo) && (obj.CotacaoFornecedor.Cotacao.Codigo == codigoCotacao || obj.CotacaoProduto.Cotacao.Codigo == codigoCotacao) select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto> BuscarPorCotacao(int codigoCotacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto>();
            var result = from obj in query where obj.CotacaoFornecedor.Cotacao.Codigo == codigoCotacao || obj.CotacaoProduto.Cotacao.Codigo == codigoCotacao select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto> BuscarPorCotacaoEProduto(int codigoCotacao, int produto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto>();
            var result = from obj in query where obj.CotacaoProduto.Cotacao.Codigo == codigoCotacao && obj.CotacaoProduto.Produto.Codigo == produto select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto> BuscarPorCotacaoEFornecedor(int codigoCotacao, double fornecedor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto>();
            var result = from obj in query
                         where
                         (
                             obj.CotacaoFornecedor.Cotacao.Codigo == codigoCotacao
                             || obj.CotacaoProduto.Cotacao.Codigo == codigoCotacao
                         )
                         && obj.CotacaoFornecedor.Fornecedor.CPF_CNPJ == fornecedor
                         select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto> BuscarPorFornecedorParaReplicar(double fornecedor, int codigoNaoReplicar)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto>();
            var result = from obj in query
                         where
                            obj.Codigo != codigoNaoReplicar
                            && obj.CotacaoFornecedor.Fornecedor.CPF_CNPJ == fornecedor
                         select obj;
            return result.ToList();
        }

        #endregion
    }
}
