using SGT.BackgroundWorkers.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 5000)]

    public class Pacotes : LongRunningProcessBase<Pacotes>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            new Servicos.Embarcador.Carga.Pacote.Pacote(unitOfWork, _tipoServicoMultisoftware).AvancarEtapaAutomaticamenteTempoParaRecebimentoDosPacotes(_auditado);
        }
    }
}
