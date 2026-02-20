using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Produtos
{
    public class OrdemDeCompraParceiro : RepositorioBase<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraParceiro>
    {
        #region Construtores
        public OrdemDeCompraParceiro(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region Metodos Publicos
        public List<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraParceiro> BuscarPorCodigoOrdemDocumento(int codigoOrdemDocumento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraParceiro> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraParceiro>()
                .Where(c => c.OrdemDeCompraDocumento.Codigo == codigoOrdemDocumento);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraParceiro> BuscarPorCodigoOrdemPrincipal(int codigoOrdemPrincipal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraParceiro> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraParceiro>()
                .Where(c => c.OrdemDeCompraDocumento.OrdemDeCompraPrincipal.Codigo == codigoOrdemPrincipal);

            return query.ToList();
        }
        #endregion
    }
}
