var EnumPermissaoPersonalizada = {
    Acerto_PermiteLiberarAutorizarAbastecimento: 1,
    Acerto_PermiteLiberarAutorizarArla: 2,
    Acerto_PermiteLiberarPedagioAcerto: 3,
    Acerto_PermiteAdicionarBonificacao: 4,
    Acerto_PermiteAdicionarDesconto: 5,
    Acerto_PermiteFecharAcerto: 6,
    Acerto_PermiteReabirAcerto: 7,
    Acerto_PermiteInformarCargaFracionada: 8,
    Acerto_PermiteInformarBonificacaoCliente: 9,
    Acerto_PermiteAlterarReboquesCarga: 10,
    Acerto_PermiteAjustarKMTotal: 11,
    Fatura_PermiteIniciarFatura: 12,
    Fatura_PermiteCancelarCarga: 13,
    Fatura_PermiteAdicionarNovasCargas: 14,
    Fatura_PermiteSalvarCarga: 15,
    Fatura_PermiteRemoverConhecimento: 16,
    Fatura_PermiteSalvarValoresFechamento: 17,
    Fatura_PermiteGerarParcelas: 18,
    Fatura_PermiteFecharFatura: 19,
    Fatura_PermiteVisualizarFatura: 20,
    Fatura_PermiteReAbrirFatura: 21,
    Fatura_PermiteEnviarEDI: 22,
    Fatura_PermiteEnviarFatura: 23,
    BaixaReceber_PermiteBaixarTitulo: 24,
    BaixaReceber_PermiteSalvarValores: 25,
    BaixaReceber_PermiteGerarParcelas: 26,
    BaixaReceber_PermiteFecharBaixa: 27,
    BaixaPagar_PermiteBaixarTitulo: 28,
    BaixaPagar_PermiteSalvarValores: 29,
    BaixaPagar_PermiteGerarParcelas: 30,
    BaixaPagar_PermiteFecharBaixa: 31,
    Fatura_PermiteLiquidarFatura: 32,
    Criar: 33,//Usar o enum como padrão sempre que a permissão for para Criar
    Alterar: 34,//Usar o enum como padrão sempre que a permissão for para Alterar
    Excluir: 35,//Usar o enum como padrão sempre que a permissão for para Excluir
    Cancelar: 36,//Usar o enum como padrão sempre que a permissão for para Cancelar
    ReAbrir: 37,//Usar o enum como padrão sempre que a permissão for para ReAbrir
    Finalizar: 38,//Usar o enum como padrão sempre que a permissão for para Finalizar
    ComissaoFuncionario_PermiteEditarDadosGerados: 39,
    ContratoFrete_PermiteAutorizarContrato: 40,
    ContratoFrete_PermiteBloquearContrato: 41,
    Carga_SalvarDadosTransporte: 42,
    Carga_InformarDocumentosFiscais: 43,
    Carga_AlterarValorFrete: 44,
    Carga_AutorizarFreteInconsistente: 45,
    Carga_AdicionarComponentes: 46,
    Carga_AlterarFreteTerceiros: 47,
    Carga_AlterarConfiguracao: 48,
    Carga_AlterarObservacao: 49,
    Carga_AlterarDadosPedido: 50,
    Carga_AlterarDadosSeguro: 51,
    Carga_AutorizarSeguro: 52,
    Carga_AlterarRota: 53,
    Carga_AlterarLacres: 54,
    Carga_AlterarPassagens: 55,
    Carga_AlterarPercurso: 56,
    Carga_AutorizarEmissaoDocumentos: 57,
    Carga_RetornarEtapaNotasFiscais: 58,
    Carga_EditarCTe: 59,
    Carga_ConfirmarIntegracao: 60,
    Carga_ConfirmarImpressao: 61,
    Carga_AutorizarModalidadePagamentoNota: 62,
    DocumentoEntrada_LancarDuplicata: 63,
    MovimentoFinanceiro_InformarPlanoEntradaSaida: 64,
    MovimentoFinanceiro_InformarCentroResultado: 65,
    MovimentoFinanceiro_InformarColaborador: 66,
    Carga_ReenviarIntegracoes: 67,
    Carga_FinalizarCargaMDFeRejeitado: 68,
    CargaCancelamento_Anular: 69,
    CargaCancelamento_ReenviarCancelamentoComoAnulacao: 70,
    CargaCancelamento_AdicionarComoCancelamento: 71,
    MontagemCarga_LiberacaoVeiculo: 72,
    OcorrenciaCancelamento_Anular: 73,
    OcorrenciaCancelamento_ReenviarCancelamentoComoAnulacao: 74,
    OcorrenciaCancelamento_AdicionarComoCancelamento: 75,
    FluxoGestaoPatio_InformarDoca: 89,
    FluxoGestaoPatio_InformarGuaritaEntrada: 90,
    FluxoGestaoPatio_InformarChecklist: 91,
    FluxoGestaoPatio_TravarChave: 92,
    FluxoGestaoPatio_InformarExpedicao: 93,
    FluxoGestaoPatio_LiberarChave: 94,
    FluxoGestaoPatio_InformarInicioViagem: 95,
    WMS_Autorizar_Volumes_Faltantes: 96,
    FluxoGestaoPatio_InformarChegadaVeiculo: 102,
    FluxoGestaoPatio_InformarDeslocamentoPatio: 103,
    FluxoGestaoPatio_PermiteRetornarEtapaSaidaCD: 104,
    FechamentoFrete_Finalizar: 99,
    FechamentoFrete_Cancelar: 100,
    FechamentoFrete_Reabrir: 101,
    Carga_LiberarAverbacaoRejeitada: 105,
    PreCarga_Supervisor: 106,
    Acerto_OcultarResultadosViagem: 107,
    CargaCancelamento_LiberarAverbacaoRejeitada: 108,
    Carga_PermiteAlterarTransportador: 109,
    Carga_PermiteLiberarComDiferencaNoValorFrete: 110,
    Pallets_PermiteDataRetroativa_ValePallet: 111,
    Pallets_PermiteDataRetroativa_DevolucaoPallet: 112,
    Canhotos_ReverterJustificativa_Canhotos: 113,
    Carga_AutorizarPesoCarga: 114,
    Carga_AutorizarSemArquivoOrtec: 115,
    Pagamento_AutorizarPagmentoComCargaCancelada: 116,
    Carga_LiberarEmissaoSemNF: 117,
    Carga_AutorizarManutencaoPendenteVeiculo: 118,
    Carga_DownloadArquivoIntegracoes: 119,
    Carga_AutorizarValorMaximoPendentePagamento: 120,
    DocumentoEntrada_AutorizarPrecoCombustivelDiferenteFornecedor: 121,
    VendaDireta_PermitirFinalizar: 122,
    VendaDireta_PermitirAlterarParcelamento: 123,
    Titulo_GerarMultiplosTitulos: 124,
    Usuario_PermiteRemoverAnexos: 125,
    Veiculo_PermiteRemoverAnexos: 126,
    Motorista_PermiteRemoverAnexos: 127,
    Carga_PermiteAdicionarOutrosDocumentos: 128,
    Veiculo_PermiteAlterarPlaca: 129,
    CTeManual_PermiteCancelarCTe: 130,
    Carga_PermiteAlterarInclusaoICMS: 131,
    FilaCarregamento_PermitirAceitarCarga: 132,
    FilaCarregamento_PermitirAlocarCarga: 133,
    FilaCarregamento_PermitirAlterarCentroCarregamento: 134,
    FilaCarregamento_PermitirAlterarPrimeiraPosicao: 135,
    FilaCarregamento_PermitirAlterarUltimaPosicao: 136,
    FilaCarregamento_PermitirConfirmarChegada: 137,
    FilaCarregamento_PermitirEnviarNotificacao: 138,
    FilaCarregamento_PermitirInformarConjuntoMotorista: 139,
    FilaCarregamento_PermitirInformarConjuntoVeiculo: 140,
    FilaCarregamento_PermitirLiberar: 141,
    FilaCarregamento_PermitirLiberarSaidaFila: 142,
    FilaCarregamento_PermitirRecusarCarga: 143,
    FilaCarregamento_PermitirRemover: 144,
    FilaCarregamento_PermitirRemoverConjuntoMotorista: 145,
    FilaCarregamento_PermitirRemoverReversa: 146,
    FilaCarregamento_PermitirRemoverTracao: 147,
    FilaCarregamento_PermitirReposicionar: 148,
    Carga_PermitirRemoverPedido: 149,
    Canhoto_PermitirReverterImagem: 150,
    FilaCarregamento_PermitirDesatrelarTracao: 151,
    FilaCarregamento_PermitirAdicionar: 152,
    CargaCancelamento_CancelarCargaBloqueada: 153,
    ContratoFrete_PermiteInformarValorFrete: 154,
    ContratoFrete_PermiteInformarValorPedagio: 155,
    ContratoFrete_PermiteInformarAcrescimoDesconto: 156,
    AcordoFaturamento_PermiteAlterarApenasEmail: 157,
    GrupoPessoas_PermiteAlterarApenasObservacoes: 158,
    Carga_PermitirAvancarCargaSemTodosPreCte: 159,
    Ocorrencia_PermitirRetornarEtapaCadastro: 160,
    CTeManual_PermiteAnulacaoGerencialCTe: 161,
    ImpressaoLoteCarga_ObrigarSelecionarTerminalDestino: 162,
    ContratoFrete_PermiteInformarPercentualAdiantamento: 163,
    Carga_EmitirCartaCorrecao: 164,
    Canhotos_ControlarSituacaoPagamento: 150,
    CargaEntrega_PermiteIniciarFinalizarViagensEntregasManualmente: 166,
    CargaCancelamento_LiberarCancelamentoComCTeNaoInutilizado: 167,
    BaixaPagar_PermiteCancelarBaixa: 168,
    BaixaPagar_NaoPermiteQuitarTitulo: 169,
    Pedido_PermiteIncluirPessoa: 170,
    Pedido_PermiteAlterarTipoTomador: 171,
    Pessoa_PermiteCriarFornecedor: 172,
    PagamentoMotorista_PermiteAvancarSemIntegracao: 173,
    Pedido_PermitePreencherValoresFrete: 174,
    Fatura_BloquearAcrescimoDesconto: 175,
    DocumentoEntrada_BloquearLancamentoComDataRetroativa: 176,
    Pedido_BloquearDuplicarPedido: 177,
    ArquivoContabil_RemoverObrigatoriedadeTeminalAtracacao: 178,
    OcorrenciaCancelamento_LiberarCancelamentoComCTeNaoInutilizado: 179,
    DocumentoDestinado_BloquearGeracaoManifestacao: 180,
    DocumentoDestinado_BloquearGeracaoDocumentoEntrada: 181,
    Carga_AlterarMoeda: 182,
    ImpressaoCargaLote_ObrigarInformarTerminalDestino: 183,
    Motorista_PermiteZerarSaldo: 184,
    FaturamentoLote_RetirarCamposObrigatorios: 185,
    AlteracaoArquivoMercante_RemoverObrigatoriedadeTerminalAtracacao: 186,
    BaixaPagar_NaoPermitirLancarDescontoAcrescimo: 187,
    BaixaReceber_NaoPermitirLancarDescontoAcrescimo: 188,
    BaixaReceberNovo_NaoPermitirLancarDescontoAcrescimo: 189,
    OrdemServico_NaoPermitirLancarDescontoFechamento: 190,
    Monitoramento_AlterarOuExcluirHistoricoDeStatusDoMonitoramento: 191,
    BaixaPagar_NaoPermitirCancelarBaixaComPagamentoEletronico: 192,
    BaixaReceber_NaoPermitirLancarDesconto: 193,
    BaixaReceber_NaoPermitirLancarAcrescimo: 194,
    BaixaReceberNovo_NaoPermitirLancarDesconto: 195,
    BaixaReceberNovo_NaoPermitirLancarAcrescimo: 196,
    Fatura_NaoPermitirLancarDesconto: 197,
    Fatura_NaoPermitirLancarAcrescimo: 198,
    Pedido_PermiteCriarPedidoTomadorSemCredito: 199,
    Carga_LiberarPagamentoMotoristaRejeitado: 200,
    JanelaDescarga_PermiteAlocarJanelaExtra: 201,
    Veiculo_NaoPermiteEditarLicenca: 202,
    Carga_PermiteAvancarLicencaInvalida: 203,
    Acerto_PermiteCancelamento: 204,
    Carga_LiberarCargaSemConfirmacaoERP: 205,
    Pessoa_PermiteAlterarSituacaoFinanceira: 206,
    FluxoGestaoPatio_PermiteCancelarFluxo: 207,
    EncerramentoCarga_EncerrarCarga: 208,
    EncerramentoCarga_EncerrarMDFe: 209,
    Fatura_PermiteDuplicar: 210,
    AbastecimentoGas_PermiteLancarAbastecimentoAposHorarioLimite: 211,
    Acerto_BloquearEdicaoResumoAbastecimento: 212,
    ContratoFreteAcrescimoDesconto_PermiteLiberarComIntegracaoRejeitada: 213,
    JanelaDescarga_SobreporRegras: 214,
    Motorista_NaoPermitirAlterarCentroDeResultado: 215,
    GrupoPessoas_NaoPermitirAlterarValorLimiteFaturamento: 216,
    Carga_PermiteTrocarPedidosDefinitivosUFsDestinatariosDiferentes: 217,
    ProgramacaoVeiculo_PermiteVisualizarAuditoria: 218,
    Fatura_NaoPermitirEditarDataVencimentoParcela: 219,
    CargaEntrega_PermiteAtualizarCoordenadasDoCliente: 220,
    CargaEntrega_PermiteEditarPesquisa: 221,
    ContratoFreteAcrescimoDesconto_PermiteLiberarPagamento: 222,
    FinanceiroBloquearInformacaoDataBaixaReceberCte: 223,
    CargaEntrega_PermiteAlterarDataOcorrencias: 224,
    CargaEntrega_PermiteAlterarDataInicioFimViagem: 225,
    CargaEntrega_PermiteAlterarDataEntrega: 226,
    Carga_PermiteLiberarSemIntegracaoGR: 227,
    GestaoDocumento_PermitirDesfazerAprovacao: 228,
    ChamadoOcorrencia_PermitirDelegarParaOutroUsuario: 229,
    ChamadoOcorrencia_PermitirDelegarParaUmSetor: 230,
    Monitoramento_InformarDataEntradaSaidaRaio: 231,
    FluxoGestaoPatio_InformarChegadaLoja: 232,
    FluxoGestaoPatio_InformarFimViagem: 233,
    FluxoGestaoPatio_InformarInicioHigienizacao: 234,
    FluxoGestaoPatio_InformarFimHigienizacao: 235,
    FluxoGestaoPatio_InformarInicioCarregamento: 236,
    FluxoGestaoPatio_InformarFimCarregamento: 237,
    FluxoGestaoPatio_SolicitarVeiculo: 238,
    FluxoGestaoPatio_InformarInicioDescarregamento: 239,
    FluxoGestaoPatio_InformarFimDescarregamento: 240,
    FluxoGestaoPatio_InformarDocumentoFiscal: 241,
    FluxoGestaoPatio_InformarDocumentoTransporte: 242,
    Acerto_PermitirRemoverAdiantamento: 243,
    PagamentoMotorista_PermiteInformarDataPagamentoRetroativa: 244,
    FluxoGestaoPatio_InformarMontagemCarga: 245,
    Carga_PermiteExcluirPreCalculoFrete: 246,
    Carga_PermiteReabrirCargaFinalizada: 247,
    FluxoGestaoPatio_InformarSeparacaoMercadoria: 248,
    ComissaoFuncionario_PermitirAlterarMedia: 249,
    CIOT_EncerrarGerencialmente: 250,
    Veiculo_PermitirAlterarDadosIntegracaoGR: 251,
    Motorista_PermitirInativarComSaldo: 252,
    Carga_PermiteAvancarCargaComRejeitacaoValePedagio: 253,
    FluxoGestaoPatio_PermiteReavaliarChecklist: 254,
    FluxoGestaoPatio_PermiteVoltarEtapaFaturamento: 255,
    Abastecimento_PermitirAlterarQuilometragemAbastecimentoFechado: 256,
    CargaCancelamento_PermitirDisponibilizarCTesParaVincularEmOutraCarga: 257,
    Acerto_PermiteFinalizarSemCanhotosRecebidos: 258,
    Acerto_PermiteFinalizarSemPalletsEntregues: 259,
    AbastecimentoGas_PermiteAdicionarVolumeExtraSolicitacaoGas: 260,
    ControleContainer_PermiteMovimentarContainer: 261,
    CargaEntrega_PermiteGerarOcorrenciaEstadia: 262,
    GrupoPessoas_PermiteAlterarSituacaoFinanceira: 263,
    CargaEntrega_PermiteAlterarSituacaoOnTime: 264,
    DocumentoEntrada_PermitirAtualizarNotaCancelada: 265,
    Veiculo_PermiteBloquearVeiculo: 266,
    Motorista_PermiteBloquearMotorista: 267,
    Transportador_PermiteBloquearTransportador: 268,
    PlanejamentoPedidoTMS_PermiteDefinirVeiculoMotoristaLicencaVencida: 269,
    GrupoPessoas_PermiteAlterarConfiguracaoEmissao: 270,
    GrupoPessoas_PermiteAlterarConfiguracaoFatura: 271,
    Pessoa_PermiteAlterarConfiguracaoEmissao: 272,
    Pessoa_PermiteAlterarConfiguracaoFatura: 273,
    ChamadoOcorrencia_PermitirFinalizarMesmoNaoSendoResponsavel: 274,
    Carga_PermitirLiberarSemRetiradaContainer: 275,
    MovimentacaoPneu_PermitirSucatearPneu: 276,
    ChamadoOcorrencia_PermitirAlterarValorEMotivoChamado: 277,
    Abastecimento_PermitirAlterarHorimetroAbastecimentoFechado: 278,
    BaixaTituloReceberNovo_PermiteFinalizarBaixaTitulo: 279,
    ContratoFreteTransportador_DisponibilizarParaTodosVeiculos: 280,
    ContratoFreteTransportador_HabilitarAbaDadosContrato: 281,
    ContratoFreteTransportador_HabilitarAbaOcorrencia: 282,
    ContratoFreteTransportador_HabilitarAbaClientes: 283,
    ContratoFreteTransportador_HabilitarAbaAcordo: 284,
    ContratoFreteTransportador_HabilitarAbaAnexos: 285,
    ContratoFreteTransportador_HabilitarAbaVeiculos: 286,
    ContratoFreteTransportador_HabilitarAbaOutrosValores: 287,
    ContratoFreteTransportador_HabilitarAbaFiliais: 288,
    ContratoFreteTransportador_HabilitarAbaTipoOperacao: 289,
    ContratoFreteTransportador_HabilitarAbaValoresFreteMinimo: 290,
    ContratoFreteTransportador_HabilitarAbaFranquia: 291,
    Carga_CalcularFreteNovamente: 292,
    ContratoFreteTransportador_HabilitarAbaValoresVeiculos: 293,
    ChamadoOcorrencia_PermitirAssumirAtendimento: 294,
    ChamadoOcorrencia_PermitirFecharSemOcorrencia: 295,
    ChamadoOcorrencia_PermitirLiberarParaOcorrencia: 296,
    GrupoPessoas_AdicionarDespachanteComoConsignatario: 297,
    MovimentacaoPneu_PermitirInformarHodometro: 298,
    Motorista_PermitirInativarCadastroMotorista: 299,
    MontagemCarga_AdicionarPrefixoNaCargaViaCarregamento: 300,
    Pessoas_PermiteBloquearDesbloquearPessoa: 301,
    Canhotos_PermitirRegistrarAuditoriaNosCanhotos: 302,
    PlanejamentoPedidoTMS_PermitirDefinirMotoristaComVigenciaIndisponivel: 303,
    Usuarios_PermitirEditarAsPermissoesDeAcesso: 304,
    Booking_PermitirCancelarBooking: 305,
    Carga_PermitirAlterarModeloVeicularNaCarga: 306,
    Pedido_PermitirAlterarModeloVeicularNoPedido: 307,
    PlanejamentoPedidoTMS_PermitirAlterarPedidosCargasJaFinalizados: 308,
    Carga_PermitirReverterCargaNoShow: 309,
    ContratoFinanciamento_PermitirAlterarVeiculos: 310,
    Carga_PermitirNaoComprarValePedagio: 311,
    Veiculo_PermitirSalvarLicenca: 312,
    RetiradaProdutoLista_NaoPermitirEditarAgendamento: 313,
    RetiradaProdutoLista_NaoPermitirExcluirAgendamento: 314,
    Carga_PermiteAvancarCargaComRejeicaoPlanejamentoFrota: 315,
    Usuario_PermitirAtivarInativarVeiculo: 316,
    Carga_RemoverAnexosCarga: 317,
    PagamentoMotorista_PermiteReverterPagamentoMotorista: 318,
    FluxoPatio_PermiteVoltarEtapa: 319,
    Container_PermitirOperadorEditarTodasInformacoesContainer: 320,
    Carga_PermitirLiberarSemIntegracaoFrete: 321,
    Carga_PermitirLiberarSemLicencaValida: 322,
    Carga_PermiteAlterarExternalId: 323,
    Carga_PermiteAvancarCargaComRejeitacaoGNRE: 324,
    CTeManual_PermiteRealizarCTeManualComValorZerado: 325,
    Carga_PermitirAdicionarRemoverPedidoEtapaUm: 326,
    PlanejamentoPedidoTMS_PermiteGerarCarga: 327,
    PlanejamentoPedidoTMS_PermiteInformarAlterarRemoverVeiculo: 328,
    PlanejamentoPedidoTMS_PermiteEnviarEmailTomador: 329,
    PlanejamentoPedidoTMS_PermiteEnviarEmailCheckList: 330,
    PlanejamentoPedidoTMS_PermiteEnviarOrdemColetaTomador: 331,
    PlanejamentoPedidoTMS_PermiteInformarAlterarRemoverMotorista: 332,
    PlanejamentoPedidoTMS_PermiteInformarPendente: 333,
    PlanejamentoPedidoTMS_PermiteAbrirDetalhePedido: 334,
    PlanejamentoPedidoTMS_PermiteGerarAvisoMotorista: 335,
    PlanejamentoPedidoTMS_PermiteGerarMotoristaCiente: 336,
    PlanejamentoPedidoTMS_PermiteAbrirDetalhesAviso: 337,
    PlanejamentoPedidoTMS_PermiteAbrirTratativas: 338,
    PlanejamentoPedidoTMS_PermiteSubstituirModeloVeicular: 339,
    PlanejamentoPedidoTMS_PermiteAbrirWhatsAppMotorista: 340,
    PlanejamentoPedidoTMS_PermiteAlterarTipoDeOperacao: 341,
    PlanejamentoPedidoTMS_PermiteImprimirOrdemDeColeta: 342,
    PlanejamentoPedidoTMS_PermiteInformarDevolucao: 343,
    PlanejamentoPedidoTMS_PermiteAlterarDataDePrevisaoDeSaida: 344,
    PlanejamentoPedidoTMS_PermiteAlterarTipoDeCarga: 345,
    PlanejamentoPedidoTMS_PermiteEnviarEscala: 346,
    Carga_PermiteInserirAlterarVeiculoNaCargaOuPedido: 347,
    SugestaoMensal_PermiteGerarListaDiariaApartirListaMensal: 348,
    Canhoto_PermiteConfirmarRecebimentoCanhotoFisico: 349,
    Canhoto_PermiteAdicionarJustificativa: 350,
    Carga_PermiteAlterarObservacaoContratoFreteTerceiro: 351,
    Carga_PermiteRemoverContainerVinculadoEmCarga: 352,
    Home_VisualizarDashDoc: 353,
    Pedido_PermitirAlteracaoMotorista: 354,
    ControleDocumento_PermiteDesparquearDocumentos: 355,
    DocumentoDestinado_PermiteEmitirDesacordo: 356,
    ComprovanteCarga_PermiteReverterComprovante: 357,
    Pedido_PermiteInserirVeiculoProprio: 358,
    Pedido_PermiteInserirVeiculoTerceiro: 359,
    Carga_PermiteInserirVeiculoProprio: 360,
    Carga_PermiteInserirVeiculoTerceiro: 361,
    PlanejamentoPedidoTMS_PermiteInserirVeiculoProprio: 362,
    PlanejamentoPedidoTMS_PermiteInserirVeiculoTerceiro: 363,
    Transportador_PermiteInativarTransportador: 364,
    Transportador_PermiteAlterarStatusFinanceiro: 365,
    ChamadoOcorrencia_PermiteFinalizarEmLiberadoParaOcorrencia: 366,
    Carga_NaoPermiteAlterarOperador: 367,
    AuditoriaEMP_AcessoJustificativa: 368,
    Carga_PermitirAjusteManualImportadasEmbarcador: 369,
    AuditoriaEMP_PermiteVisualizarJustificativa: 370,
    Veiculo_PermitirAlterarModeloVeicular: 371,
    Veiculo_PermitirAcessoRastreador: 372,
    Carga_PermitirAnexoDocumentoNaoContribuinte: 373,
    Guias_PermiteAprovarRejeitarOCRGuiasManualmente: 374,
    Guias_PermiteReverterAprovacaoOCRGuias: 375,
    Veiculo_PermiteEditarCompraValePedagio: 376,
    Veiculo_PermiteEditarCompraValePedagioRetorno: 377,
    ControleDocumento_PermitirLiberarDocumentoComIrregularidade: 378,
    Motorista_PermitirDuplicarCadastro: 379,
    Acerto_PermitirAdicionarAlterarValorComprovadoSaldoViagem: 380,
    Carga_PermitirEmissaoDocumentosComFalhaIntegracaoAcrescimoDesconto: 381,
    ModuloControle_PermiteParquearDocumentos: 382,
    CancelamentoCargaLote_LiberarCancelamentoCargaBloqueada: 383,
    Carga_LiberarCargaComValorLimiteApoliceDivergente: 384,
    VendaDireta_PermiteCancelamento: 385,
    Carga_PermiteFinalizarIntegracaoEtapa1Carga: 386,
    TermoQuitacaoFinanceiro_PermiteExcluirTermoQuitacao: 387,
    CargaEntrega_PermiteAlterarDataInicioFimPreTrip: 388,
    MonitoramentoMapa_PermiteAlterarAbaFiltrosPersonalizados: 389,
    Carga_NaoPermiteReenviarIntegracaoCargasAppTrizy: 390,
    Carga_PermiteAcessarEtapasDocumentosQuandoAcessoEstiverRestrito: 391,
    Carga_AlterarValePedagio: 392,
    Carga_AlterarPercentualExecucao: 393,
    ChamadoOcorrencia_AprovarValorSuperior: 394,
    CTeManual_VincularCTeEmbarcador: 395,
    ChamadoOcorrencia_PermitirEstornarOcorrencia: 396,
    GrupoPessoas_PermitirAdicionarClienteAoGrupoCliente: 397,
    Acerto_PermitirLancarDespesasAcertoViagem: 398,
    Acerto_PermitirLancarDiariaAvulsoAcertoViagem: 399,
    Canhoto_PermitirRetornarStatusCanhotoAPIDigitalizacao: 400,
    QualidadeEntrega_PermiteLiberarNotasBloqueadas: 401,
    FluxoGestaoPatio_OcultarDetalhesCarga: 402,
    CargaCancelamento_PermitirModificarTipoCancelamento: 403,
    Pagamento_Reverter: 404,
    IntegracaoPorCTe_GerenciarNotasFiscaisAdicionais: 405,
    ImportarAtendimentos: 406,
    Carga_PermitirAlterarValorAdiantamentoEtapaContainer: 407,
    GestaoAtendimento_VerificarMeusIndicadores: 408,
    GestaoAtendimento_VerificarIndicadoresGerais: 409,
    Carga_NaoPermitirCancelarValePedagio: 411,
    CargaCancelamento_NaoPermitirFinalizarEtapaIntegracao: 412,
    Carga_AlterarValorFreteApenasComTabelaFrete: 413,
    GrupoPessoas_PermitirBloquearOuDesbloquearGrupoDePessoas : 414,
    Target_PermiteEditarValor : 415,
    Target_PermiteAprovarRejeitarRegistro : 416,
};

