using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.ColetaEntrega
{
    public class FluxoColetaEntregaEtapas : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntregaEtapas>
    {
        public FluxoColetaEntregaEtapas(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntregaEtapas> BuscarPorColetaEntrega(int coletaEntrega)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntregaEtapas>();
            var result = from obj in query where obj.FluxoColetaEntrega.Codigo == coletaEntrega select obj;
            return result.ToList();
        }

    }
}
