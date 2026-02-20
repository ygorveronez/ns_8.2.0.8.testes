using SGT.BackgroundWorkers.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 600000)]
    public class ColetaNotaFiscal : LongRunningProcessBase<ColetaNotaFiscal>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            new Servicos.Embarcador.Pedido.ColetaNotaFiscal(unitOfWork, _tipoServicoMultisoftware).ProcessarColetaNotaFiscal();
            Servicos.Embarcador.Pedido.Container.CTeAnteriorBookingContainer.ProcessarCTeAnteriorBookingContainer(unitOfWork, _tipoServicoMultisoftware);
        }
    }
}