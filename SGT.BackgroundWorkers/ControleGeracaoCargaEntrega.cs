using SGT.BackgroundWorkers;
using SGT.BackgroundWorkers.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.WebAdmin.Models.Threads
{
    [RunningConfig(DuracaoPadrao = 60000)]

    public class ControleGeracaoCargaEntrega : LongRunningProcessBase<ControleGeracaoCargaEntrega>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Servicos.Embarcador.Carga.CargaRotaFrete.GerarCargaEntregaPendentes(unitOfWork, _tipoServicoMultisoftware);
        }        
    }
}