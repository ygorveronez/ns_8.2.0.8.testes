using SGT.BackgroundWorkers.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 10000)]

    public class ControleAverbacao : LongRunningProcessBase<ControleAverbacao>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Servicos.Embarcador.CTe.Averbacao servicoAverbacao = new Servicos.Embarcador.CTe.Averbacao();

            await servicoAverbacao.IntegrarAverbacoesPendentesAutorizacaoAsync(_tipoServicoMultisoftware, unitOfWork, _stringConexao);
            Servicos.Embarcador.CTe.Averbacao.IntegrarAverbacoesPendentesAutorizacaoImportadoEmbarcador(_tipoServicoMultisoftware, unitOfWork, _stringConexao);
            Servicos.Embarcador.CTe.Averbacao.IntegrarAverbacoesPendentesCancelamento(_tipoServicoMultisoftware, unitOfWork, _stringConexao);

            Servicos.Embarcador.MDFe.Averbacao.IntegrarAverbacoesPendentesAutorizacao(_tipoServicoMultisoftware, unitOfWork, _stringConexao);
            Servicos.Embarcador.MDFe.Averbacao.IntegrarAverbacoesPendentesCancelamento(_tipoServicoMultisoftware, unitOfWork, _stringConexao);
            Servicos.Embarcador.MDFe.Averbacao.IntegrarAverbacoesPendentesEncerramento(_tipoServicoMultisoftware, unitOfWork, _stringConexao);
        }
    }
}