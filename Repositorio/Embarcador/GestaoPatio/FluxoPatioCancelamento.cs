using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.GestaoPatio
{
    public class FluxoPatioCancelamento : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioCancelamento>
    {
        public FluxoPatioCancelamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioCancelamento BuscarPorFluxoPatio(int codigoFluxo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioCancelamento>()
                .Where(obj => obj.FluxoGestaoPatio.Codigo == codigoFluxo);

            return query.FirstOrDefault();
        }

        public bool BuscarSeVeiculoEstaBloqueadoPorCargaVeiculo(int codigoCarga, int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioCancelamento>()
                .Where(obj => obj.FluxoGestaoPatio.Carga.Codigo == codigoCarga && obj.VeiculoBloqueado.Codigo == codigoVeiculo);
            
            return query.Count() > 0;
        }
    }
}
