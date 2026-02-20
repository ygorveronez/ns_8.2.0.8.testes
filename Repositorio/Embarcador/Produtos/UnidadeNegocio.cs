using System.Linq;

namespace Repositorio.Embarcador.Produtos
{
    public class UnidadeNegocio : RepositorioBase<Dominio.Entidades.Embarcador.Produtos.UnidadeNegocio>
    {
        #region Construtores
        public UnidadeNegocio(Repositorio.UnitOfWork unitOfWork): base(unitOfWork) { }
        #endregion

        #region MetodosPublicos
        public Dominio.Entidades.Embarcador.Produtos.UnidadeNegocio BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Produtos.UnidadeNegocio> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.UnidadeNegocio>()
                                                                                                   .Where(d => d.CodigoIntegracao == codigoIntegracao) ;
            return query.FirstOrDefault();
        }
        #endregion
    }
}
