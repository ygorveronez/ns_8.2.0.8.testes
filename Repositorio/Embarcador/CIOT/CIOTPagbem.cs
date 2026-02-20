using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.CIOT
{
    public class CIOTPagbem : RepositorioBase<Dominio.Entidades.Embarcador.CIOT.CIOTPagbem>
    {
        public CIOTPagbem(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.CIOT.CIOTPagbem BuscarPorCodigo(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.CIOT.CIOTPagbem> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CIOT.CIOTPagbem>();

            query = query.Where(obj => obj.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.CIOT.CIOTPagbem BuscarPorConfiguracaoCIOT(int codigoConfiguracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.CIOT.CIOTPagbem> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CIOT.CIOTPagbem>();

            query = query.Where(obj => obj.ConfiguracaoCIOT.Codigo == codigoConfiguracao);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.CIOT.CIOTPagbem BuscarPorConfiguracaoCIOT()
        {
            IQueryable<Dominio.Entidades.Embarcador.CIOT.CIOTPagbem> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CIOT.CIOTPagbem>();

            return query.FirstOrDefault();
        }

    }
}