function EnumPermissaoPersonalizadaDescricao(permissaoPersonalizada) {
    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Acerto_PermiteLiberarAutorizarAbastecimento)
        return "Permitir Autorizar Abastecimento no Acerto";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Acerto_PermiteLiberarAutorizarArla)
        return "Permitir Autorizar Arla no Acerto";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Acerto_PermiteLiberarPedagioAcerto)
        return "Permitir Autorizar Pedágio no Acerto";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Acerto_PermiteAdicionarBonificacao)
        return "Permitir Adicionar Bonificacao no Acerto";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Acerto_PermiteAdicionarDesconto)
        return "Permitir Adicionar Desconto no Acerto";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Acerto_PermiteFecharAcerto)
        return "Permitir Fechar o Acerto";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Acerto_PermiteReabirAcerto)
        return "Permitir Re-abrir o Acerto";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Acerto_PermiteInformarCargaFracionada)
        return "Permitir Informar Carga Fracionada";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Acerto_PermiteInformarBonificacaoCliente)
        return "Permitir Informar Bonificação do Cliente";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Acerto_PermiteAlterarReboquesCarga)
        return "Permitir Alterar Reboques da Carga";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Acerto_PermiteAjustarKMTotal)
        return "Permitir Ajustar KM Total";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Acerto_OcultarResultadosViagem)
        return "Ocultar Resultados da Viagem";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Fatura_PermiteIniciarFatura)
        return "Permitir Iniciar a Fatura";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Fatura_PermiteCancelarCarga)
        return "Permitir Cancelar a Fatura";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Fatura_PermiteAdicionarNovasCargas)
        return "Permitir Adicionar Novas Cargas a Fatura";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Fatura_PermiteSalvarCarga)
        return "Permitir Salvar Carga da Fatura";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Fatura_PermiteRemoverConhecimento)
        return "Permitir Remover Conhecimento da Fatura";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Fatura_PermiteSalvarValoresFechamento)
        return "Permitir Salvar Valores para Fechamento da Fatura";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Fatura_PermiteGerarParcelas)
        return "Permitir Gerar Parcelas da Fatura";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Fatura_PermiteFecharFatura)
        return "Permitir Fechar Fatura";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Fatura_PermiteVisualizarFatura)
        return "Permitir Visualizar Fatura";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Fatura_PermiteReAbrirFatura)
        return "Permitir Re-abrir Fatura";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Fatura_PermiteEnviarEDI)
        return "Permitir Enviar Layout de EDI da Fatura";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Fatura_PermiteEnviarFatura)
        return "Permitir Enviar Layout de Fatura";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Fatura_PermiteLiquidarFatura)
        return "Permitir Liquidar Fatura";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.BaixaReceber_PermiteBaixarTitulo)
        return "Permitir Baixar o Título a Receber";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.BaixaReceber_PermiteSalvarValores)
        return "Permitir Salvar Valores de Negociação do Título a Receber";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.BaixaReceber_PermiteGerarParcelas)
        return "Permitir Gerar Parcelas de Negociação do Título a Receber";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.BaixaReceber_PermiteFecharBaixa)
        return "Permitir Fechar Baixa a Receber";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.BaixaPagar_PermiteBaixarTitulo)
        return "Permitir Baixar o Título a Pagar";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.BaixaPagar_PermiteSalvarValores)
        return "Permitir Salvar Valores de Negociação do Título a Pagar";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.BaixaPagar_PermiteGerarParcelas)
        return "Permitir Gerar Parcelas de Negociação do Título a Pagar";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.BaixaPagar_PermiteFecharBaixa)
        return "Permitir Fechar Baixa a Pagar";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Criar)
        return "Criar";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Alterar)
        return "Alterar";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Excluir)
        return "Excluir";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Cancelar)
        return "Cancelar";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ReAbrir)
        return "Re-abrir";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Finalizar)
        return "Finalizar";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ComissaoFuncionario_PermiteEditarDadosGerados)
        return "Permite Editar Dados Gerados Na Comissao";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ContratoFrete_PermiteAutorizarContrato)
        return "Contrato de Frete - Permite Autorizar";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ContratoFrete_PermiteBloquearContrato)
        return "Contrato de Frete - Permite Bloquear";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ContratoFrete_PermiteInformarAcrescimoDesconto)
        return "Contrato de Frete - Permite Informar Acréscimo/Desconto";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ContratoFrete_PermiteInformarValorFrete)
        return "Contrato de Frete - Permite Informar Valor de Frete";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ContratoFrete_PermiteInformarValorPedagio)
        return "Contrato de Frete - Permite Informar Valor de Pedágio";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ContratoFrete_PermiteInformarPercentualAdiantamento)
        return "Contrato de Frete - Permite Informar Percentual de Adiantamento";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_SalvarDadosTransporte)
        return "Carga - Salvar Dados do Transporte";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_InformarDocumentosFiscais)
        return "Carga - Informar documentos para Emissão";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_PermiteAdicionarOutrosDocumentos)
        return "Carga - Permite Informar outros Documentos";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_AlterarValorFrete)
        return "Carga - Alterar Valor do Frete";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_AutorizarFreteInconsistente)
        return "Carga - Autorizar Frete com Inconsistente";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_AdicionarComponentes)
        return "Carga - Adicionar Componentes";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_AlterarFreteTerceiros)
        return "Carga - Alterar Valor de Frete de Terceiros";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_AlterarConfiguracao)
        return "Carga - Alterar dados da Configuração";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_AlterarObservacao)
        return "Carga - Alterar dados da Observação";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_AlterarDadosPedido)
        return "Carga - Alterar dados dos Pedidos";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_AlterarDadosSeguro)
        return "Carga - Alterar dados do Seguro";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_AutorizarSeguro)
        return "Carga - Autorizar Seguro";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_AlterarRota)
        return "Carga - Alterar Rota";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_AlterarLacres)
        return "Carga - Alterar Lacres";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_AlterarPassagens)
        return "Carga - Alterar Passagens entre estados";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_AlterarPercurso)
        return "Carga - Alterar Percurso da Carga";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_AutorizarEmissaoDocumentos)
        return "Carga - Autorizar Emissão dos Documentos";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_RetornarEtapaNotasFiscais)
        return "Carga - Retornar a Etapa de Notas Fiscais";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_EditarCTe)
        return "Carga - Editar CT-e";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_ConfirmarIntegracao)
        return "Carga - Confirmar Integração";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_ConfirmarImpressao)
        return "Carga - Confirmar Impressão";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_AutorizarModalidadePagamentoNota)
        return "Carga - Autorizar Modalidade Pagamento Nota";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_LiberarAverbacaoRejeitada)
        return "Carga - Autorizar Emissão com Averbação Rejeitada";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_FinalizarCargaMDFeRejeitado)
        return "Carga - Finalizar Carga com MFD-e Rejeitado";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_ReenviarIntegracoes)
        return "Carga - Reenviar integrações";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_PermiteAlterarTransportador)
        return "Carga - Permite alterar o transportador";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_PermiteLiberarComDiferencaNoValorFrete)
        return "Carga - Permite liberar com diferença do valor do frete (embarcador x tabela)";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_AutorizarPesoCarga)
        return "Carga - Autorizar Peso";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_AutorizarSemArquivoOrtec)
        return "Carga - Autorizar Sem Arquivo Ortec";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_LiberarEmissaoSemNF)
        return "Carga - Liberar Emissão Sem NF";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_AutorizarManutencaoPendenteVeiculo)
        return "Carga - Autorizar Veículo com Manutenções Pendentes";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_DownloadArquivoIntegracoes)
        return "Carga - Download arquivos de integração";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_AutorizarValorMaximoPendentePagamento)
        return "Carga - Autorizar Valor Máximo Pendente para Pagamento";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_PermitirRemoverPedido)
        return "Carga - Remover Pedido";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_PermiteAlterarInclusaoICMS)
        return "Carga - Permite alterar inclusão do ICMS";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_PermitirAvancarCargaSemTodosPreCte)
        return "Carga - Permite avançar sem todos os pré CT-e";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_AlterarMoeda)
        return "Carga - Alterar a moeda";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.DocumentoEntrada_AutorizarPrecoCombustivelDiferenteFornecedor)
        return "Documento de Entrada - Autorizar Preço de Combustível Diferente do Fornecedor";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.DocumentoEntrada_LancarDuplicata)
        return "Documento de Entrada - Lançar Duplicata";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.GestaoDocumento_PermitirDesfazerAprovacao)
        return "Gestão de Documentos - Permitir Desfazer a Aprovação";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.MovimentoFinanceiro_InformarCentroResultado)
        return "Movimento Financeiro - Informar Centro de Resultado";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.MovimentoFinanceiro_InformarColaborador)
        return "Movimento Financeiro - Informar Colaborador";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.MovimentoFinanceiro_InformarPlanoEntradaSaida)
        return "Movimento Financeiro - Informar Plano de Entrada e Saída";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.CargaCancelamento_Anular)
        return "Cancelamento de Carga - Anular Carga";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.CargaCancelamento_ReenviarCancelamentoComoAnulacao)
        return "Cancelamento de Carga - Reenviar Cancelamento como Anulação";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.CargaCancelamento_AdicionarComoCancelamento)
        return "Cancelamento de Carga - Adicionar como Cancelamento (Anulação/Doc. do Embarcador)";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.CargaCancelamento_CancelarCargaBloqueada)
        return "Cancelamento de Carga - Permitir Cancelar Carga Bloqueada";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.MontagemCarga_LiberacaoVeiculo)
        return "Montagem de Carga - Liberação do veículo";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Ocorrencia_PermitirRetornarEtapaCadastro)
        return "Ocorrência - Permitir retornar para a etapa de cadastro";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.OcorrenciaCancelamento_Anular)
        return "Cancelamento de Ocorrência - Anular Ocorrência";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.OcorrenciaCancelamento_ReenviarCancelamentoComoAnulacao)
        return "Cancelamento de Ocorrência - Reenviar Cancelamento como Anulação";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.OcorrenciaCancelamento_AdicionarComoCancelamento)
        return "Cancelamento de Ocorrência - Adicionar como Cancelamento (Anulação/Doc. do Embarcador)";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarDoca)
        return "Fluxo Gestão de Pátio - Informar Doca";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarGuaritaEntrada)
        return "Fluxo Gestão de Pátio - Informar entrada na Guarita";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarChecklist)
        return "Fluxo Gestão de Pátio - Informar checklist";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FluxoGestaoPatio_TravarChave)
        return "Fluxo Gestão de Pátio - Travar Chave";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarExpedicao)
        return "Fluxo Gestão de Pátio - Informar dados da Expedição";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FluxoGestaoPatio_LiberarChave)
        return "Fluxo Gestão de Pátio - Liberar Chave";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarInicioViagem)
        return "Fluxo Gestão de Pátio - Informar inicio de viagem";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarChegadaVeiculo)
        return "Fluxo Gestão de Pátio - Informar chegada do veículo";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarDeslocamentoPatio)
        return "Fluxo Gestão de Pátio - Informar deslocamento no pátio";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.WMS_Autorizar_Volumes_Faltantes)
        return "Autorizar com volumes faltantes";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FluxoGestaoPatio_PermiteRetornarEtapaSaidaCD)
        return "Fluxo Gestão de Pátio - Permite Retornar Etapa Saída CD";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FechamentoFrete_Finalizar)
        return "Fechamento de Frete - Finalizar";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FechamentoFrete_Cancelar)
        return "Fechamento de Frete - Cancelar";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FechamentoFrete_Reabrir)
        return "Fechamento de Frete - Re-abrir";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.PreCarga_Supervisor)
        return "Pré Carga - Supervisor";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.CargaCancelamento_LiberarAverbacaoRejeitada)
        return "Cancelamento de Carga - Liberar Averbações Rejeitadas";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Pallets_PermiteDataRetroativa_ValePallet)
        return "Pallets - Permite Data Retroativa Vale Pallet";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Pallets_PermiteDataRetroativa_DevolucaoPallet)
        return "Pallets - Permite Data Retroativa Devolução Pallet";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Canhotos_ReverterJustificativa_Canhotos)
        return "Canhotos - Permite Reverter Justificativa Canhotos";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Canhoto_PermitirReverterImagem)
        return "Canhotos - Permitir Reverter Imagem";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Pagamento_AutorizarPagmentoComCargaCancelada)
        return "Pagamento - Autorizar Pagmento Com Carga Cancelada";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.VendaDireta_PermitirFinalizar)
        return "Venda Direta - Permitir Finalizar";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.VendaDireta_PermitirAlterarParcelamento)
        return "Venda Direta - Permitir Alterar Parcelamento";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Titulo_GerarMultiplosTitulos)
        return "Título - Gerar múltiplos títulos";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Usuario_PermiteRemoverAnexos)
        return "Usuário - Permite Remover Anexos";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Veiculo_PermiteRemoverAnexos)
        return "Veículo - Permite Remover Anexos";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Motorista_PermiteRemoverAnexos)
        return "Motorista - Permite Remover Anexos";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Veiculo_PermiteAlterarPlaca)
        return "Veículo - Permite Alterar Placa";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.CTeManual_PermiteCancelarCTe)
        return "CT-e Manual - Permite Cancelar/Inutilizar CT-e";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FilaCarregamento_PermitirAceitarCarga)
        return "Fila de Carregamento - Permitir Aceitar Carga";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FilaCarregamento_PermitirAdicionar)
        return "Fila de Carregamento - Permitir Adicionar";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FilaCarregamento_PermitirAlocarCarga)
        return "Fila de Carregamento - Permitir Alocar Carga";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FilaCarregamento_PermitirAlterarCentroCarregamento)
        return "Fila de Carregamento - Permitir Alterar Centro de Carregamento";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FilaCarregamento_PermitirAlterarPrimeiraPosicao)
        return "Fila de Carregamento - Permitir Alterar para Primeira Posição";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FilaCarregamento_PermitirAlterarUltimaPosicao)
        return "Fila de Carregamento - Permitir Alterar para Última Posição";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FilaCarregamento_PermitirConfirmarChegada)
        return "Fila de Carregamento - Permitir Confirmar Chegada";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FilaCarregamento_PermitirDesatrelarTracao)
        return "Fila de Carregamento - Permitir Desatrelar Tração";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FilaCarregamento_PermitirEnviarNotificacao)
        return "Fila de Carregamento - Permitir Enviar Notificação";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FilaCarregamento_PermitirInformarConjuntoMotorista)
        return "Fila de Carregamento - Permitir Informar Conjunto Motorista";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FilaCarregamento_PermitirInformarConjuntoVeiculo)
        return "Fila de Carregamento - Permitir Informar Conjunto Veículo";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FilaCarregamento_PermitirLiberar)
        return "Fila de Carregamento - Permitir Liberar";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FilaCarregamento_PermitirLiberarSaidaFila)
        return "Fila de Carregamento - Permitir Liberar Saída";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FilaCarregamento_PermitirRecusarCarga)
        return "Fila de Carregamento - Permitir Recusar Carga";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FilaCarregamento_PermitirRemover)
        return "Fila de Carregamento - Permitir Remover";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FilaCarregamento_PermitirRemoverConjuntoMotorista)
        return "Fila de Carregamento - Permitir Remover Conjunto Motorista";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FilaCarregamento_PermitirRemoverReversa)
        return "Fila de Carregamento - Permitir Remover Reversa";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FilaCarregamento_PermitirRemoverTracao)
        return "Fila de Carregamento - Permitir Remover Tração";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FilaCarregamento_PermitirReposicionar)
        return "Fila de Carregamento - Permitir Reposicionar";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.AcordoFaturamento_PermiteAlterarApenasEmail)
        return "Acordo de Faturamento - Permite alterar apenas E-mail";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.GrupoPessoas_PermiteAlterarApenasObservacoes)
        return "Grupo de Pessoas - Pemite alterar apenas observações";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.CTeManual_PermiteAnulacaoGerencialCTe)
        return "CTe Manual - Pemite anulação gerencial de CT-e";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ImpressaoLoteCarga_ObrigarSelecionarTerminalDestino)
        return "Impressão Lote da Carga - Obriga selecionar terminal de destino";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_EmitirCartaCorrecao)
        return "Carga - Emitir carta de correção";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Canhotos_ControlarSituacaoPagamento)
        return "Canhotos - Permitir controlar situação Pagamento";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.CargaEntrega_PermiteIniciarFinalizarViagensEntregasManualmente)
        return "Controle de Entregas - Permite iniciar e finalizar viagens e entregas manualmente";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.CargaCancelamento_LiberarCancelamentoComCTeNaoInutilizado)
        return "Cancelamento de Carga - Permite liberar o Cancelamento sem inutilizar os CT-es";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.BaixaPagar_PermiteCancelarBaixa)
        return "Permitir Cancelar a Baixa a Pagar";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.BaixaPagar_NaoPermiteQuitarTitulo)
        return "Não Permitir Quitar o Título a Pagar";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Pedido_PermiteIncluirPessoa)
        return "Pedido - Permitir Incluir Pessoa";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Pedido_PermiteAlterarTipoTomador)
        return "Pedido - Permitir Alterar o Tipo do Tomador";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Pessoa_PermiteCriarFornecedor)
        return "Pessoa - Permitir criar a Pessoa como Fornecedor";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.PagamentoMotorista_PermiteAvancarSemIntegracao)
        return "Pagamento Motorista - Permitir avançar sem integração";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Pedido_PermitePreencherValoresFrete)
        return "Pedido - Permitir preencher valor de frete e componentes";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Fatura_BloquearAcrescimoDesconto)
        return "Fatura - Bloquear lançamento de desconto e acréscimo";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.DocumentoEntrada_BloquearLancamentoComDataRetroativa)
        return "Documento de Entrada - Bloquear lançamento com data retroativa";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Pedido_BloquearDuplicarPedido)
        return "Pedido - Bloquear duplicar pedido";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ArquivoContabil_RemoverObrigatoriedadeTeminalAtracacao)
        return "Arquivo Contábil - Remover obrigatoriedade do terminal de atracação";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.OcorrenciaCancelamento_LiberarCancelamentoComCTeNaoInutilizado)
        return "Cancelamento Ocorrência - Permite liberar o Cancelamento sem inutilizar os CT-es";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.DocumentoDestinado_BloquearGeracaoManifestacao)
        return "Documentos Destinados - Bloquear geração de manifestações";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.DocumentoDestinado_BloquearGeracaoDocumentoEntrada)
        return "Documentos Destinados - Bloquear geração de Documento de Entrada";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ImpressaoCargaLote_ObrigarInformarTerminalDestino)
        return "Impressão Carga Lote - Obrigar Informar Terminal Destino";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Motorista_PermiteZerarSaldo)
        return "Motorista - Permite zerar saldo";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FaturamentoLote_RetirarCamposObrigatorios)
        return "Faturamento em Lote - Retirar campos obrigatórios";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.AlteracaoArquivoMercante_RemoverObrigatoriedadeTerminalAtracacao)
        return "Alteração Arquivo Mercante - Remover obrigatoriedade do terminal de atracação";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.BaixaPagar_NaoPermitirLancarDescontoAcrescimo)
        return "Baixa a Pagar - Não permitir lançar desconto e acréscimo";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.BaixaReceber_NaoPermitirLancarDescontoAcrescimo)
        return "Baixa a Receber - Não permitir lançar desconto e acréscimo";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.BaixaReceberNovo_NaoPermitirLancarDescontoAcrescimo)
        return "Baixa a Receber Novo - Não permitir lançar desconto e acréscimo";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.OrdemServico_NaoPermitirLancarDescontoFechamento)
        return "Ordem de Serviço - Não permitir lançar desconto no fechamento";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Monitoramento_AlterarOuExcluirHistoricoDeStatusDoMonitoramento)
        return "Monitoramento - Permite alterar ou excluir o histórico de status do monitoramento";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.BaixaPagar_NaoPermitirCancelarBaixaComPagamentoEletronico)
        return "Baixa a Pagar - Não permitir cancelar baixa com pagamento eletrônico";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.BaixaReceber_NaoPermitirLancarDesconto)
        return "Baixa a Receber - Não permitir lançar desconto";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.BaixaReceber_NaoPermitirLancarAcrescimo)
        return "Baixa a Receber - Não permitir lançar acréscimo";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.BaixaReceberNovo_NaoPermitirLancarDesconto)
        return "Baixa a Receber Novo - Não permitir lançar desconto";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.BaixaReceberNovo_NaoPermitirLancarAcrescimo)
        return "Baixa a Receber Novo - Não permitir lançar acréscimo";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Fatura_NaoPermitirLancarDesconto)
        return "Fatura - Não permitir lançar desconto";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Fatura_NaoPermitirLancarAcrescimo)
        return "Fatura - Não permitir lançar acréscimo";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Pedido_PermiteCriarPedidoTomadorSemCredito)
        return "Pedido - Permitir criar pedido sem crédito no tomador";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_LiberarPagamentoMotoristaRejeitado)
        return "Carga - Liberar Pagamento Motorista Rejeitado";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.JanelaDescarga_PermiteAlocarJanelaExtra)
        return "Janela Descarga - Permite Alocar Janela Extra";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.JanelaDescarga_SobreporRegras)
        return "Janela Descarga - Sobrepor Regras Validação";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Veiculo_NaoPermiteEditarLicenca)
        return "Veículo - Não Permitir Editar Licença";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_PermiteAvancarLicencaInvalida)
        return "Carga - Permite Avançar com Licença Inválida";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Acerto_PermiteCancelamento)
        return "Acerto - Permite Cancelamento";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_LiberarCargaSemConfirmacaoERP)
        return "Carga - Permite Permite Liberar Carga sem a Confirmação do ERP";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Pessoa_PermiteAlterarSituacaoFinanceira)
        return "Pessoa - Permite alterar Situação Financeira";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FluxoGestaoPatio_PermiteCancelarFluxo)
        return "Fluxo Gestão de Pátio - Permite Cancelar Fluxo"

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.EncerramentoCarga_EncerrarCarga)
        return "Encerramento Carga - Permite encerrar carga";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.EncerramentoCarga_EncerrarMDFe)
        return "Encerramento Carga - Permite encerrar MDF-e";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Fatura_PermiteDuplicar)
        return "Fatura - Permitir duplicar";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.AbastecimentoGas_PermiteLancarAbastecimentoAposHorarioLimite)
        return "Abastecimento de Gás - Permite lançar abastecimento após horário limite";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Acerto_BloquearEdicaoResumoAbastecimento)
        return "Acerto - Bloquear edição resumo do abastecimento";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ContratoFreteAcrescimoDesconto_PermiteLiberarComIntegracaoRejeitada)
        return "Acréscimo/Desconto no Contrato de Frete - Permite Liberar com Integração Rejeitada";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Motorista_NaoPermitirAlterarCentroDeResultado)
        return "Motorista - Não permite alterar o centro de resultado";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.GrupoPessoas_NaoPermitirAlterarValorLimiteFaturamento)
        return "Grupo de Pessoas - Não permite alterar valor limite de faturamento";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_PermiteTrocarPedidosDefinitivosUFsDestinatariosDiferentes)
        return "Carga - Permite trocar pedidos definitivos de destinatários com UFs diferentes";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ProgramacaoVeiculo_PermiteVisualizarAuditoria)
        return "Programação Veículo - Permite Visualizar Auditoria";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Fatura_NaoPermitirEditarDataVencimentoParcela)
        return "Fatura - Não Permitir Editar a Data de Vencimento da Parcela";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.CargaEntrega_PermiteAtualizarCoordenadasDoCliente)
        return "Controle de Entregas - Permitir atualizar coordenadas do cliente";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.CargaEntrega_PermiteEditarPesquisa)
        return "Controle de Entregas - Permitir editar pesquisa";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ContratoFreteAcrescimoDesconto_PermiteLiberarPagamento)
        return "Acréscimo/Desconto no Contrato de Frete - Permite Liberar Pagamento";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FinanceiroBloquearInformacaoDataBaixaReceberCte)
        return "Bloquear informação da data da baixa a receber por CT-e";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.CargaEntrega_PermiteAlterarDataOcorrencias)
        return "Controle de Entregas - Permitir alterar data de ocorrências";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.CargaEntrega_PermiteAlterarDataInicioFimViagem)
        return "Controle de Entregas - Permitir alterar data de início e fim de viagem";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.CargaEntrega_PermiteAlterarDataEntrega)
        return "Controle de Entregas - Permitir alterar datas da entrega";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_PermiteLiberarSemIntegracaoGR)
        return "Carga - Permitir liberar sem integração com a GR";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ChamadoOcorrencia_PermitirDelegarParaOutroUsuario)
        return "Atendimento - Permitir delegar para outro usuário";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ChamadoOcorrencia_PermitirDelegarParaUmSetor)
        return "Atendimento - Permitir delegar para um setor";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Monitoramento_InformarDataEntradaSaidaRaio)
        return "Monitoramento - Permite informar Entrada raio e Saída raio manualmente";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarChegadaLoja)
        return "Fluxo Gestão de Pátio - Informar Chegada Loja";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarFimViagem)
        return "Fluxo Gestão de Pátio - Informar Fim Viagem";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarInicioHigienizacao)
        return "Fluxo Gestão de Pátio - Informar Início Higienização";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarFimHigienizacao)
        return "Fluxo Gestão de Pátio - Informar Fim Higienização";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarInicioCarregamento)
        return "Fluxo Gestão de Pátio - Informar Início Carregamento";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarFimCarregamento)
        return "Fluxo Gestão de Pátio - Informar Fim Carregamento";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FluxoGestaoPatio_SolicitarVeiculo)
        return "Fluxo Gestão de Pátio - Solicitar Veículo";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarInicioDescarregamento)
        return "Fluxo Gestão de Pátio - Informar Início Descarregamento";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarFimDescarregamento)
        return "Fluxo Gestão de Pátio - Informar Fim Descarregamento";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarDocumentoFiscal)
        return "Fluxo Gestão de Pátio - Informar Documento Fiscal";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarDocumentoTransporte)
        return "Fluxo Gestão de Pátio - Informar Documento Transporte";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarMontagemCarga)
        return "Fluxo Gestão de Pátio - Informar Montagem de Carga";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarSeparacaoMercadoria)
        return "Fluxo Gestão de Pátio - Informar Separação de Mercadoria";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Acerto_PermitirRemoverAdiantamento)
        return "Acerto - Permitir remover adiantamento";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.PagamentoMotorista_PermiteInformarDataPagamentoRetroativa)
        return "Pagamento Motorista - Permitir informar data de pagamento retroativa";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_PermiteExcluirPreCalculoFrete)
        return "Carga - Permite excluir pré calculo de frete da carga";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_PermiteReabrirCargaFinalizada)
        return "Carga - Permite reabrir carga finalizada";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ComissaoFuncionario_PermitirAlterarMedia)
        return "Comissão Funcionário - Permitir alterar média";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.CIOT_EncerrarGerencialmente)
        return "CIOT - Encerrar Gerencialmente";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Veiculo_PermitirAlterarDadosIntegracaoGR)
        return "Veículo - Permitir alterar dados de integração com GR";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Motorista_PermitirInativarComSaldo)
        return "Motorista - Permitir inativar com saldo";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_PermiteAvancarCargaComRejeitacaoValePedagio)
        return "Carga - Permitir avançar carga com rejeição no vale pedágio";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FluxoGestaoPatio_PermiteReavaliarChecklist)
        return "Fluxo Gestão de Pátio - Permite reavaliar checklist";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FluxoGestaoPatio_PermiteVoltarEtapaFaturamento)
        return "Fluxo Gestão de Pátio - Permite voltar etapa de faturamento";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Abastecimento_PermitirAlterarQuilometragemAbastecimentoFechado)
        return "Abastecimento - Permitir alterar quilometragem de abastecimentos fechados";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.CargaCancelamento_PermitirDisponibilizarCTesParaVincularEmOutraCarga)
        return "Cancelamento de Carga - Permitir disponibilizar CT-es para vincular em outra carga";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Acerto_PermiteFinalizarSemCanhotosRecebidos)
        return "Acerto - Permitir finalizar sem canhotos recebidos";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Acerto_PermiteFinalizarSemPalletsEntregues)
        return "Acerto - Permitir finalizar sem palletes entregues";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.AbastecimentoGas_PermiteAdicionarVolumeExtraSolicitacaoGas)
        return "Abastecimento de Gás - Permitir adicionar volume extra solicitação de gás";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ControleContainer_PermiteMovimentarContainer)
        return "Controle Container - Permite Movimentar Container";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.CargaEntrega_PermiteGerarOcorrenciaEstadia)
        return "Controle de Entregas - Permite gerar ocorrência de estadia";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.GrupoPessoas_PermiteAlterarSituacaoFinanceira)
        return "Grupo de Pessoas - Permite alterar a situação financeira";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.CargaEntrega_PermiteAlterarSituacaoOnTime)
        return "Controle de Entregas - Permite alterar a situação On Time";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.DocumentoEntrada_PermitirAtualizarNotaCancelada)
        return "Documento de Entrada - Permitir atualizar dados de uma nota cancelada";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Veiculo_PermiteBloquearVeiculo)
        return "Veículo - Permite bloquear veículo";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Motorista_PermiteBloquearMotorista)
        return "Motorista - Permite bloquear motorista";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Transportador_PermiteBloquearTransportador)
        return "Transportador - Permite bloquear transportador";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteDefinirVeiculoMotoristaLicencaVencida)
        return "Planejamento de Pedidos - Permite definir Veículo/Motorista com Licença vencida";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.GrupoPessoas_PermiteAlterarConfiguracaoEmissao)
        return "Grupo de Pessoas - Permite alterar as configurações de Emissão";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.GrupoPessoas_PermiteAlterarConfiguracaoFatura)
        return "Grupo de Pessoas - Permite alterar as configurações de Fatura";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Pessoa_PermiteAlterarConfiguracaoEmissao)
        return "Pessoa - Permite alterar as configurações de Emissão";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Pessoa_PermiteAlterarConfiguracaoFatura)
        return "Pessoa - Permite alterar as configurações de Fatura";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ChamadoOcorrencia_PermitirFinalizarMesmoNaoSendoResponsavel)
        return "Atendimento - Permitir finalizar ocorrência mesmo não sendo Responsável";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_PermitirLiberarSemRetiradaContainer)
        return "Carga - Permitir liberar sem informar a retirada de container";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.MovimentacaoPneu_PermitirSucatearPneu)
        return "Movimentação de Pneu - Permitir Sucatear Pneu";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ChamadoOcorrencia_PermitirAlterarValorEMotivoChamado)
        return "Atendimento - Permitir alterar Valor e Motivo do Chamado";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Abastecimento_PermitirAlterarHorimetroAbastecimentoFechado)
        return "Abastecimento - Permitir alterar Horímetro de abastecimentos fechados";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.BaixaTituloReceberNovo_PermiteFinalizarBaixaTitulo)
        return "Baixa a Receber Novo - Permite finalizar a Baixa de Títulos";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ContratoFreteTransportador_DisponibilizarParaTodosVeiculos)
        return "Contrato de Frete Transportador - Disponibilizar para todos os veículos";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaDadosContrato)
        return "Contrato de Frete Transportador - Habilitar aba de Dados do Contrato";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaOcorrencia)
        return "Contrato de Frete Transportador - Habilitar aba de Ocorrência";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaClientes)
        return "Contrato de Frete Transportador - Habilitar aba de Clientes";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaAcordo)
        return "Contrato de Frete Transportador - Habilitar aba de Acordo";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaAnexos)
        return "Contrato de Frete Transportador - Habilitar aba de Anexos";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaVeiculos)
        return "Contrato de Frete Transportador - Habilitar aba de Veículos";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaOutrosValores)
        return "Contrato de Frete Transportador - Habilitar aba de Outros Valores";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaFiliais)
        return "Contrato de Frete Transportador - Habilitar aba de Filiais";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaTipoOperacao)
        return "Contrato de Frete Transportador - Habilitar aba de Tipo de Operação";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaValoresFreteMinimo)
        return "Contrato de Frete Transportador - Habilitar aba de Valores de Frete Mínimo";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaFranquia)
        return "Contrato de Frete Transportador - Habilitar aba de Franquia";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaValoresVeiculos)
        return "Contrato de Frete Transportador - Habilitar aba de Valores Veículos";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_CalcularFreteNovamente)
        return "Carga - Calcular o Frete Novamente";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ChamadoOcorrencia_PermitirAssumirAtendimento)
        return "Atendimento - Permitir assumir atendimento";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ChamadoOcorrencia_PermitirFecharSemOcorrencia)
        return "Atendimento - Permitir fechar sem ocorrência";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ChamadoOcorrencia_PermitirLiberarParaOcorrencia)
        return "Atendimento - Permitir liberar para ocorrência";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.GrupoPessoas_AdicionarDespachanteComoConsignatario)
        return "Grupo de Pessoas - Permitir adicionar despachante como consignatário";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.MovimentacaoPneu_PermitirInformarHodometro)
        return "Movimentação de Pneu - Permitir informar Hodômetro (Km)";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Motorista_PermitirInativarCadastroMotorista)
        return "Motorista - Permitir ativar/inativar cadastro de motorista";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.MontagemCarga_AdicionarPrefixoNaCargaViaCarregamento)
        return "Montagem de Carga - Adicionar prefixo na carga via carregamento";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Pessoas_PermiteBloquearDesbloquearPessoa)
        return "Pessoas - Permite bloquear/desbloquear pessoa";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Canhotos_PermitirRegistrarAuditoriaNosCanhotos)
        return "Canhotos - Permitir Registrar Aduditoria nos Canhotos";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermitirDefinirMotoristaComVigenciaIndisponivel)
        return "Planejamento de Pedidos - Permitir definir Motorista com vigência indisponível";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Usuarios_PermitirEditarAsPermissoesDeAcesso)
        return "Usuário - Permitir editar as permissões de acesso";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Booking_PermitirCancelarBooking)
        return "Booking - Permitir cancelar Booking";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_PermitirAlterarModeloVeicularNaCarga)
        return "Carga - Permitir Alterar o Modelo Veicular na Carga";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Pedido_PermitirAlterarModeloVeicularNoPedido)
        return "Pedido - Permitir Alterar o Modelo Veicular no Pedido";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermitirAlterarPedidosCargasJaFinalizados)
        return "Planejamento de Pedidos - Permitir alterar Pedidos/Cargas já finalizados";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_PermitirReverterCargaNoShow)
        return "Carga - Permitir reverter Carga No-Show";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ContratoFinanciamento_PermitirAlterarVeiculos)
        return "Contrato Financiamento - Permitir Alterar Veiculos";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_PermitirNaoComprarValePedagio)
        return "Carga - Permitir não comprar vale pedágio";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_PermitirLiberarSemIntegracaoFrete)
        return "Carga - Permitir liberar sem integração de Frete";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Veiculo_PermitirSalvarLicenca)
        return "Veículo - Permitir Salvar Licença";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.RetiradaProdutoLista_NaoPermitirEditarAgendamento)
        return "Retirada Produto Lista - Não Permitir Editar Agendamento";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.RetiradaProdutoLista_NaoPermitirExcluirAgendamento)
        return "Retirada Produto Lista - Não Permitir Excluir Agendamento";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_PermiteAvancarCargaComRejeicaoPlanejamentoFrota)
        return "Carga - Permitir avançar a carga com rejeição do planejamento de frota";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Usuario_PermitirAtivarInativarVeiculo)
        return "Usuário - Permitir Ativar/Inativar Veículo";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_RemoverAnexosCarga)
        return "Carga - Remover Anexos da Carga";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.PagamentoMotorista_PermiteReverterPagamentoMotorista)
        return "Pagamento Motorista - Permitir Reverter Pagamento do Motorista";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FluxoPatio_PermiteVoltarEtapa)
        return "Fluxo Pátio - Permite Voltar Etapa";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Container_PermitirOperadorEditarTodasInformacoesContainer)
        return "Container - Permitir Operador Editar Todas as Informacoes do Container";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_PermitirLiberarSemLicencaValida)
        return "Carga - Permitir liberar sem licença válida";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_PermiteAlterarExternalId)
        return "Carga - Permitir alterar ExternalID";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_PermiteAvancarCargaComRejeitacaoGNRE)
        return "Carga - Permitir Avançar Carga Com Rejeição GNRE";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.CTeManual_PermiteRealizarCTeManualComValorZerado)
        return "CTe Manual - Permitir Realizar CTe Manual Com Valor Zerado";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_PermitirAdicionarRemoverPedidoEtapaUm)
        return "Carga - Permitir Adicionar ou Remover Pedido na Etapa 1 da Carga";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteGerarCarga)
        return "Planejamento Pedido TMS - Permitir Gerar Carga";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteInformarAlterarRemoverVeiculo)
        return "Planejamento Pedido TMS - Permitir Informar/Alterar/Remover Veículo";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteEnviarEmailTomador)
        return "Planejamento Pedido TMS - Permitir Enviar E-mail Para o Tomador";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteEnviarEmailCheckList)
        return "Planejamento Pedido TMS - Permitir Enviar E-mail Para o Check List";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteEnviarOrdemColetaTomador)
        return "Planejamento Pedido TMS - Permitir Enviar Ordem de Coleta Para o Tomador";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteInformarAlterarRemoverMotorista)
        return "Planejamento Pedido TMS - Permitir Informar/Alterar/Remover Motorista";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteInformarPendente)
        return "Planejamento Pedido TMS - Permitir Informar Pendente";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteAbrirDetalhePedido)
        return "Planejamento Pedido TMS - Permitir Abrir Detalhes do Pedido";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteGerarAvisoMotorista)
        return "Planejamento Pedido TMS - Permitir Gerar Aviso ao Motorista";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteGerarMotoristaCiente)
        return "Planejamento Pedido TMS - Permitir Gerar Motorista CLiente";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteAbrirDetalhesAviso)
        return "Planejamento Pedido TMS - Permitir Abrir Detalhes do Aviso";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteAbrirTratativas)
        return "Planejamento Pedido TMS - Permitir Abrir Tratativas";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteSubstituirModeloVeicular)
        return "Planejamento Pedido TMS - Permitir Substituir Modelo Veicular";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteAbrirWhatsAppMotorista)
        return "Planejamento Pedido TMS - Permitir Abrir WhatsApp Motorista";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteAlterarTipoDeOperacao)
        return "Planejamento Pedido TMS - Permitir Alterar Tipo de Operação";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteImprimirOrdemDeColeta)
        return "Planejamento Pedido TMS - Permitir Imprimir Ordem de Coleta";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteInformarDevolucao)
        return "Planejamento Pedido TMS - Permitir Informar Devolução";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteAlterarDataDePrevisaoDeSaida)
        return "Planejamento Pedido TMS - Permitir Alterar Data de Previsão de Saída";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteAlterarTipoDeCarga)
        return "Planejamento Pedido TMS - Permitir Alterar Tipo de Carga";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteEnviarEscala)
        return "Planejamento Pedido TMS - Permitir Enviar Escala";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_PermiteInserirAlterarVeiculoNaCargaOuPedido)
        return "Carga - Permitir Alterar/Inserir Veículo na Carga ou no Pedido";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.SugestaoMensal_PermiteGerarListaDiariaApartirListaMensal)
        return "Sugestão Mensal - Permitir Gerar a Lista Diária a partir da Lista Mensal";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Canhoto_PermiteConfirmarRecebimentoCanhotoFisico)
        return "Canhoto - Permitir Confirmar Recebimento do Canhoto Físico";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Canhoto_PermiteAdicionarJustificativa)
        return "Canhoto - Permitir Adicionar Justificativa";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_PermiteAlterarObservacaoContratoFreteTerceiro)
        return "Carga - Permitir Alterar a Observação do Contrato de Frete";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_PermiteRemoverContainerVinculadoEmCarga)
        return "Carga - Permitir Remover Conainers já vinculados em Cargas";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Home_VisualizarDashDoc)
        return "Home - Visualizar Dash Doc";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Pedido_PermitirAlteracaoMotorista)
        return "Pedido - Permitir alteração de motorista";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ControleDocumento_PermiteDesparquearDocumentos)
        return "Controle Documento - Permitir Desparquear Documentos";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.DocumentoDestinado_PermiteEmitirDesacordo)
        return "Documento Destinado - Permitir Emissão de Desacordo";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ComprovanteCarga_PermiteReverterComprovante)
        return "Comprovante de carga - Permitir Reverter Comprovante";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Pedido_PermiteInserirVeiculoProprio)
        return "Pedido - Permitir Inserir Veículo Próprio";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Pedido_PermiteInserirVeiculoTerceiro)
        return "Pedido - Permitir Inserir Veículo de Terceiro";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_PermiteInserirVeiculoProprio)
        return "Carga - Permitir Inserir Veículo Próprio";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_PermiteInserirVeiculoTerceiro)
        return "Carga - Permitir Inserir Veículo de Terceiro";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteInserirVeiculoProprio)
        return "Planejamento Pedidos TMS - Permitir Inserir Veículo Próprio";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteInserirVeiculoTerceiro)
        return "Planejamento Pedidos TMS - Permitir Inserir Veículo de Terceiro";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Transportador_PermiteInativarTransportador)
        return "Transportador - Permite Inativar Transportador";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Transportador_PermiteAlterarStatusFinanceiro)
        return "Transportador - Permite Alterar Status Financeiro para Com Pendencias";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ChamadoOcorrencia_PermiteFinalizarEmLiberadoParaOcorrencia)
        return "Atendimento - Permite Finalizar na Situação Liberado para Ocorrência";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_NaoPermiteAlterarOperador)
        return "Carga - Não Permite Alterar Operador";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.AuditoriaEMP_AcessoJustificativa)
        return "Auditoria EMP - Permite Inserir Justificativa";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_PermitirAjusteManualImportadasEmbarcador)
        return "Carga - Permitir ajuste manual nas cargas importadas do Embarcador";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.AuditoriaEMP_PermiteVisualizarJustificativa)
        return "Auditoria EMP - Permite Visualizar Justificativa";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Veiculo_PermitirAlterarModeloVeicular)
        return "Veiculo - Permite Alterar Modelo Veicular";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Veiculo_PermitirAcessoRastreador)
        return "Veiculo - Permite Acesso a Rastreador";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_PermitirAnexoDocumentoNaoContribuinte)
        return "Carga - Anexos de documentos não contribuinte";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Guias_PermiteAprovarRejeitarOCRGuiasManualmente)
        return "Guias - Permite Aprovar/Rejeitar OCR Guias Manualmente";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Guias_PermiteReverterAprovacaoOCRGuias)
        return "Guias - Permite reverter aprovação OCR guias";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Veiculo_PermiteEditarCompraValePedagio)
        return "Veiculo - Permite editar Compra de Vale Pedágio";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Veiculo_PermiteEditarCompraValePedagioRetorno)
        return "Veiculo - Permite editar Compra de Vale Pedágio Retorno";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ControleDocumento_PermitirLiberarDocumentoComIrregularidade)
        return "Controle Documento - Permitir liberar Documento com Irregularidade";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Motorista_PermitirDuplicarCadastro)
        return "Motorista - Permitir duplicar cadastro de motorista";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Acerto_PermitirAdicionarAlterarValorComprovadoSaldoViagem)
        return "Acerto - Permitir Adicionar/Alterar Valor Comprovado no saldo de Viagem";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_PermitirEmissaoDocumentosComFalhaIntegracaoAcrescimoDesconto)
        return "Carga - Permitir emissão de documentos com falha na integração de acréscimo e desconto";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ModuloControle_PermiteParquearDocumentos)
        return "Modulo de Controle - Permite Parquear Documentos";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.CancelamentoCargaLote_LiberarCancelamentoCargaBloqueada)
        return "Cancelamento Carga Lote - Liberar Cancelamento com Carga Bloqueada";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_LiberarCargaComValorLimiteApoliceDivergente)
        return "Carga - Liberar a Carga com o Valor Limite da Apólice divergente";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.VendaDireta_PermiteCancelamento)
        return "Venda Direta - Permitir Cancelar";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_PermiteFinalizarIntegracaoEtapa1Carga)
        return "Carga - Permite finalizar integração na etapa 1 da carga";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.TermoQuitacaoFinanceiro_PermiteExcluirTermoQuitacao)
        return "Termo Quitação Financeiro - Permitir Excluir";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.CargaEntrega_PermiteAlterarDataInicioFimPreTrip)
        return "Controle entrega - Permite alterar data de início e fim da pré trip";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.MonitoramentoMapa_PermiteAlterarAbaFiltrosPersonalizados)
        return "Monitoramento Mapa - Permite alterar aba de filtros personalizados";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_NaoPermiteReenviarIntegracaoCargasAppTrizy)
        return "Carga - Não permite reenviar a Integração app Trizy nas etapas 1 e 6";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_PermiteAcessarEtapasDocumentosQuandoAcessoEstiverRestrito)
        return "Carga - Permite acessar etapas de documentos quando acesso estiver restrito (tipo de operação)";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_AlterarValePedagio)
        return "Carga - Alterar Vale Pedágio";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_AlterarPercentualExecucao)
        return "Carga - Alterar % de Execução";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ChamadoOcorrencia_AprovarValorSuperior)
        return "Atendimento - Permite aprovar valores superiores no atendimento";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.CTeManual_VincularCTeEmbarcador)
        return "CT-e Manual - Vincular CT-e Embarcador";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ChamadoOcorrencia_PermitirEstornarOcorrencia)
        return "Chamado Ocorrência - Permitir estornar ocorrência";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.GrupoPessoas_PermitirAdicionarClienteAoGrupoCliente)
        return "Grupo de Pessoas - Permitir Adicionar Clientes ao Grupo de Cliente"

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Acerto_PermitirLancarDespesasAcertoViagem)
        return "Acerto - Permitir lançar despesas no Acerto de Viagem"

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Acerto_PermitirLancarDiariaAvulsoAcertoViagem)
        return "Acerto - Permitir lançar diária avulso no Acerto de Viagem"

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Canhoto_PermitirRetornarStatusCanhotoAPIDigitalizacao)
        return "Canhoto - Permitir retornar status do canhoto na API de digitalização"

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.QualidadeEntrega_PermiteLiberarNotasBloqueadas)
        return "Qualidade da entrega - Permitir liberar notas bloqueadas na qualidade da entrega";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.FluxoGestaoPatio_OcultarDetalhesCarga)
        return "Fluxo Gestão de Pátio - Ocultar detalhes da Carga";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.CargaCancelamento_PermitirModificarTipoCancelamento)
        return "Cancelamento de Carga - Permitir Modificar Tipo de Cancelamento";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Pagamento_Reverter)
        return "Pagamento - Reverter";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Pagamento_Reverter)
        return "Pagamento - Reverter";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.IntegracaoPorCTe_GerenciarNotasFiscaisAdicionais)
        return "Integração Por CTe - Permitir gerenciar Notas Fiscais Adicionais";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.ImportarAtendimentos)
        return "Atendimento - Permitir importar atendimentos";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_PermitirAlterarValorAdiantamentoEtapaContainer)
        return "Carga - Permitir Alterar Valor do Adiantamento na Etapa de Container";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.GestaoAtendimento_VerificarMeusIndicadores)
        return "Gestão Atendimentos - Verificar Meus Indicadores";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.GestaoAtendimento_VerificarIndicadoresGerais)
        return "Gestão Atendimentos - Verificar Indicadores Gerais";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_AlterarValorFreteApenasComTabelaFrete)
        return "Carga - Alterar valor frete apenas com tabela de frete";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.CargaCancelamento_NaoPermitirFinalizarEtapaIntegracao)
        return "Carga Cancelamento - Não permitir finalizar etapa de integração";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Carga_NaoPermitirCancelarValePedagio)
        return "Cargas - Não permitir Cancelar vale pedágio";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.GrupoPessoas_PermitirBloquearOuDesbloquearGrupoDePessoas)
        return "Grupo de Pessoas - Permitir bloquear ou desbloquear grupo de pessoas";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Target_PermiteEditarValor)
        return "Target - Permitir editar valor";

    if (permissaoPersonalizada == EnumPermissaoPersonalizada.Target_PermiteAprovarRejeitarRegistro)
        return "Target - Permitir aprovar/rejeitar registro";


    return "";
}

