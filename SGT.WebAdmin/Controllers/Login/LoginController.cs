using Infrastructure.Services.Cache;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Servicos.Embarcador.Configuracoes;
using SGT.BackgroundWorkers;
using SGT.WebAdmin.Models.Threads;
using System.Net;

namespace SGT.WebAdmin.Controllers.Login
{

    public class LoginController : SignController
    {
        #region Construtores

        public LoginController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Públicos


        [AllowAnonymous]
        public async Task<IActionResult> Index(string returnUrl, int multi = 0, string errorMessage = "")
        {
            _conexao.MigrateDatabase(_conexao.StringConexao);

            string chaveCacheEmpresa = "EmpresaMultisoftware" + _conexao.ObterHost;
            string stringConexaoAdmin = _conexao.AdminStringConexao;

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(stringConexaoAdmin);
            AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWorkAdmin);
            AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorURL(_conexao.ObterHost);

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Embarcador.Configuracoes.Layout.MontarLayout(ViewBag, clienteURLAcesso, unitOfWork);
            Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork);

            if (clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                ViewBag.ClasseCorBotoes = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().LayoutPersonalizadoFornecedor;
            else if (clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ||
                    clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ||
                    clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                ViewBag.GTAG = "G-W9PLBNR31T";

            unitOfWorkAdmin.Dispose();

            int codigoEmpresa = CacheProvider.Instance.Get<int>(chaveCacheEmpresa);
            string stringConexao = _conexao.StringConexao;
            string caminhoBaseViews = "~/Views/Login";

            bool portalCliente = clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor && Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().NovoLayoutPortalFornecedor.Value;

            string caminhosViewLogin = portalCliente ? $"{caminhoBaseViews}/IndexCliente.cshtml" : $"{caminhoBaseViews}/Index.cshtml";
            string host = _conexao.ObterHost;

            ConfigurationInstance.GetInstance(unitOfWork);
            SGT.BackgroundWorkers.MSMQ.GetInstance().QueueItem(unitOfWork, clienteURLAcesso.Cliente.Codigo, stringConexao, stringConexaoAdmin, clienteURLAcesso.TipoServicoMultisoftware);

            bool utilizarLockNasThreads = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().Autoscaling.HasValue ? Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().Autoscaling.Value : false;


            if (clienteURLAcesso.TipoExecucao == AdminMultisoftware.Dominio.Enumeradores.TipoExecucao.Thread && ExecutarThread())
            {
                int port = _conexao.ObterPortaHost.HasValue ? _conexao.ObterPortaHost.Value : 0;

                if (clienteURLAcesso.PossuiFila && port != 8443 &&
                   (clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ||
                    clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS))
                {
                    string stringEmissao = stringConexao + "Application Name=SGT.WebAdmin.Geral";
                    stringConexao += "Application Name=SGT.WebAdmin.Threads";

                    await LongRunningProcessFactory.Instance.AddProcessAsync(ImportacaoTicketLog.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3600000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ConsultaAbastecimentoAngellira.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 14400000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(CargaIntegracaoHUBOfertas.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(MensageriaHUBOfertas.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ConsultaEmissoesPendentes.Instance, codigoEmpresa, clienteURLAcesso, stringEmissao + ".ConsultaEmissoesPendentes", stringConexaoAdmin, utilizarLockNasThreads, 5000);

                    await LongRunningProcessFactory.Instance.AddProcessAsync(EmissaoDocumentos.Instance, codigoEmpresa, clienteURLAcesso, stringEmissao + ".EmissaoDocumentos", stringConexaoAdmin, utilizarLockNasThreads, 3000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EmissaoDocumentosNFe.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000, true);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EmissaoDocumentosEtapaFrete.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000, true);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EmissaoDocumentosEtapaFreteLeve.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EmissaoDocumentosEtapaFreteMedia.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EmissaoDocumentosEtapaFretePesada.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EmissaoDocumentosSubContratacaoFilialEmissora.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000, true);

                    await LongRunningProcessFactory.Instance.AddProcessAsync(EmissaoDocumentosIntegracao.Instance, codigoEmpresa, clienteURLAcesso, stringEmissao + ".EmissaoDocumentosIntegracao", stringConexaoAdmin, utilizarLockNasThreads, 6000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EmissaoDocumentosNFeIntegracao.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000, true);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EmissaoDocumentosEtapaFreteIntegracao.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000, true);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EmissaoDocumentosEtapaFreteIntegracaoLeve.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EmissaoDocumentosEtapaFreteIntegracaoMedia.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EmissaoDocumentosEtapaFreteIntegracaoPesada.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EmissaoDocumentosSubContratacaoFilialEmissoraIntegracao.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000, true);

                    await LongRunningProcessFactory.Instance.AddProcessAsync(EmissaoDocumentosIntegracaoReprocessamento.Instance, codigoEmpresa, clienteURLAcesso, stringEmissao + ".EmissaoDocumentosIntegracaoReprocessamento", stringConexaoAdmin, utilizarLockNasThreads, 6000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EmissaoDocumentosIntegracaoMDFe.Instance, codigoEmpresa, clienteURLAcesso, stringEmissao + ".EmissaoDocumentosIntegracaoMDFe", stringConexaoAdmin, utilizarLockNasThreads, 20000);

                    await LongRunningProcessFactory.Instance.AddProcessAsync(Fatura.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracoesEntreSistemas.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3600000);

                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoCarga.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 6000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoCargaEvento.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 6000);

                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoDocumentosCarga.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoCargaFrete.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000, true);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoMotorista.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoMinerva.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoMinervaLote.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000, false);

                    await LongRunningProcessFactory.Instance.AddProcessAsync(ControleCargaEmEmissaoLote.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ControleCargaEmEmissao.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 5000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(AgrupamentoCargas.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 30000);

                    await LongRunningProcessFactory.Instance.AddProcessAsync(MDFeManual.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 6000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ExecucaoDiaria.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 86400000);

                    await LongRunningProcessFactory.Instance.AddProcessAsync(ConsultarCaixaEntradaEmail.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ConsultaFTPDocsys.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 1800000);

                    await LongRunningProcessFactory.Instance.AddProcessAsync(SGT.BackgroundWorkers.RH.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 30000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(BaixaTituloReceber.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(SGT.BackgroundWorkers.PagamentoAgregado.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(Financeiro.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 900000);

                    await LongRunningProcessFactory.Instance.AddProcessAsync(FaturaFechamento.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EnvioDocumentacaoLote.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 180000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EnvioDocumentacaoAFRMM.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 180000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(FaturamentoLote.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 180000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ColetaNotaFiscal.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 180000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(GeracaoPDFDANFE.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 900000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoPedido.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 180000);

                    await LongRunningProcessFactory.Instance.AddProcessAsync(ControleCalculosGerais.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ControleCargaCalculoFrete.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 5000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ControleCargaCalculoFreteIntegracao.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 5000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ControleCargaCalculoFreteReprocessamento.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 5000, false);

                    await LongRunningProcessFactory.Instance.AddProcessAsync(ControleCargaEmFinalizacaoEmissao.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 7000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ControleCargaEmFinalizacaoEmissaoIntegracao.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 5000);

                    await LongRunningProcessFactory.Instance.AddProcessAsync(ControleCargaRoteirizacao.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ControleCargaRoteirizacaoLote.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000);

                    await LongRunningProcessFactory.Instance.AddProcessAsync(ControleAverbacao.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);

                    await LongRunningProcessFactory.Instance.AddProcessAsync(NotificacaoChamado.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoCIOT.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(Alerta.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 86400000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(AlertaCargasParadas.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 600000);

                    await LongRunningProcessFactory.Instance.AddProcessAsync(VincularRotasSemParar.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);

                    await LongRunningProcessFactory.Instance.AddProcessAsync(AjusteFrete.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 20000);

                    await LongRunningProcessFactory.Instance.AddProcessAsync(GeracaoPlanoContabil.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);

                    await LongRunningProcessFactory.Instance.AddProcessAsync(FilaCarregamento.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(JanelaCarregamento.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ProgramacaoCarga.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 30000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(BiddingConvite.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 5000);

                    await LongRunningProcessFactory.Instance.AddProcessAsync(SGT.BackgroundWorkers.Patrimonio.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 86400000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ImportacaoBoletoRetorno.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoEmbarcador.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 900000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoCTeCarga.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 0);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ControleStatusFuncionario.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 43200000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(GeracaoTituloNFSe.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoPamcard.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads);

                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoCadastrosMulti.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 900000);

                    await LongRunningProcessFactory.Instance.AddProcessAsync(ControleRoteirizacao.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(DownloadLoteCTe.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);

                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoMontagemCarga.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 6000);

                    await LongRunningProcessFactory.Instance.AddProcessAsync(EncerramentoMDFeAquaviario.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 900000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ImportacaoPedido.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ImportacaoTakePay.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(NotaDeDebito.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ImportacaoTabelaFrete.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoProcessamentoEDIFTP.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EnvioEmailBoleto.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 300000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EnvioEmailAvisoVencimentoCobranca.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 43200000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(PracaPedagioTarifaIntegracao.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 6000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(MonitorarMonitoramento.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(PedidoOcorrenciaColetaEntregaIntegracao.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(CargaEntregaEventoIntegracao.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ControleTendenciaEntrega.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 180000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoMercadoLivre.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 60000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoMarfrig.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 86400000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoVtex.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 600000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoEmillenium.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 600000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoEmilleniumBuscaMassiva.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 600000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(DiariaAutomatica.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 60000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(AlertaEventosCarga.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 600000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(GeracaoConciliacaoTransportador.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 600000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(Canhoto.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 600000, ativo: false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(CargaFechamento.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 600000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(SGT.BackgroundWorkers.Ocorrencia.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000, true);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EventoEntrega.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 300000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoCTASmart.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 600000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegrarCargaUnilever.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 100000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoCartaCorrecao.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 200000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoCTEManual.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 200000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoMDFe.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 200000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoMercante.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 200000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ControleDasIntegracoes.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 14400000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ValePedagio.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000, true);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EnvioEmailDocumentacao.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 60000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoSIC.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 60000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ConsultarExtratosValePedagio.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 85800000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ConsultaDocumentosPedido.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 21600000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EncerramentoCarga.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 30000, true);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoDiageo.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 900000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoCorreios.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 60000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ProcessarDocumentoTransporte.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 1000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EnvioEmailGlobalizadoFornecedor.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 900000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoComprovei.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 20000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoCTeCanhoto.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 100000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ProcessamentoArquivoXMLNotaFiscalIntegracao.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 100000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ValidarIrregularidadesControleDocumento.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 300000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ProcessarCargaEmProcessamentoDocumentosFiscais.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 6000, true);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ProcessarCargaEmProcessamentoDocumentosFiscaisEmLote.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000, true);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(AutorizacaoPagamentoContratoFrete.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 60000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(Pacotes.Instance, codigoEmpresa, clienteURLAcesso, stringEmissao + ".Pacotes", stringConexaoAdmin, utilizarLockNasThreads, 5000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(AnulacaoDocumentos.Instance, codigoEmpresa, clienteURLAcesso, stringEmissao + ".AnulacaoDocumentos", stringConexaoAdmin, utilizarLockNasThreads, 5000, true);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(LiberacaoEtapaDocumento.Instance, codigoEmpresa, clienteURLAcesso, stringEmissao + ".LiberacaoEtapaDocumento", stringConexaoAdmin, utilizarLockNasThreads, 3000, true);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(FaturamentoAutomaticoCTe.Instance, codigoEmpresa, clienteURLAcesso, stringEmissao + ".FaturamentoAutomaticoCTe", stringConexaoAdmin, utilizarLockNasThreads, 3600000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ProcessarRetornoAprovacaoTabelaFrete.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 60000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ProcessarFinalizacaoColetaEntregaEmLote.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3600000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoGhost.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 6000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ProcessarIntegracaoSuperApp.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ProcessarIntegracaoSuperAppDemaisEventos.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ProcessarIntegracaoNotificacaoApp.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(SGT.BackgroundWorkers.AbastecimentoInterno.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 100000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ProcessarDadosGestaoDevolucao.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(CargaEntregaFinalizacaoAssincrona.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(SGT.BackgroundWorkers.IntegracaoNFSe.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 100000, false);

                    await LongRunningProcessFactory.Instance.AddProcessAsync(OcorrenciaEntrega.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 60000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ProcessarUsuariosPortal.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 60000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ControleGeracaoCargaEntrega.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 60000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(GerarNFSesIntegracao.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 30000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(GerarCargaPorMDFe.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 30000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(GerarCTesIntegracao.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 30000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(CargaOferta.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 30000, false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(GrupoMotoristas.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 5000, ativo: false);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(SolicitacaoConfirmacaoDocumentosFiscais.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000, false);
                }
                else if (clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    await LongRunningProcessFactory.Instance.AddProcessAsync(SGT.BackgroundWorkers.FaturamentoMensal.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(GeracaoTituloNFSe.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(GeracaoTituloCTe.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(Alerta.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 86400000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(SGT.BackgroundWorkers.Patrimonio.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 86400000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ImportacaoBoletoRetorno.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 10000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(ControleStatusFuncionario.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 43200000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(EnvioEmailChat.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 600000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoDocumentosDestinados.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 3600000);
                    await LongRunningProcessFactory.Instance.AddProcessAsync(IntegracaoArquivosDocumentosDestinados.Instance, codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, utilizarLockNasThreads, 1800000);
                }
            }

            string erroLoginSSO = errorMessage ?? string.Empty;

            // We do not want to use any existing identity information
            EnsureLoggedOut();

            if (!string.IsNullOrWhiteSpace(erroLoginSSO))
                ViewBag.ErroLoginSSO = erroLoginSSO;

            Repositorio.Embarcador.Configuracoes.ConfiguracaoSSO repositorioCOnfiguracaoSSo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoSSO(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSO oAuth2Settings = repositorioCOnfiguracaoSSo.BuscarConfiguracaoPadrao();
            //Ajuste técnico para importar a configuração do Saml2 para o banco de dados....
            if (oAuth2Settings == null && Startup.AutenticacaoSaml2AtivaArquivoConfiguracao())
            {
                oAuth2Settings = Startup.ConfiguracaoSSoArquivo(Dominio.Enumeradores.TipoSso.Saml2);
                repositorioCOnfiguracaoSSo.Inserir(oAuth2Settings);
            }

            Startup.SamlAuthentication = ((oAuth2Settings?.Ativo ?? false) && (oAuth2Settings?.TipoSSo ?? Dominio.Enumeradores.TipoSso.OAuth2) == Dominio.Enumeradores.TipoSso.Saml2);

            bool autenticacaoAD = Startup.AzureAdAuthentication;
            bool autenticacaoSaml = Startup.SamlAuthentication;
            if (!autenticacaoSaml)
                autenticacaoSaml = Startup.OktaAuthentication;
            bool autenticacaoForms = true;
            bool habilitarRegistro = false;
            try
            {
                autenticacaoForms = Startup.FormsAuthentication;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao obter configuração de autenticação Forms: {ex.ToString()}", "CatchNoAction");
            }

            if (clienteURLAcesso.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador &&
                clienteURLAcesso.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS &&
                (clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor && !Startup.OktaHabilitaPortalTransportador))
            {
                autenticacaoAD = false;
                autenticacaoForms = true;
                autenticacaoSaml = false;
            }

            if (clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
            {
                habilitarRegistro = true;
            }

            //Se estiver passando o parametro multi, vamos habilitar a digitação de usuário e senha.
            if (multi == 1)
                autenticacaoForms = true;

            // Quando estiver acessando o /Login e tiver autenticação external AD ou SAML e não tem erro de autenticação, e o formulário
            // de login estiver desabilitado, vamos fazer o redirecionamento automático do login sem precisar clicar no botão...
            if ((autenticacaoAD || autenticacaoSaml) && string.IsNullOrEmpty(erroLoginSSO) && !autenticacaoForms)
                return this.ExternalSignIn(autenticacaoSaml).Result;

            string textoBotaoSSo = (autenticacaoSaml ? (Startup.OktaAuthentication ? Startup.OktaDisplay : (oAuth2Settings?.Display ?? string.Empty)) : Startup.AzureAdDisplay);

            // Vamos ver se não estã no Banco de dados a configuração.... (Migrando)...
            if (!autenticacaoAD && !autenticacaoSaml)
            {
                if (oAuth2Settings?.Ativo ?? false)
                {
                    autenticacaoSaml = true;
                    textoBotaoSSo = oAuth2Settings.Display;
                }
            }
            // Store the originating URL so we can attach it to a form field
            Models.Login viewModel = new Models.Login
            {
                ReturnUrl = returnUrl,
                AzureAD = (autenticacaoAD || autenticacaoSaml), // Habilita o botão de external login se for AD ou SAML..
                AzureAdDisplay = textoBotaoSSo, // Texto do botão de autenticação SAML ou Azure AD.
                AutenticacaoForms = autenticacaoForms, // Se apresenta o formulário de autenticação
                HabilitarRegistro = habilitarRegistro,
                Saml = autenticacaoSaml
            };

            Servicos.Embarcador.Configuracoes.Layout.MontarLayout(ViewBag, clienteURLAcesso, unitOfWork);

            try
            {
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                repChamado.AjustarSequenciaSeNecessario();

            }
            catch (Exception)
            {

            }

            return View(caminhosViewLogin, viewModel);
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Index(Models.Login viewModel)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);
            AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWork);
            AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorURL(_conexao.ObterHost);

            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            bool portalCliente = clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor && Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoAmbiente().NovoLayoutPortalFornecedor.Value;
            string caminhoBaseViews = "~/Views/Login";
            string caminhosViewLogin = portalCliente ? $"{caminhoBaseViews}/IndexCliente.cshtml" : $"{caminhoBaseViews}/Index.cshtml";

            bool alteraCorPrincipalPortal = false;

            if (clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
            {
                ViewBag.ExibirConteudoColog = !(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoAmbiente().OcultarConteudoColog.Value);
                ViewBag.LogoPersonalizada = !string.IsNullOrWhiteSpace(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoAmbiente().LogoPersonalizadaFornecedor) ? Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoAmbiente().LogoPersonalizadaFornecedor : "cologist-logo.png";
                ViewBag.ClasseCorBotoes = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoAmbiente().LayoutPersonalizadoFornecedor;

                if (ViewBag.LogoPersonalizada == "logvett-logo.png")
                {
                    alteraCorPrincipalPortal = true;
                }
            }

            ViewBag.AlteraCorPrincipalPortal = alteraCorPrincipalPortal;

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSO oAuth2Settings = new Repositorio.Embarcador.Configuracoes.ConfiguracaoSSO(unidadeDeTrabalho).BuscarConfiguracaoPadrao();

            bool autenticacaoAzure = Startup.AzureAdAuthentication;
            bool autenticacaoSAML = ((oAuth2Settings?.Ativo ?? false) && (oAuth2Settings?.TipoSSo ?? Dominio.Enumeradores.TipoSso.OAuth2) == Dominio.Enumeradores.TipoSso.Saml2);
            bool autenticacaoAD = Startup.LoginAD;
            bool autenticacaoForms = true;
            try
            {
                autenticacaoForms = Startup.FormsAuthentication;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao obter configuração de autenticação Forms: {ex.ToString()}", "CatchNoAction");
            }

            if (clienteURLAcesso.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador &&
                clienteURLAcesso.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS &&
                (clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor && !Startup.OktaHabilitaPortalTransportador))
            {
                autenticacaoAzure = false;
                autenticacaoSAML = false;
                autenticacaoForms = true;
            }

            if (Startup.OktaAuthentication)
                Challenge(new AuthenticationProperties { RedirectUri = "#Home" }, CookieAuthenticationDefaults.AuthenticationScheme);


            viewModel.AutenticacaoForms = autenticacaoForms;
            viewModel.AzureAD = (autenticacaoAzure || autenticacaoSAML);
            viewModel.AzureAdDisplay = (autenticacaoSAML ? (oAuth2Settings?.Display ?? string.Empty) : Startup.AzureAdDisplay);

            // Ensure we have a valid viewModel to work with
            if (!ModelState.IsValid)
            {
                unitOfWork.Dispose();
                Servicos.Embarcador.Configuracoes.Layout.MontarLayout(ViewBag, clienteURLAcesso, unidadeDeTrabalho);
                return View(caminhosViewLogin, viewModel);
            }

            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();

            try
            {
#if !DEBUG
                new Servicos.Embarcador.Configuracoes.Arquivo().AjustarConfiguracoes(unidadeDeTrabalho, clienteURLAcesso.URLHomologacao);
#else
                Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho);
#endif
                Servicos.Embarcador.Pessoa.PoliticaSenha serPoliticaSenha = new Servicos.Embarcador.Pessoa.PoliticaSenha();
                // Verify if a user exists with the provided identity information
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);
                Repositorio.Embarcador.Usuarios.PoliticaSenha repPoliticaSenha = new Repositorio.Embarcador.Usuarios.PoliticaSenha(unidadeDeTrabalho);

                AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoAcesso = clienteURLAcesso.TipoServicoMultisoftware;

                Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha politicaSenha = serPoliticaSenha.BuscarPoliticaSenha(unidadeDeTrabalho, tipoServicoAcesso);

                Dominio.Enumeradores.TipoAcesso tipoAcesso = Dominio.Enumeradores.TipoAcesso.Embarcador;

                if (clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    tipoAcesso = Dominio.Enumeradores.TipoAcesso.Emissao;

                if (clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros || clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                    tipoAcesso = Dominio.Enumeradores.TipoAcesso.Terceiro;

                if (clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                    tipoAcesso = Dominio.Enumeradores.TipoAcesso.Fornecedor;

                if (clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe || clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                    tipoAcesso = Dominio.Enumeradores.TipoAcesso.Emissao;

                Dominio.Entidades.Usuario usuario = null;

                if (viewModel.LoginSSO && viewModel.StatusSSO != 1)
                {
                    //statusCode = 3; // Usuário autenticou no AD, porem não faz parte do grupo permitido de acesso ao sistema.
                    //statusCode = 2; // Usuário autenticou no AD, porem o mesmo não faz parte de nenhum grupo.
                    var erroLoginSSO = (viewModel.StatusSSO == 2
                        ? Localization.Resources.Login.Login.UsuarioAutenticouADPoremNaoFazParteNenhumGrupo
                        : Localization.Resources.Login.Login
                            .UsuarioAutenticouADPoremNaoFazParteNenhumGrupoPermitidoAcessoSitemaMultiSoftware);
                    return Redirect("/Login?errorMessage=" + WebUtility.UrlEncode(erroLoginSSO));
                }

                bool loginAdSucesso = false;
                if (autenticacaoAD)
                {
                    string servidorAD = Startup.ServidorAD;
                    try
                    {
                        using (System.DirectoryServices.AccountManagement.PrincipalContext pc = new System.DirectoryServices.AccountManagement.PrincipalContext(System.DirectoryServices.AccountManagement.ContextType.Domain, servidorAD))
                        {
                            // Validando as credenciais no servidor AD
                            loginAdSucesso = pc.ValidateCredentials(viewModel.Usuario, viewModel.Senha);
                            if (!loginAdSucesso && !autenticacaoForms)
                            {
                                ModelState.AddModelError("", Localization.Resources.Login.Login.UsuarioSenhaInvalidoServidorDominio);
                                Servicos.Embarcador.Configuracoes.Layout.MontarLayout(ViewBag, clienteURLAcesso, unidadeDeTrabalho);
                                return View(caminhosViewLogin, viewModel);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", string.Format(Localization.Resources.Login.Login.OcorreuFalhaTentarConectarServidorDominio, servidorAD));
                        Servicos.Embarcador.Configuracoes.Layout.MontarLayout(ViewBag, clienteURLAcesso, unidadeDeTrabalho);
                        return View(caminhosViewLogin, viewModel);
                    }
                }

                string mensagemRetornoPoliticaSenha = "";
                string senha = viewModel.Senha;

                //POLITICA DE SENHA..
                if (tipoAcesso == Dominio.Enumeradores.TipoAcesso.Embarcador || tipoAcesso == Dominio.Enumeradores.TipoAcesso.Emissao || tipoAcesso == Dominio.Enumeradores.TipoAcesso.Fornecedor || clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe || clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                {
                    usuario = repUsuario.BuscarPorLogin(viewModel.Usuario, tipoAcesso);

                    if (politicaSenha != null && politicaSenha.HabilitarPoliticaSenha)
                    {
                        if (usuario != null)
                        {
                            if (usuario.UsuarioAcessoBloqueado && !usuario.UsuarioMultisoftware && !viewModel.LoginSSO)
                            {
                                if (politicaSenha.TempoEmMinutosBloqueioUsuario == 0)
                                {
                                    ModelState.AddModelError("", Localization.Resources.Login.Login.UsuarioBloqueadoTentativasAcessoInvalidasPorFavorVerifiqueComADMSistema);
                                    Servicos.Embarcador.Configuracoes.Layout.MontarLayout(ViewBag, clienteURLAcesso, unidadeDeTrabalho);
                                    return View(caminhosViewLogin, viewModel);
                                }
                                else
                                {
                                    if (usuario.DataHoraBloqueio.HasValue)
                                    {
                                        if (usuario.DataHoraBloqueio.Value.AddMinutes(politicaSenha.TempoEmMinutosBloqueioUsuario) <= DateTime.Now)
                                        {
                                            usuario.TentativasInvalidas = 0;
                                            usuario.UsuarioAcessoBloqueado = false;
                                            repUsuario.Atualizar(usuario);
                                        }
                                        else
                                        {
                                            ModelState.AddModelError("", string.Format(Localization.Resources.Login.Login.UsuarioBloqueadoTentativasAcessoInvalidasPorFavorVerifiqueComADMSistemaOuAguarde, politicaSenha.TempoEmMinutosBloqueioUsuario));
                                            Servicos.Embarcador.Configuracoes.Layout.MontarLayout(ViewBag, clienteURLAcesso, unidadeDeTrabalho);
                                            return View(caminhosViewLogin, viewModel);
                                        }
                                    }
                                    else
                                    {
                                        ModelState.AddModelError("", Localization.Resources.Login.Login.UsuarioBloqueadoTentativasAcessoInvalidasPorFavorVerifiqueComADMSistema);
                                        Servicos.Embarcador.Configuracoes.Layout.MontarLayout(ViewBag, clienteURLAcesso, unidadeDeTrabalho);
                                        return View(caminhosViewLogin, viewModel);
                                    }
                                }
                            }
                            else if (usuario.UsuarioAcessoBloqueado && viewModel.LoginSSO)
                            {
                                var erroLoginSSO = Localization.Resources.Login.Login.UsuarioAutenticouADPoremMesmoEstaBloqueadoSistemaMultiSoftware;
                                return Redirect("/Login?errorMessage=" + WebUtility.UrlEncode(erroLoginSSO));
                            }

                            bool direcionarTrocaSenha = ValidarPolitica(politicaSenha, usuario, senha, autenticacaoAD, loginAdSucesso, viewModel, unidadeDeTrabalho, out mensagemRetornoPoliticaSenha);
                            if (direcionarTrocaSenha)
                            {
                                base.SignIn(usuario, loginSSO: viewModel.LoginSSO);
                                return Redirect("AtualizacaoSenhaObrigatoria");
                            }
                        }
                        else
                        {
                            if (tipoAcesso == Dominio.Enumeradores.TipoAcesso.Fornecedor)
                            {
                                usuario = repUsuario.BuscarPorLoginVendedorOuGerente(viewModel.Usuario);

                                if (usuario != null)
                                {
                                    politicaSenha = repPoliticaSenha.BuscarPoliticaPadraoPorServicoMultiSoftware(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
                                    bool direcionarTrocaSenha = ValidarPolitica(politicaSenha, usuario, senha, autenticacaoAD, loginAdSucesso, viewModel, unidadeDeTrabalho, out mensagemRetornoPoliticaSenha);
                                    if (direcionarTrocaSenha)
                                    {
                                        base.SignIn(usuario, loginSSO: viewModel.LoginSSO);
                                        return Redirect("AtualizacaoSenhaObrigatoria");
                                    }
                                }
                                else
                                {
                                    usuario = repUsuario.BuscarPorLogin(viewModel.Usuario, Dominio.Enumeradores.TipoAcesso.Emissao);
                                    if (usuario != null)
                                    {
                                        politicaSenha = repPoliticaSenha.BuscarPoliticaPadraoPorServicoMultiSoftware(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
                                        bool direcionarTrocaSenha = ValidarPolitica(politicaSenha, usuario, senha, autenticacaoAD, loginAdSucesso, viewModel, unidadeDeTrabalho, out mensagemRetornoPoliticaSenha);
                                        if (direcionarTrocaSenha)
                                        {
                                            base.SignIn(usuario, loginSSO: viewModel.LoginSSO);
                                            return Redirect("AtualizacaoSenhaObrigatoria");
                                        }
                                    }
                                }
                            }

                        }
                    }
                    else
                    {
                        if (!viewModel.LoginSSO || viewModel.StatusSSO != 1)
                            usuario = ValidarSenha(repUsuario.BuscarPorLogin(viewModel.Usuario, tipoAcesso), viewModel.Senha);

                        if (usuario == null && tipoAcesso == Dominio.Enumeradores.TipoAcesso.Fornecedor)
                        {
                            usuario = ValidarSenha(repUsuario.BuscarPorLoginVendedorOuGerente(viewModel.Usuario), viewModel.Senha);

                            if (usuario == null)
                                usuario = ValidarSenha(repUsuario.BuscarPorLogin(viewModel.Usuario, Dominio.Enumeradores.TipoAcesso.Emissao), viewModel.Senha);
                        }

                        if (usuario != null && usuario.AlterarSenhaAcesso && tipoAcesso == Dominio.Enumeradores.TipoAcesso.Emissao)
                        {
                            base.SignIn(usuario, loginSSO: viewModel.LoginSSO);
                            return Redirect("AtualizacaoSenhaObrigatoria");
                        }
                    }
                }
                else if (tipoAcesso == Dominio.Enumeradores.TipoAcesso.Terceiro)
                {
                    usuario = ValidarSenha(repUsuario.BuscarTerceiroPorLogin(viewModel.Usuario, tipoAcesso), viewModel.Senha);
                }
                else
                    usuario = repUsuario.BuscarPorLogin(viewModel.Usuario, tipoAcesso);

                // If a user was found
                if (usuario != null && string.IsNullOrWhiteSpace(mensagemRetornoPoliticaSenha))
                {
                    if (usuario.FormaAutenticacaoUsuario.HasValue)
                    {
                        if (usuario.FormaAutenticacaoUsuario == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaAutenticacaoUsuario.UsuarioSenha)
                        {
                            string mensagemRetorno = Localization.Resources.Login.Login.VoceNaoTemPermissaoParaAcessarViaAD;
                            if (autenticacaoAD && loginAdSucesso)
                            {
                                ModelState.AddModelError("", mensagemRetorno);
                                Servicos.Embarcador.Configuracoes.Layout.MontarLayout(ViewBag, clienteURLAcesso, unidadeDeTrabalho);
                                return View(caminhosViewLogin, viewModel);
                            }
                            else if (viewModel.LoginSSO)
                            {
                                return Redirect("/Login?errorMessage=" + WebUtility.UrlEncode(mensagemRetorno));
                            }
                        }
                        else if (usuario.FormaAutenticacaoUsuario == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaAutenticacaoUsuario.AD && !string.IsNullOrWhiteSpace(viewModel.Usuario) && !string.IsNullOrWhiteSpace(viewModel.Senha) && !viewModel.LoginSSO)
                        {
                            ModelState.AddModelError("", Localization.Resources.Login.Login.VoceNaoTemPermissaoParaAcessarViaUsuarioSenha);
                            Servicos.Embarcador.Configuracoes.Layout.MontarLayout(ViewBag, clienteURLAcesso, unidadeDeTrabalho);
                            return View(caminhosViewLogin, viewModel);
                        }
                    }

                    Dominio.Entidades.Empresa empresa = usuario.Empresa;

                    if (empresa != null && empresa.Status != "A")
                    {
                        ModelState.AddModelError("", string.Format(Localization.Resources.Login.Login.EmpresaEstaInativa, empresa.CNPJ_Formatado));
                        Servicos.Embarcador.Configuracoes.Layout.MontarLayout(ViewBag, clienteURLAcesso, unidadeDeTrabalho);
                        return View(caminhosViewLogin, viewModel);
                    }

                    Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso perfilAcesso = usuario.PerfilAcesso;
                    Dominio.Entidades.Embarcador.Filiais.Turno turno = usuario.Turno;

                    if (turno == null && perfilAcesso != null && perfilAcesso.Turno != null)
                        turno = perfilAcesso.Turno;

                    Servicos.Embarcador.Login.Login svcLogin = new Servicos.Embarcador.Login.Login(unidadeDeTrabalho);
                    svcLogin.ValidarAcessoEmpresaCommerce(empresa, unidadeDeTrabalho, clienteURLAcesso.TipoServicoMultisoftware);

                    // Then create an identity for it and sign it in
                    base.SignIn(usuario, loginSSO: viewModel.LoginSSO);

                    if ((clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ||
                        clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin) &&
                        empresa.StatusFinanceiro == "B")
                    {
                        // No existing user was found that matched the given criteria
                        ModelState.AddModelError("", Localization.Resources.Login.Login.EmpresaPendenciasContateSetorSuporte);

                        // If we got this far, something failed, redisplay form
                        Servicos.Embarcador.Configuracoes.Layout.MontarLayout(ViewBag, clienteURLAcesso, unidadeDeTrabalho);

                        return View(caminhosViewLogin, viewModel);
                    }
                    else if ((perfilAcesso != null && perfilAcesso.HoraInicialAcesso.HasValue && perfilAcesso.HoraFinalAcesso.HasValue && (perfilAcesso.HoraInicialAcesso.Value > DateTime.Now.TimeOfDay || perfilAcesso.HoraFinalAcesso.Value < DateTime.Now.TimeOfDay)) ||
                        (perfilAcesso != null && perfilAcesso.HoraInicialAcesso == null && usuario.HoraInicialAcesso.HasValue && usuario.HoraFinalAcesso.HasValue && (usuario.HoraInicialAcesso.Value > DateTime.Now.TimeOfDay || usuario.HoraFinalAcesso.Value < DateTime.Now.TimeOfDay)) ||
                        (perfilAcesso == null && usuario.HoraInicialAcesso.HasValue && usuario.HoraFinalAcesso.HasValue && (usuario.HoraInicialAcesso.Value > DateTime.Now.TimeOfDay || usuario.HoraFinalAcesso.Value < DateTime.Now.TimeOfDay)) ||
                        (turno != null && !ValidarHorarioAcessoTurno(turno, unidadeDeTrabalho)))
                    {
                        ModelState.AddModelError("", Localization.Resources.Login.Login.VoceNaoEstaLiberadoParaAcessarSitemaNesseHorario);
                        Servicos.Embarcador.Configuracoes.Layout.MontarLayout(ViewBag, clienteURLAcesso, unidadeDeTrabalho);
                        return View(caminhosViewLogin, viewModel);
                    }

                    if (!viewModel.LoginSSO && (!autenticacaoAD || !loginAdSucesso))
                        return RedirectToLocal(viewModel.ReturnUrl);
                    //else
                    //    if (integracaoIntercab?.AtivarNovoHomeDash ?? false)
                    //    return Redirect("/#HomeCabotagem");
                    else
                        return Redirect("/#Home");

                }

                string msg = Localization.Resources.Login.Login.UsuarioSenhaInvalidos;

                if (viewModel.LoginSSO)
                    msg = string.Format(Localization.Resources.Login.Login.UsuarioComEmailAutententicouSucessoPorem, viewModel.Usuario);
                else if (autenticacaoAD && loginAdSucesso && usuario == null)
                    msg = string.Format(Localization.Resources.Login.Login.UsuarioAutenticouServidorADSucessoPorem, viewModel.Usuario);
                else if (autenticacaoAD && !loginAdSucesso)
                    msg += " no servidor de domínio.";

                ModelState.AddModelError("", string.IsNullOrWhiteSpace(mensagemRetornoPoliticaSenha) ? msg : mensagemRetornoPoliticaSenha);

                Servicos.Embarcador.Configuracoes.Layout.MontarLayout(ViewBag, clienteURLAcesso, unidadeDeTrabalho);
                if (!viewModel.LoginSSO && (!autenticacaoAD || !loginAdSucesso))
                    return View(caminhosViewLogin, viewModel);
                else if (autenticacaoAD && loginAdSucesso)
                    return View(caminhosViewLogin, viewModel);
                else if (viewModel.LoginSSO)
                {
                    var erroLoginSSO = string.Format(Localization.Resources.Login.Login.UsuarioComEmailAutententicouSucessoPorem, viewModel.Usuario);
                    return Redirect("/Login?errorMessage=" + WebUtility.UrlEncode(erroLoginSSO));
                }
                else
                {
                    var erroLoginSSO = string.Format(Localization.Resources.Login.Login.UsuarioAutenticouServidorADSucessoPorem, viewModel.Usuario);
                    return Redirect("/Login?errorMessage=" + WebUtility.UrlEncode(erroLoginSSO));
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                ModelState.AddModelError("", Localization.Resources.Login.Login.OcorreuFalhaAcessarSistemaFavorTentarNovamente);
                Servicos.Embarcador.Configuracoes.Layout.MontarLayout(ViewBag, clienteURLAcesso, unidadeDeTrabalho);
                return View(caminhosViewLogin, viewModel);
            }
            finally
            {
                unitOfWork.Dispose();
                unidadeDeTrabalho.Dispose();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            base.SignOut();
            // Last we redirect to a controller/action that requires authentication to ensure a redirect takes place
            // this clears the Request.IsAuthenticated flag since this triggers a new request
            return RedirectToLocal();
        }

        #endregion

        #region EXTERNAL LOGIN

        private Dominio.Entidades.Usuario ValidarSenha(Dominio.Entidades.Usuario usuario, string senha)
        {
            if (usuario == null)
                return null;

            // se não tem senha cadastrada → inválido
            if (string.IsNullOrWhiteSpace(usuario.Senha))
                return null;

            // calcula hashes somente se indicado
            string senhaSHA256 = null;
            string senhaMD5 = null;

            if (usuario.SenhaCriptografada)
            {
                senhaSHA256 = Servicos.Criptografia.GerarHashSHA256(senha);
                senhaMD5 = Servicos.Criptografia.GerarHashMD5(senha);
            }

            // ---- TAUTOLOGIA (aceita qualquer uma das senhas válidas) ----
            bool senhaValida =
                usuario.Senha == senha ||          // senha pura
                usuario.Senha == senhaMD5 ||       // senha MD5
                usuario.Senha == senhaSHA256;      // senha SHA256

            if (!senhaValida)
                return null;

            return usuario;
        }


        // Sends an OpenIDConnect Sign-In Request.  
        [AllowAnonymous]
        public async Task<IActionResult> ExternalSignIn(bool saml)
        {
            if (!saml || Startup.OktaAuthentication)
            {
                if (!User.Identity.IsAuthenticated)
                {
                    if (Startup.OktaAuthentication)
                    {
                        if (!Startup.OktaHabilitaPortalTransportador)
                            return Challenge(new AuthenticationProperties { RedirectUri = "#Home" }, CookieAuthenticationDefaults.AuthenticationScheme);
                        else
                            return Challenge(new AuthenticationProperties { RedirectUri = Startup.OktaIssuerUrlPortalTransportador }, CookieAuthenticationDefaults.AuthenticationScheme);
                    }
                    else
                        return Challenge(new AuthenticationProperties { RedirectUri = "#Home" }, OpenIdConnectDefaults.AuthenticationScheme);

                }
                return null;
            }
            else
            {
                // Nova Configuração DB.
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSO configuracaoSSo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoSSO(unitOfWork).BuscarConfiguracaoPadrao();

                if ((configuracaoSSo?.Ativo ?? false) && (configuracaoSSo?.TipoSSo ?? Dominio.Enumeradores.TipoSso.OAuth2) == Dominio.Enumeradores.TipoSso.Saml2)
                {
                    //  "https://login.microsoftonline.com/64877511-d0b9-4f56-aafa-f890989620ed/saml2";
                    string samlEndpoint = string.Format(configuracaoSSo.UrlAutenticacao, configuracaoSSo.ClientId);

                    App_Start.AuthRequest request = new App_Start.AuthRequest(
                        configuracaoSSo.UrlDominio,
                        configuracaoSSo.UrlDominio + "Login/Saml2"
                    );

                    //redirect the user to the SAML provider
                    return Redirect(request.GetRedirectUrl(samlEndpoint));
                }
                else
                {
                    string authenticationScheme = "OAuth2";
                    if ((configuracaoSSo?.Ativo ?? false) && (configuracaoSSo?.TipoSSo ?? Dominio.Enumeradores.TipoSso.OAuth2) == Dominio.Enumeradores.TipoSso.CyberArk)
                        authenticationScheme = "CyberArk";

                    return Challenge(new AuthenticationProperties { RedirectUri = "#Home" }, authenticationScheme);
                }
            }
        }

        public async Task<IActionResult> CallbackSSo()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync();

            if (!authenticateResult.Succeeded)
            {
                return RedirectToAction("Login");
            }

            var claimsPrincipal = authenticateResult.Principal;
            var email = claimsPrincipal?.FindFirst("email")?.Value;
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login");

            Models.Login login = new Models.Login()
            {
                Usuario = email,
                Senha = email,
                LoginSSO = true,
                ReturnUrl = "/#Home",
                StatusSSO = 1 // Sucesso..
            };
            return Index(login).Result;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> SigninOidc(string code, int status)
        {
            string chave = string.Concat("CT3##MULT1@#$S0FTW4R3", DateTime.Now.ToString("ddMMyyyyhh"));
            //email|nome
            string decode = Servicos.Criptografia.Descriptografar(code, chave);
            string[] parts = decode.Split('|');
            Models.Login login = new Models.Login()
            {
                Usuario = parts[0],
                Senha = parts[0],
                LoginSSO = true,
                ReturnUrl = "/#Home",
                StatusSSO = status
            };
            return Index(login).Result;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Saml2(string returnUrl)
        {
            //            string samlCertificate = @"-----BEGIN CERTIFICATE-----
            //MIIC8DCCAdigAwIBAgIQOU/plqU0upJABgcIP6uzjzANBgkqhkiG9w0BAQsFADA0MTIwMAYDVQQD
            //EylNaWNyb3NvZnQgQXp1cmUgRmVkZXJhdGVkIFNTTyBDZXJ0aWZpY2F0ZTAeFw0yMDA4MDcxNTM1
            //MDdaFw0yMzA4MDcxNTM1MDdaMDQxMjAwBgNVBAMTKU1pY3Jvc29mdCBBenVyZSBGZWRlcmF0ZWQg
            //U1NPIENlcnRpZmljYXRlMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAsTKnAG8kljN5
            //UdS4alfh7WF3v71hLpeQtwgQUy8f/TfeWm8CbpXisxNNeAkCwjwWscNDxhmczmyz9HDaANmUnq+Y
            //9DZ9Lg4dzdDNz8nhlMYuI68yiAyXB4mKAGOkUXlkz3qvaq8vfpocP4rJmMQiwDk6On4trTR9uZGH
            //sLgCyPvE+fbQKguxy1q3PZqhoiA64u/4AroRytDWfAgCIQS630C9vOZLyumFYv5E3ZaZdKkJgYS0
            //BuUKX0/M/EUaBINbfmg9/OjEe2CSkGqwQ5kb9KStyxa80b10O5xpAgV1YeKISw7LSYzh18r0Q/G1
            //QzhJmIuTiLp58GqUKo/10LVWyQIDAQABMA0GCSqGSIb3DQEBCwUAA4IBAQByB6fCes1ir6qfJW7t
            //FGEpaTiUJ4CXDEY7LJJaLzQ7MiWw1/mCCrPGORp0mluOVj37C8EFblmMKHmIwV6cVdZ1fhERxDGW
            //3oeyZN865k9LqERd2+KIF+MgpyjEWXJWBJmExleeOlDnH4MIC+jiX8HZ+gFMXyT3e5yvHeGVl/5y
            //dttVhLkzvd2WbK7dC81/jPvwFEoESnW5bNdgiLlO0z+E26TXfiPCCx4TR/AWdn24W88LOlmwZBos
            //yoCgk2MyzXUzRtWPvG/2t38P7HOrXZfj/QhSPVeNHEy8TRtM42yxqhMGKzdotMj8ZT9UVXfj9ARm
            //181PSV3Ygt5jPekKhPi6
            //-----END CERTIFICATE-----";

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSO configuracaoSSo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoSSO(unitOfWork).BuscarConfiguracaoPadrao();

            App_Start.Response samlResponse = new App_Start.Response();
            if ((configuracaoSSo?.Ativo ?? false) && (configuracaoSSo?.TipoSSo ?? Dominio.Enumeradores.TipoSso.OAuth2) == Dominio.Enumeradores.TipoSso.Saml2)
            {
                //Metadata...xml...
                string extensao = System.IO.Path.GetExtension(configuracaoSSo.CaminhoArquivoCertificado).ToLower();
                if (extensao.Equals(".xml"))
                    samlResponse.SetMetadataXml(Utilidades.IO.FileStorageService.Storage.ReadAllText(configuracaoSSo.CaminhoArquivoCertificado));
                else
                    samlResponse.SetCertificateStr(Utilidades.IO.FileStorageService.Storage.ReadAllText(configuracaoSSo.CaminhoArquivoCertificado));
            }
            else if (Utilidades.IO.FileStorageService.Storage.Exists(Startup.OktaCertificado))
                samlResponse.SetCertificateStr(Utilidades.IO.FileStorageService.Storage.ReadAllText(Startup.OktaCertificado));

            //Vamos gravar um log do retorno para saber o que retorna no Saml2
            List<string> listValues = new List<string>();

            foreach (var key in Request.Form.Keys)
                listValues.Add($"{key} = {Request.Form[key]}");

            Servicos.Log.TratarErro(string.Join(Environment.NewLine, listValues), "saml");

            samlResponse.LoadXmlFromBase64(Request.Form["SAMLResponse"]);


            string erroLoginSSO = string.Empty;
            if (samlResponse.IsValid(ref erroLoginSSO))
            {
                string username, email, firstname, lastname;
                try
                {
                    username = samlResponse.GetNameID();
                    email = samlResponse.GetEmail();
                    firstname = samlResponse.GetFirstName();
                    lastname = samlResponse.GetLastName();
                    string customerID = samlResponse.GetCustomAttribute("customerID");

                    if (string.IsNullOrWhiteSpace(email))
                        email = samlResponse.GetCustomAttribute("email");

                    // sup.multisoftware@galloww.com - cesar@multisoftware.com.br - Suporte - Multisoftware (Externo)
                    Models.Login login = new Models.Login()
                    {
                        Usuario = Startup.OktaAuthentication && !string.IsNullOrWhiteSpace(customerID) ? customerID : (Startup.OktaAuthentication && !string.IsNullOrWhiteSpace(email) ? email : username),
                        Senha = username,
                        LoginSSO = true,
                        ReturnUrl = "/#Home",
                        StatusSSO = 1 // Success
                    };
                    return Index(login).Result;
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    erroLoginSSO = Localization.Resources.Login.Login.AutenticacaoSAMLValidaPoremOcorreuErroIdentificar;
                    return Redirect("/Login?errorMessage=" + WebUtility.UrlEncode(erroLoginSSO));
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(erroLoginSSO))
                    erroLoginSSO = Localization.Resources.Login.Login.NaoFoiPossivelValidarUsuarioAutenticacaoSAML;
                return Redirect("/Login?errorMessage=" + WebUtility.UrlEncode(erroLoginSSO));
            }
        }

        // Signs the user out and clears the cache of access tokens.  
        public void ExternalSignOut()
        {
            base.SignOutExternal();
        }

        #endregion

        #region Métodos Privados

        private void EnsureLoggedOut()
        {
            // If the request is (still) marked as authenticated we send the user to the logout action
            if (User.Identity.IsAuthenticated)
                Logout();
        }

        private bool ExecutarThread()
        {
#if DEBUG
            return false;
#endif
            return true;
        }

        private IActionResult RedirectToLocal(string returnUrl = "")
        {
            // If the return url starts with a slash "/" we assume it belongs to our site
            // so we will redirect to this "action"
            if (!string.IsNullOrWhiteSpace(returnUrl) && returnUrl.Length > 0)
                return Redirect("/#" + returnUrl);

            // If we cannot verify if the url is local to our host we redirect to a default location
            return Redirect("/#Home");
        }

        private bool ValidarPolitica(Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha politicaSenha, Dominio.Entidades.Usuario usuario, string senha, bool autenticacaoAD, bool loginAdSucesso, Models.Login viewModel, Repositorio.UnitOfWork unidadeDeTrabalho, out string mensagemRetornoPoliticaSenha)
        {
            if (!(usuario.PerfilAcesso?.Ativo ?? true))
            {
                mensagemRetornoPoliticaSenha = Localization.Resources.Login.Login.UsuarioSenhaInvalidos;
                return false;
            }

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);

            mensagemRetornoPoliticaSenha = "";

            if (politicaSenha != null)// && !viewModel.LoginSSO)
            {
                bool direcionarTrocaSenha = false;

                if (viewModel.LoginSSO)
                    usuario.AlterarSenhaAcesso = false;

                if (usuario.AlterarSenhaAcesso && !usuario.UsuarioMultisoftware)
                {
                    var senhaValidada = ValidarSenha(usuario, senha);

                    direcionarTrocaSenha = senhaValidada is not null;

                    //se a senha for o  cf força redirecinar para troca  de senha pois isso é uma falha de segurança é um legado, logo vamos manter
                    //if (senha == usuario.CPF || senha == Utilidades.String.OnlyNumbers(usuario.CPF))
                    //    direcionarTrocaSenha = true;

                    if (!direcionarTrocaSenha)
                    {
                        mensagemRetornoPoliticaSenha = Localization.Resources.Login.Login.UsuarioSenhaInvalidos;
                    }
                }
                else
                {
                    if (politicaSenha.PrazoExpiraSenha > 0 && !usuario.UsuarioMultisoftware && !viewModel.LoginSSO)
                    {
                        if (!usuario.DataUltimaAlteracaoSenhaObrigatoria.HasValue || usuario.DataUltimaAlteracaoSenhaObrigatoria.Value.AddDays(politicaSenha.PrazoExpiraSenha) < DateTime.Now)
                        {
                            direcionarTrocaSenha = true;
                            usuario.AlterarSenhaAcesso = true;
                        }
                    }
                }

                if (!direcionarTrocaSenha && (!usuario.AlterarSenhaAcesso || usuario.UsuarioMultisoftware))
                {
                    string senhaMD5 = "", senhaSHA256 = "";

                    if (usuario.SenhaCriptografada)
                    {
                        senhaSHA256 = Servicos.Criptografia.GerarHashSHA256(senha);
                        senhaMD5 = Servicos.Criptografia.GerarHashMD5(senha);
                    }

                    if (usuario.Senha != senha && usuario.Senha != senhaMD5 && usuario.Senha != senhaSHA256 && viewModel.LoginSSO == false && (autenticacaoAD == false || loginAdSucesso == false))
                        mensagemRetornoPoliticaSenha = Localization.Resources.Login.Login.UsuarioSenhaInvalidos;
                }

                if (!string.IsNullOrWhiteSpace(mensagemRetornoPoliticaSenha))
                    usuario.TentativasInvalidas++;
                else
                    usuario.TentativasInvalidas = 0;

                if (politicaSenha.BloquearUsuarioAposQuantidadeTentativas > 0 && usuario.TentativasInvalidas >= politicaSenha.BloquearUsuarioAposQuantidadeTentativas)
                {
                    usuario.DataHoraBloqueio = DateTime.Now;
                    usuario.TentativasInvalidas = 0;
                    usuario.UsuarioAcessoBloqueado = true;
                }

                if (politicaSenha.InativarUsuarioAposDiasSemAcessarSistema > 0 && usuario.UltimoAcesso.HasValue)
                    if (usuario.UltimoAcesso.Value.AddDays(politicaSenha.InativarUsuarioAposDiasSemAcessarSistema) < DateTime.Now)
                    {
                        usuario.DataHoraBloqueio = DateTime.Now;
                        usuario.TentativasInvalidas = 0;
                        usuario.UsuarioAcessoBloqueado = true;
                        usuario.Status = "I";
                    }

                repUsuario.Atualizar(usuario);
                return direcionarTrocaSenha;
            }
            else
            {
                if (usuario.Senha != senha && viewModel.LoginSSO == false && (autenticacaoAD == false || loginAdSucesso == false))
                    mensagemRetornoPoliticaSenha = string.Format(Localization.Resources.Login.Login.UsuarioSenhaInvalidosZero, (autenticacaoAD && loginAdSucesso == false ? Localization.Resources.Login.Login.NoServidorDominio : ""));
            }

            return false;
        }

        private bool ValidarHorarioAcessoTurno(Dominio.Entidades.Embarcador.Filiais.Turno turno, Repositorio.UnitOfWork unitOfWork)
        {
            if (turno == null) return false;

            Repositorio.Embarcador.Filiais.TurnoHorarioAcesso repositorioTurnoHorarioAcesso = new Repositorio.Embarcador.Filiais.TurnoHorarioAcesso(unitOfWork);
            List<Dominio.Entidades.Embarcador.Filiais.TurnoHorarioAcesso> horariosAcesso = repositorioTurnoHorarioAcesso.BuscarPorTurno(turno.Codigo);

            int dia = ((int)DateTime.Now.DayOfWeek) + 1;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana diaDaSemana = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana)dia;

            foreach (Dominio.Entidades.Embarcador.Filiais.TurnoHorarioAcesso horarioAcesso in horariosAcesso)
            {
                if (horarioAcesso.DiasDaSemana.Contains(diaDaSemana))
                {
                    if (horarioAcesso.HoraInicial.Value <= DateTime.Now.TimeOfDay && horarioAcesso.HoraFinal.Value >= DateTime.Now.TimeOfDay)
                        return true;
                }
            }

            return false;
        }

        #endregion
    }
}