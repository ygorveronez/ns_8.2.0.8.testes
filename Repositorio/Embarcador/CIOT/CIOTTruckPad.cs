using System.Linq;

namespace Repositorio.Embarcador.CIOT
{
    public class CIOTTruckPad : RepositorioBase<Dominio.Entidades.Embarcador.CIOT.CIOTTruckPad>
    {
        public CIOTTruckPad(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.CIOT.CIOTTruckPad BuscarPorCodigo(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.CIOT.CIOTTruckPad> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CIOT.CIOTTruckPad>();

            query = query.Where(obj => obj.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.CIOT.CIOTTruckPad BuscarPorConfiguracaoCIOT(int codigoConfiguracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.CIOT.CIOTTruckPad> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CIOT.CIOTTruckPad>();

            query = query.Where(obj => obj.ConfiguracaoCIOT.Codigo == codigoConfiguracao);

            return query.FirstOrDefault();
        }
    }
}