var _SelectPermissaoPersonalizada = [
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Criar), value: EnumPermissaoPersonalizada.Criar },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Alterar), value: EnumPermissaoPersonalizada.Alterar },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Excluir), value: EnumPermissaoPersonalizada.Excluir },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Cancelar), value: EnumPermissaoPersonalizada.Cancelar },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ReAbrir), value: EnumPermissaoPersonalizada.ReAbrir },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Finalizar), value: EnumPermissaoPersonalizada.Finalizar },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Acerto_PermiteLiberarAutorizarAbastecimento), value: EnumPermissaoPersonalizada.Acerto_PermiteLiberarAutorizarAbastecimento },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Acerto_PermiteLiberarAutorizarArla), value: EnumPermissaoPersonalizada.Acerto_PermiteLiberarAutorizarArla },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Acerto_PermiteLiberarPedagioAcerto), value: EnumPermissaoPersonalizada.Acerto_PermiteLiberarPedagioAcerto },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Acerto_PermiteAdicionarBonificacao), value: EnumPermissaoPersonalizada.Acerto_PermiteAdicionarBonificacao },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Acerto_PermiteAdicionarDesconto), value: EnumPermissaoPersonalizada.Acerto_PermiteAdicionarDesconto },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Acerto_OcultarResultadosViagem), value: EnumPermissaoPersonalizada.Acerto_OcultarResultadosViagem },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Acerto_PermiteFecharAcerto), value: EnumPermissaoPersonalizada.Acerto_PermiteFecharAcerto },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Acerto_PermiteReabirAcerto), value: EnumPermissaoPersonalizada.Acerto_PermiteReabirAcerto },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Acerto_PermiteInformarCargaFracionada), value: EnumPermissaoPersonalizada.Acerto_PermiteInformarCargaFracionada },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Acerto_PermiteInformarBonificacaoCliente), value: EnumPermissaoPersonalizada.Acerto_PermiteInformarBonificacaoCliente },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Acerto_PermiteAlterarReboquesCarga), value: EnumPermissaoPersonalizada.Acerto_PermiteAlterarReboquesCarga },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Acerto_PermiteAjustarKMTotal), value: EnumPermissaoPersonalizada.Acerto_PermiteAjustarKMTotal },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Fatura_PermiteIniciarFatura), value: EnumPermissaoPersonalizada.Fatura_PermiteIniciarFatura },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Fatura_PermiteCancelarCarga), value: EnumPermissaoPersonalizada.Fatura_PermiteCancelarCarga },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Fatura_PermiteAdicionarNovasCargas), value: EnumPermissaoPersonalizada.Fatura_PermiteAdicionarNovasCargas },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Fatura_PermiteSalvarCarga), value: EnumPermissaoPersonalizada.Fatura_PermiteSalvarCarga },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Fatura_PermiteRemoverConhecimento), value: EnumPermissaoPersonalizada.Fatura_PermiteRemoverConhecimento },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Fatura_PermiteSalvarValoresFechamento), value: EnumPermissaoPersonalizada.Fatura_PermiteSalvarValoresFechamento },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Fatura_PermiteGerarParcelas), value: EnumPermissaoPersonalizada.Fatura_PermiteGerarParcelas },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Fatura_PermiteFecharFatura), value: EnumPermissaoPersonalizada.Fatura_PermiteFecharFatura },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Fatura_PermiteVisualizarFatura), value: EnumPermissaoPersonalizada.Fatura_PermiteVisualizarFatura },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Fatura_PermiteReAbrirFatura), value: EnumPermissaoPersonalizada.Fatura_PermiteReAbrirFatura },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Fatura_PermiteEnviarEDI), value: EnumPermissaoPersonalizada.Fatura_PermiteEnviarEDI },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Fatura_PermiteEnviarFatura), value: EnumPermissaoPersonalizada.Fatura_PermiteEnviarFatura },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.BaixaReceber_PermiteBaixarTitulo), value: EnumPermissaoPersonalizada.BaixaReceber_PermiteBaixarTitulo },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.BaixaReceber_PermiteSalvarValores), value: EnumPermissaoPersonalizada.BaixaReceber_PermiteSalvarValores },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.BaixaReceber_PermiteGerarParcelas), value: EnumPermissaoPersonalizada.BaixaReceber_PermiteGerarParcelas },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.BaixaReceber_PermiteFecharBaixa), value: EnumPermissaoPersonalizada.BaixaReceber_PermiteFecharBaixa },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.BaixaPagar_PermiteBaixarTitulo), value: EnumPermissaoPersonalizada.BaixaPagar_PermiteBaixarTitulo },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.BaixaPagar_PermiteSalvarValores), value: EnumPermissaoPersonalizada.BaixaPagar_PermiteSalvarValores },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.BaixaPagar_PermiteGerarParcelas), value: EnumPermissaoPersonalizada.BaixaPagar_PermiteGerarParcelas },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.BaixaPagar_PermiteFecharBaixa), value: EnumPermissaoPersonalizada.BaixaPagar_PermiteFecharBaixa },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Fatura_PermiteLiquidarFatura), value: EnumPermissaoPersonalizada.Fatura_PermiteLiquidarFatura },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ComissaoFuncionario_PermiteEditarDadosGerados), value: EnumPermissaoPersonalizada.ComissaoFuncionario_PermiteEditarDadosGerados },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ContratoFrete_PermiteAutorizarContrato), value: EnumPermissaoPersonalizada.ContratoFrete_PermiteAutorizarContrato },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ContratoFrete_PermiteBloquearContrato), value: EnumPermissaoPersonalizada.ContratoFrete_PermiteBloquearContrato },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ContratoFrete_PermiteInformarValorFrete), value: EnumPermissaoPersonalizada.ContratoFrete_PermiteInformarValorFrete },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ContratoFrete_PermiteInformarValorPedagio), value: EnumPermissaoPersonalizada.ContratoFrete_PermiteInformarValorPedagio },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ContratoFrete_PermiteInformarAcrescimoDesconto), value: EnumPermissaoPersonalizada.ContratoFrete_PermiteInformarAcrescimoDesconto },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ContratoFrete_PermiteInformarPercentualAdiantamento), value: EnumPermissaoPersonalizada.ContratoFrete_PermiteInformarPercentualAdiantamento },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_SalvarDadosTransporte), value: EnumPermissaoPersonalizada.Carga_SalvarDadosTransporte },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_InformarDocumentosFiscais), value: EnumPermissaoPersonalizada.Carga_InformarDocumentosFiscais },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_AlterarValorFrete), value: EnumPermissaoPersonalizada.Carga_AlterarValorFrete },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_AutorizarFreteInconsistente), value: EnumPermissaoPersonalizada.Carga_AutorizarFreteInconsistente },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_AdicionarComponentes), value: EnumPermissaoPersonalizada.Carga_AdicionarComponentes },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_AlterarFreteTerceiros), value: EnumPermissaoPersonalizada.Carga_AlterarFreteTerceiros },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_AlterarConfiguracao), value: EnumPermissaoPersonalizada.Carga_AlterarConfiguracao },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_AlterarObservacao), value: EnumPermissaoPersonalizada.Carga_AlterarObservacao },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_AlterarDadosPedido), value: EnumPermissaoPersonalizada.Carga_AlterarDadosPedido },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_AlterarDadosSeguro), value: EnumPermissaoPersonalizada.Carga_AlterarDadosSeguro },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_AutorizarSeguro), value: EnumPermissaoPersonalizada.Carga_AutorizarSeguro },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_AlterarRota), value: EnumPermissaoPersonalizada.Carga_AlterarRota },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_AlterarLacres), value: EnumPermissaoPersonalizada.Carga_AlterarLacres },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_AlterarPassagens), value: EnumPermissaoPersonalizada.Carga_AlterarPassagens },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_AlterarPercurso), value: EnumPermissaoPersonalizada.Carga_AlterarPercurso },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_AutorizarEmissaoDocumentos), value: EnumPermissaoPersonalizada.Carga_AutorizarEmissaoDocumentos },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_RetornarEtapaNotasFiscais), value: EnumPermissaoPersonalizada.Carga_RetornarEtapaNotasFiscais },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_EditarCTe), value: EnumPermissaoPersonalizada.Carga_EditarCTe },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_ConfirmarIntegracao), value: EnumPermissaoPersonalizada.Carga_ConfirmarIntegracao },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_ConfirmarImpressao), value: EnumPermissaoPersonalizada.Carga_ConfirmarImpressao },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_AutorizarModalidadePagamentoNota), value: EnumPermissaoPersonalizada.Carga_AutorizarModalidadePagamentoNota },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_LiberarAverbacaoRejeitada), value: EnumPermissaoPersonalizada.Carga_LiberarAverbacaoRejeitada },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_FinalizarCargaMDFeRejeitado), value: EnumPermissaoPersonalizada.Carga_FinalizarCargaMDFeRejeitado },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_ReenviarIntegracoes), value: EnumPermissaoPersonalizada.Carga_ReenviarIntegracoes },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_PermiteAlterarTransportador), value: EnumPermissaoPersonalizada.Carga_PermiteAlterarTransportador },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_PermiteLiberarComDiferencaNoValorFrete), value: EnumPermissaoPersonalizada.Carga_PermiteLiberarComDiferencaNoValorFrete },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_AutorizarPesoCarga), value: EnumPermissaoPersonalizada.Carga_AutorizarPesoCarga },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_AutorizarValorMaximoPendentePagamento), value: EnumPermissaoPersonalizada.Carga_AutorizarValorMaximoPendentePagamento },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_PermiteAlterarInclusaoICMS), value: EnumPermissaoPersonalizada.Carga_PermiteAlterarInclusaoICMS },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_PermitirRemoverPedido), value: EnumPermissaoPersonalizada.Carga_PermitirRemoverPedido },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_AutorizarManutencaoPendenteVeiculo), value: EnumPermissaoPersonalizada.Carga_AutorizarManutencaoPendenteVeiculo },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_DownloadArquivoIntegracoes), value: EnumPermissaoPersonalizada.Carga_DownloadArquivoIntegracoes },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_PermiteAdicionarOutrosDocumentos), value: EnumPermissaoPersonalizada.Carga_PermiteAdicionarOutrosDocumentos },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_PermitirAvancarCargaSemTodosPreCte), value: EnumPermissaoPersonalizada.Carga_PermitirAvancarCargaSemTodosPreCte },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_EmitirCartaCorrecao), value: EnumPermissaoPersonalizada.Carga_EmitirCartaCorrecao },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.CargaEntrega_PermiteIniciarFinalizarViagensEntregasManualmente), value: EnumPermissaoPersonalizada.CargaEntrega_PermiteIniciarFinalizarViagensEntregasManualmente },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_AlterarMoeda), value: EnumPermissaoPersonalizada.Carga_AlterarMoeda },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.DocumentoEntrada_LancarDuplicata), value: EnumPermissaoPersonalizada.DocumentoEntrada_LancarDuplicata },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.MovimentoFinanceiro_InformarCentroResultado), value: EnumPermissaoPersonalizada.MovimentoFinanceiro_InformarCentroResultado },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.MovimentoFinanceiro_InformarColaborador), value: EnumPermissaoPersonalizada.MovimentoFinanceiro_InformarColaborador },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.MovimentoFinanceiro_InformarPlanoEntradaSaida), value: EnumPermissaoPersonalizada.MovimentoFinanceiro_InformarPlanoEntradaSaida },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.CargaCancelamento_Anular), value: EnumPermissaoPersonalizada.CargaCancelamento_Anular },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.CargaCancelamento_ReenviarCancelamentoComoAnulacao), value: EnumPermissaoPersonalizada.CargaCancelamento_ReenviarCancelamentoComoAnulacao },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.CargaCancelamento_AdicionarComoCancelamento), value: EnumPermissaoPersonalizada.CargaCancelamento_AdicionarComoCancelamento },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.MontagemCarga_LiberacaoVeiculo), value: EnumPermissaoPersonalizada.MontagemCarga_LiberacaoVeiculo },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Ocorrencia_PermitirRetornarEtapaCadastro), value: EnumPermissaoPersonalizada.Ocorrencia_PermitirRetornarEtapaCadastro },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.OcorrenciaCancelamento_Anular), value: EnumPermissaoPersonalizada.OcorrenciaCancelamento_Anular },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.OcorrenciaCancelamento_ReenviarCancelamentoComoAnulacao), value: EnumPermissaoPersonalizada.OcorrenciaCancelamento_ReenviarCancelamentoComoAnulacao },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.OcorrenciaCancelamento_AdicionarComoCancelamento), value: EnumPermissaoPersonalizada.OcorrenciaCancelamento_AdicionarComoCancelamento },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarDoca), value: EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarDoca },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarGuaritaEntrada), value: EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarGuaritaEntrada },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarChecklist), value: EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarChecklist },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FluxoGestaoPatio_TravarChave), value: EnumPermissaoPersonalizada.FluxoGestaoPatio_TravarChave },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarExpedicao), value: EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarExpedicao },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FluxoGestaoPatio_LiberarChave), value: EnumPermissaoPersonalizada.FluxoGestaoPatio_LiberarChave },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarInicioViagem), value: EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarInicioViagem },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarChegadaVeiculo), value: EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarChegadaVeiculo },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarDeslocamentoPatio), value: EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarDeslocamentoPatio },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FluxoGestaoPatio_PermiteRetornarEtapaSaidaCD), value: EnumPermissaoPersonalizada.FluxoGestaoPatio_PermiteRetornarEtapaSaidaCD },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FechamentoFrete_Finalizar), value: EnumPermissaoPersonalizada.FechamentoFrete_Finalizar },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FechamentoFrete_Reabrir), value: EnumPermissaoPersonalizada.FechamentoFrete_Reabrir },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FechamentoFrete_Cancelar), value: EnumPermissaoPersonalizada.FechamentoFrete_Cancelar },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.PreCarga_Supervisor), value: EnumPermissaoPersonalizada.PreCarga_Supervisor },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.CargaCancelamento_LiberarAverbacaoRejeitada), value: EnumPermissaoPersonalizada.CargaCancelamento_LiberarAverbacaoRejeitada },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.WMS_Autorizar_Volumes_Faltantes), value: EnumPermissaoPersonalizada.WMS_Autorizar_Volumes_Faltantes },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Pallets_PermiteDataRetroativa_ValePallet), value: EnumPermissaoPersonalizada.Pallets_PermiteDataRetroativa_ValePallet },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Pallets_PermiteDataRetroativa_DevolucaoPallet), value: EnumPermissaoPersonalizada.Pallets_PermiteDataRetroativa_DevolucaoPallet },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Canhotos_ReverterJustificativa_Canhotos), value: EnumPermissaoPersonalizada.Canhotos_ReverterJustificativa_Canhotos },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Canhoto_PermitirReverterImagem), value: EnumPermissaoPersonalizada.Canhoto_PermitirReverterImagem },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.DocumentoEntrada_AutorizarPrecoCombustivelDiferenteFornecedor), value: EnumPermissaoPersonalizada.DocumentoEntrada_AutorizarPrecoCombustivelDiferenteFornecedor },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.VendaDireta_PermitirFinalizar), value: EnumPermissaoPersonalizada.VendaDireta_PermitirFinalizar },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.VendaDireta_PermitirAlterarParcelamento), value: EnumPermissaoPersonalizada.VendaDireta_PermitirAlterarParcelamento },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Titulo_GerarMultiplosTitulos), value: EnumPermissaoPersonalizada.Titulo_GerarMultiplosTitulos },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Usuario_PermiteRemoverAnexos), value: EnumPermissaoPersonalizada.Usuario_PermiteRemoverAnexos },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Veiculo_PermiteRemoverAnexos), value: EnumPermissaoPersonalizada.Veiculo_PermiteRemoverAnexos },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Veiculo_PermiteAlterarPlaca), value: EnumPermissaoPersonalizada.Veiculo_PermiteAlterarPlaca },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Veiculo_PermitirSalvarLicenca), value: EnumPermissaoPersonalizada.Veiculo_PermitirSalvarLicenca },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Veiculo_PermitirAlterarDadosIntegracaoGR), value: EnumPermissaoPersonalizada.Veiculo_PermitirAlterarDadosIntegracaoGR },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Veiculo_PermiteBloquearVeiculo), value: EnumPermissaoPersonalizada.Veiculo_PermiteBloquearVeiculo },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Motorista_PermiteRemoverAnexos), value: EnumPermissaoPersonalizada.Motorista_PermiteRemoverAnexos },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.CTeManual_PermiteCancelarCTe), value: EnumPermissaoPersonalizada.CTeManual_PermiteCancelarCTe },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FilaCarregamento_PermitirAceitarCarga), value: EnumPermissaoPersonalizada.FilaCarregamento_PermitirAceitarCarga },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FilaCarregamento_PermitirAdicionar), value: EnumPermissaoPersonalizada.FilaCarregamento_PermitirAdicionar },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FilaCarregamento_PermitirAlocarCarga), value: EnumPermissaoPersonalizada.FilaCarregamento_PermitirAlocarCarga },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FilaCarregamento_PermitirAlterarCentroCarregamento), value: EnumPermissaoPersonalizada.FilaCarregamento_PermitirAlterarCentroCarregamento },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FilaCarregamento_PermitirAlterarPrimeiraPosicao), value: EnumPermissaoPersonalizada.FilaCarregamento_PermitirAlterarPrimeiraPosicao },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FilaCarregamento_PermitirAlterarUltimaPosicao), value: EnumPermissaoPersonalizada.FilaCarregamento_PermitirAlterarUltimaPosicao },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FilaCarregamento_PermitirConfirmarChegada), value: EnumPermissaoPersonalizada.FilaCarregamento_PermitirConfirmarChegada },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FilaCarregamento_PermitirDesatrelarTracao), value: EnumPermissaoPersonalizada.FilaCarregamento_PermitirDesatrelarTracao },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FilaCarregamento_PermitirEnviarNotificacao), value: EnumPermissaoPersonalizada.FilaCarregamento_PermitirEnviarNotificacao },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FilaCarregamento_PermitirInformarConjuntoMotorista), value: EnumPermissaoPersonalizada.FilaCarregamento_PermitirInformarConjuntoMotorista },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FilaCarregamento_PermitirInformarConjuntoVeiculo), value: EnumPermissaoPersonalizada.FilaCarregamento_PermitirInformarConjuntoVeiculo },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FilaCarregamento_PermitirLiberar), value: EnumPermissaoPersonalizada.FilaCarregamento_PermitirLiberar },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FilaCarregamento_PermitirLiberarSaidaFila), value: EnumPermissaoPersonalizada.FilaCarregamento_PermitirLiberarSaidaFila },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FilaCarregamento_PermitirRecusarCarga), value: EnumPermissaoPersonalizada.FilaCarregamento_PermitirRecusarCarga },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FilaCarregamento_PermitirRemover), value: EnumPermissaoPersonalizada.FilaCarregamento_PermitirRemover },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FilaCarregamento_PermitirRemoverConjuntoMotorista), value: EnumPermissaoPersonalizada.FilaCarregamento_PermitirRemoverConjuntoMotorista },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FilaCarregamento_PermitirRemoverReversa), value: EnumPermissaoPersonalizada.FilaCarregamento_PermitirRemoverReversa },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FilaCarregamento_PermitirRemoverTracao), value: EnumPermissaoPersonalizada.FilaCarregamento_PermitirRemoverTracao },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FilaCarregamento_PermitirReposicionar), value: EnumPermissaoPersonalizada.FilaCarregamento_PermitirReposicionar },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.CargaCancelamento_CancelarCargaBloqueada), value: EnumPermissaoPersonalizada.CargaCancelamento_CancelarCargaBloqueada },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.AcordoFaturamento_PermiteAlterarApenasEmail), value: EnumPermissaoPersonalizada.AcordoFaturamento_PermiteAlterarApenasEmail },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.GrupoPessoas_PermiteAlterarApenasObservacoes), value: EnumPermissaoPersonalizada.GrupoPessoas_PermiteAlterarApenasObservacoes },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.CTeManual_PermiteAnulacaoGerencialCTe), value: EnumPermissaoPersonalizada.CTeManual_PermiteAnulacaoGerencialCTe },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ImpressaoLoteCarga_ObrigarSelecionarTerminalDestino), value: EnumPermissaoPersonalizada.ImpressaoLoteCarga_ObrigarSelecionarTerminalDestino },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.CargaCancelamento_LiberarCancelamentoComCTeNaoInutilizado), value: EnumPermissaoPersonalizada.CargaCancelamento_LiberarCancelamentoComCTeNaoInutilizado },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.BaixaPagar_PermiteCancelarBaixa), value: EnumPermissaoPersonalizada.BaixaPagar_PermiteCancelarBaixa },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.BaixaPagar_NaoPermiteQuitarTitulo), value: EnumPermissaoPersonalizada.BaixaPagar_NaoPermiteQuitarTitulo },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Pedido_PermiteIncluirPessoa), value: EnumPermissaoPersonalizada.Pedido_PermiteIncluirPessoa },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Pedido_PermiteAlterarTipoTomador), value: EnumPermissaoPersonalizada.Pedido_PermiteAlterarTipoTomador },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Pessoa_PermiteCriarFornecedor), value: EnumPermissaoPersonalizada.Pessoa_PermiteCriarFornecedor },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.PagamentoMotorista_PermiteAvancarSemIntegracao), value: EnumPermissaoPersonalizada.PagamentoMotorista_PermiteAvancarSemIntegracao },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Pedido_PermitePreencherValoresFrete), value: EnumPermissaoPersonalizada.Pedido_PermitePreencherValoresFrete },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Fatura_BloquearAcrescimoDesconto), value: EnumPermissaoPersonalizada.Fatura_BloquearAcrescimoDesconto },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.DocumentoEntrada_BloquearLancamentoComDataRetroativa), value: EnumPermissaoPersonalizada.DocumentoEntrada_BloquearLancamentoComDataRetroativa },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Pedido_BloquearDuplicarPedido), value: EnumPermissaoPersonalizada.Pedido_BloquearDuplicarPedido },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ArquivoContabil_RemoverObrigatoriedadeTeminalAtracacao), value: EnumPermissaoPersonalizada.ArquivoContabil_RemoverObrigatoriedadeTeminalAtracacao },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.OcorrenciaCancelamento_LiberarCancelamentoComCTeNaoInutilizado), value: EnumPermissaoPersonalizada.OcorrenciaCancelamento_LiberarCancelamentoComCTeNaoInutilizado },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.DocumentoDestinado_BloquearGeracaoManifestacao), value: EnumPermissaoPersonalizada.DocumentoDestinado_BloquearGeracaoManifestacao },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.DocumentoDestinado_BloquearGeracaoDocumentoEntrada), value: EnumPermissaoPersonalizada.DocumentoDestinado_BloquearGeracaoDocumentoEntrada },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ImpressaoCargaLote_ObrigarInformarTerminalDestino), value: EnumPermissaoPersonalizada.ImpressaoCargaLote_ObrigarInformarTerminalDestino },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Motorista_PermiteZerarSaldo), value: EnumPermissaoPersonalizada.Motorista_PermiteZerarSaldo },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FaturamentoLote_RetirarCamposObrigatorios), value: EnumPermissaoPersonalizada.FaturamentoLote_RetirarCamposObrigatorios },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.AlteracaoArquivoMercante_RemoverObrigatoriedadeTerminalAtracacao), value: EnumPermissaoPersonalizada.AlteracaoArquivoMercante_RemoverObrigatoriedadeTerminalAtracacao },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.BaixaPagar_NaoPermitirLancarDescontoAcrescimo), value: EnumPermissaoPersonalizada.BaixaPagar_NaoPermitirLancarDescontoAcrescimo },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.BaixaReceber_NaoPermitirLancarDescontoAcrescimo), value: EnumPermissaoPersonalizada.BaixaReceber_NaoPermitirLancarDescontoAcrescimo },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.BaixaReceberNovo_NaoPermitirLancarDescontoAcrescimo), value: EnumPermissaoPersonalizada.BaixaReceberNovo_NaoPermitirLancarDescontoAcrescimo },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.OrdemServico_NaoPermitirLancarDescontoFechamento), value: EnumPermissaoPersonalizada.OrdemServico_NaoPermitirLancarDescontoFechamento },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Monitoramento_AlterarOuExcluirHistoricoDeStatusDoMonitoramento), value: EnumPermissaoPersonalizada.Monitoramento_AlterarOuExcluirHistoricoDeStatusDoMonitoramento },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.BaixaPagar_NaoPermitirCancelarBaixaComPagamentoEletronico), value: EnumPermissaoPersonalizada.BaixaPagar_NaoPermitirCancelarBaixaComPagamentoEletronico },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.BaixaReceber_NaoPermitirLancarDesconto), value: EnumPermissaoPersonalizada.BaixaReceber_NaoPermitirLancarDesconto },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.BaixaReceber_NaoPermitirLancarAcrescimo), value: EnumPermissaoPersonalizada.BaixaReceber_NaoPermitirLancarAcrescimo },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.BaixaReceberNovo_NaoPermitirLancarDesconto), value: EnumPermissaoPersonalizada.BaixaReceberNovo_NaoPermitirLancarDesconto },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.BaixaReceberNovo_NaoPermitirLancarAcrescimo), value: EnumPermissaoPersonalizada.BaixaReceberNovo_NaoPermitirLancarAcrescimo },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Fatura_NaoPermitirLancarDesconto), value: EnumPermissaoPersonalizada.Fatura_NaoPermitirLancarDesconto },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Fatura_NaoPermitirLancarAcrescimo), value: EnumPermissaoPersonalizada.Fatura_NaoPermitirLancarAcrescimo },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Pedido_PermiteCriarPedidoTomadorSemCredito), value: EnumPermissaoPersonalizada.Pedido_PermiteCriarPedidoTomadorSemCredito },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_LiberarPagamentoMotoristaRejeitado), value: EnumPermissaoPersonalizada.Carga_LiberarPagamentoMotoristaRejeitado },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.JanelaDescarga_PermiteAlocarJanelaExtra), value: EnumPermissaoPersonalizada.JanelaDescarga_PermiteAlocarJanelaExtra },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_PermiteAvancarLicencaInvalida), value: EnumPermissaoPersonalizada.Carga_PermiteAvancarLicencaInvalida },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Acerto_PermiteCancelamento), value: EnumPermissaoPersonalizada.Acerto_PermiteCancelamento },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_LiberarCargaSemConfirmacaoERP), value: EnumPermissaoPersonalizada.Carga_LiberarCargaSemConfirmacaoERP },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Pessoa_PermiteAlterarSituacaoFinanceira), value: EnumPermissaoPersonalizada.Pessoa_PermiteAlterarSituacaoFinanceira },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FluxoGestaoPatio_PermiteCancelarFluxo), value: EnumPermissaoPersonalizada.FluxoGestaoPatio_PermiteCancelarFluxo },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.EncerramentoCarga_EncerrarCarga), value: EnumPermissaoPersonalizada.EncerramentoCarga_EncerrarCarga },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.EncerramentoCarga_EncerrarMDFe), value: EnumPermissaoPersonalizada.EncerramentoCarga_EncerrarMDFe },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Fatura_PermiteDuplicar), value: EnumPermissaoPersonalizada.Fatura_PermiteDuplicar },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.AbastecimentoGas_PermiteLancarAbastecimentoAposHorarioLimite), value: EnumPermissaoPersonalizada.AbastecimentoGas_PermiteLancarAbastecimentoAposHorarioLimite },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Acerto_BloquearEdicaoResumoAbastecimento), value: EnumPermissaoPersonalizada.Acerto_BloquearEdicaoResumoAbastecimento },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ContratoFreteAcrescimoDesconto_PermiteLiberarComIntegracaoRejeitada), value: EnumPermissaoPersonalizada.ContratoFreteAcrescimoDesconto_PermiteLiberarComIntegracaoRejeitada },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.JanelaDescarga_SobreporRegras), value: EnumPermissaoPersonalizada.JanelaDescarga_SobreporRegras },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Motorista_NaoPermitirAlterarCentroDeResultado), value: EnumPermissaoPersonalizada.Motorista_NaoPermitirAlterarCentroDeResultado },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.GrupoPessoas_NaoPermitirAlterarValorLimiteFaturamento), value: EnumPermissaoPersonalizada.GrupoPessoas_NaoPermitirAlterarValorLimiteFaturamento },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_PermiteTrocarPedidosDefinitivosUFsDestinatariosDiferentes), value: EnumPermissaoPersonalizada.Carga_PermiteTrocarPedidosDefinitivosUFsDestinatariosDiferentes },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ProgramacaoVeiculo_PermiteVisualizarAuditoria), value: EnumPermissaoPersonalizada.ProgramacaoVeiculo_PermiteVisualizarAuditoria },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Fatura_NaoPermitirEditarDataVencimentoParcela), value: EnumPermissaoPersonalizada.Fatura_NaoPermitirEditarDataVencimentoParcela },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.CargaEntrega_PermiteAtualizarCoordenadasDoCliente), value: EnumPermissaoPersonalizada.CargaEntrega_PermiteAtualizarCoordenadasDoCliente },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.CargaEntrega_PermiteEditarPesquisa), value: EnumPermissaoPersonalizada.CargaEntrega_PermiteEditarPesquisa },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ContratoFreteAcrescimoDesconto_PermiteLiberarPagamento), value: EnumPermissaoPersonalizada.ContratoFreteAcrescimoDesconto_PermiteLiberarPagamento },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FinanceiroBloquearInformacaoDataBaixaReceberCte), value: EnumPermissaoPersonalizada.FinanceiroBloquearInformacaoDataBaixaReceberCte },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.CargaEntrega_PermiteAlterarDataOcorrencias), value: EnumPermissaoPersonalizada.CargaEntrega_PermiteAlterarDataOcorrencias },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.CargaEntrega_PermiteAlterarDataInicioFimViagem), value: EnumPermissaoPersonalizada.CargaEntrega_PermiteAlterarDataInicioFimViagem },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.CargaEntrega_PermiteAlterarDataEntrega), value: EnumPermissaoPersonalizada.CargaEntrega_PermiteAlterarDataEntrega },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_PermiteLiberarSemIntegracaoGR), value: EnumPermissaoPersonalizada.Carga_PermiteLiberarSemIntegracaoGR },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.GestaoDocumento_PermitirDesfazerAprovacao), value: EnumPermissaoPersonalizada.GestaoDocumento_PermitirDesfazerAprovacao },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ChamadoOcorrencia_PermitirDelegarParaOutroUsuario), value: EnumPermissaoPersonalizada.ChamadoOcorrencia_PermitirDelegarParaOutroUsuario },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ChamadoOcorrencia_PermitirDelegarParaUmSetor), value: EnumPermissaoPersonalizada.ChamadoOcorrencia_PermitirDelegarParaUmSetor },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Monitoramento_InformarDataEntradaSaidaRaio), value: EnumPermissaoPersonalizada.Monitoramento_InformarDataEntradaSaidaRaio },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarChegadaLoja), value: EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarChegadaLoja },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarFimViagem), value: EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarFimViagem },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarInicioHigienizacao), value: EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarInicioHigienizacao },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarFimHigienizacao), value: EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarFimHigienizacao },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarInicioCarregamento), value: EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarInicioCarregamento },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarFimCarregamento), value: EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarFimCarregamento },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FluxoGestaoPatio_SolicitarVeiculo), value: EnumPermissaoPersonalizada.FluxoGestaoPatio_SolicitarVeiculo },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarInicioDescarregamento), value: EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarInicioDescarregamento },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarFimDescarregamento), value: EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarFimDescarregamento },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarDocumentoFiscal), value: EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarDocumentoFiscal },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarDocumentoTransporte), value: EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarDocumentoTransporte },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarMontagemCarga), value: EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarMontagemCarga },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarSeparacaoMercadoria), value: EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarSeparacaoMercadoria },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Acerto_PermitirRemoverAdiantamento), value: EnumPermissaoPersonalizada.Acerto_PermitirRemoverAdiantamento },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.PagamentoMotorista_PermiteInformarDataPagamentoRetroativa), value: EnumPermissaoPersonalizada.PagamentoMotorista_PermiteInformarDataPagamentoRetroativa },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_PermiteExcluirPreCalculoFrete), value: EnumPermissaoPersonalizada.Carga_PermiteExcluirPreCalculoFrete },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_PermiteReabrirCargaFinalizada), value: EnumPermissaoPersonalizada.Carga_PermiteReabrirCargaFinalizada },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ComissaoFuncionario_PermitirAlterarMedia), value: EnumPermissaoPersonalizada.ComissaoFuncionario_PermitirAlterarMedia },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.CIOT_EncerrarGerencialmente), value: EnumPermissaoPersonalizada.CIOT_EncerrarGerencialmente },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Motorista_PermitirInativarComSaldo), value: EnumPermissaoPersonalizada.Motorista_PermitirInativarComSaldo },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_PermiteAvancarCargaComRejeitacaoValePedagio), value: EnumPermissaoPersonalizada.Carga_PermiteAvancarCargaComRejeitacaoValePedagio },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FluxoGestaoPatio_PermiteReavaliarChecklist), value: EnumPermissaoPersonalizada.FluxoGestaoPatio_PermiteReavaliarChecklist },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FluxoGestaoPatio_PermiteVoltarEtapaFaturamento), value: EnumPermissaoPersonalizada.FluxoGestaoPatio_PermiteVoltarEtapaFaturamento },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Abastecimento_PermitirAlterarQuilometragemAbastecimentoFechado), value: EnumPermissaoPersonalizada.Abastecimento_PermitirAlterarQuilometragemAbastecimentoFechado },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.CargaCancelamento_PermitirDisponibilizarCTesParaVincularEmOutraCarga), value: EnumPermissaoPersonalizada.CargaCancelamento_PermitirDisponibilizarCTesParaVincularEmOutraCarga },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Acerto_PermiteFinalizarSemCanhotosRecebidos), value: EnumPermissaoPersonalizada.Acerto_PermiteFinalizarSemCanhotosRecebidos },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Acerto_PermiteFinalizarSemPalletsEntregues), value: EnumPermissaoPersonalizada.Acerto_PermiteFinalizarSemPalletsEntregues },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.AbastecimentoGas_PermiteAdicionarVolumeExtraSolicitacaoGas), value: EnumPermissaoPersonalizada.AbastecimentoGas_PermiteAdicionarVolumeExtraSolicitacaoGas },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ControleContainer_PermiteMovimentarContainer), value: EnumPermissaoPersonalizada.ControleContainer_PermiteMovimentarContainer },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.CargaEntrega_PermiteGerarOcorrenciaEstadia), value: EnumPermissaoPersonalizada.CargaEntrega_PermiteGerarOcorrenciaEstadia },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.GrupoPessoas_PermiteAlterarSituacaoFinanceira), value: EnumPermissaoPersonalizada.GrupoPessoas_PermiteAlterarSituacaoFinanceira },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.CargaEntrega_PermiteAlterarSituacaoOnTime), value: EnumPermissaoPersonalizada.CargaEntrega_PermiteAlterarSituacaoOnTime },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.DocumentoEntrada_PermitirAtualizarNotaCancelada), value: EnumPermissaoPersonalizada.DocumentoEntrada_PermitirAtualizarNotaCancelada },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Motorista_PermiteBloquearMotorista), value: EnumPermissaoPersonalizada.Motorista_PermiteBloquearMotorista },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Transportador_PermiteBloquearTransportador), value: EnumPermissaoPersonalizada.Transportador_PermiteBloquearTransportador },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteDefinirVeiculoMotoristaLicencaVencida), value: EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteDefinirVeiculoMotoristaLicencaVencida },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.GrupoPessoas_PermiteAlterarConfiguracaoEmissao), value: EnumPermissaoPersonalizada.GrupoPessoas_PermiteAlterarConfiguracaoEmissao },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.GrupoPessoas_PermiteAlterarConfiguracaoFatura), value: EnumPermissaoPersonalizada.GrupoPessoas_PermiteAlterarConfiguracaoFatura },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Pessoa_PermiteAlterarConfiguracaoEmissao), value: EnumPermissaoPersonalizada.Pessoa_PermiteAlterarConfiguracaoEmissao },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Pessoa_PermiteAlterarConfiguracaoFatura), value: EnumPermissaoPersonalizada.Pessoa_PermiteAlterarConfiguracaoFatura },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ChamadoOcorrencia_PermitirFinalizarMesmoNaoSendoResponsavel), value: EnumPermissaoPersonalizada.ChamadoOcorrencia_PermitirFinalizarMesmoNaoSendoResponsavel },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_PermitirLiberarSemRetiradaContainer), value: EnumPermissaoPersonalizada.Carga_PermitirLiberarSemRetiradaContainer },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.MovimentacaoPneu_PermitirSucatearPneu), value: EnumPermissaoPersonalizada.MovimentacaoPneu_PermitirSucatearPneu },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ChamadoOcorrencia_PermitirAlterarValorEMotivoChamado), value: EnumPermissaoPersonalizada.ChamadoOcorrencia_PermitirAlterarValorEMotivoChamado },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Abastecimento_PermitirAlterarHorimetroAbastecimentoFechado), value: EnumPermissaoPersonalizada.Abastecimento_PermitirAlterarHorimetroAbastecimentoFechado },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.BaixaTituloReceberNovo_PermiteFinalizarBaixaTitulo), value: EnumPermissaoPersonalizada.BaixaTituloReceberNovo_PermiteFinalizarBaixaTitulo },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ContratoFreteTransportador_DisponibilizarParaTodosVeiculos), value: EnumPermissaoPersonalizada.ContratoFreteTransportador_DisponibilizarParaTodosVeiculos },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaDadosContrato), value: EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaDadosContrato },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaOcorrencia), value: EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaOcorrencia },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaClientes), value: EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaClientes },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaAcordo), value: EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaAcordo },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaAnexos), value: EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaAnexos },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaVeiculos), value: EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaVeiculos },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaOutrosValores), value: EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaOutrosValores },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaFiliais), value: EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaFiliais },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaTipoOperacao), value: EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaTipoOperacao },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaValoresFreteMinimo), value: EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaValoresFreteMinimo },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaFranquia), value: EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaFranquia },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaValoresVeiculos), value: EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaValoresVeiculos },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_CalcularFreteNovamente), value: EnumPermissaoPersonalizada.Carga_CalcularFreteNovamente },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ChamadoOcorrencia_PermitirAssumirAtendimento), value: EnumPermissaoPersonalizada.ChamadoOcorrencia_PermitirAssumirAtendimento },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ChamadoOcorrencia_PermitirFecharSemOcorrencia), value: EnumPermissaoPersonalizada.ChamadoOcorrencia_PermitirFecharSemOcorrencia },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ChamadoOcorrencia_PermitirLiberarParaOcorrencia), value: EnumPermissaoPersonalizada.ChamadoOcorrencia_PermitirLiberarParaOcorrencia },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.GrupoPessoas_AdicionarDespachanteComoConsignatario), value: EnumPermissaoPersonalizada.GrupoPessoas_AdicionarDespachanteComoConsignatario },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.MovimentacaoPneu_PermitirInformarHodometro), value: EnumPermissaoPersonalizada.MovimentacaoPneu_PermitirInformarHodometro },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Motorista_PermitirInativarCadastroMotorista), value: EnumPermissaoPersonalizada.Motorista_PermitirInativarCadastroMotorista },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.MontagemCarga_AdicionarPrefixoNaCargaViaCarregamento), value: EnumPermissaoPersonalizada.MontagemCarga_AdicionarPrefixoNaCargaViaCarregamento },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Pessoas_PermiteBloquearDesbloquearPessoa), value: EnumPermissaoPersonalizada.Pessoas_PermiteBloquearDesbloquearPessoa },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Canhotos_PermitirRegistrarAuditoriaNosCanhotos), value: EnumPermissaoPersonalizada.Canhotos_PermitirRegistrarAuditoriaNosCanhotos },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermitirDefinirMotoristaComVigenciaIndisponivel), value: EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermitirDefinirMotoristaComVigenciaIndisponivel },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Usuarios_PermitirEditarAsPermissoesDeAcesso), value: EnumPermissaoPersonalizada.Usuarios_PermitirEditarAsPermissoesDeAcesso },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Booking_PermitirCancelarBooking), value: EnumPermissaoPersonalizada.Booking_PermitirCancelarBooking },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_PermitirAlterarModeloVeicularNaCarga), value: EnumPermissaoPersonalizada.Carga_PermitirAlterarModeloVeicularNaCarga },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Pedido_PermitirAlterarModeloVeicularNoPedido), value: EnumPermissaoPersonalizada.Pedido_PermitirAlterarModeloVeicularNoPedido },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermitirAlterarPedidosCargasJaFinalizados), value: EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermitirAlterarPedidosCargasJaFinalizados },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_PermitirReverterCargaNoShow), value: EnumPermissaoPersonalizada.Carga_PermitirReverterCargaNoShow },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ContratoFinanciamento_PermitirAlterarVeiculos), value: EnumPermissaoPersonalizada.ContratoFinanciamento_PermitirAlterarVeiculos },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_PermitirNaoComprarValePedagio), value: EnumPermissaoPersonalizada.Carga_PermitirNaoComprarValePedagio },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.RetiradaProdutoLista_NaoPermitirEditarAgendamento), value: EnumPermissaoPersonalizada.RetiradaProdutoLista_NaoPermitirEditarAgendamento },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.RetiradaProdutoLista_NaoPermitirExcluirAgendamento), value: EnumPermissaoPersonalizada.RetiradaProdutoLista_NaoPermitirExcluirAgendamento },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_PermiteAvancarCargaComRejeicaoPlanejamentoFrota), value: EnumPermissaoPersonalizada.Carga_PermiteAvancarCargaComRejeicaoPlanejamentoFrota },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Usuario_PermitirAtivarInativarVeiculo), value: EnumPermissaoPersonalizada.Usuario_PermitirAtivarInativarVeiculo },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_RemoverAnexosCarga), value: EnumPermissaoPersonalizada.Carga_RemoverAnexosCarga },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.PagamentoMotorista_PermiteReverterPagamentoMotorista), value: EnumPermissaoPersonalizada.PagamentoMotorista_PermiteReverterPagamentoMotorista },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FluxoPatio_PermiteVoltarEtapa), value: EnumPermissaoPersonalizada.FluxoPatio_PermiteVoltarEtapa },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Container_PermitirOperadorEditarTodasInformacoesContainer), value: EnumPermissaoPersonalizada.Container_PermitirOperadorEditarTodasInformacoesContainer },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_PermitirLiberarSemIntegracaoFrete), value: EnumPermissaoPersonalizada.Carga_PermitirLiberarSemIntegracaoFrete },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_PermitirLiberarSemLicencaValida), value: EnumPermissaoPersonalizada.Carga_PermitirLiberarSemLicencaValida },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_PermiteAlterarExternalId), value: EnumPermissaoPersonalizada.Carga_PermiteAlterarExternalId },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_PermiteAvancarCargaComRejeitacaoGNRE), value: EnumPermissaoPersonalizada.Carga_PermiteAvancarCargaComRejeitacaoGNRE },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.CTeManual_PermiteRealizarCTeManualComValorZerado), value: EnumPermissaoPersonalizada.CTeManual_PermiteRealizarCTeManualComValorZerado },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_PermitirAdicionarRemoverPedidoEtapaUm), value: EnumPermissaoPersonalizada.Carga_PermitirAdicionarRemoverPedidoEtapaUm },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteGerarCarga), value: EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteGerarCarga },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteInformarAlterarRemoverVeiculo), value: EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteInformarAlterarRemoverVeiculo },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteEnviarEmailTomador), value: EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteEnviarEmailTomador },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteEnviarEmailCheckList), value: EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteEnviarEmailCheckList },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteEnviarOrdemColetaTomador), value: EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteEnviarOrdemColetaTomador },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteInformarAlterarRemoverMotorista), value: EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteInformarAlterarRemoverMotorista },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteInformarPendente), value: EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteInformarPendente },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteAbrirDetalhePedido), value: EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteAbrirDetalhePedido },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteGerarAvisoMotorista), value: EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteGerarAvisoMotorista },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteGerarMotoristaCiente), value: EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteGerarMotoristaCiente },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteAbrirDetalhesAviso), value: EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteAbrirDetalhesAviso },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteAbrirTratativas), value: EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteAbrirTratativas },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteSubstituirModeloVeicular), value: EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteSubstituirModeloVeicular },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteAbrirWhatsAppMotorista), value: EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteAbrirWhatsAppMotorista },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteAlterarTipoDeOperacao), value: EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteAlterarTipoDeOperacao },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteImprimirOrdemDeColeta), value: EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteImprimirOrdemDeColeta },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteInformarDevolucao), value: EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteInformarDevolucao },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteAlterarDataDePrevisaoDeSaida), value: EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteAlterarDataDePrevisaoDeSaida },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteAlterarTipoDeCarga), value: EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteAlterarTipoDeCarga },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteEnviarEscala), value: EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteEnviarEscala },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_PermiteInserirAlterarVeiculoNaCargaOuPedido), value: EnumPermissaoPersonalizada.Carga_PermiteInserirAlterarVeiculoNaCargaOuPedido },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.SugestaoMensal_PermiteGerarListaDiariaApartirListaMensal), value: EnumPermissaoPersonalizada.SugestaoMensal_PermiteGerarListaDiariaApartirListaMensal },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Canhoto_PermiteConfirmarRecebimentoCanhotoFisico), value: EnumPermissaoPersonalizada.Canhoto_PermiteConfirmarRecebimentoCanhotoFisico },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Canhoto_PermiteAdicionarJustificativa), value: EnumPermissaoPersonalizada.Canhoto_PermiteAdicionarJustificativa },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_PermiteAlterarObservacaoContratoFreteTerceiro), value: EnumPermissaoPersonalizada.Carga_PermiteAlterarObservacaoContratoFreteTerceiro },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_PermiteRemoverContainerVinculadoEmCarga), value: EnumPermissaoPersonalizada.Carga_PermiteRemoverContainerVinculadoEmCarga },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Home_VisualizarDashDoc), value: EnumPermissaoPersonalizada.Home_VisualizarDashDoc },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Pedido_PermitirAlteracaoMotorista), value: EnumPermissaoPersonalizada.Pedido_PermitirAlteracaoMotorista },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ControleDocumento_PermiteDesparquearDocumentos), value: EnumPermissaoPersonalizada.ControleDocumento_PermiteDesparquearDocumentos },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.DocumentoDestinado_PermiteEmitirDesacordo), value: EnumPermissaoPersonalizada.DocumentoDestinado_PermiteEmitirDesacordo },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ComprovanteCarga_PermiteReverterComprovante), value: EnumPermissaoPersonalizada.ComprovanteCarga_PermiteReverterComprovante },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Pedido_PermiteInserirVeiculoProprio), value: EnumPermissaoPersonalizada.Pedido_PermiteInserirVeiculoProprio },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Pedido_PermiteInserirVeiculoTerceiro), value: EnumPermissaoPersonalizada.Pedido_PermiteInserirVeiculoTerceiro },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_PermiteInserirVeiculoProprio), value: EnumPermissaoPersonalizada.Carga_PermiteInserirVeiculoProprio },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_PermiteInserirVeiculoTerceiro), value: EnumPermissaoPersonalizada.Carga_PermiteInserirVeiculoTerceiro },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteInserirVeiculoProprio), value: EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteInserirVeiculoProprio },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteInserirVeiculoTerceiro), value: EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteInserirVeiculoTerceiro },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Transportador_PermiteInativarTransportador), value: EnumPermissaoPersonalizada.Transportador_PermiteInativarTransportador },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Transportador_PermiteAlterarStatusFinanceiro), value: EnumPermissaoPersonalizada.Transportador_PermiteAlterarStatusFinanceiro },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ChamadoOcorrencia_PermiteFinalizarEmLiberadoParaOcorrencia), value: EnumPermissaoPersonalizada.ChamadoOcorrencia_PermiteFinalizarEmLiberadoParaOcorrencia },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_NaoPermiteAlterarOperador), value: EnumPermissaoPersonalizada.Carga_NaoPermiteAlterarOperador },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.AuditoriaEMP_AcessoJustificativa), value: EnumPermissaoPersonalizada.AuditoriaEMP_AcessoJustificativa },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_PermitirAjusteManualImportadasEmbarcador), value: EnumPermissaoPersonalizada.Carga_PermitirAjusteManualImportadasEmbarcador },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.AuditoriaEMP_PermiteVisualizarJustificativa), value: EnumPermissaoPersonalizada.AuditoriaEMP_PermiteVisualizarJustificativa },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Veiculo_PermitirAlterarModeloVeicular), value: EnumPermissaoPersonalizada.Veiculo_PermitirAlterarModeloVeicular },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Veiculo_PermitirAcessoRastreador), value: EnumPermissaoPersonalizada.Veiculo_PermitirAcessoRastreador },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_PermitirAnexoDocumentoNaoContribuinte), value: EnumPermissaoPersonalizada.Carga_PermitirAnexoDocumentoNaoContribuinte },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Guias_PermiteAprovarRejeitarOCRGuiasManualmente), value: EnumPermissaoPersonalizada.Guias_PermiteAprovarRejeitarOCRGuiasManualmente },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Guias_PermiteReverterAprovacaoOCRGuias), value: EnumPermissaoPersonalizada.Guias_PermiteReverterAprovacaoOCRGuias },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Veiculo_PermiteEditarCompraValePedagio), value: EnumPermissaoPersonalizada.Veiculo_PermiteEditarCompraValePedagio },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Veiculo_PermiteEditarCompraValePedagioRetorno), value: EnumPermissaoPersonalizada.Veiculo_PermiteEditarCompraValePedagioRetorno },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ControleDocumento_PermitirLiberarDocumentoComIrregularidade), value: EnumPermissaoPersonalizada.ControleDocumento_PermitirLiberarDocumentoComIrregularidade },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Motorista_PermitirDuplicarCadastro), value: EnumPermissaoPersonalizada.Motorista_PermitirDuplicarCadastro },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Acerto_PermitirAdicionarAlterarValorComprovadoSaldoViagem), value: EnumPermissaoPersonalizada.Acerto_PermitirAdicionarAlterarValorComprovadoSaldoViagem },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_PermitirEmissaoDocumentosComFalhaIntegracaoAcrescimoDesconto), value: EnumPermissaoPersonalizada.Carga_PermitirEmissaoDocumentosComFalhaIntegracaoAcrescimoDesconto },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ModuloControle_PermiteParquearDocumentos), value: EnumPermissaoPersonalizada.ModuloControle_PermiteParquearDocumentos },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.CancelamentoCargaLote_LiberarCancelamentoCargaBloqueada), value: EnumPermissaoPersonalizada.CancelamentoCargaLote_LiberarCancelamentoCargaBloqueada },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_LiberarCargaComValorLimiteApoliceDivergente), value: EnumPermissaoPersonalizada.Carga_LiberarCargaComValorLimiteApoliceDivergente },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.VendaDireta_PermiteCancelamento), value: EnumPermissaoPersonalizada.VendaDireta_PermiteCancelamento },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_PermiteFinalizarIntegracaoEtapa1Carga), value: EnumPermissaoPersonalizada.Carga_PermiteFinalizarIntegracaoEtapa1Carga },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.TermoQuitacaoFinanceiro_PermiteExcluirTermoQuitacao), value: EnumPermissaoPersonalizada.TermoQuitacaoFinanceiro_PermiteExcluirTermoQuitacao },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.CargaEntrega_PermiteAlterarDataInicioFimPreTrip), value: EnumPermissaoPersonalizada.CargaEntrega_PermiteAlterarDataInicioFimPreTrip },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.MonitoramentoMapa_PermiteAlterarAbaFiltrosPersonalizados), value: EnumPermissaoPersonalizada.MonitoramentoMapa_PermiteAlterarAbaFiltrosPersonalizados },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_NaoPermiteReenviarIntegracaoCargasAppTrizy), value: EnumPermissaoPersonalizada.Carga_NaoPermiteReenviarIntegracaoCargasAppTrizy },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_PermiteAcessarEtapasDocumentosQuandoAcessoEstiverRestrito), value: EnumPermissaoPersonalizada.Carga_PermiteAcessarEtapasDocumentosQuandoAcessoEstiverRestrito },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_AlterarValePedagio), value: EnumPermissaoPersonalizada.Carga_AlterarValePedagio },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_AlterarPercentualExecucao), value: EnumPermissaoPersonalizada.Carga_AlterarPercentualExecucao },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ChamadoOcorrencia_AprovarValorSuperior), value: EnumPermissaoPersonalizada.ChamadoOcorrencia_AprovarValorSuperior },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.CTeManual_VincularCTeEmbarcador), value: EnumPermissaoPersonalizada.CTeManual_VincularCTeEmbarcador },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ChamadoOcorrencia_PermitirEstornarOcorrencia), value: EnumPermissaoPersonalizada.ChamadoOcorrencia_PermitirEstornarOcorrencia },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.GrupoPessoas_PermitirAdicionarClienteAoGrupoCliente), value: EnumPermissaoPersonalizada.GrupoPessoas_PermitirAdicionarClienteAoGrupoCliente },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Acerto_PermitirLancarDespesasAcertoViagem), value: EnumPermissaoPersonalizada.Acerto_PermitirLancarDespesasAcertoViagem },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Acerto_PermitirLancarDiariaAvulsoAcertoViagem), value: EnumPermissaoPersonalizada.Acerto_PermitirLancarDiariaAvulsoAcertoViagem },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Canhoto_PermitirRetornarStatusCanhotoAPIDigitalizacao), value: EnumPermissaoPersonalizada.Canhoto_PermitirRetornarStatusCanhotoAPIDigitalizacao },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.QualidadeEntrega_PermiteLiberarNotasBloqueadas), value: EnumPermissaoPersonalizada.QualidadeEntrega_PermiteLiberarNotasBloqueadas },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.FluxoGestaoPatio_OcultarDetalhesCarga), value: EnumPermissaoPersonalizada.FluxoGestaoPatio_OcultarDetalhesCarga },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.CargaCancelamento_PermitirModificarTipoCancelamento), value: EnumPermissaoPersonalizada.CargaCancelamento_PermitirModificarTipoCancelamento },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Pagamento_Reverter), value: EnumPermissaoPersonalizada.Pagamento_Reverter },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.IntegracaoPorCTe_GerenciarNotasFiscaisAdicionais), value: EnumPermissaoPersonalizada.IntegracaoPorCTe_GerenciarNotasFiscaisAdicionais },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.ImportarAtendimentos), value: EnumPermissaoPersonalizada.ImportarAtendimentos },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_PermitirAlterarValorAdiantamentoEtapaContainer), value: EnumPermissaoPersonalizada.Carga_PermitirAlterarValorAdiantamentoEtapaContainer },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.GestaoAtendimento_VerificarMeusIndicadores), value: EnumPermissaoPersonalizada.GestaoAtendimento_VerificarMeusIndicadores },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.GestaoAtendimento_VerificarIndicadoresGerais), value: EnumPermissaoPersonalizada.GestaoAtendimento_VerificarIndicadoresGerais },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_AlterarValorFreteApenasComTabelaFrete), value: EnumPermissaoPersonalizada.Carga_AlterarValorFreteApenasComTabelaFrete },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.CargaCancelamento_NaoPermitirFinalizarEtapaIntegracao), value: EnumPermissaoPersonalizada.CargaCancelamento_NaoPermitirFinalizarEtapaIntegracao },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Carga_NaoPermitirCancelarValePedagio), value: EnumPermissaoPersonalizada.Carga_NaoPermitirCancelarValePedagio },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.GrupoPessoas_PermitirBloquearOuDesbloquearGrupoDePessoas), value: EnumPermissaoPersonalizada.GrupoPessoas_PermitirBloquearOuDesbloquearGrupoDePessoas },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Target_PermiteEditarValor), value: EnumPermissaoPersonalizada.Target_PermiteEditarValor },
    { text: EnumPermissaoPersonalizadaDescricao(EnumPermissaoPersonalizada.Target_PermiteAprovarRejeitarRegistro), value: EnumPermissaoPersonalizada.Target_PermiteAprovarRejeitarRegistro },
];