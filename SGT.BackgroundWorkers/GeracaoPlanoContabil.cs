using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class GeracaoPlanoContabil : LongRunningProcessBase<GeracaoPlanoContabil>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repositorioConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repositorioConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();
            Servicos.Embarcador.Escrituracao.Escrituracao servicoEscrituracao = new Servicos.Embarcador.Escrituracao.Escrituracao(unitOfWork, configuracao, configuracaoFinanceiro);
            Servicos.Embarcador.Escrituracao.Provisao servicoProvisao = new Servicos.Embarcador.Escrituracao.Provisao(unitOfWork, _auditado, configuracaoFinanceiro);

            Servicos.Embarcador.Escrituracao.Pagamento.ProcessarPagamentoEmFechamento(unitOfWork, _stringConexao, _tipoServicoMultisoftware, configuracao, _urlAcesso);
            servicoProvisao.ProcessarProvisoesEmFechamento();
            servicoProvisao.FecharProvisoesAutomaticamente();
            servicoProvisao.GerarLotesProvisao();
            servicoProvisao.LiberarDocumentosFaturamentoComProvisaoGerada();
            servicoProvisao.VerificarProvisoesAbertasGeracaoEstornoAutomatico(_tipoServicoMultisoftware);

            Servicos.Embarcador.Escrituracao.CancelamentoProvisao.ProcessarProvisoesEmCancelamento(unitOfWork, _stringConexao, _tipoServicoMultisoftware);
            Servicos.Embarcador.Escrituracao.CancelamentoPagamento.ProcessarPagamentosEmCancelamento(unitOfWork, _stringConexao, _tipoServicoMultisoftware);

            Servicos.Embarcador.Financeiro.Contabilizacao.GerarLoteContabilizacao(unitOfWork, _tipoServicoMultisoftware, configuracao);

            servicoEscrituracao.GerarLotesEscrituracao(_tipoServicoMultisoftware);
            servicoEscrituracao.GerarLotesEscrituracaoCancelamento(_tipoServicoMultisoftware);

            Servicos.Embarcador.Escrituracao.Pagamento.GerarLotePagamento(unitOfWork, _stringConexao, _tipoServicoMultisoftware, configuracao, false, configuracaoFinanceiro, _auditado);

            if (configuracao.GerarPagamentoBloqueado && !configuracao.GerarSomenteDocumentosDesbloqueados)
                Servicos.Embarcador.Escrituracao.Pagamento.GerarLotePagamento(unitOfWork, _stringConexao, _tipoServicoMultisoftware, configuracao, true, configuracaoFinanceiro, _auditado);

            Servicos.Embarcador.Escrituracao.Pagamento.FinalizarLotesPagamentoGeradosAutomaticamente(unitOfWork, _stringConexao, _tipoServicoMultisoftware, configuracao);
            Servicos.Embarcador.Escrituracao.Pagamento.DesbloquearTituloPagamentoCanhotosPendentes(unitOfWork);
        }
    }
}