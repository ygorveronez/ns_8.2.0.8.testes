using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Compras
{
    public class CotacaoProduto : RepositorioBase<Dominio.Entidades.Embarcador.Compras.CotacaoProduto>
    {
        public CotacaoProduto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Compras.CotacaoProduto BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.CotacaoProduto>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Compras.CotacaoProduto> BuscarPorCotacao(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.CotacaoProduto>();
            var result = from obj in query where obj.Cotacao.Codigo == codigo select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Compras.CotacaoProduto BuscarPorCodigoProduto(int codigoCotacao, int codigoProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.CotacaoProduto>();
            var result = from obj in query where obj.Cotacao.Codigo == codigoCotacao && obj.Produto.Codigo == codigoProduto select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Compras.CotacaoProduto> BuscarPorProdutosMantidos(List<int> produtosMantidos, int codigoCotacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.CotacaoProduto>();
            var result = from obj in query where !produtosMantidos.Contains(obj.Codigo) && obj.Cotacao.Codigo == codigoCotacao select obj;
            return result.ToList();
        }
    }
}
