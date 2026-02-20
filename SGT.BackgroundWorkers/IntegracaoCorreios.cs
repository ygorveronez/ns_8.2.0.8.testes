using AdminMultisoftware.Repositorio;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class IntegracaoCorreios : LongRunningProcessBase<IntegracaoCorreios>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Servicos.Embarcador.Pedido.Pedido.ProcessarBuscaOcorrenciaCorreios(unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware, _auditado);
        }
    }
}