using SGT.BackgroundWorkers.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 10000)]

    public class CargaEntregaEventoIntegracao : LongRunningProcessBase<CargaEntregaEventoIntegracao>
    {
        #region Métodos protegidos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaEventoIntegracao servicoCargaEntregaEventoIntegracao = new Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaEventoIntegracao(unitOfWork);
            await servicoCargaEntregaEventoIntegracao.ProcessarIntegracoesPendentesAsync();
        }

        #endregion
    }
}