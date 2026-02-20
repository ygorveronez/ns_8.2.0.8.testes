using System.Linq;

namespace Repositorio.Embarcador.CIOT
{
    public class CIOTRepom : RepositorioBase<Dominio.Entidades.Embarcador.CIOT.CIOTRepom>
    {
        public CIOTRepom(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.CIOT.CIOTRepom BuscarPorCodigo(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.CIOT.CIOTRepom> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CIOT.CIOTRepom>();

            query = query.Where(obj => obj.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.CIOT.CIOTRepom BuscarPorConfiguracaoCIOT(int codigoConfiguracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.CIOT.CIOTRepom> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CIOT.CIOTRepom>();

            query = query.Where(obj => obj.ConfiguracaoCIOT.Codigo == codigoConfiguracao);

            return query.FirstOrDefault();
        }
    }
}
