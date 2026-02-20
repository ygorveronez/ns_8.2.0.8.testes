
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Produtos
{
    public class OrdemCompraDocumento : RepositorioBase<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraDocumento>
    {
        #region Construtores
        public OrdemCompraDocumento(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region Metodos Publicos
        public List<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraDocumento> BuscarPorCodigoOrdemPrincipal(int codigoOrdemPrincipal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraDocumento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraDocumento>()
                .Where(c => c.OrdemDeCompraPrincipal.Codigo == codigoOrdemPrincipal);

            return query
                .Fetch(obj => obj.Filial)
                .Fetch(obj => obj.Fornecedor)
                .ToList();
        }
        #endregion
    }
}
