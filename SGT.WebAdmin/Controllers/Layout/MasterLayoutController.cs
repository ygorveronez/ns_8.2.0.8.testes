using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.Cache;
using Microsoft.AspNetCore.Mvc;
using SGT.BackgroundWorkers;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace SGT.WebAdmin.Controllers.Layout
{
    public class MasterLayoutController : BaseController
    {
        #region Construtores

        public MasterLayoutController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            string stringConexaoAdmin = _conexao.AdminStringConexao;



#if DEBUG
            _conexao.MigrateDatabase(_conexao.StringConexao);
            string chaveCacheEmpresa = "EmpresaMultisoftware" + _conexao.ObterHost;
            int codigoEmpresa = CacheProvider.Instance.Get<int>(chaveCacheEmpresa);
            string stringConexao = _conexao.StringConexao;
            string host = _conexao.ObterHost;

            Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork);

            bool utilizarLockNasThreads = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().Autoscaling.HasValue ? Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().Autoscaling.Value : false;

            //Necessário para as notificações dos relatórios, não remover

            SGT.BackgroundWorkers.MSMQ.GetInstance().QueueItem(unitOfWork, ClienteAcesso.Cliente.Codigo, stringConexao, stringConexaoAdmin, ClienteAcesso.TipoServicoMultisoftware);

            if (ClienteAcesso.TipoExecucao == AdminMultisoftware.Dominio.Enumeradores.TipoExecucao.Thread && ExecutarThread())
            {
                int port = _conexao.ObterPortaHost.HasValue ? _conexao.ObterPortaHost.Value : 0;

                if (port != 8443 &&
                    (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ||
                    TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS))
                {

                    await LongRunningProcessFactory.Instance.AddProcessAsync(CargaIntegracaoHUBOfertas.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 1000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(MensageriaHUBOfertas.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 1000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ImportacaoTicketLog.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3600000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ConsultaAbastecimentoAngellira.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 14400000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(MDFeManual.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 6000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(PracaPedagioTarifaIntegracao.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 6000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(BackgroundWorkers.RH.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 30000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(PedidoOcorrenciaColetaEntregaIntegracao.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(CargaEntregaEventoIntegracao.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(BackgroundWorkers.Patrimonio.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 86400000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(BackgroundWorkers.PagamentoAgregado.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(NotificacaoChamado.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoMontagemCarga.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 6000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ExclusaoPedido.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 60000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(MonitorarMonitoramento.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(MonitoramentoControle.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(JanelaCarregamento.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ProgramacaoCarga.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 30000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(VincularRotasSemParar.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracoesEntreSistemas.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3600000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoProcessamentoEDIFTP.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoPedido.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 180000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoPamcard.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoMotorista.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoEmbarcador.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 900000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoCadastrosMulti.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 900000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoMinerva.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoMinervaLote.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoDocumentosCarga.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoCargaFrete.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000, true);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoCTeCarga.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 0);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoCIOT.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ImportacaoTabelaFrete.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoCarga.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 6000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoCargaEvento.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 12000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ImportacaoTakePay.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(NotaDeDebito.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ImportacaoPedido.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ImportacaoBoletoRetorno.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(GeracaoTituloNFSe.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(GeracaoTituloCTe.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(GeracaoPlanoContabil.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(GeracaoPDFDANFE.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 900000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(Financeiro.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 900000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(FilaCarregamento.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(BackgroundWorkers.FaturamentoMensal.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(FaturamentoLote.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 180000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(FaturaFechamento.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(Fatura.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ExecucaoDiaria.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 86400000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EnvioEmailBoleto.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 300000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EnvioEmailAvisoVencimentoCobranca.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 43200000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EnvioDocumentacaoLote.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 180000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EnvioDocumentacaoAFRMM.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 180000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EncerramentoMDFeAquaviario.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 900000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EmissaoDocumentosIntegracaoMDFe.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 20000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EmissaoDocumentos.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EmissaoDocumentosIntegracao.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 6000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EmissaoDocumentosIntegracaoReprocessamento.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 6000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(DownloadLoteCTe.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ControleStatusFuncionario.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 43200000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ControleCargaRoteirizacaoLote.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ControleCargaRoteirizacao.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ControleCargaEmFinalizacaoEmissaoIntegracao.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 5000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ControleCargaEmFinalizacaoEmissao.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 7000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ControleCargaEmEmissaoLote.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ControleCargaEmEmissao.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 5000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ControleCargaCalculoFreteIntegracao.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 5000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ControleCalculosGerais.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ControleRoteirizacao.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ControleCargaCalculoFrete.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 5000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ControleCargaCalculoFreteReprocessamento.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 5000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ControleAverbacao.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ConsultarCaixaEntradaEmail.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ConsultaFTPDocsys.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 1800000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ConsultaEmissoesPendentes.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 5000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ConsultaDocumentosPedido.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 21600000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ColetaNotaFiscal.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 180000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(BiddingConvite.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 5000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(BaixaTituloReceber.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(AlertaCargasParadas.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 600000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(Alerta.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 86400000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(AjusteFrete.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 20000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(AgrupamentoCargas.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 30000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ControleTendenciaEntrega.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 180000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoMercadoLivre.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 60000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(DiariaAutomatica.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 60000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoVtex.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 600000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoEmillenium.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 600000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoMarfrig.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 86400000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(AlertaEventosCarga.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(Canhoto.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 600000, ativo: false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(GeracaoConciliacaoTransportador.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 600000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(CargaFechamento.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 600000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(BackgroundWorkers.Ocorrencia.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EventoEntrega.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 300000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoCTASmart.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 600000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegrarCargaUnilever.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 300000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoCTEManual.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 200000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoCartaCorrecao.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 200000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoMDFe.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 200000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoMercante.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 200000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoEmilleniumBuscaMassiva.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 600000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ControleDasIntegracoes.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 14400000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ValePedagio.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000, true);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EnvioEmailDocumentacao.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 60000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoSIC.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 60000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ConsultarExtratosValePedagio.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 85800000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EncerramentoCarga.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 30000, true);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoDiageo.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 900000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoCorreios.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 60000, true);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ProcessarDocumentoTransporte.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 1000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EnvioEmailGlobalizadoFornecedor.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 900000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoComprovei.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 20000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoCTeCanhoto.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 300000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ProcessamentoArquivoXMLNotaFiscalIntegracao.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 300000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ValidarIrregularidadesControleDocumento.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 300000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ProcessarCargaEmProcessamentoDocumentosFiscais.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 6000, true);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ProcessarCargaEmProcessamentoDocumentosFiscaisEmLote.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000, true);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(AutorizacaoPagamentoContratoFrete.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 60000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(Pacotes.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 5000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(AnulacaoDocumentos.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 5000, true);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(LiberacaoEtapaDocumento.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000, true);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(FaturamentoAutomaticoCTe.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3600000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ProcessarRetornoAprovacaoTabelaFrete.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 60000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(Abastecimento.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3600000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ProcessarFinalizacaoColetaEntregaEmLote.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3600000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoGhost.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 6000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ProcessarIntegracaoSuperApp.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ProcessarIntegracaoSuperAppDemaisEventos.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ProcessarIntegracaoNotificacaoApp.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EmissaoDocumentosNFe.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000, true);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EmissaoDocumentosEtapaFrete.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000, true);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EmissaoDocumentosEtapaFreteLeve.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EmissaoDocumentosEtapaFreteMedia.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EmissaoDocumentosEtapaFretePesada.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EmissaoDocumentosSubContratacaoFilialEmissora.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000, true);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EmissaoDocumentosNFeIntegracao.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000, true);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EmissaoDocumentosEtapaFreteIntegracao.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000, true);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EmissaoDocumentosEtapaFreteIntegracaoLeve.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EmissaoDocumentosEtapaFreteIntegracaoMedia.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EmissaoDocumentosEtapaFreteIntegracaoPesada.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EmissaoDocumentosSubContratacaoFilialEmissoraIntegracao.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000, true);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(SGT.BackgroundWorkers.AbastecimentoInterno.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 100000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ProcessarDadosGestaoDevolucao.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(CargaEntregaFinalizacaoAssincrona.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(OcorrenciaEntrega.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 60000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ProcessarUsuariosPortal.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 60000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(SGT.BackgroundWorkers.IntegracaoNFSe.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 100000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(Models.Threads.ControleGeracaoCargaEntrega.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 60000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(GerarNFSesIntegracao.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 30000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(GerarCargaPorMDFe.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 30000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(GerarCTesIntegracao.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 30000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(CargaOferta.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 30000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(GrupoMotoristas.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 5000, ativo: false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(SolicitacaoConfirmacaoDocumentosFiscais.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000, false);
                }
                else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {

                    await LongRunningProcessFactory.Instance.AddProcessAsync(SGT.BackgroundWorkers.Patrimonio.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 86400000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(SGT.BackgroundWorkers.FaturamentoMensal.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(GeracaoTituloNFSe.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(GeracaoTituloCTe.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(BaixaTituloReceber.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(Alerta.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 86400000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ImportacaoBoletoRetorno.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ControleStatusFuncionario.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 43200000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EnvioEmailChat.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 600000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoDocumentosDestinados.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3600000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoArquivosDocumentosDestinados.Instance, codigoEmpresa, ClienteAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 1800000);

                }
            }
#endif

            if (this.Usuario == null)
                return Redirect("/Login");

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            StringBuilder versionSB = new StringBuilder($"{fvi.FileMajorPart}.{fvi.FileMinorPart}");

            if (fvi.FileBuildPart > 0)
                versionSB.Append($".{fvi.FileBuildPart}");
            else if (fvi.FilePrivatePart > 0)
                versionSB.Append($".{fvi.FileBuildPart}.{fvi.FilePrivatePart}");

            string version = versionSB.ToString();

            ViewBag.UltimaAtualizacao = ObterDataAtualizacao();
            ViewBag.Versao = version;

#if !DEBUG
            AtualizarVersaoEmSegundoPlano(version, stringConexaoAdmin);
#endif

            ViewBag.AnoCorrente = DateTime.Now.Year.ToString();
            ViewBag.DadosEmpresaPai = "";
            ViewBag.AmbienteKMM = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS & !ClienteAcesso.Cliente.Cabotagem;

            if (!ViewBag.AmbienteKMM)
            {
                ViewBag.EmailDPO = "dpo@multisoftware.com.br";
                ViewBag.Copyright = "Multisoftware";
            }
            else
            {
                ViewBag.EmailDPO = "privacidade@kmm.com.br";
                ViewBag.Copyright = "KMM";
            }

            string protocolo = (Request.IsHttps ? "https" : "http");
            if (Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().TipoProtocolo.HasValue && Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().TipoProtocolo.Value.ObterProtocolo() != "")
                protocolo = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().TipoProtocolo.Value.ObterProtocolo();

            ViewBag.HTTPConnection = protocolo;
            ViewBag.AmbienteMultiNFe = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin;
            if (ViewBag.AmbienteMultiNFe)
                ViewBag.EmailDPO = "dpo@commerce.inf.br";

            ObterMenu(ClienteAcesso, unitOfWork);

            bool portalCliente = IsLayoutClienteAtivo(unitOfWork);
            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ||
                    TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                {
                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                    if (Usuario != null && Usuario.Empresa != null && Usuario.Empresa.EmpresaPai != null)
                    {
                        Dominio.Entidades.Empresa empresaUsuario = repEmpresa.BuscarPorCodigo(Usuario.Empresa.EmpresaPai.Codigo);

                        if (empresaUsuario != null)
                        {
                            if (!string.IsNullOrWhiteSpace(empresaUsuario.Telefone) && !string.IsNullOrWhiteSpace(empresaUsuario.TelefoneContato))
                                ViewBag.DadosEmpresaPai += " - Fone: " + empresaUsuario.Telefone + " | " + empresaUsuario.TelefoneContato;
                            else if (!string.IsNullOrWhiteSpace(empresaUsuario.Telefone))
                                ViewBag.DadosEmpresaPai += " - Fone: " + empresaUsuario.Telefone;

                            if (!string.IsNullOrWhiteSpace(empresaUsuario.Email))
                                ViewBag.DadosEmpresaPai += " - E-mail: " + empresaUsuario.Email;

                            if (!string.IsNullOrWhiteSpace(empresaUsuario.Contato))
                                ViewBag.DadosEmpresaPai += " - Contato/Skype: " + empresaUsuario.Contato;
                        }
                    }
                }

                ObterFormulariosFavoritos(unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }

            buscarConfiguracaoPadrao();

            string caminhoBaseViews = "~/Views/MasterLayout";
            string caminhosViewMasterLayout = portalCliente ? $"{caminhoBaseViews}/IndexCliente.cshtml" : $"{caminhoBaseViews}/Index.cshtml";

            return View(caminhosViewMasterLayout);
        }

        #endregion

        #region Métodos Privados
        private void AtualizarVersaoEmSegundoPlano(string version, string stringConexaoAdmin)
        {
            var host = _conexao.ObterHost;
            var chaveCacheEmpresa = "EmpresaMultisoftware" + host;

            _ = Task.Run(() =>
            {
                try
                {
                    var codigoEmpresa = CacheProvider.Instance.Get<int>(chaveCacheEmpresa);
                    if (codigoEmpresa == 0) return;

                    using var uow = new Repositorio.UnitOfWork(stringConexaoAdmin);
                    var versaoAplicacao = new Repositorio.VersaoAplicacao(uow);

                    var ambiente = IsHomologacao ? "HOMOLOG" : "PROD";
                    var versao = versaoAplicacao.ConsultarPorAmbienteECliente(ambiente, codigoEmpresa)
                                ?? new Dominio.Entidades.VersaoAplicacao();

                    if (versao.VersaoSGTWebAdmin == version) return;

                    versao.CodigoCliente = codigoEmpresa;
                    versao.VersaoSGTWebAdmin = version;
                    versao.Ambiente = ambiente;

                    versaoAplicacao.AtualizarNumeroVersao(versao);
                }
                catch (Exception ex)
                {

                }
            });
        }

        private string ObterDataAtualizacao()
        {
            string assemblyPath = Assembly.GetExecutingAssembly().Location;
            DateTime buildDate = Utilidades.IO.FileStorageService.LocalStorage.GetLastWriteTime(assemblyPath);
            try
            {
                string timeZoneId = Usuario?.Empresa?.FusoHorario ?? string.Empty;
                if (!string.IsNullOrEmpty(timeZoneId))
                {
                    TimeZoneInfo userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                    buildDate = TimeZoneInfo.ConvertTime(buildDate, userTimeZone);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }

            return buildDate.ToString("dd/MM/yyyy HH:mm");
        }

        private bool ExecutarThread()
        {
#if DEBUG
            return false;
#endif
            return true;
        }

        /// <summary>
        /// Retorna campos da configuração geral para ficar disponível no front-end
        /// </summary>
        private void buscarConfiguracaoPadrao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoEMP repIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoAprovacao repositorioConfiguracaoAprovacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAprovacao(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFatura repConfiguracaoFatura = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFatura(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento repConfiguracaoCargaEmissaoDocumento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia repositorioConfiguracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo repositorioConfiguracaoVeiculo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoMotorista repositorioConfiguracaoMotorista = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMotorista(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro repositorioConfiguracaoContratoFreteTerceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repositorioConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete repositorioConfiguracaoTabelaFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada repConfiguracaoDocumentoEntrada = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta repositorioConfiguracaoAgendamentoColeta = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga repositorioConfiguracaoMontagemCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguraoAcertoViagem repConfiguraoAcertoViagem = new Repositorio.Embarcador.Configuracoes.ConfiguraoAcertoViagem(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repositorioConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte repositorioConfiguracaoCargaDadosTransporte = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoTransportador reposotorioConfiguracaoTransportador = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTransportador(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto repConfiguracaoCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado repConfiguracaoChamado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao repositorioConfiguracaoRoteirizacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao(unitOfWork);
                Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoEmissaoDocumento repTipoOperacaoEmissao = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoEmissaoDocumento(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repositorioConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repositorioConfiguracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoComissaoMotorista repositorioConfiguracaoComissaoMotorista = new Repositorio.Embarcador.Configuracoes.ConfiguracaoComissaoMotorista(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPortalMultiClifor repositorioConfiguracaoPortalMultiCliFor = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPortalMultiClifor(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPessoa repositorioConfiguracaoPessoa = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPessoa(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoDownloadArquivos repositorioConfiguracaoDownloadArquivos = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDownloadArquivos(unitOfWork);

                //APENAS CAMPOS DA CONFIGURAÇÃO GERAL PODE SER ADICIONADO NESSA BUSCA
                //Outras configurações, por exemplo, de integrações, devesse fazer um método no próprio controller da tela que irá utilizar

                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(Usuario?.Codigo ?? 0);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada configuracaoDocumentoEntrada = repConfiguracaoDocumentoEntrada.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP integracaoEMP = repIntegracaoEMP.Buscar();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAprovacao configuracaoAprovacao = repositorioConfiguracaoAprovacao.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFatura configuracaoFatura = repConfiguracaoFatura.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repConfiguracaoPedido.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento configuracaoCargaEmissaoDocumento = repConfiguracaoCargaEmissaoDocumento.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = repositorioConfiguracaoOcorrencia.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVeiculo configuracaoVeiculo = repositorioConfiguracaoVeiculo.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMotorista configuracaoMotorista = repositorioConfiguracaoMotorista.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro configuracaoContratoFreteTerceiro = repositorioConfiguracaoContratoFreteTerceiro.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repositorioConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete = repositorioConfiguracaoTabelaFrete.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta configuracaoAgendamentoColeta = repositorioConfiguracaoAgendamentoColeta.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMontagemCarga configuracaoMontagemCarga = repositorioConfiguracaoMontagemCarga.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguraoAcertoViagem configuraoAcertoViagem = repConfiguraoAcertoViagem.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = repositorioConfiguracaoControleEntrega.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte configuracaoCargaDadosTransporte = repositorioConfiguracaoCargaDadosTransporte.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento = repConfiguracaoMonitoramento.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTransportador configuracaoTransportador = reposotorioConfiguracaoTransportador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = repConfiguracaoCanhoto.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repositorioConfiguracaoGeral.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado = repConfiguracaoChamado.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRoteirizacao configuracaoRoteirizacao = repositorioConfiguracaoRoteirizacao.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repositorioConfiguracaoWebService.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = repositorioConfiguracaoJanelaCarregamento.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoComissaoMotorista configuracaoComissaoMotorista = repositorioConfiguracaoComissaoMotorista.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPortalMultiClifor configuracaoPortalMultiCliFor = repositorioConfiguracaoPortalMultiCliFor.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPessoa configuracaoPessoa = repositorioConfiguracaoPessoa.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDownloadArquivos configuracaoDownloadArquivos = repositorioConfiguracaoDownloadArquivos.BuscarPrimeiroRegistro();

                //LER AS INFORMAÇÕES ABAIXO ANTES DE ADICIONAR NOVA CONSULTA:

                //APENAS CAMPOS DA CONFIGURAÇÃO GERAL PODE SER ADICIONADO NESSA BUSCA
                //Outras configurações, por exemplo, de integrações, devesse fazer um método no próprio controller da tela que irá utilizar

                if (configuracaoEmbarcador == null)
                    throw new Exception("Não existe uma configuração padrão para o " + Cliente.RazaoSocial + ", por favor configure");

                var retorno = new
                {
                    ConfiguracaoPedido = ObterConfiguracaoPedido(configuracaoPedido),
                    configuracaoEmbarcador.Codigo,
                    configuracaoEmbarcador.QuantidadeMaximaDiasRelatorios,
                    configuracaoEmbarcador.FiltrarCargasSemDocumentosParaChamados,
                    configuracaoEmbarcador.BuscarProdutoPredominanteNoPedido,
                    configuracaoEmbarcador.PreencherMotoristaAutomaticamenteAoInformarVeiculo,
                    configuracaoEmbarcador.ControlarCanhotosDasNFEs,
                    configuracaoEmbarcador.PermiteAdicionarNotaManualmente,
                    configuracaoEmbarcador.ExigeNumeroDeAprovadoresNasAlcadas,
                    configuracaoEmbarcador.DescricaoProdutoPredominatePadrao,
                    configuracaoEmbarcador.FiltrarBuscaVeiculosPorEmpresa,
                    configuracaoEmbarcador.SempreDuplicarCargaCancelada,
                    configuracaoEmbarcador.ExibirKmUtilizadoContratoFretePorPeriodoVigenciaContrato,
                    configuracaoEmbarcador.DefaultTrueDuplicarCarga,
                    configuracaoEmbarcador.PermitirCancelamentoTotalCarga,
                    configuracaoEmbarcador.PermitirInformarDadosTransportadorCargaEtapaNFe,
                    configuracaoEmbarcador.PermitirConfirmacaoImpressaoME,
                    configuracaoEmbarcador.CalcularFreteFilialEmissoraPorTabelaDeFrete,
                    configuracaoEmbarcador.NaoPermiteEmitirCargaSemAverbacao,
                    configuracaoEmbarcador.PermitirOperadorInformarValorFreteMaiorQueTabela,
                    configuracaoEmbarcador.PermitirRetornoAgNotasFiscais,
                    configuracaoEmbarcador.PermitirTransportadorAlterarModeloVeicular,
                    configuracaoEmbarcador.ObrigatorioInformarDadosContratoFrete,
                    configuracaoEmbarcador.SituacaoCargaAposConfirmacaoImpressao,
                    configuracaoEmbarcador.SituacaoCargaAposEmissaoDocumentos,
                    configuracaoEmbarcador.SituacaoCargaAposFinalizacaoDaCarga,
                    configuracaoEmbarcador.TempoSegundosParaInicioEmissaoDocumentos,
                    configuracaoEmbarcador.UtilizarIntegracaoPedido,
                    configuracaoEmbarcador.CadastrarMotoristaMobileAutomaticamente,
                    configuracaoEmbarcador.TipoMontagemCargaPadrao,
                    configuracaoEmbarcador.TipoFiltroDataMontagemCarga,
                    configuracaoEmbarcador.InformaHorarioCarregamentoMontagemCarga,
                    configuracaoEmbarcador.OcultaGerarCarregamentosMontagemCarga,
                    configuracaoEmbarcador.LimparTelaAoSalvarMontagemCarga,
                    configuracaoEmbarcador.UtilizarAlcadaAprovacaoCarregamento,
                    configuracaoEmbarcador.InformaApoliceSeguroMontagemCarga,
                    configuracaoEmbarcador.InformarTipoCondicaoPagamentoMontagemCarga,
                    configuracaoEmbarcador.TipoChamado,
                    configuracaoEmbarcador.UtilizarNFeEmHomologacao,
                    configuracaoEmbarcador.UtilizarSituacaoNaJanelaDescarregamento,
                    configuracaoEmbarcador.UtilizarAlcadaAprovacaoPagamento,
                    configuracaoEmbarcador.UtilizarAlcadaAprovacaoAlteracaoRegraICMS,
                    configuracaoEmbarcador.PermitirTrocarPedidoCarga,
                    configuracaoEmbarcador.UtilizarTempoCarregamentoPorPeriodo,
                    configuracaoEmbarcador.UtilizarFilaCarregamento,
                    configuracaoEmbarcador.PermitirDesagendarCargaJanelaCarregamento,
                    configuracaoEmbarcador.NaoExigeInformarDisponibilidadeDeVeiculo,
                    configuracaoEmbarcador.PermitirInformarTipoTransportadorPorDataCarregamentoJanelaCarregamento,
                    configuracaoEmbarcador.ExigeInformarCienciaDoEnvioDasNotasAntesDeEmitirDocumentos,
                    configuracaoEmbarcador.FiltrarAlcadasDoUsuario,
                    configuracaoEmbarcador.ProvisionarDocumentosEmitidos,
                    configuracaoEmbarcador.ExigirChamadoParaAbrirOcorrencia,
                    configuracaoEmbarcador.AlterarDataCarregamentoEDescarregamentoPorPeriodo,
                    ObrigatorioRegrasOcorrencia = configuracaoEmbarcador.ObrigatorioRegrasOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Sim,

                    TipoServicoMultisoftware,
                    PermiteAuditar = usuario?.PermiteAuditar ?? false,
                    UsuarioAdministrador = usuario?.UsuarioAdministrador ?? false,
                    UsuarioMultisoftware = usuario?.UsuarioMultisoftware ?? false,
                    LoginAD = Startup.LoginAD || Startup.AzureAdAuthentication || Startup.SamlAuthentication,
                    UsuarioLogado = usuario?.Nome.ToString() ?? string.Empty,
                    CodigoUsuarioLogado = usuario?.Codigo.ToString() ?? string.Empty,
                    CPFUsuarioLogado = usuario?.CPF.ToString() ?? string.Empty,
                    PermiteAssumirOcorrencia = usuario?.PermiteAssumirOcorrencia ?? false,
                    PermiteVisualizarTitulosPagamentoSalario = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? (usuario?.PermiteVisualizarTitulosPagamentoSalario ?? false) : true,
                    PermiteFaturamentoPermissaoExclusiva = usuario?.PerfilAcesso?.PermiteFaturamentoPermissaoExclusiva ?? false,

                    configuracaoEmbarcador.PermitirSalvarDadosTransporteCargaSemSolicitarNFes,
                    configuracaoEmbarcador.SolicitarNotasFiscaisAoSalvarDadosTransportador,
                    configuracaoEmbarcador.PossuiWMS,
                    configuracaoEmbarcador.TipoContratoFreteTerceiro,
                    configuracaoEmbarcador.ValidarTabelaFreteNoPedido,
                    configuracaoEmbarcador.CargaTransbordoNaEtapaInicial,
                    configuracaoEmbarcador.PermiteSelecionarQualquerNaturezaNFEntrada,
                    configuracaoEmbarcador.PadraoArmazenamentoFisicoCanhotoCTe,
                    configuracaoEmbarcador.NumeroCasasDecimaisQuantidadeProduto,
                    configuracaoEmbarcador.ExigirCodigoIntegracaoTransportador,
                    configuracaoEmbarcador.AcertoDeViagemComDiaria,
                    configuracaoEmbarcador.PermiteEmissaoCargaSomenteComTracao,
                    configuracaoEmbarcador.TipoPagamentoContratoFrete,
                    configuracaoEmbarcador.PermitirAdicionarCargaFluxoPatio,
                    configuracaoEmbarcador.ExigirDatasValidadeCadastroMotorista,
                    configuracaoEmbarcador.UtilizaChat,
                    configuracaoEmbarcador.DataCompetenciaDocumentoEntrada,
                    configuracaoEmbarcador.OcultarBuscaRotaNaCarga,
                    configuracaoEmbarcador.TrocarPreCargaPorCarga,
                    configuracaoEmbarcador.HabilitarRelatorioDeTroca,
                    configuracaoEmbarcador.HabilitarRelatorioBoletimViagem,
                    configuracaoEmbarcador.HabilitarRelatorioDiarioBordo,
                    configuracaoEmbarcador.UtilizarAlcadaAprovacaoAlteracaoValorFrete,
                    configuracaoEmbarcador.UtilizarAlcadaAprovacaoTabelaFrete,
                    configuracaoAprovacao.UtilizarAlcadaAprovacaoTabelaFretePorTabelaFreteCliente,
                    configuracaoEmbarcador.ExibirValoresPedidosNaCarga,
                    configuracaoEmbarcador.BloquearCamposTransportadorQuandoEtapaNotas,
                    configuracaoEmbarcador.DataEntradaDocumentoEntrada,
                    configuracaoEmbarcador.ExigirRotaRoteirizadaNaCarga,
                    configuracaoEmbarcador.ExigirCargaRoteirizada,
                    configuracaoEmbarcador.TipoUltimoPontoRoteirizacao,
                    configuracaoEmbarcador.UtilizarControlePallets,
                    configuracaoEmbarcador.VisualizarTodosItensOrdemCompraDocumentoEntrada,
                    configuracaoEmbarcador.ExibirAprovadoresOcorrenciaPortalTransportador,
                    configuracaoEmbarcador.PermitirAutomatizarPagamentoTransportador,
                    QuantidadeRegistrosGridDocumentoEntrada = configuracaoEmbarcador.QuantidadeRegistrosGridDocumentoEntrada ?? 0,
                    configuracaoEmbarcador.NaoPermitirExclusaoPedido,
                    configuracaoEmbarcador.ObrigarMotivoSolicitacaoFrete,
                    configuracaoEmbarcador.PermitirDisponibilizarCargaParaTransportador,
                    configuracaoEmbarcador.PermitirLiberarCargaSemNFe,
                    configuracaoEmbarcador.ExigirCategoriaCadastroPessoa,
                    configuracaoEmbarcador.TipoOrdemServicoObrigatorio,
                    configuracaoEmbarcador.UtilizarBuscaRotaFreteManualCarga,
                    configuracaoEmbarcador.ReplicarCadastroVeiculoIntegracaoTransportadorDiferente,
                    configuracaoEmbarcador.CamposSecundariosObrigatoriosPedido,
                    configuracaoEmbarcador.ImportarCargasMultiEmbarcador,
                    configuracaoEmbarcador.ForcarFiltroModeloNaConsultaVeiculo,
                    configuracaoEmbarcador.NaoExibirOpcaoParaDelegar,
                    configuracaoEmbarcador.ExibirEspecieDocumentoCteComplementarOcorrencia,
                    configuracaoEmbarcador.PermitirAlterarLacres,
                    configuracaoEmbarcador.ExibirTipoLacre,
                    configuracaoEmbarcador.NaoUtilizarUsuarioTransportadorTerceiro,
                    configuracaoEmbarcador.AbrirRateioDespesaVeiculoAutomaticamente,
                    configuracaoEmbarcador.RatearFretePedidosAposLiberarEmissaoSemNFe,
                    configuracaoEmbarcador.BloquearDatasRetroativasPedido,
                    configuracaoEmbarcador.PermitirInformarDataRetiradaCtrnCarga,
                    configuracaoEmbarcador.PermitirInformarNumeroContainerCarga,
                    configuracaoEmbarcador.PermitirInformarTaraContainerCarga,
                    configuracaoEmbarcador.PermitirInformarMaxGrossCarga,
                    configuracaoEmbarcador.TipoRecibo,
                    configuracaoEmbarcador.PermitirInformarAnexoContainerCarga,
                    configuracaoEmbarcador.PermitirInformarDatasCarregamentoCarga,
                    configuracaoEmbarcador.PermitirInformarLacreJanelaCarregamentoTransportador,
                    configuracaoEmbarcador.PermitirRejeitarCargaJanelaCarregamentoTransportador,
                    configuracaoEmbarcador.NaoLancarDescontosDasOcorrenciasNoAcertoDeViagem,
                    configuracaoEmbarcador.CamposSecundariosObrigatoriosOrdemServico,
                    configuracaoEmbarcador.ExibirLimiteCarregamento,
                    configuracaoEmbarcador.ExibirPrevisaoCarregamento,
                    configuracaoEmbarcador.DisponibilizarCargaAutomaticamenteParaTransportadorComMenorValorFreteTabela,
                    configuracaoEmbarcador.ExibirDisponibilidadeFrotaCarregamento,
                    configuracaoEmbarcador.AjustarTipoOperacaoPeloPeso,
                    configuracaoEmbarcador.PermitirSalvarDadosParcialmenteInformadosEtapaTransportador,
                    configuracaoEmbarcador.ExibirPedidoDeColeta,
                    configuracaoEmbarcador.ExibirFaixaTemperaturaNaCarga,
                    configuracaoEmbarcador.UtilizarPercentualEmRelacaoValorFreteLiquidoCarga,
                    configuracaoEmbarcador.UtilizaEmissaoMultimodal,
                    configuracaoEmbarcador.UtilizarEtiquetaDetalhadaWMS,
                    configuracaoEmbarcador.NaoExigeAceiteTransportadorParaNFDebito,
                    configuracaoEmbarcador.ExigirEmailPrincipalCadastroPessoa,
                    configuracaoEmbarcador.ExibirAssociacaoClientesNoPedido,
                    configuracaoEmbarcador.NaoGerarCarregamentoRedespacho,
                    configuracaoEmbarcador.UtilizarRegraICMSParaDescontarValorICMS,
                    configuracaoEmbarcador.NaoPermitirImpressaoContratoFretePendente,
                    configuracaoEmbarcador.ExibirAbaDetalhePedidoExportacaoNaMontagemCarga,
                    configuracaoEmbarcador.PermiteSelecionarRotaMontagemCarga,
                    configuracaoEmbarcador.ValidarSomenteFreteLiquidoNaImportacaoCTe,
                    configuracaoEmbarcador.ValidarPorRaizDoTransportadorNaImportacaoCTe,
                    configuracaoEmbarcador.NaoValidarDadosParticipantesNaImportacaoCTe,
                    configuracaoEmbarcador.ExigirAceiteTermoUsoSistema,
                    configuracaoEmbarcador.PermitirDownloadDANFE,
                    configuracaoEmbarcador.PermitirDownloadXmlEtapaNfe,
                    configuracaoEmbarcador.DesabilitarSaldoViagemAcerto,
                    configuracaoEmbarcador.PadraoTagValePedagioVeiculos,
                    configuracaoEmbarcador.GerarCargaDeMDFesNaoVinculadosACargas,
                    configuracaoEmbarcador.ServerChatURL,
                    configuracaoEmbarcador.ReduzirRetencaoISSValorAReceberNFSManual,
                    TipoIntegracaoValePedagio = configuracaoEmbarcador.TipoIntegracaoValePedagio != null ? configuracaoEmbarcador.TipoIntegracaoValePedagio.Tipo : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada,
                    configuracaoEmbarcador.NaoExigirExpedidorNoRedespacho,
                    configuracaoEmbarcador.ExigirTipoSeparacaoMontagemCarga,
                    configuracaoEmbarcador.AcoplarMotoristaAoVeiculoAoSelecionarNaCarga,
                    configuracaoEmbarcador.InformarPercentualAdiantamentoCarga,
                    configuracaoEmbarcador.CentroResultadoPedidoObrigatorio,
                    configuracaoEmbarcador.ExibirInformacoesBovinos,
                    configuracaoEmbarcador.PermitirAssumirChamadoDeOutroResponsavel,
                    configuracaoEmbarcador.UtilizaMoedaEstrangeira,
                    configuracaoEmbarcador.TipoFechamentoFrete,
                    configuracaoEmbarcador.UtilizaPgtoCanhoto,
                    configuracaoEmbarcador.NaoCadastrarProdutoAutomaticamenteDocumentoEntrada,
                    configuracaoEmbarcador.PreencherUltimoKMEntradaGuaritaTMS,
                    configuracaoEmbarcador.GerarPagamentoBloqueado,
                    configuracaoEmbarcador.GerarSomenteDocumentosDesbloqueados,
                    configuracaoEmbarcador.HabilitarMultiplaSelecaoEmpresaNFSManual,
                    configuracaoEmbarcador.InformarDataViagemExecutadaPedido,
                    configuracaoEmbarcador.GerarTituloFolhaPagamento,
                    configuracaoEmbarcador.BloquearFechamentoAbastecimentoSemplaca,
                    configuracaoEmbarcador.AgruparIntegracaoCargaComTipoOperacaoDiferente,
                    configuracaoEmbarcador.RatearValorOcorrenciaPeloValorFreteCTeOriginal,
                    configuracaoEmbarcador.IniciarCadastroFuncionarioMotoristaSempreInativo,
                    configuracaoEmbarcador.NaoPermitirCancelarCargaComInicioViagem,
                    configuracaoEmbarcador.ExibirObservacaoAprovadorAutorizacaoOcorrencia,
                    configuracaoEmbarcador.SolicitarConfirmacaoPedidoSemMotoristaVeiculo,
                    configuracaoEmbarcador.SolicitarConfirmacaoPedidoDuplicado,
                    configuracaoEmbarcador.SolicitarConfirmacaoMovimentoFinanceiroDuplicado,
                    configuracaoEmbarcador.NaoObrigarDataSaidaRetornoPedido,
                    configuracaoEmbarcador.PermitirSelecionarReboquePedido,
                    configuracaoEmbarcador.EncerrarMDFeAutomaticamente,
                    configuracaoEmbarcador.BloquearAlteracaoVeiculoPortalTransportador,
                    configuracaoEmbarcador.HabilitarFSDA,
                    configuracaoEmbarcador.HabilitarHoraFiltroDataInicialFinalRelatorioCargas,
                    configuracaoEmbarcador.PermiteEmitirCTeComplementarManualmente,
                    configuracaoEmbarcador.PermiteInformarChamadosNoLancamentoOcorrencia,
                    configuracaoEmbarcador.ExibirAliquotaEtapaFreteCarga,
                    configuracaoEmbarcador.ExigirEmpresaTituloFinanceiro,
                    configuracaoEmbarcador.ExibirJustificativaCancelamentoCarga,
                    configuracaoEmbarcador.PermitirImportarOcorrencias,
                    configuracaoEmbarcador.EmitirNFeRemessaNaCarga,
                    configuracaoEmbarcador.PermitirAlterarDataCarregamentoCargaNoPedido,
                    configuracaoEmbarcador.GerarCargaComAgrupamentoNaMontagemCargaComoCargaDeComplemento,
                    configuracaoEmbarcador.BloquearCamposOcorrenciaImportadosDoAtendimento,
                    configuracaoEmbarcador.ExibirOpcaoReenviarNotfisComFalhas,
                    configuracaoEmbarcador.ExibirOpcaoDownloadPlanilhaRateioOcorrencia,
                    configuracaoEmbarcador.ExibirNumeroPagerEtapaInicialCarga,
                    configuracaoEmbarcador.ExigirDataEntregaNotaClienteCanhotos,
                    configuracaoEmbarcador.HabilitarControleFluxoNFeDevolucaoChamado,
                    configuracaoEmbarcador.LancarOsServicosDaOrdemDeServicoAutomaticamente,
                    configuracaoEmbarcador.ExibirClassificacaoNFe,
                    configuracaoEmbarcador.ExibirSenhaCadastroPessoa,
                    configuracaoEmbarcador.VisualizarTipoOperacaoDoPedidoPorTomador,
                    configuracaoEmbarcador.UsarGrupoDeTipoDeOperacaoNoMonitoramento,
                    configuracaoEmbarcador.UsarGrupoDeTipoDeOperacaoNoMonitoramentoOcultarGrupoStatusViagem,
                    configuracaoEmbarcador.OcultarInformacoesFaturamentoAcertoViagem,
                    configuracaoEmbarcador.SalvarAnaliseEmAnexoAoLiberarOcorrenciaChamado,
                    configuracaoEmbarcador.PermitirEstornarAprovacaoChamadoLiberado,
                    configuracaoEmbarcador.UtilizaListaDinamicaDatasChamado,
                    QuantidadeWidgetControleColetaEntregaUsuario = 3,
                    configuracaoEmbarcador.GerarOSAutomaticamenteCadastroVeiculoEquipamento,
                    configuracaoEmbarcador.Pais,
                    configuracaoEmbarcador.ControlarAgendamentoSKU,
                    configuracaoEmbarcador.UtilizaAppTrizy,
                    configuracaoEmbarcador.GerarOcorrenciaComplementoSubcontratacao,
                    configuracaoEmbarcador.GerarPreviewDOCCOBFatura,
                    configuracaoEmbarcador.PermitirAlterarDataPrevisaoEntregaPedidoNoCarga,
                    configuracaoEmbarcador.NecessarioInformarJustificativaAoAlterarDataSaidaOuPrevisaoEntregaPedidoNaCarga,
                    configuracaoEmbarcador.ExibirFiltrosNotasCompativeisCarga,
                    configuracaoEmbarcador.ExigirMotivoOcorrencia,
                    configuracaoEmbarcador.OrdenarCargasMobileCrescente,
                    configuracaoCanhoto.DisponibilizarOpcaoDeCanhotoExtraviado,
                    configuracaoCanhoto.PrazoParaReverterDigitalizacaoAposAprovacao,
                    configuracaoCanhoto.NaoAtualizarTelaCanhotosAposAprovacaoRejeicao,

                    CNPJPostoPadrao = configuracaoEmbarcador.PostoPadrao?.CPF_CNPJ ?? 0,
                    PostoPadrao = configuracaoEmbarcador.PostoPadrao?.Descricao ?? string.Empty,
                    CodigoCombustivelPadrao = configuracaoEmbarcador.CombustivelPadrao?.Codigo ?? 0,
                    CombustivelPadrao = configuracaoEmbarcador.CombustivelPadrao?.Descricao ?? string.Empty,
                    ValorCombustivelPadrao = configuracaoEmbarcador.CombustivelPadrao?.UltimoCusto.ToString("n4") ?? string.Empty,

                    configuracaoEmbarcador.UtilizarLocalidadePrestacaoPedido,
                    configuracaoEmbarcador.NaoGerarSenhaAgendamento,
                    configuracaoEmbarcador.PermiteAlterarRotaEmCargaFinalizada,
                    configuracaoEmbarcador.NaoUtilizarDataTerminoProgramacaoVeiculo,
                    configuracaoEmbarcador.NaoExibirInfosAdicionaisGridPatio,
                    configuracaoEmbarcador.NaoPreencherSerieCTeManual,
                    configuracaoEmbarcador.NaoObrigarInformarSegmentoNoAcertoDeViagem,
                    configuracaoEmbarcador.PermiteCadastrarLatLngEntregaLocalidade,
                    configuracaoEmbarcador.NaoDescontarValorSaldoMotorista,
                    configuracaoEmbarcador.ExibirJanelaDescargaPorPeriodo,
                    configuracaoEmbarcador.NaoUtilizarSerieCargaCTeManual,
                    configuracaoEmbarcador.UtilizaMultiplosLocaisArmazenamento,
                    configuracaoEmbarcador.HabilitarEnvioAbastecimentoExterno,
                    configuracaoEmbarcador.VisualizarDatasRaioNoAtendimento,
                    configuracaoEmbarcador.ExigirClienteResponsavelPeloAtendimento,
                    configuracaoEmbarcador.ModeloVeicularCargaNaoObrigatorioMontagemCarga,
                    configuracaoEmbarcador.MotoristaObrigatorioMontagemCarga,
                    configuracaoEmbarcador.VeiculoObrigatorioMontagemCarga,
                    configuracaoEmbarcador.GerarOcorrenciaParaCargaAgrupada,
                    configuracaoEmbarcador.SomenteAutorizadoresPodemDelegarOcorrencia,
                    configuracaoEmbarcador.FormaPreenchimentoCentroResultadoPedido,
                    configuracaoEmbarcador.ExigirNumeroDocumentoTituloFinanceiro,
                    configuracaoEmbarcador.UsaPermissaoControladorRelatorios,
                    configuracaoEmbarcador.RaioMaximoGeoLocalidadeGeoCliente,
                    configuracaoEmbarcador.TelaMonitoramentoPadraoFiltroDataInicialFinal,
                    configuracaoEmbarcador.DiasAnterioresPesquisaCarga,
                    configuracaoEmbarcador.CalcularFreteCliente,
                    configuracaoEmbarcador.PermitirGerarNotaMesmoPedidoCarga,
                    configuracaoEmbarcador.PermitirGerarNotaMesmaCarga,
                    configuracaoEmbarcador.OcultarInformacoesResultadoViagemAcertoViagem,
                    configuracaoEmbarcador.BuscarMotoristaDaCargaLancamentoAbastecimentoAutomatico,
                    configuracaoEmbarcador.PermitirInformarQuilometragemTabelaFreteCliente,
                    configuracaoEmbarcador.TabelaFretePrecisaoDinheiroDois,
                    configuracaoEmbarcador.VisualizarValorNFSeDescontandoISSRetido,
                    configuracaoEmbarcador.DocumentoImpressaoPadraoCarga,
                    configuracaoEmbarcador.NaoConsiderarProdutosSemPesoParaSumarizarVolumes,
                    configuracaoEmbarcador.ExibirSaldoPrevistoAcertoViagem,
                    configuracaoEmbarcador.NaoGerarAverbacaoCTeQuandoPedidoTiverAverbacao,
                    configuracaoFatura.PreencherPeriodoFaturaComDataAtual,
                    configuracaoFatura.InformarDataCancelamentoCancelamentoFatura,
                    configuracaoFatura.DisponbilizarProvisaoContraPartidaParaCancelamento,
                    configuracaoEmbarcador.NaoBuscarDataInicioViagemAcerto,
                    configuracaoEmbarcador.VincularNotasParciaisPedidoPorProcesso,
                    configuracaoEmbarcador.PermiteInformarModeloVeicularCargaOrigem,
                    configuracaoEmbarcador.RetornarCargaDocumentoEmitido,
                    configuracaoEmbarcador.ExigirTipoMovimentoLancamentoMovimentoFinanceiroManual,
                    configuracaoEmbarcador.PossuiValidacaoParaLiberacaoCargaComNotaJaUtilizada,
                    configuracaoEmbarcador.UtilizaSuprimentoDeGas,
                    configuracaoEmbarcador.PermiteImportarPlanilhaValoresFreteNFSManual,
                    configuracaoEmbarcador.PossuiMonitoramento,
                    configuracaoEmbarcador.ExibirFiltroEColunaCodigoPedidoClienteGestaoDocumentos,
                    configuracaoEmbarcador.UtilizarCentroResultadoNoRateioDespesaVeiculo,
                    configuracaoPedido.ApagarCampoRotaAoDuplicarPedido,
                    configuracaoPedido.PessoasNaoObrigatorioProdutoEmbarcador,
                    configuracaoPedido.NaoApagarCamposDatasAoDuplicarPedido,
                    configuracaoPedido.PermitirBuscarValoresTabelaFrete,
                    configuracaoPedido.UtilizarRelatorioPedidoComoStatusEntrega,
                    configuracaoPedido.BloquearDuplicarPedido,
                    configuracaoPedido.NaoPreencherRotaFreteAutomaticamente,
                    configuracaoPedido.NaoSelecionarModeloVeicularAutomaticamente,
                    configuracaoPedido.NaoPermitirInformarVeiculoDuplicadoPedidoCargaAberta,
                    configuracaoPedido.HabilitarBIDTransportePedido,
                    configuracaoPedido.UsarFatorConversaoProdutoEmPedidoPaletizado,
                    configuracaoPedido.PermitirSelecionarCentroDeCarregamentoNoPedido,
                    configuracaoEmbarcador.HabilitarDescontoGestaoDocumento,
                    configuracaoEmbarcador.GerarContratoTerceiroSemInformacaoDoFrete,
                    configuracaoEmbarcador.LiberarIntegracaoTransportadorDeCargaImportarDocumentoManual,
                    configuracaoEmbarcador.NaoValidarTabelaFreteMesmaIncidenciaImportacao,
                    configuracaoEmbarcador.UsarAlcadaAprovacaoGestaoDocumentos,
                    configuracaoEmbarcador.UtilizarMultiplosModelosVeicularesPedido,
                    configuracaoEmbarcador.SolicitarValorFretePorTonelada,
                    configuracaoEmbarcador.GerarFluxoPatioDestino,
                    configuracaoCargaEmissaoDocumento.ConsultarDocumentosDestinadosCarga,
                    configuracaoCargaEmissaoDocumento.UtilizarNumeroOutroDocumento,
                    configuracaoEmbarcador.HabilitarFichaMotoristaTodos,
                    configuracaoEmbarcador.PermitirDarBaixaFaturasCTe,
                    configuracaoEmbarcador.LiberarSelecaoQualquerVeiculoJanelaTransportadorComConfirmacao,
                    configuracaoEmbarcador.LiberarSelecaoQualquerVeiculoJanelaTransportador,
                    configuracaoEmbarcador.SolicitarAprovacaoFolgaAcertoViagem,
                    configuracaoEmbarcador.AtivarBotaoImportacao,
                    configuracaoEmbarcador.TelaCargaApresentarUltimaPosicaoVeiculo,
                    configuracaoEmbarcador.RelatorioEntregaPorPedido,
                    configuracaoEmbarcador.NaoObrigarInformarFrotaNoAcertoDeViagem,
                    configuracaoEmbarcador.PermitirConsultaDeValoresPedagio,
                    PercentualAdiantamentoTerceiroPadrao = configuracaoEmbarcador.PercentualAdiantamentoTerceiroPadrao.ToString("n2"),
                    PercentualMinimoAdiantamentoTerceiroPadrao = configuracaoEmbarcador.PercentualMinimoAdiantamentoTerceiroPadrao.ToString("n2"),
                    PercentualMaximoAdiantamentoTerceiroPadrao = configuracaoEmbarcador.PercentualMaximoAdiantamentoTerceiroPadrao.ToString("n2"),
                    configuracaoOcorrencia.PermiteInformarCentroResultadoAprovacaoOcorrencia,
                    configuracaoOcorrencia.PermiteDownloadCompactadoArquivoOcorrencia,
                    configuracaoOcorrencia.UtilizarBonificacaoParaTransportadoresViaOcorrencia,
                    configuracaoOcorrencia.PermitirDefinirCSTnoTipoDeOcorrencia,
                    configuracaoEmbarcador.PermitirEnviarEmailAutorizacaoEmbarque,
                    PercentualComissaoPadrao = configuracaoEmbarcador.PercentualComissaoPadrao.ToString("n2"),
                    PercentualMediaEquivalente = configuracaoEmbarcador.PercentualMediaEquivalente.ToString("n2"),
                    PercentualEquivaleEquivalente = configuracaoEmbarcador.PercentualEquivaleEquivalente.ToString("n2"),
                    PercentualAdvertenciaEquivalente = configuracaoEmbarcador.PercentualAdvertenciaEquivalente.ToString("n2"),
                    configuracaoEmbarcador.ExigirAnexosNoCadastroDoTransportador,
                    configuracaoEmbarcador.GerarReciboDetalhadoAcertoViagem,
                    configuracaoEmbarcador.UtilizarComissaoPorCargo,
                    configuracaoEmbarcador.PermitirCancelarPedidosSemDocumentos,
                    configuracaoEmbarcador.VisualizarVeiculosPropriosETerceiros,
                    configuracaoEmbarcador.NaoExigirMotivoAprovacaoCTeInconsistente,
                    configuracaoEmbarcador.VisualizarReciboPorMotoristaNoAcertoDeViagem,
                    configuracaoEmbarcador.InformarAjusteManualCargasImportadasEmbarcador,
                    SistemaEstrangeiro = configuracaoEmbarcador.Pais == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPais.Exterior ? true : false,
                    configuracaoVeiculo.ObrigatorioSegmentoVeiculo,
                    configuracaoVeiculo.InformarKmMovimentacaoPlaca,
                    configuracaoVeiculo.VisualizarApenasVeiculosAtivos,
                    configuracaoContratoFreteTerceiro.UtilizarTaxaPagamentoContratoFreteTerceiro,
                    configuracaoContratoFreteTerceiro.UtilizarNovoLayoutPagamentoAgregado,
                    GeraNumeroSequenciar = configuracaoContratoFreteTerceiro.GerarNumeroContratoTransportadorFreteSequencial,
                    configuracaoGeralCarga.PermitirAlterarInformacoesAgrupamentoCarga,
                    configuracaoGeralCarga.ObrigatorioOperadorResponsavelCancelamentoCarga,
                    configuracaoGeralCarga.CalcularPautaFiscal,
                    configuracaoGeralCarga.NaoPermitirRemoverUltimoPedidoCarga,
                    configuracaoGeralCarga.PermitirRemoverCargasAgrupamentoCarga,
                    configuracaoGeralCarga.HabilitarRelatorioDeEmbarque,
                    configuracaoGeralCarga.PermitirAgrupamentoDeCargasOrdenavel,
                    configuracaoGeralCarga.PermitirGerarRegistroDeDesembarqueNoCIOT,
                    configuracaoGeralCarga.PermitirCancelarDocumentosCargaPeloCancelamentoCarga,
                    configuracaoGeralCarga.ObrigarJustificativaCancelamentoCarga,
                    configuracaoGeralCarga.PadraoVisualizacaoOperadorLogistico,
                    configuracaoGeralCarga.PermiteInformarRemetenteLancamentoNotaManualCarga,
                    configuracaoGeralCarga.PermitirSelecionarMultiplasCargasParaAgruparNoTransbordo,
                    configuracaoGeralCarga.NaoPermitirAlterarDadosCargaQuandoTiverIntegracaoIntegrada,
                    configuracaoGeralCarga.ValidarContratanteOrigemVPIntegracaoPamcard,
                    PermiteHabilitarContingenciaEPECAutomaticamente = (configuracaoGeralCarga.PermiteHabilitarContingenciaEPECAutomaticamente ?? false),
                    configuracaoMotorista.NaoPermitirTransportadoAlterarDataValidadeSeguradora,
                    configuracaoMotorista.BloquearCamposMotoristaLGPD,
                    configuracaoMotorista.ExibirCamposSuspensaoMotorista,
                    configuracaoMotorista.PermitirCadastrarMotoristaEstrangeiro,
                    configuracaoMotorista.MotoristaUsarFotoDoApp,
                    configuracaoMotorista.ExibirConfiguracoesPortalTransportador,
                    configuracaoMotorista.HabilitarUsoCentroResultadoComissaoMotorista,
                    configuracaoFinanceiro.AtivarControleDespesas,
                    configuracaoFinanceiro.PermitirDeixarDocumentoEmTratativa,
                    configuracaoFinanceiro.UtilizarCustoParaRealizarRateiosSobreDocumentoEntrada,
                    configuracaoFinanceiro.NaoObrigarTipoOperacaoFatura,
                    configuracaoFinanceiro.HabilitarOpcaoGerarFaturasApenasCanhotosAprovados,
                    configuracaoTabelaFrete.PermitirInformarLeadTimeTabelaFreteCliente,
                    Culture = System.Globalization.CultureInfo.CurrentCulture.Name,
                    configuracaoCargaEmissaoDocumento.NaoPermitirAcessarDocumentosAntesCargaEmTransporte,
                    configuracaoCargaEmissaoDocumento.ControlarValoresComponentesCTe,
                    configuracaoDocumentoEntrada.FormulaCustoPadrao,
                    configuracaoAgendamentoColeta.RemoverEtapaAgendamentoAgendamentoColeta,
                    configuracaoVeiculo.ObrigatorioInformarReboqueParaVeiculosDoTipoRodadoCavalo,
                    configuraoAcertoViagem.VisualizarPalletsCanhotosNasCargas,
                    configuraoAcertoViagem.HabilitarFormaRecebimentoTituloAoMotorista,
                    configuraoAcertoViagem.HabilitarLancamentoTacografo,
                    configuraoAcertoViagem.SepararValoresAdiantamentoMotoristaPorTipo,
                    configuraoAcertoViagem.HabilitarInformacaoAcertoMotorista,
                    configuraoAcertoViagem.HabilitarControlarOutrasDespesas,
                    configuracaoOcorrencia.ExibirCampoInformativoPagadorAutorizacaoOcorrencia,
                    configuracaoOcorrencia.SalvarDocumentosDoCteAnteriorAoImportarCTeComplementar,
                    ConfiguracaoRelatorio = ObterConfiguracaoRelatorio(unitOfWork),
                    configuracaoControleEntrega.ExibirOpcaoAjustarEntregaOnTime,
                    configuracaoControleEntrega.ExibirDataEntregaNotaControleEntrega,
                    configuracaoCargaDadosTransporte.ExigirProtocoloLiberacaoSemIntegracaoGR,
                    configuracaoMonitoramento.AgruparVeiculosMapaPosicaoFrota,
                    configuracaoMonitoramento.EnviarAlertasMonitoramentoEmail,
                    configuracaoEmbarcador.PermitirContatoWhatsApp,
                    configuracaoEmbarcador.EmitirNFSManualParaTransportadorEFiliais,
                    configuracaoEmbarcador.ObrigatorioCadastrarRastreadorNosVeiculos,
                    configuracaoGeral.PermitirVincularVeiculoMotoristaViaPlanilha,
                    configuracaoGeral.PermitirCriacaoDiretaMalotes,
                    configuracaoFinanceiro.UtilizarValorDesproporcionalRateioDespesaVeiculo,
                    configuracaoChamado.HabilitarArvoreDecisaoEscalationList,
                    configuracaoChamado.ObrigatorioInformarNotaFiscalParaAberturaChamado,
                    configuracaoChamado.CalcularValorDasDevolucoes,
                    OcultarTomadorNoAtendimento = configuracaoChamado?.OcultarTomadorNoAtendimento ?? false,
                    configuracaoChamado.PermitirRegistrarObservacoesSemVisualizacaoTransportadora,
                    configuracaoRoteirizacao.NaoCalcularTempoDeViagemAutomatico,
                    configuracaoVeiculo.NaoPermitirQueTransportadorInativeVeiculo,
                    configuracaoVeiculo.LancamentoServicoManualNaOSMarcadadoPorDefault,
                    configuracaoTransportador.ExigirRetencaoISSQuandoMunicipioPrestacaoForDiferenteTransportador,
                    configuracaoTransportador.PermitirTransportadorRetornarEtapaNFe,
                    configuracaoTransportador.NaoHabilitarDetalhesCarga,
                    configuracaoWebService.PermiteReceberDataCriacaoPedidoERP,
                    configuracaoWebService.AtualizarNumeroPedidoVinculado,
                    configuracaoWebService.HabilitarFluxoPedidoEcommerce,
                    configuracaoJanelaCarregamento.ExibirDetalhesAgendamentoJanelaTransportador,
                    configuracaoPessoa.PermitirCadastroDeTelefoneInternacional,
                    NaoPermitirLiberarSemValePedagio = repTipoOperacaoEmissao.ExisteConfiguracaoNaoAvancarCargaSemValePedagio(),
                    AtivarControleCarregamentoNavio = configuracaoTransportador?.AtivarControleCarregamentoNavio ?? false,
                    configuracaoContratoFreteTerceiro.GerarCargaTerceiroApenasProvedorPedido,
                    MontagemCarga = new
                    {
                        ExigirDefinicaoTipoCarregamentoPedido = configuracaoMontagemCarga?.ExigirDefinicaoTipoCarregamentoPedido ?? false,
                        ApresentaOpcaoRemoverCancelarPedidos = configuracaoMontagemCarga?.ApresentaOpcaoRemoverCancelarPedidos ?? false,
                        ApresentaOpcaoCancelarReserva = configuracaoMontagemCarga?.ApresentaOpcaoCancelarReserva ?? false,
                        FiltroPeriodoVazioAoIniciar = configuracaoMontagemCarga?.FiltroPeriodoVazioAoIniciar ?? false,
                        DataAtualNovoCarregamento = configuracaoMontagemCarga?.DataAtualNovoCarregamento ?? false,
                        OcultarBipagem = configuracaoMontagemCarga?.OcultarBipagem ?? false,
                        PermitirGerarCarregamentoPedidoBloqueado = configuracaoMontagemCarga?.PermitirGerarCarregamentoPedidoBloqueado ?? false,
                        TipoControleSaldoPedido = configuracaoMontagemCarga?.TipoControleSaldoPedido ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoControleSaldoPedido.Peso,
                        RoteirizarAutomaticamenteAposRoteirizadoAoAdicionarRemoverPedido = configuracaoMontagemCarga?.RoteirizarAutomaticamenteAposRoteirizadoAoAdicionarRemoverPedido ?? false,
                        ExibirAlertaRestricaoEntregaClienteCardCarregamento = configuracaoMontagemCarga?.ExibirAlertaRestricaoEntregaClienteCardCarregamento ?? false,
                        AtivarTratativaDuplicidadeEmissaoCargasFeeder = configuracaoMontagemCarga?.AtivarTratativaDuplicidadeEmissaoCargasFeeder ?? false,
                        AtivarMontagemCargaPorNFe = configuracaoMontagemCarga?.AtivarMontagemCargaPorNFe ?? false,
                        configuracaoEmbarcador.ValidarCapacidadeModeloVeicularCargaNaMontagemCarga,
                        PermitirEditarPedidosAtravesTelaMontagemCargaMapa = configuracaoMontagemCarga?.PermitirEditarPedidosAtravesTelaMontagemCargaMapa ?? false,
                        IgnorarRotaFretePedidosMontagemCargaMapa = configuracaoMontagemCarga?.IgnorarRotaFretePedidosMontagemCargaMapa ?? false
                    },
                    Transportador = new
                    {
                        ExisteTransportadorPadraoContratacao = configuracaoTransportador?.ExisteTransportadorPadraoContratacao ?? false,
                        TransportadorPadraoContratacao = new
                        {
                            Codigo = configuracaoTransportador?.TransportadorPadraoContratacao?.Codigo ?? 0,
                            Descricao = configuracaoTransportador?.TransportadorPadraoContratacao?.Descricao ?? string.Empty
                        },
                        AtivarControleCarregamentoNavio = configuracaoTransportador?.AtivarControleCarregamentoNavio ?? false,
                        CadastrarProdutoAutomaticamenteDocumentoEntrada = usuario?.Empresa?.CadastrarProdutoAutomaticamenteDocumentoEntrada ?? false,
                        DeixarPadraoFinalizadoDocumentoEntrada = usuario?.Empresa?.DeixarPadraoFinalizadoDocumentoEntrada ?? false
                    },
                    configuracaoGeral.TransformarJanelaDeDescarregamentoEmMultiplaSelecao,
                    configuracaoGeral.PermitirAgendamentoPedidosSemCarga,
                    configuracaoGeral.NaoPermitirDesabilitarCompraValePedagioVeiculo,
                    configuracaoFinanceiro.PermitirProvisionamentoDeNotasCTesNaTelaProvisao,
                    configuracaoDocumentoEntrada.PermitirSelecionarOSFinalizadaDocumentoEntrada,
                    configuracaoComissaoMotorista.DataBase,
                    Percentual = configuracaoComissaoMotorista.Percentual.ToString("n2"),
                    PercentualDaBaseDeCalculo = configuracaoComissaoMotorista.PercentualDaBaseDeCalculo.ToString("n2"),
                    UtilizaControlePercentualExecucao = configuracaoComissaoMotorista.UtilizaControlePercentualExecucao,
                    ModificarTimelineDeAcordoComTipoServicoDocumento = integracaoIntercab?.ModificarTimelineDeAcordoComTipoServicoDocumento ?? false,
                    LayoutAmarelo = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().UtilizaLayoutPersonalizado,
                    SelecionarSomenteOperacoesDeRedespachoNaTelaDeRedespacho = configuracaoGeralCarga.SelecionarSomenteOperacoesDeRedespachoNaTelaDeRedespacho,
                    AtivarNovosFiltrosConsultaCarga = integracaoIntercab?.AtivarNovosFiltrosConsultaCarga ?? false,
                    PermitirAlterarEmpresaNoCTeManual = configuracaoGeralCarga.PermitirAlterarEmpresaNoCTeManual,
                    Geocodificacao = new
                    {
                        GeoServiceGeocoding = configuracaoIntegracao?.GeoServiceGeocoding ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoServiceGeocoding.Google,
                        NominatimServerUrl = configuracaoIntegracao?.ServidorNominatim ?? "http://20.195.231.113:8080/nominatim/{0}?"
                    },
                    NovoLayoutCabotagem = IsLayoutCabotagem(unitOfWork),
                    Carga = new
                    {
                        PadraoVisualizacaoOperadorLogistico = configuracaoGeralCarga.PadraoVisualizacaoOperadorLogistico,
                        configuracaoGeralCarga.UsarPrioridadeDaCargaParaImpressaoDeObservacaoNoCTE,
                        PermitirRemoverMultiplosPedidosCarga = configuracaoGeralCarga?.PermitirRemoverMultiplosPedidosCarga ?? false,
                        SolicitarJustificativaAoRemoverPedidoCarga = configuracaoGeralCarga?.SolicitarJustificativaAoRemoverPedidoCarga ?? false,
                    },
                    PermitirAvancarCargasEmitidasEmbarcadorPorTipoOperacao = configuracaoGeralCarga.PermitirAvancarCargasEmitidasEmbarcadorPorTipoOperacao,
                    configuracaoOcorrencia.HabilitarImportacaoOcorrenciaViaNOTFIS,
                    configuracaoFinanceiro.RateioProvisaoPorGrupoProduto,
                    configuracaoFinanceiro.NaoGerarPagamentoParaMotoristaTerceiro,
                    configuracaoPortalMultiCliFor.SomenteVisualizacaoBI,
                    configuracaoFinanceiro.TravarFluxoCompraCasoFornecedorDivergenteNaOrdemCompra,
                    configuracaoGeral.UtilizarLocalidadeTomadorNFSManual,
                    AtivarIntegracaoRecebimentoNavioEMP = integracaoEMP?.AtivarIntegracaoRecebimentoNavioEMP,
                    PossuiIntegracaoIntercab = integracaoIntercab?.PossuiIntegracaoIntercab,
                    DesabilitarIconeNotificacao = configuracaoPortalMultiCliFor?.DesabilitarIconeNotificacao ?? false,
                    configuracaoEmbarcador.UtilizarPesoLiquidoNFeParaCTeMDFe,
                    configuracaoEmbarcador.HabilitarFuncionalidadeProjetoNFTP,
                    PermitirBaixarArquivosConembOcorenManualmente = configuracaoDownloadArquivos?.PermitirBaixarArquivosConembOcorenManualmente ?? false,
                    configuracaoGeralCarga.AjustarValorFreteAposAprovacaoPreCTe,
                    integracaoEMP?.AtivarIntegracaoNFTPEMP,
                };

                //APENAS CAMPOS DA CONFIGURAÇÃO GERAL PODE SER ADICIONADO NESSA BUSCA
                //Outras configurações, por exemplo, de integrações, devesse fazer um método no próprio controller da tela que irá utilizar

                ViewBag.CodigoReportMenuBI = configuracaoPortalMultiCliFor.CodigoReportMenuBI;
                ViewBag.SomenteVisualizacaoBI = configuracaoPortalMultiCliFor.SomenteVisualizacaoBI;
                ViewBag.DesabilitarIconeNotificacao = configuracaoPortalMultiCliFor?.DesabilitarIconeNotificacao ?? false;

                ViewBag.ConfiguracaoPadrao = Newtonsoft.Json.JsonConvert.SerializeObject(retorno);
                ViewBag.APIKeyGoogle = configuracaoIntegracao?.APIKeyGoogle ?? "AIzaSyB6e6zUspWGFYrLmABRgI3rsMss_nKW_s4";
                ViewBag.IDUsuario = this.Usuario.Codigo;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private dynamic ObterConfiguracaoPedido(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido)
        {
            return new
            {
                UtilizarEnderecoExpedidorRecebedorPedido = configuracaoPedido?.UtilizarEnderecoExpedidorRecebedorPedido ?? false,
                HabilitarOpcoesDeDuplicacaoDoPedidoParaDevolucaoTotalParcial = configuracaoPedido?.HabilitarOpcoesDeDuplicacaoDoPedidoParaDevolucaoTotalParcial ?? false,
                QuantidadeDiasDataColeta = configuracaoPedido?.QuantidadeDiasDataColeta > 0 ? configuracaoPedido?.QuantidadeDiasDataColeta : 120,
            };
        }

        //private bool FiltraMenuEmpresa(dynamic modulosIn, ref dynamic modulosOut, List<Dominio.Entidades.Embarcador.Transportadores.TransportadorFormulario> transportadorFormularios, bool moduloPaiAcessoLiberado, bool raiz = false)
        //{
        //    bool permiteadicionarModuloPai = false;

        //    // Inicia módulo
        //    if (modulosOut == null)
        //        modulosOut = new List<dynamic>();

        //    foreach (var modulo in modulosIn)
        //    {
        //        // Modulo liberado completo
        //        bool moduloLiberado = this.Empresa.ModulosLiberados.Contains(modulo.CodigoModulo) ?? false;
        //        if (moduloPaiAcessoLiberado)
        //            moduloLiberado = true;

        //        bool possuiPaginaAcesso = false;
        //        List<dynamic> formulariosOut = new List<dynamic>();
        //        foreach (var formulario in modulo.Formularios)
        //        {
        //            bool acessoLiberado = false;

        //            // Busca permissao do formularios
        //            Dominio.Entidades.Embarcador.Transportadores.TransportadorFormulario transportadorFormulario = (from obj in transportadorFormularios where obj.CodigoFormulario == formulario.CodigoFormulario select obj).FirstOrDefault();
        //            if (transportadorFormulario != null || moduloLiberado)
        //            {
        //                acessoLiberado = true;
        //                permiteadicionarModuloPai = true;
        //                possuiPaginaAcesso = true;
        //            }

        //            if (this.Empresa.TransportadorAdministrador || acessoLiberado)
        //                formulariosOut.Add(formulario);
        //        }

        //        dynamic modulosFilhosOut = new List<dynamic>();
        //        bool podeAdicionarModuloPai = FiltraMenuEmpresa(modulo.ModulosFilho, ref modulosFilhosOut, transportadorFormularios, moduloLiberado);

        //        dynamic moduloOut = new
        //        {
        //            CodigoModulo = modulo.CodigoModulo,
        //            Descricao = modulo.Descricao,
        //            Icone = modulo.Icone,
        //            Sequencia = modulo.Sequencia,
        //            Formularios = formulariosOut,
        //            ModulosFilho = modulosFilhosOut,
        //            modulo.TranslationResourcePath
        //        };

        //        if (this.Empresa.TransportadorAdministrador || moduloLiberado || possuiPaginaAcesso || podeAdicionarModuloPai)
        //        {
        //            permiteadicionarModuloPai = true;
        //            modulosOut.Add(moduloOut);
        //        }
        //    }

        //    return permiteadicionarModuloPai;
        //}

        private bool FiltraMenuEmpresa(dynamic modulosIn, ref dynamic modulosOut, Dominio.ObjetosDeValor.PerfilAcesso.PerfilAcesso perfilAcesso, bool moduloPaiAcessoLiberado, bool raiz = false)
        {
            bool permiteadicionarModuloPai = false;

            if (modulosOut == null)
                modulosOut = new List<dynamic>();

            foreach (dynamic modulo in modulosIn)
            {
                bool moduloLiberado = perfilAcesso.Modulos.Contains(modulo.CodigoModulo) ?? false;
                if (moduloPaiAcessoLiberado)
                    moduloLiberado = true;

                bool possuiPaginaAcesso = false;
                List<dynamic> formulariosOut = new List<dynamic>();
                foreach (dynamic formulario in modulo.Formularios)
                {
                    bool acessoLiberado = false;

                    bool transportadorPossuiFormulario = perfilAcesso.CodigosFormularios.Contains(formulario.CodigoFormulario);
                    if (transportadorPossuiFormulario || moduloLiberado)
                    {
                        acessoLiberado = true;
                        permiteadicionarModuloPai = true;
                        possuiPaginaAcesso = true;
                    }

                    if (perfilAcesso.TransportadorAdministrador || acessoLiberado)
                        formulariosOut.Add(formulario);
                }

                dynamic modulosFilhosOut = new List<dynamic>();
                bool podeAdicionarModuloPai = FiltraMenuEmpresa(modulo.ModulosFilho, ref modulosFilhosOut, perfilAcesso, moduloLiberado);

                dynamic moduloOut = new
                {
                    CodigoModulo = modulo.CodigoModulo,
                    Descricao = modulo.Descricao,
                    Icone = modulo.Icone,
                    Sequencia = modulo.Sequencia,
                    Formularios = formulariosOut,
                    ModulosFilho = modulosFilhosOut,
                    modulo.TranslationResourcePath
                };

                if (perfilAcesso.TransportadorAdministrador || moduloLiberado || possuiPaginaAcesso || podeAdicionarModuloPai)
                {
                    permiteadicionarModuloPai = true;
                    modulosOut.Add(moduloOut);
                }
            }

            return permiteadicionarModuloPai;
        }

        private bool RecursiveMenu(ref StringBuilder strMenu, dynamic modulos, List<Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormulario> funcionarioFormularios, bool moduloPaiAcessoLiberado, bool raiz = false)
        {
            Localization.Service.Common svcLocalization = new Localization.Service.Common();
            bool traduzir = System.Globalization.CultureInfo.CurrentCulture.Name != "pt-BR";

            bool permiteadicionarModuloPai = false;
            foreach (dynamic modulo in modulos)
            {
                bool moduloLiberado = Usuario?.ModulosLiberados.Contains(modulo.CodigoModulo) ?? false;
                if (moduloPaiAcessoLiberado)
                    moduloLiberado = true;

                StringBuilder stBuilderInteracao = new StringBuilder();

                bool possuiPaginaAcesso = false;
                foreach (dynamic formulario in modulo.Formularios)
                {
                    bool acessoLiberado = false;
                    Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormulario funcionarioFormulario = (from obj in funcionarioFormularios where obj.CodigoFormulario == formulario.CodigoFormulario select obj).FirstOrDefault();
                    if (funcionarioFormulario != null || moduloLiberado)
                    {
                        acessoLiberado = true;
                        permiteadicionarModuloPai = true;
                        possuiPaginaAcesso = true;
                    }

                    if ((Usuario?.UsuarioAdministrador ?? false) || acessoLiberado || (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor))
                    {
                        string descricaoFormulario = svcLocalization.GetTranslationByResourcePath(formulario.TranslationResourcePath, formulario.Descricao, traduzir);
                        stBuilderInteracao.Append("<li><a href=\"#").Append(formulario.CaminhoPagina).Append("\" title=\"").Append(descricaoFormulario).Append("\"><span class=\"nav-link-text\">").Append(descricaoFormulario).Append("</span></a></li>");
                    }
                }
                bool podeAdicionarModuloPai = RecursiveMenu(ref stBuilderInteracao, modulo.ModulosFilho, funcionarioFormularios, moduloLiberado);

                if ((Usuario?.UsuarioAdministrador ?? false) || moduloLiberado || possuiPaginaAcesso || podeAdicionarModuloPai || (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor))
                {
                    string descricaoModulo = svcLocalization.GetTranslationByResourcePath(modulo.TranslationResourcePath, modulo.Descricao);
                    strMenu.Append("<li><a href=\"#\" class=\"waves-effect waves-themed\" aria-expanded=\"false\" title=\"").Append(descricaoModulo).Append("\"><i class=\"fal ").Append(modulo.Icone != null ? modulo.Icone : "").Append("\"></i><span class=\"nav-link-text\">").Append(descricaoModulo).Append("</span></a><ul>");
                    strMenu.Append(stBuilderInteracao.ToString());
                    strMenu.Append("</ul>");
                    permiteadicionarModuloPai = true;
                }
            }
            return permiteadicionarModuloPai;
        }

        //private bool RecursiveMenuTransportador(ref StringBuilder strMenu, dynamic modulos, List<Dominio.Entidades.Embarcador.Transportadores.TransportadorFormulario> formulariosTransportador, bool moduloPaiAcessoLiberado, bool raiz = false)
        //{
        //    Localization.Service.Common svcLocalization = new Localization.Service.Common();
        //    bool traduzir = System.Globalization.CultureInfo.CurrentCulture.Name != "pt-BR";

        //    bool permiteadicionarModuloPai = false;
        //    foreach (var modulo in modulos)
        //    {
        //        bool moduloLiberado = Usuario?.ModulosLiberados.Contains(modulo.CodigoModulo) ?? false;
        //        if (moduloPaiAcessoLiberado)
        //            moduloLiberado = true;

        //        StringBuilder stBuilderInteracao = new StringBuilder();

        //        bool possuiPaginaAcesso = false;
        //        foreach (var formulario in modulo.Formularios)
        //        {
        //            bool acessoLiberado = false;
        //            Dominio.Entidades.Embarcador.Transportadores.TransportadorFormulario formularioTransportador = (from obj in formulariosTransportador where obj.CodigoFormulario == formulario.CodigoFormulario select obj).FirstOrDefault();
        //            if (formulario != null || moduloLiberado)
        //            {
        //                acessoLiberado = true;
        //                permiteadicionarModuloPai = true;
        //                possuiPaginaAcesso = true;
        //            }

        //            if ((Usuario?.UsuarioAdministrador ?? false) || acessoLiberado || (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor))
        //            {
        //                string descricaoFormulario = svcLocalization.GetTranslationByResourcePath(formulario.TranslationResourcePath, formulario.Descricao, traduzir);
        //                stBuilderInteracao.Append("<li><a href=\"#").Append(formulario.CaminhoPagina).Append("\" title=\"").Append(descricaoFormulario).Append("\"><span class=\"nav-link-text\">").Append(descricaoFormulario).Append("</span></a></li>");
        //            }
        //        }
        //        bool podeAdicionarModuloPai = RecursiveMenuTransportador(ref stBuilderInteracao, modulo.ModulosFilho, formulariosTransportador, moduloLiberado);

        //        if ((Usuario?.UsuarioAdministrador ?? false) || moduloLiberado || possuiPaginaAcesso || podeAdicionarModuloPai || (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor))
        //        {
        //            string descricaoModulo = svcLocalization.GetTranslationByResourcePath(modulo.TranslationResourcePath, modulo.Descricao);
        //            strMenu.Append("<li><a href=\"#\" class=\"waves-effect waves-themed\" aria-expanded=\"false\" title=\"").Append(descricaoModulo).Append("\"><i class=\"fal ").Append(modulo.Icone != null ? modulo.Icone : "").Append("\"></i><span class=\"nav-link-text\">").Append(descricaoModulo).Append("</span></a><ul>");
        //            strMenu.Append(stBuilderInteracao.ToString());
        //            strMenu.Append("</ul>");
        //            permiteadicionarModuloPai = true;
        //        }
        //    }
        //    return permiteadicionarModuloPai;
        //}

        private bool RecursiveMenuTransportador(ref StringBuilder strMenu, dynamic modulos, Dominio.ObjetosDeValor.PerfilAcesso.PerfilAcesso perfilAcesso, bool moduloPaiAcessoLiberado, bool raiz = false)
        {
            Localization.Service.Common svcLocalization = new Localization.Service.Common();
            bool traduzir = System.Globalization.CultureInfo.CurrentCulture.Name != "pt-BR";

            bool permiteadicionarModuloPai = false;
            foreach (dynamic modulo in modulos)
            {
                bool moduloLiberado = perfilAcesso.Modulos.Contains(modulo.CodigoModulo) ?? false;

                if (moduloPaiAcessoLiberado)
                    moduloLiberado = true;

                StringBuilder stBuilderInteracao = new StringBuilder();

                bool possuiPaginaAcesso = false;
                foreach (dynamic formulario in modulo.Formularios)
                {
                    bool acessoLiberado = false;

                    bool transportadorPossuiFormulario = perfilAcesso.CodigosFormularios.Contains(formulario.CodigoFormulario);
                    if (transportadorPossuiFormulario || moduloLiberado)
                    {
                        acessoLiberado = true;
                        permiteadicionarModuloPai = true;
                        possuiPaginaAcesso = true;
                    }

                    if ((Usuario?.UsuarioAdministrador ?? false) || acessoLiberado || (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor))
                    {
                        string descricaoFormulario = svcLocalization.GetTranslationByResourcePath(formulario.TranslationResourcePath, formulario.Descricao, traduzir);
                        stBuilderInteracao.Append("<li><a href=\"#").Append(formulario.CaminhoPagina).Append("\" title=\"").Append(descricaoFormulario).Append("\"><span class=\"nav-link-text\">").Append(descricaoFormulario).Append("</span></a></li>");
                    }
                }
                bool podeAdicionarModuloPai = RecursiveMenuTransportador(ref stBuilderInteracao, modulo.ModulosFilho, perfilAcesso, moduloLiberado);

                if ((Usuario?.UsuarioAdministrador ?? false) || moduloLiberado || possuiPaginaAcesso || podeAdicionarModuloPai || (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor))
                {
                    string descricaoModulo = svcLocalization.GetTranslationByResourcePath(modulo.TranslationResourcePath, modulo.Descricao);
                    strMenu.Append("<li><a href=\"#\" class=\"waves-effect waves-themed\" aria-expanded=\"false\" title=\"").Append(descricaoModulo).Append("\"><i class=\"fal ").Append(modulo.Icone != null ? modulo.Icone : "").Append("\"></i><span class=\"nav-link-text\">").Append(descricaoModulo).Append("</span></a><ul>");
                    strMenu.Append(stBuilderInteracao.ToString());
                    strMenu.Append("</ul>");
                    permiteadicionarModuloPai = true;
                }
            }
            return permiteadicionarModuloPai;
        }

        private void ObterMenu(AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso, Repositorio.UnitOfWork unitOfWork)
        {
            /* ObterMenu
             * O fluxo da montagem do menu se ta
             * - Módulos sistema
             * - Módulos do  transportador (No caso do MultiCTe)
             * - Formulários dos módulos
             * - Verifica autorização do usuário
             */

            bool alteraCorPrincipalPortal = false;

            try
            {
                // Repositórios e Controllers
                Repositorio.Embarcador.Transportadores.TransportadorFormulario repTransportadorFormulario = new Repositorio.Embarcador.Transportadores.TransportadorFormulario(unitOfWork);
                Repositorio.Embarcador.Transportadores.PerfilTransportadorFormulario repPerfilTransportadorFormulario = new Repositorio.Embarcador.Transportadores.PerfilTransportadorFormulario(unitOfWork);
                Repositorio.Embarcador.Usuarios.FuncionarioFormulario repFuncionarFormulario = new Repositorio.Embarcador.Usuarios.FuncionarioFormulario(unitOfWork);
                Modulos controllerModulos = new Modulos(_conexao);

                // Busca todos módulos do sistema (TipoServicoMultisoftware)
                dynamic modulos = controllerModulos.RetornarListaModulosFormularios();

                //Constrói um objeto com os dados do perfil de acesso do usuário
                Dominio.ObjetosDeValor.PerfilAcesso.PerfilAcesso perfilAcesso = ObterObjetoPerfilAcesso(unitOfWork);

                // Quando o sistema for MultiCTe busca apenas os módulos pertencentes ao transportador
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                {
                    // Filtra os módulos do sistema pelos que a transportadora tem permissao
                    dynamic modulosFiltrados = null;
                    FiltraMenuEmpresa(modulos, ref modulosFiltrados, perfilAcesso, false, true);
                    modulos = modulosFiltrados; // Vinculado módulos filtrados
                }

                // Cria o cache dos módulos permitidos
                controllerModulos.CriarCacheFormularios(modulos);

                // String do HTML do menu e com o menu Home
                StringBuilder strMenu = new StringBuilder();
                strMenu.Append(@"<li><a href='#Home' title='Home' data-filter-tags='Home'><i class='fal fa-home'></i><span class='nav-link-text'>Home</span></a></li>");

                // Busca permissões por usuário e criar o HTML com os módulos permitidos do usuário
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                {
                    RecursiveMenuTransportador(ref strMenu, modulos, perfilAcesso, false, true);
                }
                else
                {
                    List<Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormulario> funcionarioFormularios = repFuncionarFormulario.buscarPorUsuario(Usuario?.Codigo ?? 0);
                    RecursiveMenu(ref strMenu, modulos, funcionarioFormularios, false, true);
                }

                ViewBag.Menu = strMenu.ToString();

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                {
                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(Empresa.Codigo);

                    ViewBag.NomeEmpresa = System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase(empresa.NomeFantasia.ToLower()) + " -";
                    ViewBag.ClasseEmpresa = "txt-color-grayDark";
                    ViewBag.Ambiente = empresa.DescricaoTipoAmbiente;
                    ViewBag.ClasseAmbiente = empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao ? "txt-color-blue" : "txt-color-red";
                    ViewBag.LocalidadeEmpresa = System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase(empresa?.Localidade?.DescricaoCidadeEstado ?? string.Empty);
                }
                else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros ||
                         TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                {
                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(Empresa.Codigo);

                    ViewBag.NomeEmpresa = System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase(Usuario.Nome) + " -";
                    ViewBag.ClasseEmpresa = "txt-color-grayDark";
                    ViewBag.Ambiente = empresa.DescricaoTipoAmbiente;
                    ViewBag.ClasseAmbiente = empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao ? "txt-color-blue" : "txt-color-red";
                    ViewBag.LocalidadeEmpresa = System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase(string.Empty);
                }
                else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(Empresa.Codigo);

                    ViewBag.NomeEmpresa = System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase(empresa.NomeFantasia.ToUpper()) + " -";
                    ViewBag.ClasseEmpresa = "txt-color-white";
                    ViewBag.Ambiente = empresa.DescricaoTipoAmbiente;
                    ViewBag.ClasseAmbiente = empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao ? "txt-color-blue" : "txt-color-red";
                    ViewBag.LocalidadeEmpresa = System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase(string.Empty);
                }
                else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                {
                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(Empresa.Codigo);

                    ViewBag.NomeEmpresa = System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase(empresa.NomeFantasia.ToUpper()) + " - Administrador";
                    ViewBag.ClasseEmpresa = "txt-color-white";
                    ViewBag.Ambiente = "";// empresa.DescricaoTipoAmbiente;
                    ViewBag.ClasseAmbiente = empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao ? "txt-color-red" : "txt-color-red";
                    ViewBag.LocalidadeEmpresa = System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase(string.Empty);
                }
                else
                {
                    ViewBag.NomeEmpresa = "";
                    ViewBag.ClasseEmpresa = "hidden";
                    ViewBag.Ambiente = "";
                    ViewBag.ClasseEmpresa = "hidden";
                    ViewBag.LocalidadeEmpresa = "";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                ViewBag.Menu = @"<li><a href='#Home' title='Home'><i class='fal fa-error'></i><span class='nav-link-text'>Falha ao carregar o menu</span></a></li>";
            }

            ViewBag.LogoCliente = "";
            ViewBag.NomeUsuario = System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase(Usuario?.DescricaoUsuarioLogado ?? string.Empty);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ||
                TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                ViewBag.Titulo = "MultiTMS";
                ViewBag.IconeLogo = "img/Logos/user.png";
                ViewBag.LogoLight = "img/Logos/multitms_semendosso_positiva.png";
                ViewBag.LogoDark = "img/Logos/multitms_semendosso_negativa.png";
                ViewBag.Favicon = string.IsNullOrWhiteSpace(Favicon) ? "img/Logos/favicon-site-nstech.svg" : Favicon;
                ViewBag.GTAG = "G-W9PLBNR31T";
                ViewBag.LogoTerceiroLight = string.IsNullOrWhiteSpace(clienteURLAcesso.LogoClienteTerceiroLight) ? "" : clienteURLAcesso.LogoClienteTerceiroLight;
                ViewBag.LogoTerceiroDark = string.IsNullOrWhiteSpace(clienteURLAcesso.LogoClienteTerceiroDark) ? "" : clienteURLAcesso.LogoClienteTerceiroDark;
            }
            else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                if (clienteURLAcesso.Cliente.Cabotagem)
                {
                    ViewBag.Titulo = "MultiTMS";
                    ViewBag.IconeLogo = "img/Logos/logo-tms-icon.svg";
                    ViewBag.LogoLight = "img/Logos/logo-tms-text-light.svg";
                    ViewBag.LogoDark = "img/Logos/logo-tms-text-dark.svg";
                    ViewBag.Favicon = string.IsNullOrWhiteSpace(Favicon) ? "img/favicon/logo-tms-icon.ico?v=5" : Favicon;
                }
                else
                {
                    ViewBag.Titulo = "KMM";
                    ViewBag.IconeLogo = "img/Logos/kmm_endossada_positiva.svg";
                    ViewBag.LogoLight = "img/Logos/kmm_endossada_positiva.svg";
                    ViewBag.LogoDark = "img/Logos/kmm_endossada_negativa.svg";
                    ViewBag.Favicon = string.IsNullOrWhiteSpace(Favicon) ? "img/favicon/kmm-icon.ico?v=5" : Favicon;
                }
                ViewBag.GTAG = "G-W9PLBNR31T";
            }
            else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros ||
                     TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
            {
                ViewBag.Titulo = "MultiCTe";
                ViewBag.IconeLogo = "img/Logos/logo-cte-icon.svg";
                ViewBag.LogoLight = "img/Logos/logo-cte-text-light.svg";
                ViewBag.LogoDark = "img/Logos/logo-cte-text-dark.svg";
                ViewBag.Favicon = string.IsNullOrWhiteSpace(Favicon) ? "img/favicon/logo-cte-icon.ico?v=3" : Favicon;
                ViewBag.GTAG = "G-W9PLBNR31T";
                ViewBag.LogoTerceiroLight = string.IsNullOrWhiteSpace(clienteURLAcesso.LogoClienteTerceiroLight) ? "" : clienteURLAcesso.LogoClienteTerceiroLight;
                ViewBag.LogoTerceiroDark = string.IsNullOrWhiteSpace(clienteURLAcesso.LogoClienteTerceiroDark) ? "" : clienteURLAcesso.LogoClienteTerceiroDark;
            }
            else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
            {
                ViewBag.Titulo = "MultiCTe";
                ViewBag.IconeLogo = "img/Logos/logo-cte-icon.svg";
                ViewBag.LogoLight = "img/Logos/logo-cte-text-light.svg";
                ViewBag.LogoDark = "img/Logos/logo-cte-text-dark.svg";
                ViewBag.Favicon = string.IsNullOrWhiteSpace(Favicon) ? "img/favicon/logo-cte-icon.ico?v=3" : Favicon;
                ViewBag.GTAG = "G-W9PLBNR31T";

                ViewBag.ClasseCorBotoes = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().LayoutPersonalizadoFornecedor;
                ViewBag.HeaderOnTop = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().UtilizaLayoutPersonalizado ? "nav-function-top" : string.Empty;
                ViewBag.LogoPersonalizada = !string.IsNullOrWhiteSpace(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().LogoPersonalizadaFornecedor) ? Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().LogoPersonalizadaFornecedor : "cologist-logo.png";
                ViewBag.ExibirConteudoColog = !(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().OcultarConteudoColog.Value);

                if (ViewBag.LogoPersonalizada == "logvett-logo.png")
                {
                    alteraCorPrincipalPortal = true;
                }
            }
            else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa empresaUsuario = repEmpresa.BuscarPorCodigo(this.Usuario.Empresa.Codigo);

                ViewBag.Titulo = "MultiNFe";
                ViewBag.IconeLogo = "img/Logos/logo-nfe-icon.svg";
                ViewBag.LogoLight = "img/Logos/logo-nfe-text-light.svg";
                ViewBag.LogoDark = "img/Logos/logo-nfe-text-dark.svg";
                ViewBag.LogoCliente = empresaUsuario?.CaminhoLogoSistema;
                ViewBag.Favicon = string.IsNullOrWhiteSpace(Favicon) ? "img/favicon/logo-nfe-icon.ico?v=5" : Favicon;
            }
            else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa empresaUsuario = repEmpresa.BuscarPorCodigo(this.Usuario.Empresa.Codigo);

                ViewBag.Titulo = "MultiNFe - Admin";
                ViewBag.IconeLogo = "img/Logos/logo-nfe-icon.svg";
                ViewBag.LogoLight = "img/Logos/logo-nfe-text-light.svg";
                ViewBag.LogoDark = "img/Logos/logo-nfe-text-dark.svg";
                ViewBag.LogoCliente = empresaUsuario?.CaminhoLogoSistema;
                ViewBag.Favicon = string.IsNullOrWhiteSpace(Favicon) ? "img/favicon/logo-nfe-icon.ico?v=5" : Favicon;
            }

            ViewBag.AlteraCorPrincipalPortal = alteraCorPrincipalPortal;
        }

        private object ObterConfiguracaoRelatorio(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoRelatorio repConfiguracaoRelatorio = new Repositorio.Embarcador.Configuracoes.ConfiguracaoRelatorio(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRelatorio configuracaoRelatorio = repConfiguracaoRelatorio.BuscarConfiguracaoPadrao();

            return new
            {
                QuantidadeRelatoriosParalelo = configuracaoRelatorio?.QuantidadeRelatoriosParalelo ?? 0,
                ServicoGeracaoRelatorioHabilitado = configuracaoRelatorio?.ServicoGeracaoRelatorioHabilitado ?? false,
                UtilizaAutomacaoEnvioRelatorio = configuracaoRelatorio?.UtilizaAutomacaoEnvioRelatorio ?? false
            };
        }

        private void ObterFormulariosFavoritos(Repositorio.UnitOfWork unitOfWork)
        {
            ViewBag.FormulariosFavoritos = "[]";
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);
            List<AdminMultisoftware.Dominio.Entidades.Modulos.Formulario> formularios = new List<AdminMultisoftware.Dominio.Entidades.Modulos.Formulario>();

            try
            {
                Repositorio.Embarcador.Usuarios.UsuarioFormularioFavorito repUsuarioFormularioFavorito = new Repositorio.Embarcador.Usuarios.UsuarioFormularioFavorito(unitOfWork);

                List<int> usuarioFormulariosFavoritos = repUsuarioFormularioFavorito.BuscarCodigosFormulariosPorUsuario(Usuario.Codigo);

                if (usuarioFormulariosFavoritos.Count > 0)
                {
                    AdminMultisoftware.Repositorio.Modulos.Formulario repFormulario = new AdminMultisoftware.Repositorio.Modulos.Formulario(unitOfWorkAdmin);

                    formularios = repFormulario.BuscarPorCodigosFormularios(usuarioFormulariosFavoritos);

                    ViewBag.FormulariosFavoritos = Newtonsoft.Json.JsonConvert.SerializeObject(formularios.Select(o => new Dominio.ObjetosDeValor.Embarcador.Formulario.Formulario(o)));
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        private bool IsLayoutCabotagem(Repositorio.UnitOfWork unitOfWork)
        {
            return (ConfiguracaoEmbarcador?.UtilizaEmissaoMultimodal ?? false) && (Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente()?.NovoLayoutCabotagem.Value ?? false);
        }

        private Dominio.ObjetosDeValor.PerfilAcesso.PerfilAcesso ObterObjetoPerfilAcesso(Repositorio.UnitOfWork unitOfWork)
        {
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                return null;

            bool possuiPerfilAcessoTransportador = (this.Empresa?.PerfilAcessoTransportador ?? null) != null;

            Repositorio.Embarcador.Transportadores.TransportadorFormulario repTransportadorFormulario = new Repositorio.Embarcador.Transportadores.TransportadorFormulario(unitOfWork);
            Repositorio.Embarcador.Transportadores.PerfilTransportadorFormulario repPerfilTransportadorFormulario = new Repositorio.Embarcador.Transportadores.PerfilTransportadorFormulario(unitOfWork);

            List<Dominio.Entidades.Embarcador.Transportadores.TransportadorFormulario> transportadorFormulario = possuiPerfilAcessoTransportador ? repTransportadorFormulario.BuscarPorEmpresa(this.Empresa.Codigo) : null;
            List<Dominio.Entidades.Embarcador.Transportadores.PerfilTransportadorFormulario> perfilTransportadorFormulario = possuiPerfilAcessoTransportador ? repPerfilTransportadorFormulario.BuscarPorPerfil(this.Empresa.PerfilAcessoTransportador?.Codigo ?? 0) : null;

            Dominio.ObjetosDeValor.PerfilAcesso.PerfilAcesso perfilAcesso = new Dominio.ObjetosDeValor.PerfilAcesso.PerfilAcesso();

            perfilAcesso.CodigoEmpresa = this.Empresa?.Codigo ?? 0;
            perfilAcesso.CodigoPerfilAcessoTransportador = this.Empresa?.PerfilAcessoTransportador?.Codigo ?? 0;
            perfilAcesso.TransportadorAdministrador = possuiPerfilAcessoTransportador ? this.Empresa.PerfilAcessoTransportador.PerfilAdministrador : this.Usuario.UsuarioAdministrador;
            perfilAcesso.CodigosFormularios = new List<int>();
            perfilAcesso.Modulos = new List<int>();

            if (possuiPerfilAcessoTransportador)
            {
                perfilAcesso.CodigosFormularios.AddRange(perfilTransportadorFormulario.Select(o => o.CodigoFormulario).ToList());
                perfilAcesso.Modulos.AddRange(this.Empresa.PerfilAcessoTransportador.ModulosLiberados.ToList());
            }
            else if (transportadorFormulario?.Count() > 0)
            {
                perfilAcesso.CodigosFormularios.AddRange(transportadorFormulario.Select(o => o.CodigoFormulario).ToList());
                perfilAcesso.Modulos.AddRange(this.Empresa.ModulosLiberados.ToList());
            }

            return perfilAcesso;
        }

        #endregion
    }
}
