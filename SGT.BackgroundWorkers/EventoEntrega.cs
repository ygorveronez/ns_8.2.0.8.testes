using SGT.BackgroundWorkers.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 300000)]

    public class EventoEntrega : LongRunningProcessBase<EventoEntrega>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Servicos.Embarcador.GestaoEntregas.EventoEntrega servicoEventoEntrega = new Servicos.Embarcador.GestaoEntregas.EventoEntrega(unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware);

            servicoEventoEntrega.VerificarIntegracoesEventosEntrega();
        }
    }
}