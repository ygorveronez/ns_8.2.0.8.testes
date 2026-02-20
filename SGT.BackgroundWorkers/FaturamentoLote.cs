using SGT.BackgroundWorkers.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 180000)]

    public class FaturamentoLote : LongRunningProcessBase<FaturamentoLote>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Servicos.Embarcador.Fatura.FaturamentoLote.ProcessarFaturamentoLote(unitOfWork, _stringConexao, _tipoServicoMultisoftware);
        }
    }
}