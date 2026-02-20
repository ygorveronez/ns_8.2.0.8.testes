using SGT.BackgroundWorkers.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 5000)]

    public class BiddingConvite : LongRunningProcessBase<BiddingConvite>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            AvancarEtapas(unitOfWork);
            EnviarEmailsAbastecimentoGas(unitOfWork);
            EnviarEmailsFluxoGestaoPatio(unitOfWork);

            await unitOfWork.CommitChangesAsync();
        }

        public override bool CanRun()
        {
            return _tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador;
        }

        private void AvancarEtapas(Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Bidding.Bidding biddingServico = new Servicos.Embarcador.Bidding.Bidding(unitOfWork);
            biddingServico.AvancarEtapaUm();
            biddingServico.AvancarEtapaDois();
            biddingServico.AvancarEtapaTres();
            biddingServico.EnviarEmailConvidados();
        }

        private void EnviarEmailsAbastecimentoGas(Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Logistica.AbastecimentoGas servicoAbastecimentoGas = new Servicos.Embarcador.Logistica.AbastecimentoGas(unitOfWork);

            servicoAbastecimentoGas.EnviarEmailAbastecimentoGas();
        }

        private void EnviarEmailsFluxoGestaoPatio(Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);

            servicoFluxoGestaoPatio.EnviarEmailsAlertaSla();
        }
    }
}
