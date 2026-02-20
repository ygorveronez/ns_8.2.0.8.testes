using SGT.BackgroundWorkers.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 900000)]

    public class EncerramentoMDFeAquaviario : LongRunningProcessBase<EncerramentoMDFeAquaviario>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            {
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema
            };

            Servicos.Embarcador.Carga.Carga.EncerrarMDFeAquaviarioAutomaticamente(unitOfWork, _stringConexao, _tipoServicoMultisoftware, auditado);
            Servicos.Embarcador.Carga.Carga.EncerrarMDFeRodoviarioAutomaticamente(unitOfWork, _stringConexao, _tipoServicoMultisoftware, auditado);
        }
    }
}