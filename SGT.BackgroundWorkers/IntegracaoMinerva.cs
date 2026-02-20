using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class IntegracaoMinerva : LongRunningProcessBase<IntegracaoMinerva>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            await VerificarIntegracoesMinervaPendentesAsync(unitOfWork, cancellationToken);
        }

        private async Task VerificarIntegracoesMinervaPendentesAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Servicos.Embarcador.Integracao.Minerva.IntegracaoMinerva servicoIntegracaoCargaMinerva = new Servicos.Embarcador.Integracao.Minerva.IntegracaoMinerva(unitOfWork, _tipoServicoMultisoftware, cancellationToken);

            await servicoIntegracaoCargaMinerva.VerificarCargaIntegracaoMinervaPendentesAsync(cargasEmLote: false);
            await servicoIntegracaoCargaMinerva.VerificarCargaIntegracaoMinervaMinervaPendentesAtualizacaoSituacaoAsync(cargasEmLote: false);
        }

        public override bool CanRun()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                return new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork).ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Minerva);
        }
    }
}