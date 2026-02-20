using SGT.BackgroundWorkers.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 10000)]
    public class BaixaTituloReceber : LongRunningProcessBase<BaixaTituloReceber>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            if (_tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                Servicos.Embarcador.Financeiro.BaixaTituloReceber.ProcessarBaixasEmGeracao(unitOfWork, _tipoServicoMultisoftware);
                Servicos.Embarcador.Financeiro.BaixaTituloReceber.ProcessarBaixasEmFinalizacao(unitOfWork, _tipoServicoMultisoftware);
            }
        }        
    }
}