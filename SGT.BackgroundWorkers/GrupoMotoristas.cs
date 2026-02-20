using SGT.BackgroundWorkers.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 5000)]

    public class GrupoMotoristas : LongRunningProcessBase<GrupoMotoristas>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Servicos.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristas servGrupoMotoristas = new Servicos.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristas(unitOfWork);
            await servGrupoMotoristas.VerificarIntegracoesGrupoMotorista(Dominio.ObjetosDeValor.Embarcador.Enumeradores.IdentificadorControlePosicaoThread.GrupoMotoristas, cancellationToken);
        }
    }
}