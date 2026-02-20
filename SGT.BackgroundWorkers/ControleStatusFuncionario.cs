using Dominio.ObjetosDeValor.Enumerador;
using SGT.BackgroundWorkers.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 43200000)]
    public class ControleStatusFuncionario : LongRunningProcessBase<ControleStatusFuncionario>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            {
                TipoAuditado = TipoAuditado.Sistema,
                OrigemAuditado = OrigemAuditado.Sistema
            };
            Servicos.Embarcador.Transportadores.Motorista.AtualizarStatusColaborador(unitOfWork, auditado, true);
        }
    }
}