using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas
{
    public class CargasController : BaseController
    {
        #region Construtores

        public CargasController(Conexao conexao) : base(conexao) { }

        #endregion

        [CustomAuthorize("Cargas/Carga", "Logistica/JanelaCarregamento", "Logistica/Monitoramento")]
        public async Task<IActionResult> Carga(CancellationToken cancellationToken)
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.WMS.MontagemContainer repositorioMontagemContainer = new Repositorio.Embarcador.WMS.MontagemContainer(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro repConfiguracaoContratoFreteTerceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPaginacaoInterfaces repositorioConfiguracaoPaginacaoInterfaces = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPaginacaoInterfaces(unitOfWork);
                Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork, cancellationToken);

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga");
                Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = await ObterOperadorLogisticaAsync(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = await repCarga.BuscarPrimeiroRegistroAsync();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = await repConfiguracaoGeral.BuscarPrimeiroRegistroAsync();
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = await repositorioConfiguracao.BuscarConfiguracaoPadraoAsync();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro configuracaoContratoFreteTerceiro = await repConfiguracaoContratoFreteTerceiro.BuscarPrimeiroRegistroAsync();
                Dominio.Entidades.Usuario usuario = null;
                string configuracaoPaginacaoDataLimite = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? repositorioConfiguracaoPaginacaoInterfaces.BuscarPorDataLimiteInterface(Dominio.ObjetosDeValor.Embarcador.Enumeradores.ConfiguracaoPaginacaoInterfaces.Cargas_Carga) : string.Empty;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    usuario = await repositorioUsuario.BuscarPorCodigoAsync(this.Usuario?.Codigo ?? 0);

                //#if DEBUG
                //            permissoesPersonalizadas.Add(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_LiberarEmissaoSemNF);
                //#endif

                var retorno = new
                {
                    PermiteSelecionarHorarioEncaixe = operadorLogistica?.PermiteSelecionarHorarioEncaixe ?? false,
                    PossuiMontagemContainer = await repositorioMontagemContainer.BuscarSeExisteCadastradoAsync(),
                    PossuiTipoOperacaoConsolidacao = await repTipoOperacao.ExisteTipoOperacaoConsolidacaoAsync(cancellationToken),
                    PossuiIntegracaoTelerisco = await repTipoIntegracao.ExistePorTipoAsync(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Telerisco),
                    PossuiIntegracaoBRKVeiculoEMotorista = await repTipoIntegracao.ExistePorTipoAsync(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRiskGestao),
                    DesabilitarUtilizacaoCreditoOperadores = configuracaoGeralCarga?.DesabilitarUtilizacaoCreditoOperadores ?? false,
                    PossuiTipoOperacaoUtilizarPlanoViagem = await repTipoOperacao.ExisteTipoDeOperacaoUtilizarPlanoViagemAsync(),
                    PermitirImpressaoDAMDFEContingencia = configuracaoGeral?.PermitirImpressaoDAMDFEContingencia ?? false,
                    HabilitarFuncionalidadesProjetoGollum = configuracaoGeral?.HabilitarFuncionalidadesProjetoGollum ?? false,
                    NaoPermitirReenviarIntegracaoDasCargasAppTrizy = usuario?.Empresa?.NaoPermitirReenviarIntegracaoDasCargasAppTrizy ?? false,
                    ExisteIntegracaoLoggi = await repTipoIntegracao.ExistePorTipoAsync(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Loggi),
                    PermitirInformarPercentual100AdiantamentoCarga = configuracaoContratoFreteTerceiro?.PermitirInformarPercentual100AdiantamentoCarga ?? false,
                    PermitirDesvincularGerarCopiaCTeRejeitadoCarga = configuracaoGeralCarga?.PermitirDesvincularGerarCopiaCTeRejeitadoCarga ?? false,
                    PermitirSalvarApenasTransportadorEtapaUmCarga = configuracaoGeralCarga?.PermitirSalvarApenasTransportadorEtapaUmCarga ?? false,
                    ExigirConfirmacaoEtapaFreteNoFluxoNotaAposFrete = configuracaoGeralCarga?.ExigirConfirmacaoEtapaFreteNoFluxoNotaAposFrete ?? false,
                    InformarDocaNaEtapaUmDaCarga = configuracaoGeralCarga?.InformarDocaNaEtapaUmDaCarga ?? false,
                    HabilitarFuncionalidadeProjetoNFTP = configuracaoTMS?.HabilitarFuncionalidadeProjetoNFTP ?? false,
                    ConfiguracaoPaginacaoDataLimite = configuracaoPaginacaoDataLimite ?? string.Empty,
                    PermitirInformarValorFreteOperadorMesmoComFreteConfirmadoPeloTransportador = configuracaoGeralCarga?.PermitirInformarValorFreteOperadorMesmoComFreteConfirmadoPeloTransportador ?? false,
                };

                ViewBag.ConfiguracoesCarga = Newtonsoft.Json.JsonConvert.SerializeObject(retorno);
                ViewBag.PermissoesPersonalizadasCarga = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
                ViewBag.CodigoCarga = Request.GetIntParam("CodigoCarga");

                return View();
            }
        }

        [CustomAuthorize("Cargas/CargaAgrupada")]
        public async Task<IActionResult> CargaAgrupada()
        {
            return View();
        }

        [CustomAuthorize("Cargas/Isca")]
        public async Task<IActionResult> Isca()
        {
            return View();
        }

        [CustomAuthorize("Cargas/MotivoAvaliacao")]
        public async Task<IActionResult> MotivoAvaliacao()
        {
            return View();
        }

        [CustomAuthorize("Cargas/CargaCTeManual")]
        public async Task<IActionResult> CargaCTeManual()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/CargaCTeManual");
            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
            ViewBag.ConfiguracoesCancelamentoCteManual = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                ReplaceMotivoRegexPattern = Servicos.CTe.ReplaceMotivoRegexPatternFront
            });


            return View();
        }

        [CustomAuthorize("Cargas/CargaGestao")]
        public async Task<IActionResult> CargaGestao()
        {
            return View();
        }

        [CustomAuthorize("Cargas/EncerramentoCarga")]
        public async Task<IActionResult> EncerramentoCarga()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasEncerramento = ObterPermissoesPersonalizadas("Cargas/EncerramentoCarga");
            ViewBag.PermissoesPersonalizadasEncerramentoCarga = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasEncerramento);
            return View();
        }

        [CustomAuthorize("Cargas/LeilaoTipoOperacaoConfiguracao")]
        public async Task<IActionResult> LeilaoTipoOperacaoConfiguracao()
        {
            return View();
        }

        [CustomAuthorize("Cargas/ModeloVeicularCarga")]
        public async Task<IActionResult> ModeloVeicularCarga()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPaletes repositorioConfiguracaoPaletes = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPaletes(unitOfWork);
                Repositorio.Embarcador.GestaoPatio.CheckListOpcoes repositorioCheckListOpcoes = new Repositorio.Embarcador.GestaoPatio.CheckListOpcoes(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo repConfiguracaoVeiculo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVeiculo configuracaoVeiculo = repConfiguracaoVeiculo.BuscarConfiguracaoPadrao();

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPaletes configuracaoPaletes = repositorioConfiguracaoPaletes.BuscarConfiguracaoPadrao();
                bool ExibirOpcaoNaoSolicitarNoChecklist = repositorioCheckListOpcoes.ExistePerguntaSolicitaModeloVeicularCarga();
                bool ExibirTempoEmissaoFluxoPatio = repTipoOperacao.ExisteTipoOperacaoPermitePacoteViaEntregaPacote();

                var configuracaoModeloVeicularCarga = new
                {
                    ExibirOpcaoNaoSolicitarNoChecklist,
                    configuracaoPaletes.UtilizarControlePaletesPorModeloVeicular,
                    ExibirTempoEmissaoFluxoPatio,
                    ExibirAbaDeEixosNoModeloVeicular = configuracaoVeiculo?.ExibirAbaDeEixosNoModeloVeicular ?? false
                };

                ViewBag.ConfiguracaoModeloVeicularCarga = Newtonsoft.Json.JsonConvert.SerializeObject(configuracaoModeloVeicularCarga);

                return View();
            }
        }

        [CustomAuthorize("Cargas/TipoCarga")]
        public async Task<IActionResult> TipoCarga()
        {
            return View();
        }

        [CustomAuthorize("Cargas/TipoSeparacao")]
        public async Task<IActionResult> TipoSeparacao()
        {
            return View();
        }

        [CustomAuthorize("Cargas/ConsultaCanhoto")]
        public async Task<IActionResult> ConsultaCanhoto()
        {
            return View();
        }

        [CustomAuthorize("Cargas/EnviarCanhoto")]
        public async Task<IActionResult> EnviarCanhoto()
        {
            return View();
        }

        [CustomAuthorize("Cargas/TipoDocumentoTransporte")]
        public async Task<IActionResult> TipoDocumentoTransporte()
        {
            return View();
        }

        [CustomAuthorize("Cargas/MontagemCarga")]
        public async Task<IActionResult> MontagemCarga()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga repositorioConfiguracaoMontagemCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga(unitOfWork);
                Repositorio.Embarcador.Transportadores.GrupoTransportador repositorioGrupoTransportador = new Repositorio.Embarcador.Transportadores.GrupoTransportador(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repConfigCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento", "Cargas/MontagemCarga");
                ViewBag.PermissoesPersonalizadasCarga = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMontagemCarga configuracaoMontagemCarga = repositorioConfiguracaoMontagemCarga.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = ObterOperadorLogistica(unitOfWork);
                bool permitirInformarGrupoTransportador = repositorioGrupoTransportador.ExisteRegistroCadastrado();

                var retorno = new
                {
                    ConfiguracaoEmbarcador.DesativarMultiplosMotoristasMontagemCarga,
                    ConfiguracaoEmbarcador.ExibirTipoDeCargaNaAbaCarregamentoNaMontagemCarga,
                    ConfiguracaoEmbarcador.FronteiraObrigatoriaMontagemCarga,
                    ConfiguracaoEmbarcador.TipoCargaObrigatorioMontagemCarga,
                    ConfiguracaoEmbarcador.TipoOperacaoObrigatorioMontagemCarga,
                    ConfiguracaoEmbarcador.TransportadorObrigatorioMontagemCarga,
                    ConfiguracaoEmbarcador.InformarPeriodoCarregamentoMontagemCarga,
                    TipoServicoMultisoftware = TipoServicoMultisoftware,
                    PermiteSelecionarHorarioEncaixe = operadorLogistica?.PermiteSelecionarHorarioEncaixe ?? false,
                    PermitirInformarGrupoTransportador = permitirInformarGrupoTransportador,
                    configuracaoMontagemCarga.NaoPermitirPedidosTomadoresDiferentesMesmoCarregamento,
                    configuracaoMontagemCarga.ExibirPedidosFormatoGrid,
                    configuracaoMontagemCarga.ExibirListagemNotasFiscais,
                };

                ViewBag.ConfiguracoesMontagemCarga = Newtonsoft.Json.JsonConvert.SerializeObject(retorno);

                return View();
            }
        }

        [CustomAuthorize("Cargas/MontagemCarga", "Cargas/MontagemCargaMapa")]
        public async Task<IActionResult> MontagemCargaMapa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga repositorioConfiguracaoMontagemCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga(unitOfWork);
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento", "Cargas/MontagemCarga");
                ViewBag.PermissoesPersonalizadasCarga = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
                Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = ObterOperadorLogistica(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMontagemCarga configuracaoMontagemCarga = repositorioConfiguracaoMontagemCarga.BuscarPrimeiroRegistro();

                var retorno = new
                {
                    ConfiguracaoEmbarcador.DesativarMultiplosMotoristasMontagemCarga,
                    ConfiguracaoEmbarcador.ExibirTipoDeCargaNaAbaCarregamentoNaMontagemCarga,
                    ConfiguracaoEmbarcador.FronteiraObrigatoriaMontagemCarga,
                    ConfiguracaoEmbarcador.TipoCargaObrigatorioMontagemCarga,
                    ConfiguracaoEmbarcador.TipoOperacaoObrigatorioMontagemCarga,
                    ConfiguracaoEmbarcador.TransportadorObrigatorioMontagemCarga,
                    ConfiguracaoEmbarcador.InformarPeriodoCarregamentoMontagemCarga,
                    configuracaoMontagemCarga.UtilizarDataPrevisaoSaidaVeiculo,
                    PermiteSelecionarHorarioEncaixe = operadorLogistica?.PermiteSelecionarHorarioEncaixe ?? false
                };

                ViewBag.ConfiguracoesMontagemCarga = Newtonsoft.Json.JsonConvert.SerializeObject(retorno);

                return View();
            }
        }

        [CustomAuthorize("Cargas/MontagemCargaCarregamentoIntegracao")]
        public async Task<IActionResult> MontagemCargaCarregamentoIntegracao()
        {
            return View();
        }

        [CustomAuthorize("Cargas/EnviarCanhotoAvulso")]
        public async Task<IActionResult> EnviarCanhotoAvulso()
        {
            return View();
        }

        [CustomAuthorize("Cargas/ConsultaCanhotoAvulso")]
        public async Task<IActionResult> ConsultaCanhotoAvulso()
        {
            return View();
        }

        [CustomAuthorize("Cargas/TipoCargaModeloVeicularAutoConfig")]
        public async Task<IActionResult> TipoCargaModeloVeicularAutoConfig()
        {
            return View();
        }

        [CustomAuthorize("Cargas/CTe")]
        public async Task<IActionResult> CTe()
        {
            return View();
        }

        [CustomAuthorize("Cargas/TipoIntegracao")]
        public async Task<IActionResult> TipoIntegracao()
        {
            return View();
        }

        [CustomAuthorize("Cargas/CancelamentoCarga")]
        public async Task<IActionResult> CancelamentoCarga()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/CancelamentoCarga");
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

                ViewBag.PermissoesPersonalizadasCancelamentoCarga = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
                ViewBag.ConfiguracoesCancelamentoCarga = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    RealizarIntegracaoDadosCancelamentoCarga = configuracaoGeralCarga?.RealizarIntegracaoDadosCancelamentoCarga ?? false,
                    CancelarCIOTAutomaticamenteFluxoCancelamentoCarga = configuracaoGeralCarga?.CancelarCIOTAutomaticamenteFluxoCancelamentoCarga ?? false,
                    PermitirReverterAnulacaoGerencialTelaCancelamento = configuracaoGeralCarga?.PermitirReverterAnulacaoGerencialTelaCancelamento ?? false,
                    ReplaceMotivoRegexPattern = Servicos.CTe.ReplaceMotivoRegexPatternFront
                });

                return View();
            }
        }

        [CustomAuthorize("Cargas/JustificativaCancelamentoCarga")]
        public async Task<IActionResult> JustificativaCancelamentoCarga()
        {
            return View();
        }

        [CustomAuthorize("Cargas/ControleGeracaoEDI")]
        public async Task<IActionResult> ControleGeracaoEDI()
        {
            return View();
        }

        [CustomAuthorize("Cargas/Transbordo")]
        public async Task<IActionResult> Transbordo()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repositorioConfiguracaoGeral.BuscarPrimeiroRegistro();

                var configuracaoGeralTransbordo = new
                {
                    configuracaoGeral.PermiteSelecionarPlacaPorTipoVeiculoTransbordo
                };

                ViewBag.ConfiguracaoGeral = Newtonsoft.Json.JsonConvert.SerializeObject(configuracaoGeralTransbordo);
            }

            return View();
        }

        [CustomAuthorize("Cargas/CargaMDFeAquaviarioManual")]
        public async Task<IActionResult> CargaMDFeAquaviarioManual()
        {
            return View();
        }

        [CustomAuthorize("Cargas/CargaMDFeManual")]
        public async Task<IActionResult> CargaMDFeManual()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repConfiguracaoGeral.BuscarConfiguracaoPadrao();

                var configuracoesTransportador = new
                {
                    PermitirAdicionarMotoristaCargaMDFeManual = configuracaoGeral.PermitirAdicionarMotoristaCargaMDFeManual
                };

                ViewBag.ConfiguracoesTransportador = Newtonsoft.Json.JsonConvert.SerializeObject(configuracoesTransportador);
            }

            return View();
        }

        [CustomAuthorize("Cargas/CargaMDFeManualCancelamento")]
        public async Task<IActionResult> CargaMDFeManualCancelamento()
        {
            ViewBag.ConfiguracoesCancelamentoMdfeManual = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                ReplaceMotivoRegexPattern = Servicos.CTe.ReplaceMotivoRegexPatternFront
            });

            return View();
        }

        [CustomAuthorize("Cargas/CargaControleExpedicao")]
        public async Task<IActionResult> CargaControleExpedicao()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/CargaControleExpedicao");
            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
            return View();
        }

        [CustomAuthorize("Cargas/RegrasAgrupamentoPedidos")]
        public async Task<IActionResult> RegrasAgrupamentoPedidos()
        {
            return View();
        }

        [CustomAuthorize("Cargas/FluxoColetaEntrega")]
        public async Task<IActionResult> FluxoColetaEntrega()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga");
            ViewBag.PermissoesPersonalizadasCarga = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
            return View();
        }

        [CustomAuthorize("Cargas/GrupoModeloVeicular")]
        public async Task<IActionResult> GrupoModeloVeicular()
        {
            return View();
        }

        [CustomAuthorize("Cargas/Redespacho")]
        public async Task<IActionResult> Redespacho()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ConfiguracaoEmbarcador;

                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorio = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorio.BuscarPrimeiroRegistro();

                ViewBag.ConfiguracoesRedespacho = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    configuracaoEmbarcador.PermitirInformarDistanciaNoRedespacho,
                    configuracaoGeralCarga.PermiteSelecionarMultiplasCargasParaRedespacho,
                    configuracaoGeralCarga.GerarRedespachoDeCargasAgrupadas,
                    configuracaoGeralCarga.PermitirInformarRecebedorAoCriarUmRedespachoManual,
                });

                return View();
            }
        }

        [CustomAuthorize("Cargas/AcompanhamentoPreAgrupamentoCarga")]
        public async Task<IActionResult> AcompanhamentoPreAgrupamentoCarga()
        {
            return View();
        }

        [CustomAuthorize("Cargas/RegraAutorizacaoCarga")]
        public async Task<IActionResult> RegraAutorizacaoCarga()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repConfiguracaoGeral.BuscarPrimeiroRegistro();

                var retorno = new
                {
                    HabilitarFuncionalidadesProjetoGollum = configuracaoGeral?.HabilitarFuncionalidadesProjetoGollum ?? false,
                };

                ViewBag.ConfiguracoesCarga = Newtonsoft.Json.JsonConvert.SerializeObject(retorno);

                return View();
            }
        }

        [CustomAuthorize("Cargas/AutorizacaoCarga")]
        public async Task<IActionResult> AutorizacaoCarga()
        {
            return View();
        }

        [CustomAuthorize("Cargas/CargaDataCarregamentoImportacao")]
        public async Task<IActionResult> CargaDataCarregamentoImportacao()
        {
            return View();
        }

        [CustomAuthorize("Cargas/ReenvioIntegracaoEDI")]
        public async Task<IActionResult> ReenvioIntegracaoEDI()
        {
            return View();
        }

        [CustomAuthorize("Cargas/MotivoSolicitacaoFrete")]
        public async Task<IActionResult> MotivoSolicitacaoFrete()
        {
            return View();
        }

        [CustomAuthorize("Cargas/FaixaTemperatura")]
        public async Task<IActionResult> FaixaTemperatura()
        {
            return View();
        }

        [CustomAuthorize("Cargas/ResponsavelCarga")]
        public async Task<IActionResult> ResponsavelCarga()
        {
            return View();
        }

        [CustomAuthorize("Cargas/TipoRetornoCarga")]
        public async Task<IActionResult> TipoRetornoCarga()
        {
            return View();
        }

        [CustomAuthorize("Cargas/RetornoCarga")]
        public async Task<IActionResult> RetornoCarga()
        {
            return View();
        }

        [CustomAuthorize("Cargas/ControleEntrega")]
        public async Task<IActionResult> ControleEntrega()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/ControleEntrega");
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasChamado = ObterPermissoesPersonalizadas("Chamados/ChamadoOcorrencia");
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasCarga = ObterPermissoesPersonalizadas("Cargas/Carga");
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasOcorrencia = ObterPermissoesPersonalizadas("Ocorrencias/Ocorrencia");
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasMonitoramento = ObterPermissoesPersonalizadas("Logistica/Monitoramento");

                Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repositorioConfiguracao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracao = repositorioConfiguracao.ObterConfiguracaoPadrao();

                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repConfiguracaoGeral.BuscarConfiguracaoPadrao();

                Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia repConfiguracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = repConfiguracaoOcorrencia.BuscarConfiguracaoPadrao();

                Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);
                Dominio.Entidades.Usuario usuario = new Dominio.Entidades.Usuario();

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    usuario = repositorioUsuario.BuscarPorCodigo(this.Usuario?.Codigo ?? 0);

                bool permissaoDelegarChamado = false;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    permissaoDelegarChamado = UsuarioPossuiPermissao("Chamados/RegrasAnaliseChamados");

                Repositorio.Embarcador.GestaoPatio.CheckListTipo repositorioCheckList = new Repositorio.Embarcador.GestaoPatio.CheckListTipo(unitOfWork);
                bool existeCheckListAtivo = repositorioCheckList.ExisteCheckListAtivo();

                ViewBag.ControleEntregaVisaoPrevisao = ConfiguracaoEmbarcador.ControleEntregaVisaoPrevisao;
                ViewBag.PermissaoDelegar = permissaoDelegarChamado ? "true" : "false";
                ViewBag.PermitirContatoWhatsApp = ConfiguracaoEmbarcador.PermitirContatoWhatsApp ? "true" : "false";
                ViewBag.HabilitarWidgetAtendimento = ConfiguracaoEmbarcador.HabilitarWidgetAtendimento ? "true" : "false";
                ViewBag.HabilitarIconeEntregaAtrasada = ConfiguracaoEmbarcador.HabilitarIconeEntregaAtrasada ? "true" : "false";
                ViewBag.FiltrarWidgetAtendimentoProFiltro = ConfiguracaoEmbarcador.FiltrarWidgetAtendimentoProFiltro ? "true" : "false";
                ViewBag.ExibirPacotesOcorrencia = (configuracao?.ExibirPacotesOcorrenciaControleEntrega ?? false) ? "true" : "false";
                ViewBag.PermitirAbrirAtendimento = (configuracao?.PermitirAbrirAtendimentoViaControleEntrega ?? false) ? "true" : "false";
                ViewBag.NaoPermitirFinalizarViagemDetalhesFimViagem = (configuracaoGeral?.NaoPermitirFinalizarViagemDetalhesFimViagem ?? false) ? "true" : "false";
                ViewBag.NaoPermitirInformarInicioEFimPreTrip = (usuario?.Empresa?.NaoPermitirInformarInicioEFimPreTrip ?? false) ? "true" : "false";
                ViewBag.PermitirEnvioCanhotosPeloPortalTransportadorControleEntregas = (configuracao?.PermitirEnvioCanhotosPeloPortalTransportadorControleEntregas ?? false) ? "true" : "false";
                ViewBag.InduzirTransportadorSelecionarApenasUmComplementoSolicitacaoComplementos = Newtonsoft.Json.JsonConvert.SerializeObject(configuracaoOcorrencia.InduzirTransportadorSelecionarApenasUmComplementoSolicitacaoComplementos);
                ViewBag.ExisteCheckListAtivo = existeCheckListAtivo ? "true" : "false";

                ViewBag.PermissoesPersonalizadasChamado = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasChamado);
                ViewBag.PermissoesPersonalizadasCarga = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasCarga);
                ViewBag.PermissoesPersonalizadasOcorrencia = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasOcorrencia);
                ViewBag.PermissoesPersonalizadasControleEntrega = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
                ViewBag.PermissoesPersonalizadasMonitoramento = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasMonitoramento);

                return View();
            }
        }

        [CustomAuthorize("Cargas/CargaIntegracaoEmbarcador")]
        public async Task<IActionResult> CargaIntegracaoEmbarcador()
        {
            return View();
        }

        [CustomAuthorize("Cargas/RetornoCargaColetaBackhaul")]
        public async Task<IActionResult> RetornoCargaColetaBackhaul()
        {
            return View();
        }

        [CustomAuthorize("Cargas/MotivoCancelamentoRetornoCargaColetaBackhaul")]
        public async Task<IActionResult> MotivoCancelamentoRetornoCargaColetaBackhaul()
        {
            return View();
        }

        [CustomAuthorize("Cargas/GeracaoCargaEmbarcador")]
        public async Task<IActionResult> GeracaoCargaEmbarcador()
        {
            return View();
        }

        [CustomAuthorize("Cargas/CancelamentoCargaLote")]
        public async Task<IActionResult> CancelamentoCargaLote()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/CancelamentoCarga");
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasCancelamentoCargaLote = ObterPermissoesPersonalizadas("Cargas/CancelamentoCargaLote");
            ViewBag.PermissoesPersonalizadasCancelamentoCarga = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
            ViewBag.PermissoesPersonalizadasCancelamentoCargaLote = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasCancelamentoCargaLote);
            return View();
        }

        [CustomAuthorize("Cargas/ImpressaoLoteCarga")]
        public async Task<IActionResult> ImpressaoLoteCarga()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/ImpressaoLoteCarga");
            ViewBag.PermissoesPersonalizadasImpressaoLoteCarga = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
            return View();
        }

        [CustomAuthorize("Cargas/ResponsavelEntrega")]
        public async Task<IActionResult> ResponsavelEntrega()
        {
            return View();
        }

        [CustomAuthorize("Cargas/CargaAprovacaoFreteDetalheAnexo")]
        public async Task<IActionResult> CargaAprovacaoFreteDetalheAnexo()
        {
            return View();
        }

        [CustomAuthorize("Cargas/MotivoRejeicaoColeta")]
        public async Task<IActionResult> MotivoRejeicaoColeta()
        {
            return View();
        }

        [CustomAuthorize("Cargas/MotivoFalhaNotaFiscal")]
        public async Task<IActionResult> MotivoFalhaNotaFiscal()
        {
            return View();
        }

        [CustomAuthorize("Cargas/MotivoRetificacaoColeta")]
        public async Task<IActionResult> MotivoRetificacaoColeta()
        {
            return View();
        }

        [CustomAuthorize("Cargas/JustificativaTemperatura")]
        public async Task<IActionResult> JustificativaTemperatura()
        {
            return View();
        }

        [CustomAuthorize("Cargas/ConfiguracaoOcorrenciaEntrega")]
        public async Task<IActionResult> ConfiguracaoOcorrenciaEntrega()
        {
            return View();
        }

        [CustomAuthorize("Cargas/CargaCTeAgrupado")]
        public async Task<IActionResult> CargaCTeAgrupado()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/CargaCTeAgrupado");

            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);

            return View();
        }

        [CustomAuthorize("Cargas/AgendamentoEntrega")]
        public async Task<IActionResult> AgendamentoEntrega()
        {
            return View();
        }

        [CustomAuthorize("Cargas/RegraAutorizacaoCargaCancelamento")]
        public async Task<IActionResult> RegraAutorizacaoCargaCancelamento()
        {
            return View();
        }

        [CustomAuthorize("Cargas/AutorizacaoCargaCancelamento")]
        public async Task<IActionResult> AutorizacaoCargaCancelamento()
        {
            return View();
        }

        [CustomAuthorize("Cargas/ViaTransporte")]
        public async Task<IActionResult> ViaTransporte()
        {
            return View();
        }

        [CustomAuthorize("Cargas/PedidoCancelamentoReservaIntegracao")]
        public async Task<IActionResult> PedidoCancelamentoReservaIntegracao()
        {
            return View();
        }

        [CustomAuthorize("Cargas/PedidoProdutosCarregamentos")]
        public async Task<IActionResult> PedidoProdutosCarregamentos()
        {
            return View();
        }

        [CustomAuthorize("Cargas/ControleNotaDevolucao")]
        public async Task<IActionResult> ControleNotaDevolucao()
        {
            //Permissões referente ao chamado
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasChamado = ObterPermissoesPersonalizadas("Chamados/ChamadoOcorrencia");
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasCarga = ObterPermissoesPersonalizadas("Cargas/Carga");
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasOcorrencia = ObterPermissoesPersonalizadas("Ocorrencias/Ocorrencia");

            bool permissaoDelegar = false;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                permissaoDelegar = UsuarioPossuiPermissao("Chamados/RegrasAnaliseChamados");

            ViewBag.PermissaoDelegar = permissaoDelegar ? "true" : "false";
            ViewBag.PermissoesPersonalizadasChamado = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasChamado);
            ViewBag.PermissoesPersonalizadasCarga = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasCarga);
            ViewBag.PermissoesPersonalizadasOcorrencia = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasOcorrencia);
            //

            return View();
        }

        [CustomAuthorize("Cargas/ManutencaoEntregaCarga")]
        public async Task<IActionResult> ManutencaoEntregaCarga()
        {
            return View();
        }

        [CustomAuthorize("Cargas/TipoLacre")]
        public async Task<IActionResult> TipoLacre()
        {
            return View();
        }

        [CustomAuthorize("Cargas/RegraAutorizacaoCarregamento")]
        public async Task<IActionResult> RegraAutorizacaoCarregamento()
        {
            return View();
        }

        [CustomAuthorize("Cargas/AutorizacaoCarregamento")]
        public async Task<IActionResult> AutorizacaoCarregamento()
        {
            return View();
        }

        [CustomAuthorize("Cargas/CargaTrajeto")]
        public async Task<IActionResult> CargaTrajeto()
        {
            return View();
        }

        [CustomAuthorize("Cargas/OrdemEmbarqueIntegracao")]
        public async Task<IActionResult> OrdemEmbarqueIntegracao()
        {
            return View();
        }

        [CustomAuthorize("Cargas/MonitoramentoLogisticoIntegracao")]
        public async Task<IActionResult> MonitoramentoLogisticoIntegracao()
        {
            return View();
        }

        [CustomAuthorize("Cargas/ValePedagio")]
        public async Task<IActionResult> ValePedagio()
        {
            return View();
        }

        [CustomAuthorize("Cargas/CargaConsultaValorPedagioIntegracao")]
        public async Task<IActionResult> CargaConsultaValorPedagioIntegracao()
        {
            return View();
        }

        [CustomAuthorize("Cargas/AcompanhamentoEntrega")]
        public async Task<IActionResult> AcompanhamentoEntrega()
        {
            return View();
        }

        [CustomAuthorize("Cargas/LoteComprovanteEntrega")]
        public async Task<IActionResult> LoteComprovanteEntrega()
        {
            return View();
        }

        [CustomAuthorize("Cargas/AcompanhamentoEntregaConfiguracao")]
        public async Task<IActionResult> AcompanhamentoEntregaConfiguracao()
        {
            return View();
        }

        [CustomAuthorize("Cargas/DownloadDocumentos")]
        public async Task<IActionResult> DownloadDocumentos()
        {
            return View();
        }

        [CustomAuthorize("Cargas/MontagemFeeder")]
        public async Task<IActionResult> MontagemFeeder()
        {
            return View();
        }

        [CustomAuthorize("Cargas/CargaEntregaIntegracao")]
        public async Task<IActionResult> CargaEntregaIntegracao()
        {
            return View();
        }

        [CustomAuthorize("Cargas/CargaExportacaoIntegracao")]
        public async Task<IActionResult> CargaExportacaoIntegracao()
        {
            return View();
        }

        [CustomAuthorize("Cargas/TratativaOcorrenciaEntrega")]
        public async Task<IActionResult> TratativaOcorrenciaEntrega()
        {
            return View();
        }

        [CustomAuthorize("Cargas/ConfiguracaoAlertaCarga")]
        public async Task<IActionResult> ConfiguracaoAlertaCarga()
        {
            return View();
        }

        [CustomAuthorize("Cargas/TipoResponsavelAtrasoEntrega")]
        public async Task<IActionResult> TipoResponsavelAtrasoEntrega()
        {
            return View();
        }

        [CustomAuthorize("Cargas/CargaFretePendente")]
        public async Task<IActionResult> CargaFretePendente()
        {
            return View();
        }

        [CustomAuthorize("Cargas/CargaFechamento")]
        public async Task<IActionResult> CargaFechamento()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repositorioConfiguracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = repositorioConfiguracaoJanelaCarregamento.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = ObterOperadorLogistica(unitOfWork);
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");

                ViewBag.PermissoesPersonalizadasCarga = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
                ViewBag.ConfiguracoesJanelaCarregamento = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    ExibirOpcaoLiberarParaTransportador = configuracaoJanelaCarregamento?.ExibirOpcaoLiberarParaTransportador ?? false,
                    PermiteSelecionarHorarioEncaixe = operadorLogistica?.PermiteSelecionarHorarioEncaixe ?? false
                });

                ViewBag.PermissoesPersonalizadasControleEntrega = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);

                return View();
            }
        }

        [CustomAuthorize("Cargas/LoteIntegracaoCarregamento")]
        public async Task<IActionResult> LoteIntegracaoCarregamento()
        {
            return View();
        }

        [CustomAuthorize("Cargas/ConfiguracaoProgramacaoCarga")]
        public async Task<IActionResult> ConfiguracaoProgramacaoCarga()
        {
            return View();
        }

        [CustomAuthorize("Cargas/ProgramacaoCarga")]
        public async Task<IActionResult> ProgramacaoCarga()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFilaCarregamento repositorioConfiguracaoFilaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFilaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFilaCarregamento configuracaoFilaCarregamento = repositorioConfiguracaoFilaCarregamento.BuscarPrimeiroRegistro();

                ViewBag.ConfiguracoesProgramacaoCarga = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    configuracaoFilaCarregamento.DiasFiltrarDataProgramada
                });

                return View();
            }
        }

        [CustomAuthorize("Cargas/AvaliacaoEntrega")]
        public async Task<IActionResult> AvaliacaoEntrega()
        {
            return View();
        }

        [CustomAuthorize("Cargas/TipoPercurso")]
        public async Task<IActionResult> TipoPercurso()
        {
            return View();
        }

        [CustomAuthorize("Cargas/TipoAnexo")]
        public async Task<IActionResult> TipoAnexo()
        {
            return View();
        }

        [CustomAuthorize("Cargas/LiberacaoCargaEmMassa")]
        public async Task<IActionResult> LiberacaoCargaEmMassa()
        {
            return View();
        }

        [CustomAuthorize("Cargas/ExtratoValePedagio")]
        public async Task<IActionResult> ExtratoValePedagio()
        {
            return View();
        }

        [CustomAuthorize("Cargas/AlterarCentroResultado")]
        public async Task<IActionResult> AlterarCentroResultado()
        {
            return View();
        }

        [CustomAuthorize("Cargas/CargaRelacionada")]
        public async Task<IActionResult> CargaRelacionada()
        {
            return View();
        }

        [CustomAuthorize("Cargas/CargaEntregaEventoIntegracao")]
        public async Task<IActionResult> CargaEntregaEventoIntegracao()
        {
            return View();
        }

        [CustomAuthorize("Cargas/RegraAutorizacaoLeilao")]
        public async Task<IActionResult> RegraAutorizacaoLeilao()
        {
            return View();
        }

        [CustomAuthorize("Cargas/AutorizacaoLeilao")]
        public async Task<IActionResult> AutorizacaoLeilao()
        {
            return View();
        }

        [CustomAuthorize("Cargas/TipoCarregamento")]
        public async Task<IActionResult> TipoCarregamento()
        {
            return View();
        }

        [CustomAuthorize("Cargas/TipoComprovante")]
        public async Task<IActionResult> TipoComprovante()
        {
            return View();
        }

        [CustomAuthorize("Cargas/ComprovanteCarga")]
        public async Task<IActionResult> ComprovanteCarga()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/ComprovanteCarga");

            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
            return View();
        }

        [CustomAuthorize("Cargas/TipoTrecho")]
        public async Task<IActionResult> TipoTrecho()
        {
            return View();
        }

        [CustomAuthorize("Cargas/FluxoEncerramentoCarga")]
        public async Task<IActionResult> FluxoEncerramentoCarga()
        {
            return View();
        }

        [CustomAuthorize("Cargas/OcultarInformacoesCarga")]
        public async Task<IActionResult> OcultarInformacoesCarga()
        {
            return View();
        }

        [CustomAuthorize("Cargas/CargaIntegracaoEvento")]
        public async Task<IActionResult> CargaIntegracaoEvento()
        {
            return View();
        }

        [CustomAuthorize("Cargas/IntegracaoNFe")]
        public async Task<IActionResult> IntegracaoNFe()
        {

            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork).BuscarConfiguracaoPadrao();

                ViewBag.ConfiguracaoGeral = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    configuracaoGeral.ProcessarXMLNotasFiscaisAssincrono
                });

                return View();
            }

        }

        [AllowAuthenticate]
        [CustomAuthorize("Cargas/Carga", "Logistica/JanelaCarregamento", "Logistica/Monitoramento")]
        public async Task<IActionResult> RenderizarDANFE()
        {
            ViewBag.Codigo = Request.GetIntParam("Codigo");
            ViewBag.CargaPedido = Request.GetIntParam("CargaPedido");

            return View();
        }

        [CustomAuthorize("Cargas/AlertasTransportador")]
        public async Task<IActionResult> AlertasTransportador()
        {
            return View();
        }

        [CustomAuthorize("Cargas/ContingenciaCargaEmissao")]
        public async Task<IActionResult> ContingenciaCargaEmissao()
        {
            return View();
        }

        [CustomAuthorize("Cargas/GestaoDadosColeta")]
        public async Task<IActionResult> GestaoDadosColeta()
        {
            return View();
        }
        [CustomAuthorize("Cargas/ServicoInspecaoFederal")]
        public async Task<IActionResult> ServicoInspecaoFederal()
        {
            return View();
        }

        [CustomAuthorize("Cargas/GestaoPedido")]
        public async Task<IActionResult> GestaoPedido()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoRouteasy repositorioIntegracaoRouteasy = new Repositorio.Embarcador.Configuracoes.IntegracaoRouteasy(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRouteasy integracaoRouteasy = repositorioIntegracaoRouteasy.BuscarPrimeiroRegistro();

                ViewBag.ConfiguracoesGestaoPedido = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    PossuiIntegracaoRoutEasy = integracaoRouteasy?.PossuiIntegracao ?? false
                });

                return View();
            }
        }

        [CustomAuthorize("Cargas/JustificativaAutorizacaoCarga")]
        public async Task<IActionResult> JustificativaAutorizacaoCarga()
        {
            return View();
        }

        [CustomAuthorize("Cargas/ConfirmarDocumentoEmLote")]
        public async Task<IActionResult> ConfirmarDocumentoEmLote()
        {
            return View();
        }

        [CustomAuthorize("Cargas/GestaoDevolucao")]
        public ActionResult GestaoDevolucao()
        {

            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasCarga = ObterPermissoesPersonalizadas("Cargas/Carga");
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasOcorrencia = ObterPermissoesPersonalizadas("Ocorrencias/Ocorrencia");
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasControleEntrega = ObterPermissoesPersonalizadas("Cargas/ControleEntrega");

            bool permissaoDelegarChamado = false;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                permissaoDelegarChamado = UsuarioPossuiPermissao("Chamados/RegrasAnaliseChamados");

            ViewBag.PermissaoDelegar = permissaoDelegarChamado ? "true" : "false";
            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasOcorrencia);
            ViewBag.PermissoesPersonalizadasControleEntrega = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasControleEntrega);
            ViewBag.PermissoesPersonalizadasCarga = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasCarga);

            return View();
        }

        [CustomAuthorize("Cargas/Categoria")]
        public ActionResult Categoria()
        {
            return View();
        }

        [CustomAuthorize("Cargas/ParametrosOfertas")]
        public async Task<IActionResult> ParametrosOfertas()
        {
            return View();
        }

        [CustomAuthorize("Cargas/CargaOferta")]
        public ActionResult CargaOferta()
        {
            return View();
        }

        [CustomAuthorize("Cargas/IntegracoesComFalha")]
        public ActionResult IntegracoesComFalha()
        {
            return View();
        }
    }
}
