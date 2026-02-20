using SGT.BackgroundWorkers.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 60000)]

    public class OcorrenciaEntrega : LongRunningProcessBase<OcorrenciaEntrega>
    {
        #region Métodos Protegidos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            new Servicos.Embarcador.Carga.ControleEntrega.OcorrenciaEntrega(unitOfWork).GerarOcorrenciaEntregaPorGatilhoAutomatico(_tipoServicoMultisoftware, _clienteMultisoftware);
        }

        #endregion Métodos Protegidos
    }
}
