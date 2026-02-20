using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class IntegracaoDocumentosCarga : LongRunningProcessBase<IntegracaoDocumentosCarga>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            await VerificarIntegracoesPendentesAsync(unitOfWork, cancellationToken);
            await VerificarCargasComIntegracoesNaoConcluidasAsync(unitOfWork, cancellationToken);
        }

        private async Task VerificarIntegracoesPendentesAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Servicos.Embarcador.Integracao.IntegracaoNFSManual servicoIntegracaoNFSManual = new Servicos.Embarcador.Integracao.IntegracaoNFSManual(unitOfWork, _tipoServicoMultisoftware, cancellationToken);
            Servicos.Embarcador.Integracao.IntegracaoNFSManualCancelamento servicoIntegracaoNFSManualCancelamento = new Servicos.Embarcador.Integracao.IntegracaoNFSManualCancelamento(unitOfWork, _tipoServicoMultisoftware, cancellationToken);
            Servicos.Embarcador.Integracao.IntegracaoCarga servicoIntegracaoCarga = new Servicos.Embarcador.Integracao.IntegracaoCarga(unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware, cancellationToken);
            Servicos.Embarcador.Integracao.IntegracaoAvaria servicoIntegracaoAvaria = new Servicos.Embarcador.Integracao.IntegracaoAvaria(unitOfWork, cancellationToken);
            Servicos.Embarcador.Integracao.IntegracaoCargaCancelamento servicoIntegracaoCargaCancelamento = new Servicos.Embarcador.Integracao.IntegracaoCargaCancelamento(unitOfWork, _tipoServicoMultisoftware, _clienteUrlAcesso, cancellationToken);
            Servicos.Embarcador.Integracao.IntegracaoLoteEscrituracao servicoIntegracaoLoteEscrituracao = new Servicos.Embarcador.Integracao.IntegracaoLoteEscrituracao(unitOfWork, _tipoServicoMultisoftware, cancellationToken);
            Servicos.Embarcador.Integracao.IntegracaoLoteEscrituracaoCancelamento servicoIntegracaoLoteEscrituracaoCancelamento = new Servicos.Embarcador.Integracao.IntegracaoLoteEscrituracaoCancelamento(unitOfWork, _tipoServicoMultisoftware, cancellationToken);
            Servicos.Embarcador.Integracao.Contabilizacao.IntegracaoLoteContabilizacao servicoIntegracaoLoteContabilizacao = new Servicos.Embarcador.Integracao.Contabilizacao.IntegracaoLoteContabilizacao(unitOfWork, _tipoServicoMultisoftware, cancellationToken);
            Servicos.Embarcador.Integracao.IndicadorIntegracaoCTe servicoIndicadorIntegracaoCTe = new Servicos.Embarcador.Integracao.IndicadorIntegracaoCTe(unitOfWork, cancellationToken);
            Servicos.Embarcador.Integracao.IntegracaoLoteCliente servicoIntegracaoLoteCliente = new Servicos.Embarcador.Integracao.IntegracaoLoteCliente(unitOfWork, cancellationToken);
            Servicos.Embarcador.Integracao.IntegracaoControleIntegracaoCargaEDI servicoIntegracaoControleIntegracaoCargaEDI = new Servicos.Embarcador.Integracao.IntegracaoControleIntegracaoCargaEDI(unitOfWork, _tipoServicoMultisoftware, cancellationToken);
            Servicos.Embarcador.Integracao.IntegracaoCargaCancelamentoDadosCancelamento servicoIntegracaoCargaCancelamentoDadosCancelamento = new Servicos.Embarcador.Integracao.IntegracaoCargaCancelamentoDadosCancelamento(unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware, cancellationToken);
            Servicos.Embarcador.Integracao.IntegracaoEnvioProgramado servicoIntegracaoEnvioProgramado = new Servicos.Embarcador.Integracao.IntegracaoEnvioProgramado(unitOfWork);

            await servicoIntegracaoCarga.VerificarIntegracoesPendentesAsync(_auditado, _clienteUrlAcesso);
            await servicoIntegracaoAvaria.VerificarIntegracoesPendentesAsync();
            servicoIntegracaoEnvioProgramado.VerificarIntegracaoesCTePendentes();
            servicoIntegracaoEnvioProgramado.VerificarIntegracaoesNFSePendentes();
            servicoIntegracaoEnvioProgramado.VerificarIntegracaoesOutrosDocumentosPendentes();

            await servicoIntegracaoCargaCancelamento.VerificarIntegracoesPendentesAsync();
            await servicoIntegracaoNFSManual.VerificarIntegracoesPendentesAsync();
            await servicoIntegracaoLoteEscrituracao.VerificarIntegracoesPendentesAsync();
            await servicoIntegracaoLoteEscrituracaoCancelamento.VerificarIntegracoesPendentesAsync();
            await servicoIntegracaoNFSManualCancelamento.VerificarIntegracoesPendentesAsync();
            await servicoIntegracaoLoteContabilizacao.VerificarIntegracoesPendentesAsync();
            servicoIntegracaoCargaCancelamentoDadosCancelamento.VerificarIntegracoesPendentesCarga();
            servicoIntegracaoCargaCancelamentoDadosCancelamento.VerificarIntegracoesPendentesCTe();

            await servicoIndicadorIntegracaoCTe.VerificarIntegracoesPendentesAsync();
            await servicoIntegracaoLoteCliente.VerificarIntegracoesPendentesAsync();
            await servicoIntegracaoControleIntegracaoCargaEDI.VerificarIntegracoesPendentesAsync();

        }

        private async Task VerificarCargasComIntegracoesNaoConcluidasAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            List<int> codigosCargas = await repositorioCarga.BuscarCargasNaoConcluiuIntegracaoPorSituacaoAsync(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao, 10);

            Servicos.Embarcador.Integracao.IntegracaoCarga serIntegracaoCarga = new Servicos.Embarcador.Integracao.IntegracaoCarga(unitOfWork, _tipoServicoMultisoftware, cancellationToken);

            foreach (int codigoCarga in codigosCargas)
                await serIntegracaoCarga.AtualizarSituacaoCargaIntegracaoAsync(codigoCarga);
        }
    }
}