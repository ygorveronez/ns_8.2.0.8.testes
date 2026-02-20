using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class IntegracaoDiageo : LongRunningProcessBase<IntegracaoDiageo>
    {
        #region Métodos protegidos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Servicos.Embarcador.Carga.ControleEntrega.IntegracaoDiageo integracaoDiageo = new Servicos.Embarcador.Carga.ControleEntrega.IntegracaoDiageo(unitOfWork);
            integracaoDiageo.GerarIntegracoes();
        }

        #endregion
    }
}