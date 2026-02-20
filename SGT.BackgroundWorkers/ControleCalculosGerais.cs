using SGT.BackgroundWorkers.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 10000)]

    public class ControleCalculosGerais : LongRunningProcessBase<ControleCalculosGerais>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador(unitOfWork).CalcularFreteParaTransportadores(_tipoServicoMultisoftware);
            Servicos.Embarcador.Carga.Frete.CalcularFretePreCargasPendentes(false, _tipoServicoMultisoftware, unitOfWork, _stringConexao);
        }
    }
}