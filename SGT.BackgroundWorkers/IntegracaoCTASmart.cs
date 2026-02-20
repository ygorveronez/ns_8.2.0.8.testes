using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class IntegracaoCTASmart : LongRunningProcessBase<IntegracaoCTASmart>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            SincronizarAbastecimentos(unitOfWork);
        }

        private void SincronizarAbastecimentos(Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Integracao.CTASmart.IntegracaoCTASmart servicoIntegracaoCTASmart = new Servicos.Embarcador.Integracao.CTASmart.IntegracaoCTASmart(unitOfWork);

            servicoIntegracaoCTASmart.SincronizarAbastecimentos(_auditado);
        }
    }
}