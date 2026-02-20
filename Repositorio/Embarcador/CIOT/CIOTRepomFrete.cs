using System.Linq;

namespace Repositorio.Embarcador.CIOT
{
    public class CIOTRepomFrete : RepositorioBase<Dominio.Entidades.Embarcador.CIOT.CIOTRepomFrete>
    {
        public CIOTRepomFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.CIOT.CIOTRepomFrete BuscarPorCodigo(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.CIOT.CIOTRepomFrete> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CIOT.CIOTRepomFrete>();

            query = query.Where(obj => obj.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.CIOT.CIOTRepomFrete BuscarPorConfiguracaoCIOT(int codigoConfiguracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.CIOT.CIOTRepomFrete> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CIOT.CIOTRepomFrete>();

            query = query.Where(obj => obj.ConfiguracaoCIOT.Codigo == codigoConfiguracao);

            return query.FirstOrDefault();
        }
    }
}
