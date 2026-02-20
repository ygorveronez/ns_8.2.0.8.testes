using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.ColetaEntrega
{
    public class EtapaSaidaCD : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaSaidaCD>
    {
        public EtapaSaidaCD(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaSaidaCD BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaSaidaCD>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaSaidaCD BuscarPorFluxoColetaEntrega(int fluxoColetaEntrega)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaSaidaCD>();
            var result = query.Where(obj => obj.FluxoColetaEntrega.Codigo == fluxoColetaEntrega);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaSaidaCD BuscarPorCarga(int carga)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaSaidaCD>();
            var result = query.Where(obj => obj.FluxoColetaEntrega.Carga.Codigo == carga);
            return result.FirstOrDefault();
        }
    }
}
