
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Produtos
{
    public class OrdemCompraItem : RepositorioBase<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraItem>
    {
        #region Construtores
        public OrdemCompraItem(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region Metodos Publicos
        public List<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraItem> BuscarPorCodigoOrdemDocumento(int codigoOrdemDocumento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraItem> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraItem>()
                .Where(c => c.OrdemDeCompraDocumento.Codigo == codigoOrdemDocumento);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraItem> BuscarPorCodigoOrdemPrincipal(int codigoOrdemPrincipal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraItem> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraItem>()
                .Where(c => c.OrdemDeCompraDocumento.OrdemDeCompraPrincipal.Codigo == codigoOrdemPrincipal);

            return query.ToList();
        }
        #endregion
    }
}
