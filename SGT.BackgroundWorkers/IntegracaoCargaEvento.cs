using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class IntegracaoCargaEvento : LongRunningProcessBase<IntegracaoCargaEvento>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            new Servicos.Embarcador.Integracao.IntegracaoCargaEvento(unitOfWork, _tipoServicoMultisoftware).VerificarIntegracoesPendentes();
        }
    }
}