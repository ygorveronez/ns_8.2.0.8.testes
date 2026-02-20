using SGT.BackgroundWorkers.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 5000)]
    public class AnulacaoDocumentos : LongRunningProcessBase<AnulacaoDocumentos>
    {
        #region Metodos protegidos
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Servicos.Embarcador.CTe.CTe.FinalizarCTesAnulados(unitOfWork, _tipoServicoMultisoftware);
            Servicos.Embarcador.CTe.CTe.GerarMovimentacaoCTesAnulados(unitOfWork, _tipoServicoMultisoftware);
            Servicos.Embarcador.CTe.CTe.GerarSubstituicaoCTesAnulados(unitOfWork, _tipoServicoMultisoftware);
        }

        #endregion
    }
}