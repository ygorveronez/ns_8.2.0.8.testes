using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Cargas.ColetaEntrega
{
    public class EtapaChegadaFornecedor : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaChegadaFornecedor>
    {
        public EtapaChegadaFornecedor(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaChegadaFornecedor BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaChegadaFornecedor>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }


        public Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaChegadaFornecedor BuscarPorFluxoColetaEntrega(int fluxoColetaEntrega)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaChegadaFornecedor>();
            var result = query.Where(obj => obj.FluxoColetaEntrega.Codigo == fluxoColetaEntrega);
            return result.FirstOrDefault();
        }
        public Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaChegadaFornecedor BuscarPorCarga(int carga)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaChegadaFornecedor>();
            var result = query.Where(obj => obj.FluxoColetaEntrega.Carga.Codigo == carga);
            return result.FirstOrDefault();
        }
    }
}
