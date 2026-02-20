using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Logistica
{
    public class MonitoramentoHistoricoStatusViagemPermanenciaCliente : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagemPermanenciaCliente>
    {
        public MonitoramentoHistoricoStatusViagemPermanenciaCliente(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public MonitoramentoHistoricoStatusViagemPermanenciaCliente(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagemPermanenciaCliente> BuscarPorCodigoCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagemPermanenciaCliente>()
                .Where(obj => obj.PermanenciaCliente.CargaEntrega.Carga.Codigo == codigoCarga);

            return query.ToList();
        }
    }
}
