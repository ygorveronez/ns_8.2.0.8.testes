using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.ColetaEntrega
{
    public class EtapaAgAlocarVeiculo : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaAgAlocarVeiculo>
    {
        public EtapaAgAlocarVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }
        
        public Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaAgAlocarVeiculo BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaAgAlocarVeiculo>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaAgAlocarVeiculo BuscarPorFluxoColetaEntrega(int fluxoColetaEntrega)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaAgAlocarVeiculo>();
            var result = query.Where(obj => obj.FluxoColetaEntrega.Codigo == fluxoColetaEntrega);
            return result.FirstOrDefault();
        }


        public Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaAgAlocarVeiculo BuscarPorCarga(int carga)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaAgAlocarVeiculo>();
            var result = query.Where(obj => obj.FluxoColetaEntrega.Carga.Codigo == carga);
            return result.FirstOrDefault();
        }
    }
}
