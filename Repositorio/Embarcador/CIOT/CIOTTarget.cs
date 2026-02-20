using System.Linq;

namespace Repositorio.Embarcador.CIOT
{
    public class CIOTTarget : RepositorioBase<Dominio.Entidades.Embarcador.CIOT.CIOTTarget>
    {
        public CIOTTarget(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.CIOT.CIOTTarget BuscarPorCodigo(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.CIOT.CIOTTarget> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CIOT.CIOTTarget>();

            query = query.Where(obj => obj.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.CIOT.CIOTTarget BuscarPorConfiguracaoCIOT(int codigoConfiguracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.CIOT.CIOTTarget> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CIOT.CIOTTarget>();

            query = query.Where(obj => obj.ConfiguracaoCIOT.Codigo == codigoConfiguracao);

            return query.FirstOrDefault();
        }
    }
}
