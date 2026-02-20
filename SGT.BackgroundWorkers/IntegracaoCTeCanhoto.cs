using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class IntegracaoCTeCanhoto : LongRunningProcessBase<IntegracaoCTeCanhoto>
    {
        #region Métodos protegidos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            new Servicos.Embarcador.CTe.CTeCanhotoIntegracao(unitOfWork).ProcessarIntegracoesPendentes();
        }

        public override bool CanRun()
        {
            return _tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador;
        }

        #endregion
    }
}