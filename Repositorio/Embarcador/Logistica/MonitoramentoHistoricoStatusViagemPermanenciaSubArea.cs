using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Logistica
{
    public class MonitoramentoHistoricoStatusViagemPermanenciaSubArea : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagemPermanenciaSubArea>
    {
        public MonitoramentoHistoricoStatusViagemPermanenciaSubArea(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public MonitoramentoHistoricoStatusViagemPermanenciaSubArea(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagemPermanenciaSubArea> BuscarPorCodigoCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagemPermanenciaSubArea>()
                .Where(obj => obj.PermanenciaSubarea.CargaEntrega.Carga.Codigo == codigoCarga);

            return query.ToList();
        }
    }
}
