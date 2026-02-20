using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class MonitoramentoControle : LongRunningProcessBase<MonitoramentoControle>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Servicos.Embarcador.Logistica.Monitoramento.PrevisaoDeEntrega previsaoDeEntrega = new Servicos.Embarcador.Logistica.Monitoramento.PrevisaoDeEntrega();
           
            previsaoDeEntrega.Iniciar(unitOfWork);
        }        
    }
}
