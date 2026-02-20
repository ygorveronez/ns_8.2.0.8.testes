using SGT.BackgroundWorkers.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 3000)]

    public class ValePedagio : LongRunningProcessBase<ValePedagio>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Servicos.Embarcador.Carga.ValePedagio.ValePedagio servicoValePedagio = new Servicos.Embarcador.Carga.ValePedagio.ValePedagio(unitOfWork);

            Servicos.Log.GravarDebug("Iniciou ConsultarValoresPedagioPendenteAsync", "ValePedagio");
            await servicoValePedagio.ConsultarValoresPedagioPendenteAsync(_tipoServicoMultisoftware);

            Servicos.Log.GravarDebug("Iniciou GerarIntegracoesValePedagioAsync", "ValePedagio");
            await servicoValePedagio.GerarIntegracoesValePedagioAsync(_tipoServicoMultisoftware);

            Servicos.Log.GravarDebug("Iniciou GerarIntegracoesCancelamentoValePedagioAsync", "ValePedagio");
            await servicoValePedagio.GerarIntegracoesCancelamentoValePedagioAsync(_tipoServicoMultisoftware);

            Servicos.Log.GravarDebug("Iniciou VerificarRetornosValePedagioAsync", "ValePedagio");
            await servicoValePedagio.VerificarRetornosValePedagioAsync(_tipoServicoMultisoftware);

            Servicos.Log.GravarDebug("Iniciou ProcessarExtratoValePedagioPendentes", "ValePedagio");
            servicoValePedagio.ProcessarExtratoValePedagioPendentes(_tipoServicoMultisoftware);

            Servicos.Log.GravarDebug("Concluíu ValePedagio", "ValePedagio");
        }
    }
}