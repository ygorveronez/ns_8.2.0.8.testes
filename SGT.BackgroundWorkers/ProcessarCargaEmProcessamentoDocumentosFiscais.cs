using SGT.BackgroundWorkers.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 6000)]

    public class ProcessarCargaEmProcessamentoDocumentosFiscais : LongRunningProcessBase<ProcessarCargaEmProcessamentoDocumentosFiscais>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            new Servicos.Embarcador.Carga.Carga(unitOfWork).ProcessarCargaEmProcessamentoDocumentosFiscais(unitOfWork, _stringConexao, _tipoServicoMultisoftware, _auditado, _clienteMultisoftware);
        }

        public override bool CanRun()
        {
            return true; 
        }

    }
}
