using SGT.BackgroundWorkers.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 5000)]

    public class ControleCargaCalculoFrete : LongRunningProcessBase<ControleCargaCalculoFrete>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Servicos.Embarcador.Carga.Frete.CalcularFreteCargasPendentes(Dominio.Enumeradores.LoteCalculoFrete.Padrao, _tipoServicoMultisoftware, unitOfWork, _stringConexao, _clienteMultisoftware);
        }
    }
}