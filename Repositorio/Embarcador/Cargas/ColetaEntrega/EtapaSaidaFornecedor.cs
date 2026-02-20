using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.ColetaEntrega
{
    public class EtapaSaidaFornecedor : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaSaidaFornecedor>
    {
        public EtapaSaidaFornecedor(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaSaidaFornecedor BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaSaidaFornecedor>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaSaidaFornecedor BuscarPorFluxoColetaEntrega(int fluxoColetaEntrega)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaSaidaFornecedor>();
            var result = query.Where(obj => obj.FluxoColetaEntrega.Codigo == fluxoColetaEntrega);
            return result.FirstOrDefault();
        }
        public Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaSaidaFornecedor BuscarPorCarga(int carga)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaSaidaFornecedor>();
            var result = query.Where(obj => obj.FluxoColetaEntrega.Carga.Codigo == carga);
            return result.FirstOrDefault();
        }
    }
}
