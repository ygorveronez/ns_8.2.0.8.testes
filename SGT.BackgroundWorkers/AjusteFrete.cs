using SGT.BackgroundWorkers.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 20000)]
    public class AjusteFrete: LongRunningProcessBase<AjusteFrete>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Servicos.Embarcador.Frete.AjusteTabelaFrete srvAjusteTabelaFrete = new Servicos.Embarcador.Frete.AjusteTabelaFrete(unitOfWork);

            srvAjusteTabelaFrete.VerificarAjustesPendentesCriacao();
            srvAjusteTabelaFrete.VerificarAjustesPendentesAplicacaoAjuste();
            srvAjusteTabelaFrete.VerificarAjustesPendentesProcessamento();
        }
    }
}
