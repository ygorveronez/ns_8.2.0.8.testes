using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Compras
{
    public class CotacaoFornecedor : RepositorioBase<Dominio.Entidades.Embarcador.Compras.CotacaoFornecedor>
    {
        public CotacaoFornecedor(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Compras.CotacaoFornecedor BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.CotacaoFornecedor>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Compras.CotacaoFornecedor> BuscarPorCotacao(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.CotacaoFornecedor>();
            var result = from obj in query where obj.Cotacao.Codigo == codigo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Cliente> BuscarUltimosFornecedores(List<int> codigosProdutos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.CotacaoFornecedor>();
            var result = from obj in query where obj.Cotacao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCotacao.Cancelado && obj.Cotacao.Produtos.Any(o => codigosProdutos.Contains(o.Produto.Codigo)) select obj;
            return result.Select(obj => obj.Fornecedor).Distinct().ToList();
        }

        public Dominio.Entidades.Embarcador.Compras.CotacaoFornecedor BuscarPorCodigoFornecedor(int codigoCotacao, double cnpjFornecedor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.CotacaoFornecedor>();
            var result = from obj in query where obj.Cotacao.Codigo == codigoCotacao && obj.Fornecedor.CPF_CNPJ == cnpjFornecedor select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Compras.CotacaoFornecedor> BuscarPorCodigosMantidos(List<int> codigosMantidos, int codigoCotacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.CotacaoFornecedor>();
            var result = from obj in query where !codigosMantidos.Contains(obj.Codigo) && obj.Cotacao.Codigo == codigoCotacao select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Cliente> BuscarFornecedoresPorCotacao(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.CotacaoFornecedor>();
            var result = from obj in query where obj.Cotacao.Codigo == codigo select obj;
            return result.Select(obj => obj.Fornecedor).ToList();
        }

        #endregion
    }
}
