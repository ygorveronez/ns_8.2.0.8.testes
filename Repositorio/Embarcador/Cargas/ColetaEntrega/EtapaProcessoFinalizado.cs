using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.ColetaEntrega
{
    public class EtapaProcessoFinalizado : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaProcessoFinalizado>
    {
        public EtapaProcessoFinalizado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaProcessoFinalizado BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaProcessoFinalizado>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaProcessoFinalizado BuscarPorFluxoColetaEntrega(int fluxoColetaEntrega)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaProcessoFinalizado>();
            var result = query.Where(obj => obj.FluxoColetaEntrega.Codigo == fluxoColetaEntrega);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaProcessoFinalizado BuscarPorCarga(int carga)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaProcessoFinalizado>();
            var result = query.Where(obj => obj.FluxoColetaEntrega.Carga.Codigo == carga);
            return result.FirstOrDefault();
        }
    }
}
