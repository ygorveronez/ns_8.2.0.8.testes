using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Produtos
{
    public class OrdemDeCompraCondicao : RepositorioBase<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraCondicao>
    {
        #region Construtores
        public OrdemDeCompraCondicao(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region Metodos Publicos
        public List<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraCondicao> BuscarPorCodigoOrdemItem(int codigoOrdemCompraItem)
        {
            IQueryable<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraCondicao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraCondicao>()
                .Where(c => c.OrdemDeCompraItem.Codigo == codigoOrdemCompraItem);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraCondicao> BuscarPorCodigoOrdemPrincipal(int codigoOrdemPrincipal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraCondicao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraCondicao>()
                .Where(c => c.OrdemDeCompraItem.OrdemDeCompraDocumento.OrdemDeCompraPrincipal.Codigo == codigoOrdemPrincipal);

            return query.ToList();
        }
        #endregion
    }
}
