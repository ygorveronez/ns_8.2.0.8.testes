using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class NotificacaoChamado : LongRunningProcessBase<NotificacaoChamado>
    {
        public override bool CanRun()
        {
            return _tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe;
        }

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Servicos.Embarcador.Chamado.Chamado.VerificarDataRetornoENotificar(unitOfWork, _stringConexao, _tipoServicoMultisoftware);
        }
    }
}