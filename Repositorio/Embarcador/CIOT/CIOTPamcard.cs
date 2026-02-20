using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.CIOT
{
    public class CIOTPamcard : RepositorioBase<Dominio.Entidades.Embarcador.CIOT.CIOTPamcard>
    {
        public CIOTPamcard(UnitOfWork unitOfWork) : base(unitOfWork)        {        }

        public Dominio.Entidades.Embarcador.CIOT.CIOTPamcard BuscarPorCodigo(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.CIOT.CIOTPamcard> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CIOT.CIOTPamcard>();

            query = query.Where(obj => obj.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.CIOT.CIOTPamcard BuscarPorConfiguracaoCIOT(int codigoConfiguracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.CIOT.CIOTPamcard> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CIOT.CIOTPamcard>();

            query = query.Where(obj => obj.ConfiguracaoCIOT.Codigo == codigoConfiguracao);

            return query.FirstOrDefault();
        }

    }
}
