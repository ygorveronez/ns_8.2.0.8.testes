using System.Linq;

namespace Repositorio.Embarcador.Integracao
{
    public class MercadoLivreHandlingUnit : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnit>
    {
        public MercadoLivreHandlingUnit(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnit BuscarPorHandlingUnitID(string handlingUnitID)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnit> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnit>();

            query = query.Where(o => o.ID.Equals( handlingUnitID));

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnit BuscarPorSituacao(string handlingUnitID)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnit> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnit>();

            query = query.Where(o => o.ID.Equals(handlingUnitID));

            return query.FirstOrDefault();
        }
    }
}
