using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Entidades.Embarcador.Configuracoes;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    public class PedidosController : BaseController
    {
        #region Construtores

        public PedidosController(Conexao conexao) : base(conexao) { }

        #endregion

        [CustomAuthorize("Pedidos/LocalArmazenamentoCanhoto")]
        public async Task<IActionResult> LocalArmazenamentoCanhoto()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/MotivoImportacaoAtrasada")]
        public async Task<IActionResult> MotivoImportacaoAtrasada()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/EnvioNFePedido")]
        public async Task<IActionResult> EnvioNFePedido()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/AtendimentoCliente")]
        public async Task<IActionResult> AtendimentoCliente()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/Pedido")]
        public async Task<IActionResult> Pedido()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {

                Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repositorioConfiguracao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repConfiguracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);
                Repositorio.Embarcador.Pedidos.SituacaoComercialPedido repSituacaoComercialPedido = new Repositorio.Embarcador.Pedidos.SituacaoComercialPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repConfiguracaoPedido.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = ObterOperadorLogistica(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracao = repositorioConfiguracao.ObterConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = repConfiguracaoJanelaCarregamento.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repConfiguracaoGeral.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repConfiguracaoWebService.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao integracaoMercadoLivre = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MercadoLivre);

                bool existeSituacaoComercialPedido = TipoServicoMultisoftware == TipoServicoMultisoftware.MultiEmbarcador && repSituacaoComercialPedido.ExisteSituacaoComercialPedido();

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pedidos/Pedido");
                ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
                ViewBag.TelaPedidosResumido = (operadorLogistica?.TelaPedidosResumido ?? false) ? "true" : "false";
                ViewBag.PermiteSelecionarHorarioEncaixe = (operadorLogistica?.PermiteSelecionarHorarioEncaixe ?? false) ? "true" : "false";
                ViewBag.PermitirInformarAcrescimoDescontoNoPedido = (configuracaoPedido?.PermitirInformarAcrescimoDescontoNoPedido ?? false) ? "true" : "false";
                ViewBag.ExibirCamposRecebimentoPedidoIntegracao = (configuracaoPedido?.ExibirCamposRecebimentoPedidoIntegracao ?? false) ? "true" : "false";
                ViewBag.NaoPermitirAlterarCentroResultadoPedido = (configuracaoPedido?.NaoPermitirAlterarCentroResultadoPedido ?? false) ? "true" : "false";
                ViewBag.NaoPermitirInformarExpedidorNoPedido = (configuracaoPedido?.NaoPermitirInformarExpedidorNoPedido ?? false) ? "true" : "false";
                ViewBag.PermiteInformarPedidoSubstituicao = (configuracaoPedido?.PermiteInformarPedidoDeSubstituicao ?? false) ? "true" : "false";
                ViewBag.ExibirPacotesOcorrencia = (configuracao?.ExibirPacotesOcorrenciaControleEntrega ?? false) ? "true" : "false";
                ViewBag.IgnorarValidacoesDatasPrevisaoAoEditarPedido = (configuracaoPedido?.IgnorarValidacoesDatasPrevisaoAoEditarPedido ?? false) ? "true" : "false";
                ViewBag.GerarCargaAutomaticamenteNoPedido = (configuracaoPedido?.GerarCargaAutomaticamenteNoPedido ?? false) ? "true" : "false";
                ViewBag.GerarJanelaDeCarregamento = (configuracaoJanelaCarregamento?.GerarJanelaDeCarregamento ?? false) ? "true" : "false";
                Repositorio.Embarcador.Integracao.IntegracaoDocas repIntegracaoDocas = new Repositorio.Embarcador.Integracao.IntegracaoDocas(unitOfWork);
                ViewBag.UtilizaIntegracaoDeTemposDoca = repIntegracaoDocas.PossuiIntegracaoDocas() ? "true" : "false";
                ViewBag.HabilitarCadastroArmazem = (configuracaoGeral?.HabilitarCadastroArmazem ?? false) ? "true" : "false";
                ViewBag.ExisteSituacaoComercialPedido = existeSituacaoComercialPedido ? "true" : "false";
                ViewBag.HabilitarFluxoPedidoEcommerce = (configuracaoWebService?.HabilitarFluxoPedidoEcommerce ?? false) ? "true" : "false";
                ViewBag.UtilizarCampoDeMotivoDePedido = (configuracaoPedido?.UtilizarCampoDeMotivoDePedido ?? false) ? "true" : "false";
                ViewBag.ExibirAuditoriaPedidos = (configuracaoPedido?.ExibirAuditoriaPedidos ?? false) ? "true" : "false";
                ViewBag.UsuarioUtilizaSegregacaoPorProvedor = (this.Usuario?.UsuarioUtilizaSegregacaoPorProvedor ?? false) ? "true" : "false";
                ViewBag.HabilitarFuncionalidadesProjetoGollum = (configuracaoGeral?.HabilitarFuncionalidadesProjetoGollum ?? false) ? "true" : "false";
                ViewBag.PossuiIntegracaoMercadoLivre = integracaoMercadoLivre != null ? "true" : "false";

                bool portalCliente = IsLayoutClienteAtivo(unitOfWork);
                string caminhoBaseViews = "~/Views/Pedidos/";
                string caminhosViewMasterLayout = portalCliente ? $"{caminhoBaseViews}/PedidoCliente.cshtml" : $"{caminhoBaseViews}/Pedido.cshtml";

                return View(caminhosViewMasterLayout);
            }
        }

        [CustomAuthorize("Pedidos/CancelamentoSaldoReserva")]
        public async Task<IActionResult> CancelamentoSaldoReserva()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/DetalheEntrega")]
        public async Task<IActionResult> DetalheEntrega()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/TipoOperacao")]
        public async Task<IActionResult> TipoOperacao()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoEMP repConfiguracoesEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga repositorioConfiguracaoMontagemCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repConfiguracaoGeral.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracao.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoCarga = repConfiguracaoGeralCarga.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMontagemCarga configuracaoMontagemCarga = repositorioConfiguracaoMontagemCarga.BuscarPrimeiroRegistro();

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP integracaoEMP = repConfiguracoesEMP.Buscar();
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork).BuscarIntegracao();
                Repositorio.Embarcador.Configuracoes.IntegracaoTrizy repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoTrizy(unitOfWork);
                IntegracaoTrizy configuracaoIntegracao = await repConfiguracaoIntegracao.BuscarPrimeiroRegistroAsync();

                ViewBag.ConfiguracaoTipoOperacao = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    configuracao.PermitirRejeitarCargaJanelaCarregamentoTransportador,
                    configuracao.PermiteEmitirCargaDiferentesOrigensParcialmente,
                    UtilizarConfiguracaoTipoOperacaoGeracaoCargaPorPedido = configuracaoCarga?.UtilizarConfiguracaoTipoOperacaoGeracaoCargaPorPedido ?? false,
                    AtivarIntegracaoContainerEMP = integracaoEMP?.AtivarIntegracaoContainerEMP ?? false,
                    PossuiIntegracaoIntercab = integracaoIntercab?.PossuiIntegracaoIntercab ?? false,
                    PossuiIntegracaoEMP = integracaoEMP?.PossuiIntegracaoEMP ?? false,
                    configuracao.UtilizaAppTrizy,
                    configuracaoGeral.HabilitarFuncionalidadesProjetoGollum,
                    configuracao.IncluirCargaCanceladaProcessarDT,
                    configuracao.ControleComissaoPorTipoOperacao,
                    configuracaoMontagemCarga.ExibirListagemNotasFiscais,
                    configuracao.HabilitarFuncionalidadeProjetoNFTP,
                    VersaoIntegracaoTrizy = configuracaoIntegracao?.VersaoIntegracao ?? VersaoIntegracaoTrizy.Versao1,
                    ValidarIntegracaoPorOperacao = configuracaoIntegracao?.ValidarIntegracaoPorOperacao ?? false,
                });

                return View();
            }
        }

        [CustomAuthorize("Pedidos/GrupoTipoOperacao")]
        public async Task<IActionResult> GrupoTipoOperacao()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/TipoTerminalImportacao")]
        public async Task<IActionResult> TipoTerminalImportacao()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/RegrasPedido")]
        public async Task<IActionResult> RegrasPedido()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/AutorizacaoPedido")]
        public async Task<IActionResult> AutorizacaoPedido()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repConfiguracaoPedido.BuscarConfiguracaoPadrao();

                ViewBag.UtilizarCampoDeMotivoDePedido = (configuracaoPedido?.UtilizarCampoDeMotivoDePedido ?? false) ? "true" : "false";
                return View();
            }
        }

        [CustomAuthorize("Pedidos/ClassificacaoRiscoONU")]
        public async Task<IActionResult> ClassificacaoRiscoONU()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/PedidoCancelamento")]
        public async Task<IActionResult> PedidoCancelamento()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/RegraTomador")]
        public async Task<IActionResult> RegraTomador()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/CanalEntrega")]
        public async Task<IActionResult> CanalEntrega()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/LinhaSeparacao")]
        public async Task<IActionResult> LinhaSeparacao()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/TipoDetalhe")]
        public async Task<IActionResult> TipoDetalhe()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/PlanejamentoPedido")]
        public async Task<IActionResult> PlanejamentoPedido()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repConfiguracaoGeral.BuscarConfiguracaoPadrao();

                ViewBag.UsuarioUtilizaSegregacaoPorProvedor = (this.Usuario?.UsuarioUtilizaSegregacaoPorProvedor ?? false) ? "true" : "false";
                ViewBag.HabilitarFuncionalidadesProjetoGollum = (configuracaoGeral?.HabilitarFuncionalidadesProjetoGollum ?? false) ? "true" : "false";

                return View();
            }
        }

        [CustomAuthorize("Pedidos/Navio")]
        public async Task<IActionResult> Navio()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoEMP repConfiguracaoIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoEMP = repConfiguracaoIntegracaoEMP.Buscar();

                ViewBag.ConfiguracaoNavio = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    RemoverObrigacaoCodigoEmbarcacaoCadastroNavio = integracaoIntercab?.RemoverObrigacaoCodigoEmbarcacaoCadastroNavio ?? false,
                    PossuiIntegracaoIntercab = integracaoIntercab?.PossuiIntegracaoIntercab ?? false,
                    AtivarIntegracaoRecebimentoNavioEMP = configuracaoIntegracaoEMP?.AtivarIntegracaoRecebimentoNavioEMP ?? false,
                });
            }
            return View();
        }

        [CustomAuthorize("Pedidos/PlanejamentoPedidoConfiguracao")]
        public async Task<IActionResult> PlanejamentoPedidoConfiguracao()
        {
            return View();
        }


        [CustomAuthorize("Pedidos/TipoContainer")]
        public async Task<IActionResult> TipoContainer()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/Container")]
        public async Task<IActionResult> Container()
        {
            List<PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pedidos/Container");
            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);

            return View();
        }

        [CustomAuthorize("Pedidos/ViagemNavio")]
        public async Task<IActionResult> ViagemNavio()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/Porto")]
        public async Task<IActionResult> Porto()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/AcompanhamentoPedido")]
        public async Task<IActionResult> AcompanhamentoPedido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                ViewBag.CompartilharAcessoEntreGrupoPessoas = IsCompartilharAcessoEntreGrupoPessoas() ? "true" : "false";
                ViewBag.FornecedorTMS = string.IsNullOrWhiteSpace(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().FornecedorTMS) ? "false" : Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().FornecedorTMS;

                return View();
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [CustomAuthorize("Pedidos/PedidoSVM")]
        public async Task<IActionResult> PedidoSVM()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/PedidoTipoPagamento")]
        public async Task<IActionResult> PedidoTipoPagamento()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/MotivoRejeicaoAlteracaoPedido")]
        public async Task<IActionResult> MotivoRejeicaoAlteracaoPedido()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/RegraAutorizacaoAlteracaoPedido")]
        public async Task<IActionResult> RegraAutorizacaoAlteracaoPedido()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/AutorizacaoAlteracaoPedido")]
        public async Task<IActionResult> AutorizacaoAlteracaoPedido()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/AutorizacaoAlteracaoPedidoTransportador")]
        public async Task<IActionResult> AutorizacaoAlteracaoPedidoTransportador()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/PedidoCampo")]
        public async Task<IActionResult> PedidoCampo()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/PedidoCampoObrigatorio")]
        public async Task<IActionResult> PedidoCampoObrigatorio()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/TipoOperacaoCampo")]
        public async Task<IActionResult> TipoOperacaoCampo()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/TipoOperacaoValorPadrao")]
        public async Task<IActionResult> TipoOperacaoValorPadrao()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/PlanejamentoPedidoTMS")]
        public async Task<IActionResult> PlanejamentoPedidoTMS()
        {
            List<PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pedidos/PlanejamentoPedidoTMS");
            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
            return View();
        }

        [CustomAuthorize("Pedidos/PedidoSimplificado")]
        public async Task<IActionResult> PedidoSimplificado()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/PedidoSimplificadoTMS")]
        public async Task<IActionResult> PedidoSimplificadoTMS()
        {

            ViewBag.ConfiguracaoTMS = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                TipoServicoMultisoftware
            });

            return View();
        }

        [CustomAuthorize("Pedidos/ImportacaoPedido")]
        public async Task<IActionResult> ImportacaoPedido()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/PedidoOcorrenciaColetaEntregaIntegracao")]
        public async Task<IActionResult> PedidoOcorrenciaColetaEntregaIntegracao()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/ImportarEDI")]
        public async Task<IActionResult> ImportarEDI()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/ImportacaoTakePay")]
        public async Task<IActionResult> ImportacaoTakePay()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/NotaDeDebito")]
        public async Task<IActionResult> NotaDeDebito()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/ConfiguracaoOcorrenciaPedido")]
        public async Task<IActionResult> ConfiguracaoOcorrenciaPedido()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/NumeroCarregamentoPorLote")]
        public async Task<IActionResult> NumeroCarregamentoPorLote()
        {
            return View();
        }


        [CustomAuthorize("Pedidos/AtendimentoAPedidos")]
        public async Task<IActionResult> AtendimentoAPedidos()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/CanalVenda")]
        public async Task<IActionResult> CanalVenda()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/Booking")]
        public async Task<IActionResult> Booking()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pedidos/Booking");
            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
            return View();
        }

        [CustomAuthorize("Pedidos/PedidoBookingIntegracao")]
        public async Task<IActionResult> PedidoBookingIntegracao()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/RegraTipoOperacao")]
        public async Task<IActionResult> RegraTipoOperacao()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/CotacaoEspecial")]
        public async Task<IActionResult> CotacaoEspecial()
        {
            return View();
        }

        [AllowAuthenticate]
        public async Task<IActionResult> RetiradaProduto()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Usuario usuarioLogado = this.Usuario;

                var configuracaoRetiradaPedido = new
                {
                    PesquisarMotorista = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().UtilizarIntegracaoSaintGobainNova.Value,
                    configuracaoPedido.NaoExibirPedidosDoDiaAgendamentoPedidos,
                };

                ViewBag.NomeUsuario = usuarioLogado.Nome;
                ViewBag.ConfiguracaoRetiradaProduto = Newtonsoft.Json.JsonConvert.SerializeObject(configuracaoRetiradaPedido);

                return View();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> RetiradaProdutoLista()
        {
            Dominio.Entidades.Usuario usuarioLogado = this.Usuario;

            ViewBag.NomeUsuario = usuarioLogado.Nome;
            ViewBag.Gerente = (usuarioLogado.TipoComercial == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComercial.Gerente ? 1 : 0);
            return View();
        }

        [AllowAuthenticate]
        public async Task<IActionResult> RetiradaPedidoLista()
        {
            Dominio.Entidades.Usuario usuarioLogado = this.Usuario;

            ViewBag.NomeUsuario = usuarioLogado.Nome;
            return View();
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarAndamentoPedido()
        {

            Dominio.Entidades.Usuario usuarioLogado = this.Usuario;
            if (usuarioLogado != null)
                ViewBag.NomeUsuario = usuarioLogado.Nome;


            DefineParametrosViewConsultaAndamento();
            return View();

        }

        private void DefineParametrosViewConsultaAndamento()
        {
            string token = Request.GetStringParam("token");
            string codUserSap = Request.GetStringParam("codUserSap");
            string numOv = Request.GetStringParam("numOv");

            ViewBag.codUserSap = codUserSap;
            ViewBag.token = token;
            ViewBag.numOv = numOv;
        }

        [CustomAuthorize("Pedidos/RegraPlanejamentoFrota")]
        public async Task<IActionResult> RegraPlanejamentoFrota()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/NotificacaoRetiradaProduto")]
        public async Task<IActionResult> NotificacaoRetiradaProduto()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/AuditoriaEMP")]
        public async Task<IActionResult> AuditoriaEMP()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pedidos/AuditoriaEMP");
            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);

            return View();
        }

        [CustomAuthorize("Pedidos/LiberarPedidos")]
        public async Task<IActionResult> LiberarPedidos()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/SituacaoComercialPedido")]
        public async Task<IActionResult> SituacaoComercialPedido()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/LoteLiberacaoComercialPedido")]
        public async Task<IActionResult> LoteLiberacaoComercialPedido()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/MotivoPedido")]
        public async Task<IActionResult> MotivoPedido()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/RolagemContainer")]
        public async Task<IActionResult> RolagemContainer()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/SituacaoEstoquePedido")]
        public async Task<IActionResult> SituacaoEstoquePedido()
        {
            return View();
        }

        [CustomAuthorize("Pedidos/MotivoCancelamentoPedido")]
        public ActionResult MotivoCancelamentoPedido()
        {
            return View();
        }
    }
}
