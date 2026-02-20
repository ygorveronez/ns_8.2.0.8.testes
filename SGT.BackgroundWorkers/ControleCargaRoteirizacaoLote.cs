using SGT.BackgroundWorkers.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 3000)]
    public class ControleCargaRoteirizacaoLote: LongRunningProcessBase<ControleCargaRoteirizacaoLote>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Servicos.Embarcador.Carga.CargaRotaFrete.GerarIntegracoesRoteirizacaoCarga(true, unitOfWork, _tipoServicoMultisoftware);
        }        
    }
}