using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class IntegracaoComprovei : LongRunningProcessBase<IntegracaoComprovei>
    {
        #region Métodos protegidos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Servicos.Embarcador.Integracao.Comprovei.IntegracaoComprovei.ConsultarProtocolos(unitOfWork);
        }

        #endregion
    }
}