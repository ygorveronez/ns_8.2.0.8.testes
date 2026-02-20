/// <reference path="ConfiguracaoPaginacao.js" />
/// <reference path="Motorista.js" />
/// <reference path="Veiculo.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/ComponenteFrete.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Consultas/PlanoConta.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Enumeradores/EnumSimNao.js" />
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../Enumeradores/EnumTipoChamado.js" />
/// <reference path="../../Enumeradores/EnumTipoContratoFreteTerceiro.js" />
/// <reference path="../../Enumeradores/EnumTipoEmissaoIntramunicipal.js" />
/// <reference path="../../Enumeradores/EnumTipoFechamentoFrete.js" />
/// <reference path="../../Enumeradores/EnumTipoGeracaoTituloFatura.js" />
/// <reference path="../../Enumeradores/EnumTipoIntegracao.js" />
/// <reference path="../../Enumeradores/EnumTipoMovimentoEntidade.js" />
/// <reference path="../../Enumeradores/EnumTipoRestricaoPalletModeloVeicularCarga.js" />
/// <reference path="../../Enumeradores/EnumTipoImpressaoPedido.js" />
/// <reference path="../../Enumeradores/EnumTipoImpressaoPedidoPrestacaoServico.js" />
/// <reference path="../../Enumeradores/EnumMonitorarPosicaoAtualVeiculo.js" />
/// <reference path="../../Enumeradores/EnumTipoFiltroDataMontagemCarga.js" />
/// <reference path="../../Enumeradores/EnumDataBaseCalculoPrevisaoControleEntrega.js" />
/// <reference path="../../Enumeradores/EnumQuandoProcessarMonitoramento.js" />
/// <reference path="../../Enumeradores/EnumPaises.js" />
/// <reference path="../../Enumeradores/EnumFormaPreenchimentoCentroResultadoPedido.js" />
/// <reference path="../../Enumeradores/EnumTipoImpressaoFatura.js" />
/// <reference path="../../Enumeradores/EnumTipoCalculoPercentualViagem.js" />
/// <reference path="../../Enumeradores/EnumMonitoramentoStatusViagemTipoRegra.js" />
/// <reference path="../../Enumeradores/EnumFormatoData.js" />
/// <reference path="../../enumeradores/EnumTipoControleSaldoPedido.js" />
/// <reference path="../../Enumeradores/EnumFormatoHora.js" />
/// <reference path="../../Enumeradores/EnumTipoRomaneio.js" />
/// <reference path="../../Enumeradores/EnumFrequenciaTrackingAppTrizy.js" />
/// <reference path="../../Enumeradores/EnumTipoProtocolo.js" />
/// <reference path="../../Enumeradores/EnumConfiguracaoPaginacaoInterfaces.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _configuracaoEmbarcador;
var _CRUDConfiguracaoEmbarcador;

var _situacaoCarga = [
    { text: 'Ag. ImpressaoDocumentos', value: EnumSituacoesCarga.AgImpressaoDocumentos },
    { text: 'Ag. Integracao', value: EnumSituacoesCarga.AgIntegracao },
    { text: 'Ag. NFe', value: EnumSituacoesCarga.AgNFe },
    { text: 'Ag. Transportador', value: EnumSituacoesCarga.AgTransportador },
    { text: 'Anulada', value: EnumSituacoesCarga.Anulada },
    { text: 'Calculo Frete', value: EnumSituacoesCarga.CalculoFrete },
    { text: 'Cancelada', value: EnumSituacoesCarga.Cancelada },
    { text: 'Em Cancelamento', value: EnumSituacoesCarga.EmCancelamento },
    { text: 'Em Leilão', value: EnumSituacoesCarga.EmLeilao },
    { text: 'Em Transbordo', value: EnumSituacoesCarga.EmTransbordo },
    { text: 'Em Transporte', value: EnumSituacoesCarga.EmTransporte },
    { text: 'Encerrada', value: EnumSituacoesCarga.Encerrada },
    { text: 'Liberado Pagamento', value: EnumSituacoesCarga.LiberadoPagamento },
    { text: 'Na Logistíca', value: EnumSituacoesCarga.NaLogistica },
    { text: 'Nova', value: EnumSituacoesCarga.Nova },
    { text: 'Pendêcia de Documentos', value: EnumSituacoesCarga.PendeciaDocumentos },
    { text: 'Permite CT-e Manual', value: EnumSituacoesCarga.PermiteCTeManual },
    { text: 'Pronto p/ Transporte', value: EnumSituacoesCarga.ProntoTransporte },
    { text: 'Rejeição no Cancelamento', value: EnumSituacoesCarga.RejeicaoCancelamento }
];

var _tipoEmissaoIntramunicipal = [
    { text: 'NFE-s apenas empresas aptas demais CT-e', value: EnumTipoEmissaoIntramunicipal.NFEsApenasEmpresasAptasDemaisCTe },
    { text: 'NFE-s apenas empresas aptas demais NFs Manual', value: EnumTipoEmissaoIntramunicipal.NFEsApenasEmpresasAptasDemaisNFsManual },
    { text: 'NFE-s apenas empresas aptas demais não emite nenhum documento', value: EnumTipoEmissaoIntramunicipal.NFEsApenasEmpresasAptasDemaisNaoEmiteNenhumDocumento },
    { text: 'Não Emite Nenhum Documento', value: EnumTipoEmissaoIntramunicipal.NaoEmiteNenhumDocumento },
    { text: 'Não Especificado', value: EnumTipoEmissaoIntramunicipal.NaoEspecificado },
    { text: 'Sempre CT-e', value: EnumTipoEmissaoIntramunicipal.SempreCTe },
    { text: 'Sempre NFS Manual', value: EnumTipoEmissaoIntramunicipal.SempreNFSManual },
    { text: 'Sempre NFS-e', value: EnumTipoEmissaoIntramunicipal.SempreNFSe }
];

var _simNao = [
    { text: 'Sim', value: 1 },
    { text: 'Não', value: 2 }
];

var _simNaoBool = [
    { text: 'Sim', value: true },
    { text: 'Não', value: false }
];

var _tipoContratoFreteTerceiro = [
    { text: 'Por Carga', value: EnumTipoContratoFreteTerceiro.PorCarga },
    { text: 'Por Pagamento Agregado', value: EnumTipoContratoFreteTerceiro.PorPagamentoAgregado }
];

var _tipoChamado = [
    { text: 'Padrão Transportador', value: EnumTipoChamado.PadraoTransportador },
    { text: 'Padrão Embarcador', value: EnumTipoChamado.PadraoEmbarcador }
];

var _tipoGeracaoTituloFatura = [
    { text: 'Por Documento', value: EnumTipoGeracaoTituloFatura.PorDocumento },
    { text: 'Por Parcela', value: EnumTipoGeracaoTituloFatura.PorParcela }
];

var _dataCompetenciaDocumentoEntrada = [
    { text: 'Data Entrada', value: EnumDataCompetenciaDocumentoEntrada.DataEntrada },
    { text: 'Data Emissão', value: EnumDataCompetenciaDocumentoEntrada.DataEmissao }
];

var _dataEntradaDocumentoEntrada = [
    { text: 'Data Lançamento', value: EnumDataEntradaDocumentoEntrada.DataLancamento },
    { text: 'Data Entrada', value: EnumDataEntradaDocumentoEntrada.DataEntrada }
];

var _situacaoPagamentoAdiantamentoMotorista = [{ text: "Entrada", value: EnumTipoMovimentoEntidade.Entrada }, { text: "Saída", value: EnumTipoMovimentoEntidade.Saida }];

var _numeroCasasDecimaisQuantidadeProduto = [
    { text: "0", value: 0 },
    { text: "1", value: 1 },
    { text: "2", value: 2 },
    { text: "3", value: 3 },
    { text: "4", value: 4 },
    { text: "5", value: 5 },
    { text: "6", value: 6 },
];

var _liquidoBrutoBool = [
    { text: 'Liquido', value: true },
    { text: 'Bruto', value: false }
];

var _tipoSSo = [
    { text: "OAuth2", value: 1 },
    { text: "Saml2", value: 2 }
    //{ text: "CyberArk", value: 3 },
]

/*
 * Declaração das Classes
 */

let ConfiguracaoEmbarcador = function () {
    //#region Booleanos

    this.QuandoGeradoPreCteRetornarInformacaoDeFreteCTeIntegrado = PropertyEntity({ text: "Quando gerado pré cte, retornar informações de frete do CTe integrado", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarSituacaoEnvioProgramadoIntegracaoCanhoto = PropertyEntity({ text: "Validar situações das integrações de Envio Programado para efetuar a integração de Canhotos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ConsultarDocumentosDestinadosCarga = PropertyEntity({ text: "Habilitar consulta de documentos destinados na carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ImportarValePedagioMDFECarga = PropertyEntity({ text: "Importar vale pedágio para carga ao importar MDF-e emitido no embarcador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoPermiteEmitirCargaSemAverbacao = PropertyEntity({ text: "Não permite emitir carga sem averbação", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoPermiteInformarValorMaiorTerceiroTabelaFrete = PropertyEntity({ text: "Não permite informar valor maior ao terceiro do que o valor gerado pela tabela de frete", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UsarPrioridadeDaCargaParaImpressaoDeObservacaoNoCTE = PropertyEntity({ text: "Usar prioridade da carga para impressão de observação no CT-e", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoGerarPDFDocumentosComNotasFiscais = PropertyEntity({ text: "Não gerar o PDF dos documentos com as notas fiscais", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteInformarRemetenteLancamentoNotaManualCarga = PropertyEntity({ text: "Permitir informar remetente no lançamento de nota manual na Carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirConfirmacaoImpressaoME = PropertyEntity({ text: "Permitir confirmação de impressão ME", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoGerarAverbacaoCTeQuandoPedidoTiverAverbacao = PropertyEntity({ text: "Não gerar averbação de CTe quando tiver averbação no Pedido (NFe)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.FiltrarBuscaVeiculosPorEmpresa = PropertyEntity({ text: "Filtrar busca de veículos e motoristas por empresa", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PreencherMotoristaAutomaticamenteAoInformarVeiculo = PropertyEntity({ text: "Preencher motorista automaticamente ao informar veículo", getType: typesKnockout.bool, val: ko.observable(false) });
    this.BuscarProdutoPredominanteNoPedido = PropertyEntity({ text: "Buscar produto predominante no pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ControlarCanhotosDasNFEs = PropertyEntity({ text: "Controlar canhotos das NF-es", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarNFeEmHomologacao = PropertyEntity({ text: "Utilizar NF-e em homologação", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarTempoCarregamentoPorPeriodo = PropertyEntity({ text: "Utilizar tempo de carregamento por período", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AlterarDataCarregamentoEDescarregamentoPorPeriodo = PropertyEntity({ text: "Alterar a data de carregamento e descarregamento por período", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirInformacoesAdicionaisChamado = PropertyEntity({ text: "Exibir informações adicionais no chamado", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirNumeroCargaQuandoExistirCarregamento = PropertyEntity({ text: "Exibir o número da carga quando existir carregamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirCargaSemValorFreteJanelaCarregamentoTransportador = PropertyEntity({ text: "Exibir carga sem valor de frete na janela do transportador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirOperadorInformarValorFreteMaiorQueTabela = PropertyEntity({ text: "Permitir operador inserir frete superior a tabela de frete.", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirRetornoAgNotasFiscais = PropertyEntity({ text: "Permitir retorno à Ag. Notas Fiscais", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ObrigatorioInformarDadosContratoFrete = PropertyEntity({ text: "Obrigatório informar dados no Contrato de Frete", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirKmUtilizadoContratoFretePorPeriodoVigenciaContrato = PropertyEntity({ text: "Exibir o Km utilizado do contrato de frete por período de vigência do contrato", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirCancelamentoTotalCarga = PropertyEntity({ text: "Permitir cancelamento total da carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirCancelamentoTotalCargaViaWebService = PropertyEntity({ text: "Permitir cancelamento total da carga via Web Service", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarIntegracaoPedido = PropertyEntity({ text: "Utilizar integração pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteAdicionarNotaManualmente = PropertyEntity({ text: "Permite adicionar nota manualmente", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarValorCargaAoAdicionarNFe = PropertyEntity({ text: "Validar valor da carga ao adicionar NF-e", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoUtilizarCodigoCargaOrigemNaObservacaoCTe = PropertyEntity({ text: "Não utilizar Código da Carga de Origem na Observação do CT-e", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PossuiValidacaoParaLiberacaoCargaComNotaJaUtilizada = PropertyEntity({ text: "Possui validação para liberação de carga com nota já utilizada?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarPedagioBaseCalculoIcmsCteComplementarPorRegraEstado = PropertyEntity({ text: "Utilizar pedágio na base de cálculo de ICMS de CT-e complementar por regra de estado", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarValorDescarga = PropertyEntity({ text: "Utilizar valor de descarga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExigeInformarCienciaDoEnvioDasNotasAntesDeEmitirDocumentos = PropertyEntity({ text: "Exige informar ciência do envio das notas antes de emitir documentos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExigirNotaFiscalParaCalcularFreteCarga = PropertyEntity({ text: "Exigir que a nota fiscal seja recebida antes de calcular o Frete", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ArmazenarXMLCTeEmArquivo = PropertyEntity({ text: "Armazenar XML CT-es em arquivo", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NumeroCargaSequencialUnico = PropertyEntity({ text: "Número de carga sequencial único", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarNumeroSequencialCargaNoCarregamento = PropertyEntity({ text: "Utilizar Número Sequêncial da Carga no Carregamento", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(false) });
    this.ManterOperacaoUnicaEmCargasAgrupadas = PropertyEntity({ text: "Manter Tipo de Operação Única em cargas Agrupadas", getType: typesKnockout.bool, val: ko.observable(false) });
    this.RatearNumeroPalletsModeloVeiculoEntrePedidoPorPeso = PropertyEntity({ text: "Ratear número pallets modelo veículo entre pedido por peso", getType: typesKnockout.bool, val: ko.observable(false) });
    this.EnviarDocumentosAutomaticamenteParaImpressao = PropertyEntity({ text: "Enviar documentos automaticamente para impressão", getType: typesKnockout.bool, val: ko.observable(false) });
    this.EnviarMDFeAutomaticamenteParaImpressao = PropertyEntity({ text: "Enviar MDF-e automaticamente para impressão", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AtualizarCargaComVeiculoMDFeManual = PropertyEntity({ text: "Atualizar veículo/motorista da Carga conforme veículo/motorista do MDFe Manual", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ReduzirRetencaoISSValorAReceberNFSManual = PropertyEntity({ text: "Reduzir a retenção do ISS no valor a receber da NFS Manual", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ObrigarTerGuaritaParaLancamentoEFinalizacaoCarga = PropertyEntity({ text: "Obrigar ter lançamento de guarita para o lançamento e finalização da carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.FiltrarAlcadasDoUsuario = PropertyEntity({ text: "Filtrar alçadas do usuário", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoExecutarFecharCarga = PropertyEntity({ text: "Não executar fechar carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UsarMesmoNumeroPreCargaGerarCargaViaImportacao = PropertyEntity({ text: "Usar mesmo número pré carga gerar carga via importação", getType: typesKnockout.bool, val: ko.observable(false) });
    this.CancelarCargaExistenteAutomaticamenteNaImportacaoDePedido = PropertyEntity({ text: "Cancelar carga existente automaticamente na importação", getType: typesKnockout.bool, val: ko.observable(true), def: true });
    this.UtilizarMultiplosModelosVeicularesPedido = PropertyEntity({ text: "Utilizar múltiplos modelos veiculares no pedido?", getType: typesKnockout.bool, val: ko.observable(true), def: false });
    this.SolicitarValorFretePorTonelada = PropertyEntity({ text: "Informar o valor do frete por tonelada?", getType: typesKnockout.bool, val: ko.observable(true), def: false });
    this.GerarCargaDeNotasRecebidasPorEmail = PropertyEntity({ text: "Gerar carga de notas recebidas por e-mail", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarValorFreteNota = PropertyEntity({ text: "Utilizar valor frete da nota para gerar carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ImportarEmailCliente = PropertyEntity({ text: "Importar email cliente na importação de NFe", getType: typesKnockout.bool, val: ko.observable(false) });
    this.SempreDuplicarCargaCancelada = PropertyEntity({ text: "Sempre duplicar carga cancelada", getType: typesKnockout.bool, val: ko.observable(false) });
    this.DefaultTrueDuplicarCarga = PropertyEntity({ text: "Padrão True para duplicar carga no cancelamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExigirChamadoParaAbrirOcorrencia = PropertyEntity({ text: "Exigir chamado para abrir ocorrência", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ImprimirObservacaoPedidoMDFe = PropertyEntity({ text: "Imprimir observação do pedido no MDF-e", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirSalvarDadosTransporteCargaSemSolicitarNFes = PropertyEntity({ text: "Permitir salvar os dados de transporte sem solicitar as notas fiscais", getType: typesKnockout.bool, val: ko.observable(false) });
    this.IncluirTodosAcrescimosEDescontosNoCalculoDeImpostos = PropertyEntity({ text: "Incluir tipo todos acréscimos e descontos ao calcular impostos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.EmAcrescimoDescontoCiotNaoAlteraImpostos = PropertyEntity({ text: "Em caso de Acréscimo/Desconto do CIOT não alterar os impostos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.SolicitarNotasFiscaisAoSalvarDadosTransportador = PropertyEntity({ text: "Solicitar as notas fiscais ao salvar os dados do transportador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarCargaTrajeto = PropertyEntity({ text: "Gerar Carga Trajeto", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirAlterarCargaHorarioCarregamentoInferiorAtual = PropertyEntity({ text: "Permitir informar horário de carregamento inferior ao atual", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PossuiWMS = PropertyEntity({ text: "Possui WMS", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExigeNumeroDeAprovadoresNasAlcadas = PropertyEntity({ text: "Exige número de aprovadores nas alçadas", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarTabelaFreteNoPedido = PropertyEntity({ text: "Validar tabela frete no pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.EncerrarMDFesDeOutrasViagensAutomaticamente = PropertyEntity({ text: "Encerrar MDF-es de outras viagens automaticamente", getType: typesKnockout.bool, val: ko.observable(false) });
    this.EnviarEmailEncerramentoMDFeTransportador = PropertyEntity({ text: "Enviar e-mail de encerramento de MDF-e para o transportador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.CargaTransbordoNaEtapaInicial = PropertyEntity({ text: "Carga transbordo na etapa inicial", getType: typesKnockout.bool, val: ko.observable(false) });
    this.SempreBuscaCTePorChaveEmIntegracaoViaWS = PropertyEntity({ text: "Sempre busca CT-e por chave em integração via WS", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarDataLiberacaoSeguradora = PropertyEntity({ text: "Validar data liberação seguradora", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarDataLiberacaoSeguradoraVeiculo = PropertyEntity({ text: "Validar data liberação seguradora do veículo", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteEmissaoCargaSomenteComTracao = PropertyEntity({ text: "Permite emissão carga somente com tração", getType: typesKnockout.bool, val: ko.observable(false) });
    this.InformarDadosChegadaVeiculoNoFluxoPatio = PropertyEntity({ text: "Informar dados chegada veículo no fluxo de pátio", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteSelecionarQualquerNaturezaNFEntrada = PropertyEntity({ text: "Permite selecionar qualquer natureza NF entrada", getType: typesKnockout.bool, val: ko.observable(false) });
    this.IgnorarTipoContratoNoContratoFreteTransportador = PropertyEntity({ text: "Ignorar tipo contrato no contrato frete transportador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ChamadoOcorrenciaUsaRemetente = PropertyEntity({ text: "Chamado ocorrência usa remetente", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirCancelarPedidosSemDocumentos = PropertyEntity({ text: "Permitir cancelar os pedidos sem documentos?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PadraoArmazenamentoFisicoCanhotoCTe = PropertyEntity({ text: "Padrão armazenamento físico canhoto CT-e", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExigirCodigoIntegracaoTransportador = PropertyEntity({ text: "Exigir código de integração no cadastro do transportador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExigirEmailPrincipalCadastroTransportador = PropertyEntity({ text: "Exigir e-mail principal no cadastro do transportador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AcertoDeViagemComDiaria = PropertyEntity({ text: "Utilizar Acerto de Viagem com pagamento de Diária", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AcertoDeViagemImpressaoDetalhada = PropertyEntity({ text: "Utilizar Acerto de Viagem com impressão detalhada", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirCFOPCompra = PropertyEntity({ text: "Exibir CFOP de compra na tabela de alíquota", getType: typesKnockout.bool, val: ko.observable(false) });
    this.HabilitarFluxoEntregas = PropertyEntity({ text: "Habilitar Fluxo de Entregas", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarNumeroNotaFluxoEntregas = PropertyEntity({ text: "Utilizar Número da Nota no Fluxo de Entregas", getType: typesKnockout.bool, val: ko.observable(false) });
    this.FinalizarViagemAnteriorAoEntrarFilaCarregamento = PropertyEntity({ text: "Finalizar a viagem anterior ao entrar na fila de carregamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarFilaCarregamento = PropertyEntity({ text: "Utilizar fila de carregamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarFilaCarregamentoReversa = PropertyEntity({ text: "Utilizar fila de carregamento reversa", getType: typesKnockout.bool, val: ko.observable(false) });
    this.MarcacaoFilaCarregamentoSomentePorVeiculo = PropertyEntity({ text: "Marcação na fila de carregamento somente por veículo", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarPreCargaJanelaCarregamento = PropertyEntity({ text: "Utilizar pré carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirNumeroPedidoJanelaCarregamentoEDescarregamento = PropertyEntity({ text: "Exibir número do pedido nas janelas de carregamento e descarregamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirTrocarPedidoCarga = PropertyEntity({ text: "Permitir trocar pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirAdicionarPedidoOutraFilialCarga = PropertyEntity({ text: "Permitir adicionar pedido de outra filial", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirRemoverPedidoCargaComPendenciaDocumentos = PropertyEntity({ text: "Permitir remover pedido da carga com pendências na emissão", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarSituacaoNaJanelaDescarregamento = PropertyEntity({ text: "Utilizar a situação na janela de descarregamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirInformarTipoTransportadorPorDataCarregamentoJanelaCarregamento = PropertyEntity({ text: "Permitir informar o tipo de transportador por data de carregamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirImportarAlteracaoDataCarregamentoJanelaCarregamento = PropertyEntity({ text: "Permitir importar alterações na data de carregamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarDataCarregamentoDaJanelaCarregamentoAoSetarTransportadorPrioritarioPorRotaCarga = PropertyEntity({ text: "Utilizar a data de carregamento ao definir o transportador por rota", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoGerarSenhaAgendamento = PropertyEntity({ text: "Não Gerar Senha de Agendamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.TrocarPreCargaPorCarga = PropertyEntity({ text: "Trocar pré carga por carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarProtocoloDaPreCargaNaCarga = PropertyEntity({ text: "Utilizar protocolo da pré carga na carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoExigeInformarDisponibilidadeDeVeiculo = PropertyEntity({ text: "Não exigir que o transportador informe a disponibilidade do veículo para utilizá-lo na janela de carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarConjuntoVeiculoPermiteEntrarFilaCarregamentoMobile = PropertyEntity({ text: "Validar conjunto de veículo permite entrar na fila de carregamento mobile", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirResumoFilaCarregamentoSomentePorModeloVeicularCarga = PropertyEntity({ text: "Exibir resumo somente por modelo veicular de carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoPermitirAdicionarVeiculoEmMaisDeUmaFilaCarregamentoSimultaneamente = PropertyEntity({ text: "Não permitir adicionar o veículo em mais de uma fila de carregamento simultaneamente", getType: typesKnockout.bool, val: ko.observable(false) });
    this.InformarAreaCDAdicionarVeiculo = PropertyEntity({ text: "Informar Área do CD ao Adicionar Veículos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteAvancarPrimeiraEtapaCargaAoAlocarDadosTransportePelaFilaCarregamento = PropertyEntity({ text: "Permite avançar primeira etapa da carga ao alocar dados de transporte pela fila de carregamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AtualizarFilaCarregamentoAoAlterarDadosTransporteNaCarga = PropertyEntity({ text: "Atualizar fila de carregamento, ao alterar dados de transporte na carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarControleVeiculoEmPatio = PropertyEntity({ text: "Utilizar Controle de Veículo em Pátio", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ProvisionarDocumentosEmitidos = PropertyEntity({ text: "Permite Provisionar documentos emitidos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NotaUnicaEmCargas = PropertyEntity({ text: "Nota única em cargas", getType: typesKnockout.bool, val: ko.observable(false) });
    //this.AverbarRedespacho = PropertyEntity({ text: "Averbar Redespacho", getType: typesKnockout.bool, val: ko.observable(false) });
    //this.AverbarSubcontratacao = PropertyEntity({ text: "Averbar Subcontratação", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarTabelaFreteComDataAtual = PropertyEntity({ text: "Validar Tabela de Frete Com Data Atual", getType: typesKnockout.bool, val: ko.observable(false) });
    this.FluxoDePatioComoMonitoramento = PropertyEntity({ text: "Nomenclatura do Fluxo de Pátio como Monitoramento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UsarContratoFreteAditivo = PropertyEntity({ text: "Usar aditivo nos contratos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirAdicionarCargaFluxoPatio = PropertyEntity({ text: "Permitir adicionar carga no fluxo de pátio", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NotificarCanhotosPendentes = PropertyEntity({ text: "Notificar Canhotos Pendentes", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NotificarPaletesPendentes = PropertyEntity({ text: "Notificar Paletes Pendentes", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExigeAprovacaoDigitalizacaoCanhoto = PropertyEntity({ text: "Exige aprovação da digitalização de canhotos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.BaixarCanhotoAposAprovacaoDigitalizacao = PropertyEntity({ text: "Baixar canhotos após a aprovação da digitalização", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ObrigatorioInformarDataEnvioCanhoto = PropertyEntity({ text: "Obrigatório informar a data de envio do canhoto", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirDisponibilizarCargaParaTransportador = PropertyEntity({ text: "Permitir disponibilizar a carga para um transportador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirLiberarCargaSemNFe = PropertyEntity({ text: "Permitir liberar a carga sem NF-e", getType: typesKnockout.bool, val: ko.observable(false) });
    this.BloquearEmissaoComContratoFreteZerado = PropertyEntity({ text: "Bloquear emissão dos documentos caso o valor do contrato de frete esteja zerado", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoExibirVariacaoContratoFrete = PropertyEntity({ text: "Não exibir variação no contrato de frete", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarToleranciaPesoModeloVeicular = PropertyEntity({ text: "Validar tolerância de peso do veículo", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarDACTEOutrosDocumentosAutomaticamente = PropertyEntity({ text: "Gerar a DACTE de outros modelos de documentos automaticamente", getType: typesKnockout.bool, val: ko.observable(false) });
    this.CalcularFreteFilialEmissoraPorTabelaDeFrete = PropertyEntity({ text: "Calcular Frete da Filial Emissora por tabela de Frete", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarCargaDeCTesRecebidosPorEmail = PropertyEntity({ text: "Gerar uma nova carga à partir dos CT-es recebidos por e-mail", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UsarNumeroCargaParaNumeroCanhotoAvulso = PropertyEntity({ text: "Ao gerar um canhoto avulso (entrega via distribuídor) utilizar o número da carga como número do canhoto", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExigirDatasValidadeCadastroMotorista = PropertyEntity({ text: "Exige datas de validade no cadastro de motorista", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AlterarEmpresaEmissoraAoAjustarParticipantes = PropertyEntity({ text: "Alterar Empresa Emissora ao Ajustar Participantes", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoPermitirValorFreteLiquidoZerado = PropertyEntity({ text: "Não Permitir Valor de Frete Líquido Zerado", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoEmitirCargaComValorZerado = PropertyEntity({ text: "Não Permitir Emissão de documentos com valor do frete zerado", getType: typesKnockout.bool, val: ko.observable(false) });
    this.HabilitarRelatorioDeTroca = PropertyEntity({ text: "Habilitar Relatório de Troca (Download documentos)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.HabilitarRelatorioBoletimViagem = PropertyEntity({ text: "Habilitar Relatório Boletim Viagem (Download documentos)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.HabilitarRelatorioDiarioBordo = PropertyEntity({ text: "Habilitar Relatório Diário de Bordo (Download documentos)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.HabilitarRelatorioDeEmbarque = PropertyEntity({ text: "Habilitar Relatório de Embarque (Download documentos)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirMensagemAlertaPrevisaoEntregaNaMesmaData = PropertyEntity({ text: "Exibir mensagem de alerta de previsão de entrega na mesma data", getType: typesKnockout.bool, val: ko.observable(false) });
    this.LiberarPedidosParaMontagemCargaCancelada = PropertyEntity({ text: "Liberar pedidos para a montagem de carga após cancelamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteAdicionarPreCargaManual = PropertyEntity({ text: "Permite Adicionar Pré Carga Manual", getType: typesKnockout.bool, val: ko.observable(false) });
    this.DadosTransporteObrigatorioPreCarga = PropertyEntity({ text: "Obrigatório Informar Dados para Transporte", getType: typesKnockout.bool, val: ko.observable(false) });
    this.TransportadorObrigatorioPreCarga = PropertyEntity({ text: "Obrigatório Informar o Transportador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.LocalCarregamentoObrigatorioPreCarga = PropertyEntity({ text: "Local de Carregamento Obrigatório", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarNumeroPreCargaPorFilial = PropertyEntity({ text: "Utilizar Número de Pré Carga por Filial", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ReplicarAjusteTabelaFreteTodasTabelas = PropertyEntity({ text: "Replicar Nova Vigência em Todas Tabelas", getType: typesKnockout.bool, val: ko.observable(false) });
    this.CompararTabelasDeFreteParaCalculo = PropertyEntity({ text: "Em caso de tabelas compativeis com a mesma carga, comparar elas (o sistema irá buscar a de maior valor)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.TipoOperacaoObrigatorioMontagemCarga = PropertyEntity({ text: "Tipo de operação obrigatório", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirTiposOperacoesDistintasMontagemCarga = PropertyEntity({ text: "Permitir tipos de operações distintas", getType: typesKnockout.bool, val: ko.observable(false) });
    this.FronteiraObrigatoriaMontagemCarga = PropertyEntity({ text: "Fronteira obrigatória", getType: typesKnockout.bool, val: ko.observable(false) });
    this.TipoCargaObrigatorioMontagemCarga = PropertyEntity({ text: "Tipo de carga obrigatório", getType: typesKnockout.bool, val: ko.observable(false) });
    this.InformarPeriodoCarregamentoMontagemCarga = PropertyEntity({ text: "Informar período de carregamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.TransportadorObrigatorioMontagemCarga = PropertyEntity({ text: "Transportador obrigatório", getType: typesKnockout.bool, val: ko.observable(false) });
    this.VeiculoObrigatorioMontagemCarga = PropertyEntity({ text: "Veículo obrigatório", getType: typesKnockout.bool, val: ko.observable(false) });
    this.MotoristaObrigatorioMontagemCarga = PropertyEntity({ text: "Motorista obrigatório", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExigirTipoSeparacaoMontagemCarga = PropertyEntity({ text: "Tipo separação obrigatório", getType: typesKnockout.bool, val: ko.observable(false) });
    this.SimulacaoFreteObrigatorioMontagemCarga = PropertyEntity({ text: "Simulação de frete obrigatória", getType: typesKnockout.bool, val: ko.observable(false) });
    this.RoteirizacaoObrigatoriaMontagemCarga = PropertyEntity({ text: "Roteirização obrigatória", getType: typesKnockout.bool, val: ko.observable(false) });
    this.SubstituirRoteirizacaoCarregamentoPorRoteirizacaoRotaFreteCarregamento = PropertyEntity({ text: "Utilizar roteirização rota frete para o carregamento ao gerar a carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.InformaHorarioCarregamentoMontagemCarga = PropertyEntity({ text: "Informar horário carregamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.InformaApoliceSeguroMontagemCarga = PropertyEntity({ text: "Informar apólice seguro carregamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.FiltrarPorPedidoSemCarregamentoNaMontagemCarga = PropertyEntity({ text: "Filtrar por pedidos sem carregamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirEditarPedidosAtravesTelaMontagemCargaMapa = PropertyEntity({ text: "Permitir editar pedidos através da tela montagem carga mapa", getType: typesKnockout.bool, val: ko.observable(false) });
    this.IgnorarRotaFretePedidosMontagemCargaMapa = PropertyEntity({ text: "Ignorar Rota de Frete dos Pedidos", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.ManterPedidosComMesmoAgrupadorNaMesmaCarga = PropertyEntity({ text: "Manter pedidos com o mesmo agrupador na mesma carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.FiltrarPedidosVinculadoOutrasCarga = PropertyEntity({ text: "Filtrar pedidos vinculados a outras carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.InformarTipoCondicaoPagamentoMontagemCarga = PropertyEntity({ text: "Informar tipo de condição de pagamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.OcultaGerarCarregamentosMontagemCarga = PropertyEntity({ text: "Ocultar botão gerar carregamentos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.LimparTelaAoSalvarMontagemCarga = PropertyEntity({ text: "Limpar tela carregamento ao salvar", getType: typesKnockout.bool, val: ko.observable(false) });
    this.BloquearGeracaoCargaComJanelaCarregamentoExcedente = PropertyEntity({ text: "Bloquear geração de carga com janela de carregamento como excedente", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirTipoDeCargaNaAbaCarregamentoNaMontagemCarga = PropertyEntity({ text: "Exibir tipo de carga na aba Carregamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirAbaDetalhePedidoExportacaoNaMontagemCarga = PropertyEntity({ text: "Exibir aba detalhe do pedido de exportacao", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarCapacidadeModeloVeicularCargaNaMontagemCarga = PropertyEntity({ text: "Validar a capacidade do modelo veicular de carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarComponentesCliente = PropertyEntity({ text: "Utilizar componentes de frete do cliente", getType: typesKnockout.bool, val: ko.observable(false) });
    this.DesativarMultiplosMotoristasMontagemCarga = PropertyEntity({ text: "Desativar múltiplos motoristas", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AtivarNovaDefinicaoDoTomadorParaCargasFeederMontagemCarga = PropertyEntity({ text: "Ativar nova definição de tomador para cargas Feeder", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizaChat = PropertyEntity({ text: "Utilizar Chat", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ObrigarVigenciaNoAjusteFrete = PropertyEntity({ text: "Obrigar Vigência No Ajuste de Frete", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AuditarConsultasWebService = PropertyEntity({ text: "Auditar Consultas de Web Service", getType: typesKnockout.bool, val: ko.observable(false) });
    this.RetornosDuplicidadeWSSubstituirPorSucesso = PropertyEntity({ text: "Retornos de duplicidade retornar como sucesso", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoValidarTipoDeVeiculoNoMetodoInformarDadosTransporteCarga = PropertyEntity({ text: "Não validar tipo de veículo no método InformarDadosTransporteCarga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.RetornarCargasEmQualquerEtapaNoMetodoBuscarCargaPendenteIntegracao = PropertyEntity({ text: "Retornar cargas em qualquer etapa no método BuscarCargasPendenteIntegracao", getType: typesKnockout.bool, val: ko.observable(false) });
    this.RetornarFalhaAdicionarCargaSeExistirCancelamentoCargaEmAberto = PropertyEntity({ text: "Ao adicionar uma carga e ela já existir retornar falha na integração (código 300) se existir uma solicitação de cancelamento para a carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.OcultarBuscaRotaNaCarga = PropertyEntity({ text: "Ocultar busca de rota na tela de Cargas", getType: typesKnockout.bool, val: ko.observable(false) });
    this.RetornarCargaPendenteConsultaCarregamentoAoSalvarDadosTransporte = PropertyEntity({ text: "Retornar carga pendente consulta carregamento ao salvar dados transporte", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExigirQueVeiculoCavaloTenhaReboqueVinculado = PropertyEntity({ text: "Exigir que o veículo cavalo tenha reboque vinculado para avançar a carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoValidarDataCancelamentoTituloNaBaixaTituloReceberPorCTe = PropertyEntity({ text: "Não validar data de cancelamento dos títulos na baixa de títulos a receber por CT-e", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ObrigatorioGeracaoBlocosParaCarregamento = PropertyEntity({ text: "Obrigatório Geração de Blocos Para Carregamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AtualizarProdutosPedidoPorIntegracao = PropertyEntity({ text: "Atualizar Produtos do Pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AtualizarProdutosCarregamentoPorNota = PropertyEntity({ text: "Atualizar Produtos Carregamento por Nota", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AtualizarPedidoPorIntegracao = PropertyEntity({ text: "Atualizar pedido ao adicionar carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.EnviarEmailFluxoEntrega = PropertyEntity({ text: "Enviar E-mail Fluxo de Entrega", getType: typesKnockout.bool, val: ko.observable(false) });
    this.InformarCienciaOperacaoDocumentoDestinado = PropertyEntity({ text: "Informar a ciência da operação automaticamente ao receber um documento destinado", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarAlcadaAprovacaoAlteracaoValorFrete = PropertyEntity({ text: "Utilizar alçada de aprovação de alteracao do valor de frete", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarAlcadaAprovacaoTabelaFrete = PropertyEntity({ text: "Utilizar alçada de aprovação de tabela de frete", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarAlcadaAprovacaoTabelaFretePorTabelaFreteCliente = PropertyEntity({ text: "Utilizar alçada de aprovação de tabela de frete para cada valor da tabela", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarAlcadaAprovacaoCarregamento = PropertyEntity({ text: "Utilizar alçada de aprovação de carregamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarAlcadaAprovacaoVeiculo = PropertyEntity({ text: "Utilizar alçada de aprovação de cadastro veículo", getType: typesKnockout.bool, val: ko.observable(true) });
    this.ExibirSituacaoAjusteTabelaFrete = PropertyEntity({ text: "Exibir Situação do Ajuste da Tabela de Frete", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarAlcadaAprovacaoLiberacaoEscrituracaoPagamentoCarga = PropertyEntity({ text: "Utilizar alçada de aprovação de liberação de escrituração e pagamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarAlcadaAprovacaoPagamento = PropertyEntity({ text: "Utilizar alçada de aprovação de pagamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarAlcadaAprovacaoAlteracaoRegraICMS = PropertyEntity({ text: "Utilizar alçada de aprovação de configuração de regra para ICMS", getType: typesKnockout.bool, val: ko.observable(false) });
    this.CriarAprovacaoCargaAoConfirmarDocumentos = PropertyEntity({ text: "Criar aprovação da carga ao confirmar documentos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirValoresPedidosNaCarga = PropertyEntity({ text: "Exibir Valores do Pedidos na Carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.BloquearCamposTransportadorQuandoEtapaNotas = PropertyEntity({ text: "Bloquear Campos do Transportador na Etapa de Notas", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarServicoPendenteVeiculoExecucaoCarga = PropertyEntity({ text: "Validar manutenções de veículos pendentes ao realizar uma carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoExibirTitulosNaFatura = PropertyEntity({ text: "Não exibir títulos na impressão da fatura", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoExibirNotasFiscaisNaFatura = PropertyEntity({ text: "Não exibir notas fiscais na impressão da fatura", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExigirRotaRoteirizadaNaCarga = PropertyEntity({ text: "Exigir rota roteirizada na carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.CadastrarNovaRotaDeveSerParaTipoOperacaoCarga = PropertyEntity({ text: "Se cadastrar nova rota deve ser para o tipo de operação da carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExigirCargaRoteirizada = PropertyEntity({ text: "Exigir carga roteirizada", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarControlePallets = PropertyEntity({ text: "Utilizar controle de pallets", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoValidarDataCancelamentoTituloNoFechamentoDaFatura = PropertyEntity({ text: "Não validar data de cancelamento do título no fechamento da fatura", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ArmazenamentoCanhotoComFilial = PropertyEntity({ text: "Armazenamento de Canhoto Com Filial", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoControlarKMLancadoNoDocumentoEntrada = PropertyEntity({ text: "Não controlar KM lançado no Documento de Entrada", getType: typesKnockout.bool, val: ko.observable(false) });
    this.LancarDocumentoEntradaAbertoSeKMEstiverErrado = PropertyEntity({ text: "Salvar Documento de Entrada em Aberto se o KM estiver errado", getType: typesKnockout.bool, val: ko.observable(false) });
    this.VisualizarTodosItensOrdemCompraDocumentoEntrada = PropertyEntity({ text: "Visualizar todos os itens da O.C. no Documento de Entrada", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirAutomatizarPagamentoTransportador = PropertyEntity({ text: "Permitir automatizar pagamento do transportador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoPermitirExclusaoPedido = PropertyEntity({ text: "Não permitir exclusão de pedido (o pedido será cancelado)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ObrigarMotivoSolicitacaoFrete = PropertyEntity({ text: "Obrigar Motivo de Solicitação de Frete", getType: typesKnockout.bool, val: ko.observable(false) });
    this.RetornarCargasPentendesIntegracaoSomenteParaIntegradoraNotasFiscais = PropertyEntity({ text: "Retornar as cargas pendentes de integração (via WS) somente para a integradora que enviou as notas fiscais (via WS)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.LiberarSelecaoQualquerVeiculoJanelaTransportador = PropertyEntity({ text: "Liberar Seleção de Qualquer Veículo na Janela do Transportador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.LiberarSelecaoQualquerVeiculoJanelaTransportadorComConfirmacao = PropertyEntity({ text: "Liberar Seleção de Qualquer Veículo na Janela do Transportador com confirmação", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirTransportadorAlterarModeloVeicular = PropertyEntity({ text: "Permitir que o transportador altere o modelo veícular no seu portal", getType: typesKnockout.bool, val: ko.observable(false) });
    this.CalcularFreteCargaJanelaCarregamentoTransportador = PropertyEntity({ text: "Calcular Frete da Janela Carregamento Transportador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.DisponibilizarCargaAutomaticamenteParaTransportadorComMenorValorFreteTabela = PropertyEntity({ text: "Disponibilizar carga automaticamente para o transportador com o menor valor de frete por tabela", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirValorDetalhadoJanelaCarregamentoTransportador = PropertyEntity({ text: "Exibir Valor Detalhado para Transportador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AtivarPlanejamentoFrotaCarga = PropertyEntity({ text: "Ativar planejamento de frota na Carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AtivarPlanejamentoFrotaNoPlanejamentoVeiculo = PropertyEntity({ text: "Ativar planejamento de frota no Planejamento de Veículo", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoPermitirRecalcularValorFreteInformadoPeloTransportador = PropertyEntity({ text: "Não permitir recalcular o valor do frete informado pelo transportador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoEnviarEmailAlteracaoDataCarregamento = PropertyEntity({ text: "Não enviar e-mail de alteração de data de carregamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.BloquearVeiculoSemTagValePedagioAtiva = PropertyEntity({ text: "Bloquear veiculo sem tag vale pedágio ativa", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirHoraAgendadaParaCargasExcedentesJanelaDescarga = PropertyEntity({ text: "Exibir Hora Agendada para cargas excedentes na Janela de Descarga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.EncaixarHorarioRetiradaProduto = PropertyEntity({ text: "Encaixar horário de retirada de produto", getType: typesKnockout.bool, val: ko.observable(false) });
    this.LiberarCargaParaCotacaoAoLiberarParaTransportadores = PropertyEntity({ text: "Liberar carga para cotação ao liberar para transportadores", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirLimiteCarregamento = PropertyEntity({ text: "Exibir Limite de Carregamento", getType: typesKnockout.bool, val: ko.observable(true) });
    this.ExibirPrevisaoCarregamento = PropertyEntity({ text: "Exibir Previsão de Carregamento", getType: typesKnockout.bool, val: ko.observable(true) });
    this.ExibirDisponibilidadeFrotaCarregamento = PropertyEntity({ text: "Exibir Disponibilidade de Frota", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoValidarDadosBancariosContratoFrete = PropertyEntity({ text: "Não validar dados bancários do terceiro ao aprovar o contrato de frete", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AjustarTipoOperacaoPeloPeso = PropertyEntity({ text: "Ajustar tipo de operação da carga pelo peso (ao avançar da etapa de notas)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirEnviarNumeroPedidoEmbarcadorViaIntegracao = PropertyEntity({ text: "Permitir enviar o número do pedido do embarcador via integração", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NotificarAlteracaoCargaAoOperador = PropertyEntity({ text: "Notificar alteração na carga ao operador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarSequenciaNumeracaoCargasViaIntegracao = PropertyEntity({ text: "Utilizar o sequencial de numeração da carga do sistema via integração", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarMunicipioDiferentePedido = PropertyEntity({ text: "Validar se o múnicipio da carga está diferente do pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarPercentualEmRelacaoValorFreteLiquidoCarga = PropertyEntity({ text: "Utilizar o percentual em relacao ao valor do frete líquido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirInformarDadosTransportadorCargaEtapaNFe = PropertyEntity({ text: "Permitir informar dados do transportador na etapa de notas", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ImportarPedidoDeixarCargaPendente = PropertyEntity({ text: "Importar pedidos e gerar cargas via thread", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarRegraICMSParaDescontarValorICMS = PropertyEntity({ text: "Utilizar regra de ICMS para descontar ou não o Imposto (se não marcada opção irá olhar para a CST, só desconta se for CST 60)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarSomenteFreteLiquidoNaImportacaoCTe = PropertyEntity({ text: "Na importação de CT-es validar somente o valor do frete líquido em relação aos pré ctes (impostos, valor a receber, valor da prestação e demais componentes serão ignorados)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarPorRaizDoTransportadorNaImportacaoCTe = PropertyEntity({ text: "Na importação de CT-es validar o transportador pela raíz do CNPJ", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoValidarDadosParticipantesNaImportacaoCTe = PropertyEntity({ text: "Na importação de CT-es Não validar dados dos Participantes", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirSalvarDadosParcialmenteInformadosEtapaTransportador = PropertyEntity({ text: "Permitir salvar dados parcialmente informados na etapa do transportador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarNotasParciaisEnvioEmissao = PropertyEntity({ text: "Não permitir emitir documentos com notas parciais pendentes", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoImprimirNotasBoletosComRecebedor = PropertyEntity({ text: "Não imprimir Notas/Boletos com recebedor", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ServerChatURL = PropertyEntity({ text: "URL do servidor CHAT Mobile ", getType: typesKnockout.string, val: ko.observable("") });
    this.ExibirEntregaAntesEtapaTransporte = PropertyEntity({ text: "Exibir controle de entrega antes da etapa de transporte", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoPermitirLancarOcorrenciasEmDuplicidadeNaSequencia = PropertyEntity({ text: "Não permitir lançar ocorrências em duplicidade na sequencia", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoPermitirLancarOcorrenciasDepoisDeOcorrenciaFinalGerada = PropertyEntity({ text: "Não permitir lançar ocorrências após gerar uma ocorrência finalizadora", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirAprovadoresOcorrenciaPortalTransportador = PropertyEntity({ text: "Exibir aprovadores no portal do transportador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirInformarLacreJanelaCarregamentoTransportador = PropertyEntity({ text: "Permitir Informar Lacre na Janela do Transportador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirRejeitarCargaJanelaCarregamentoTransportador = PropertyEntity({ text: "Permitir Rejeitar Carga na Janela do Transportador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirDesagendarCargaJanelaCarregamento = PropertyEntity({ text: "Permitir desagendar carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarValorMaximoPendentePagamento = PropertyEntity({ text: "Validar valor máximo pendente de pagamento para o tomador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExigirCategoriaCadastroPessoa = PropertyEntity({ text: "Exigir Categoria no Cadastro de Pessoa", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoGerarContratoFreteParaCTeEmitidoNoEmbarcador = PropertyEntity({ text: "Não gerar contrato quando o CT-e for emitido no embarcador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PossuiMonitoramento = PropertyEntity({ text: "Possui monitoramento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarBuscaRotaFreteManualCarga = PropertyEntity({ text: "Utilizar busca manual de rota", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoEmitirDocumentoNasCargas = PropertyEntity({ text: "Não emitir documentos na carga (CT-e/MDF-e)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteBaixarCanhotoApenasComOcorrenciaEntrega = PropertyEntity({ text: "Permitir baixar canhotos apenas com ocorrência de entrega realizada", getType: typesKnockout.bool, val: ko.observable(false) });
    this.CamposSecundariosObrigatoriosPedido = PropertyEntity({ text: "Campos secundários obrigatórios no cadastro do pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirPedidoDeColeta = PropertyEntity({ text: "Exibir Pedido de Coleta", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirAssociacaoClientesNoPedido = PropertyEntity({ text: "Exibir associação de clientes", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarAutomaticamenteNumeroPedidoEmbarcardorNaoInformado = PropertyEntity({ text: "Gerar número do pedido embarcador automaticamente quando o mesmo não for informado", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteSelecionarRotaMontagemCarga = PropertyEntity({ text: "Permite selecionar rota", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExigirInformarNotasFiscaisNoPedido = PropertyEntity({ text: "Exigir informar dados da nota para aprovação do pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarDadosCargaRelatorioPedido = PropertyEntity({ text: "Utilizar dados da carga no relatório de pedidos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarRelatorioPedidoComoStatusEntrega = PropertyEntity({ text: "Utilizar o relatório de pedidos como status de entrega", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ImportarCargasMultiEmbarcador = PropertyEntity({ text: "Importar cargas do MultiEmbarcador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ForcarFiltroModeloNaConsultaVeiculo = PropertyEntity({ text: "Forçar filtro de modelo na consulta de veículo", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirAlterarLacres = PropertyEntity({ text: "Permitir alterar lacres", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirTipoLacre = PropertyEntity({ text: "Exibir tipos de lacres", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarVeiculoVinculadoContratoDeFrete = PropertyEntity({ text: "Validar veículos vinculados a contrato de frete (permite usar apenas nas cargas do contrato do veículo)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarContratoPrestacaoServico = PropertyEntity({ text: "Utilizar contrato de prestação de serviço", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ReplicarCadastroVeiculoIntegracaoTransportadorDiferente = PropertyEntity({ text: "Replicar cadastro veiculo na integração da carga quando placa possuir cadastro para outro transportador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoUtilizarUsuarioTransportadorTerceiro = PropertyEntity({ text: "Não cadastrar usuário quando a pessoa for transportador terceiro", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExigePerfilUsuario = PropertyEntity({ text: "Exige informar perfil no cadastro de usuário", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarFreteFilialEmissoraEmbarcador = PropertyEntity({ text: "Utilizar valor do frete do embarcador para Filial Emissora", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarPesoEmbalagemProdutoParaRateio = PropertyEntity({ text: "Utilizar peso da embalagem no rateio do frete por produtos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.CadastrarRotaAutomaticamente = PropertyEntity({ text: "Cadastrar rota automaticamente", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AbrirRateioDespesaVeiculoAutomaticamente = PropertyEntity({ text: "Abrir tela de rateio de despesa de veículo automaticamente", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarCustoParaRealizarRateiosSobreDocumentoEntrada = PropertyEntity({ text: "Utilizar Custo para realizar rateios sobre Documento de Entrada", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UsarPesoProdutoSumarizacaoCarga = PropertyEntity({ text: "Utilizar peso dos produtos para sumarizar peso da carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.RatearFretePedidosAposLiberarEmissaoSemNFe = PropertyEntity({ text: "Ratear frete entre pedidos após liberar emissão sem NFe (somente pedidos com notas)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoUtilizarDeafultParaPagamentoDeTributos = PropertyEntity({ text: "Não utilizar default para pagamento de tributos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoLancarDescontosDasOcorrenciasNoAcertoDeViagem = PropertyEntity({ text: "Não lançar descontos das ocorrências no acerto de viagem", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AtivarEmissaoSubcontratacaoAgrupado = PropertyEntity({ text: "Ativar a emissão de subcontratação agrupada?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AvancarCargaAoReceberNotasPorEmail = PropertyEntity({ text: "Avançar carga automaticamente ao receber notas fiscais por e-mail", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarAlcadaAprovacaoValorTabelaFreteCarga = PropertyEntity({ text: "Utilizar alçada para aprovação do valor de frete calculado pela tabela de frete na carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.BloquearDatasRetroativasPedido = PropertyEntity({ text: "Bloquear datas retroativas no pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirInformarDataRetiradaCtrnCarga = PropertyEntity({ text: "Permitir informar a data retirada CTRN", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirInformarNumeroContainerCarga = PropertyEntity({ text: "Permitir informar o número do container", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirInformarTaraContainerCarga = PropertyEntity({ text: "Permitir informar a tara do container", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirInformarMaxGrossCarga = PropertyEntity({ text: "Permitir informar o max gross", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirInformarAnexoContainerCarga = PropertyEntity({ text: "Permitir informar anexos do container", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirInformarDatasCarregamentoCarga = PropertyEntity({ text: "Permitir informar as datas de carregamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.RatearValorOcorrenciaPeloValorFreteCTeOriginal = PropertyEntity({ text: "Ratear valor da ocorrência pelo valor do frete dos CTes originais", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirEspecieDocumentoCteComplementarOcorrencia = PropertyEntity({ text: "Exibir espécie do documento dos CT-e complementares", getType: typesKnockout.bool, val: ko.observable(false) });
    this.CamposSecundariosObrigatoriosOrdemServico = PropertyEntity({ text: "Tornar campos secundários obrigatórios na ordem de serviço", getType: typesKnockout.bool, val: ko.observable(false) });
    this.RoteirizarPorCidade = PropertyEntity({ text: "Roteirizar por cidade", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarDistanciaRoteirizacaoCarregamentoNaCarga = PropertyEntity({ text: "Utilizar a distância da roteirização do carregamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirFaixaTemperaturaNaCarga = PropertyEntity({ text: "Exibir faixa de temperatura na carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoGerarCarregamentoRedespacho = PropertyEntity({ text: "Não gerar carregamento de redespacho", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoPermitirCancelarCargaComInicioViagem = PropertyEntity({ text: "Não permitir cancelar cargas com inicio de viagem", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarPDFCTeCancelado = PropertyEntity({ text: "Gerar PDF de CTe Cancelado", getType: typesKnockout.bool, val: ko.observable(false) });
    this.FiltrarCargasPorParteDoNumero = PropertyEntity({ text: "Filtrar cargas por parte do número", getType: typesKnockout.bool, val: ko.observable(false) });
    this.FiltrarPedidosSemFiltroPorFilialNoPortalDoFornecedor = PropertyEntity({ text: "Filtrar pedidos sem filtro por filial no portal do fornecedor", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoAtualizarPesoPedidoPelaNFe = PropertyEntity({ text: "Não atualizar peso do pedido pelo peso da NFe", getType: typesKnockout.bool, val: ko.observable(false) });
    this.BuscarClientesCadastradosNaIntegracaoDaCarga = PropertyEntity({ text: "Buscar clientes cadastrados na integração da carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarProdutosDiversosNaIntegracaoDaCarga = PropertyEntity({ text: "Utilizar produtos diversos quando não enviado na integração da carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarPedidoImportacaoNotfisEtapaNFe = PropertyEntity({ text: "Gerar Pedido na importação de NOTFIS na etapa da NFe quando NOTFIS possuir número de pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoExigeAceiteTransportadorParaNFDebito = PropertyEntity({ text: "Não obriga aceite do transportador em ocorrências do tipo débito", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoPermitirImpressaoContratoFretePendente = PropertyEntity({ text: "Não permitir impressão de contratos de frete de terceiros pendentes", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExigirEmailPrincipalCadastroPessoa = PropertyEntity({ text: "Exigir E-mail Principal no Cadastro de Pessoa (Tipo Jurídica)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoExigirTrocaDeSenhaCasoCadastroPorIntegracao = PropertyEntity({ text: "Não exigir troca de senha, caso o cliente tenha sido cadastrado por integração", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirPrioridadesAutorizacaoOcorrencia = PropertyEntity({ text: "Exibir colunas de Prioridades na Autorização de Ocorrência", getType: typesKnockout.bool, val: ko.observable(false) });
    this.FixarOperadorContratouCarga = PropertyEntity({ text: "Fixar operador que contratou a carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.VerificarNFeEmOutraCargaNaIntegracao = PropertyEntity({ text: "Verificar na Integração da Carga se NFe está em outra carga ativa", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarRemetenteDestinatarioUnicoIntegracaoCarga = PropertyEntity({ text: "Validar Remetente/Destinatário único na integração", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AutomatizarGeracaoLoteEscrituracao = PropertyEntity({ text: "Automatizar a geração de lotes de escrituração", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AutomatizarGeracaoLoteEscrituracaoCancelamento = PropertyEntity({ text: "Automatizar a geração de lotes de escrituração de cancelamentos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AtivarFaturamentoAutomatico = PropertyEntity({ text: "Ativar faturamento automático para multimodal?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.EnviarBoletoApenasParaEmailSecundario = PropertyEntity({ text: "Enviar boleto apenas para e-mail secundário?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AutomatizarGeracaoLotePagamento = PropertyEntity({ text: "Automatizar a geração de lotes de pagamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PadraoInclusaiISSDesmarcado = PropertyEntity({ text: "Padrão para a inclusão de ISS na NFSe Manual como desmarcado", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizaEmissaoMultimodal = PropertyEntity({ text: "Utiliza emissão de Multimodal", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarControleHigienizacao = PropertyEntity({ text: "Utilizar Controle de Higienização", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirDownloadDANFE = PropertyEntity({ text: "Permitir download da DANFE na etapa da NFe", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirDownloadXmlEtapaNfe = PropertyEntity({ text: "Permitir download do XML na etapa da NFe", getType: typesKnockout.bool, val: ko.observable(false) });
    this.TipoOrdemServicoObrigatorio = PropertyEntity({ text: "O tipo de ordem de serviço é obrigatório", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoExibirOpcaoParaDelegar = PropertyEntity({ text: "Não exibir opção para delegar as aprovações", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoPermitirDelegarAoUsuarioLogado = PropertyEntity({ text: "Não permitir delegar ao próprio usuário logado", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirDelegarParaUsuarioComTodasAlcadasRejeitadas = PropertyEntity({ text: "Permitir delegar para usuário com todas as alçadas rejeitadas", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoSomarDistanciaPedidosIntegracao = PropertyEntity({ text: "Não somar a distância dos pedidos na integração", getType: typesKnockout.bool, val: ko.observable(false) });
    this.BloquearBaixaParcialOuParcelamentoFatura = PropertyEntity({ text: "Bloquear a baixa parcial e parcelamento de fatura", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExigirAceiteTermoUsoSistema = PropertyEntity({ text: "Exigir Aceite Termo de Uso do Sistema", getType: typesKnockout.bool, val: ko.observable(false) });
    this.DesabilitarSaldoViagemAcerto = PropertyEntity({ text: "Desabilitar saldo de viagem no Acerto de Viagem", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoAdicionarCargasTransbordoAcertoViagem = PropertyEntity({ text: "Não adicionar cargas de transbordo no acerto de viagem?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.SomarSaldoAtualMotoristaNoAcerto = PropertyEntity({ text: "Somar saldo atual do motorista no acerto", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirSaldoPrevistoAcertoViagem = PropertyEntity({ text: "Exibir saldo previsto no acerto de viagem?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.BuscarAdiantamentosSemDataInicialAcertoViagem = PropertyEntity({ text: "Buscar adiantamentos sem data inicial do acerto de viagem?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.OcultarInformacoesFaturamentoAcertoViagem = PropertyEntity({ text: "Ocultar informações de faturamento no acerto de viagem?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.OcultarInformacoesResultadoViagemAcertoViagem = PropertyEntity({ text: "Ocultar informações do resultado da viagem no acerto de viagem?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarReciboAcertoViagemDetalhado = PropertyEntity({ text: "Gerar recibo do acerto de viagem de forma detalhada?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarCargaMDFeDestinado = PropertyEntity({ text: "Gerar carga automaticamente a partir do MDF-e recebido nos destinados", getType: typesKnockout.bool, val: ko.observable(false) });
    this.LancarFolgaAutomaticamenteNoAcerto = PropertyEntity({ text: "Lançar as folgas automaticamente no acerto de viagem", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirLancamentoOutrasDespesasDentroPeriodoAcerto = PropertyEntity({ text: "Permitir que seja lançado Outras Despesas apenas dentro do período do acerto?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoFecharAcertoViagemAteReceberPallets = PropertyEntity({ text: "Não fechar acerto de viagem até receber todos os pallets?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.VisualizarPalletsCanhotosNasCargas = PropertyEntity({ text: "Visualizar pallets e canhotos nas cargas?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.HabilitarFormaRecebimentoTituloAoMotorista = PropertyEntity({ text: "Habilitar forma de recebimento por título ao motorista?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.HabilitarLancamentoTacografo = PropertyEntity({ text: "Habilitar lançamento de tacógrafo?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.SepararValoresAdiantamentoMotoristaPorTipo = PropertyEntity({ text: "Separar valores dos adiantamentos dos motoristas por tipo?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.HabilitarInformacaoAcertoMotorista = PropertyEntity({ text: "Habilitar informações do acerto ao motorista?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.HabilitarControlarOutrasDespesas = PropertyEntity({ text: "Controlar Outras Despesas no Acerto por Tipo de Despesa", getType: typesKnockout.bool, val: ko.observable(false) });
    this.VisualizarReciboPorMotoristaNoAcertoDeViagem = PropertyEntity({ text: "Habilitar a visualização do recibo ao motorista no acerto de viagem?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoObrigarInformarFrotaNoAcertoDeViagem = PropertyEntity({ text: "Não obrigar informar frota no acerto de viagem?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirInformarDistanciaNoRedespacho = PropertyEntity({ text: "Permitir informar a distância no redespacho", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarCargaDeMDFesNaoVinculadosACargas = PropertyEntity({ text: "Gerar automaticamente carga de MDF-es não vinculados à cargas", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarCargaDeCTEsNaoVinculadosACargas = PropertyEntity({ text: "Gerar automaticamente carga de CT-es não vinculados à cargas", getType: typesKnockout.bool, val: ko.observable(false) });
    this.CadastrarMotoristaEVeiculoAutomaticamenteCargaImportada = PropertyEntity({ text: "Cadastrar motoristas e veículos automaticamente na geração da carga importada", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarDestinatarioPedidoDiferentePreCarga = PropertyEntity({ text: "Validar destinatário do pedido diferente da pré carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoExigirExpedidorNoRedespacho = PropertyEntity({ text: "Não exigir que no redespacho o expedidor seja informado", getType: typesKnockout.bool, val: ko.observable(false) });
    this.RetornarDataCarregamentoDaCargaNaConsulta = PropertyEntity({ text: "Retornar data carregamento da carga na consulta", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ObterNovaNumeracaoAoDuplicarContratoFreteTerceiro = PropertyEntity({ text: "Obter nova numeração ao duplicar o contrato de frete ao cancelar a carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarContratoTerceiroSemInformacaoDoFrete = PropertyEntity({ text: "Gerar o contrato do frete sem a informação do valor do frete a receber?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ImprimirPercursoMDFe = PropertyEntity({ text: "Imprimir percurso (Estados) no MDF-e", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoAvancarEtapaComRejeicaoIntegracaoTransportadorRejeitada = PropertyEntity({ text: "Não avançar etapa NFe com rejeição integração transportador rejeitada", getType: typesKnockout.bool, val: ko.observable(false) });
    this.CadastrarMotoristaMobileAutomaticamente = PropertyEntity({ text: "Cadastrar motoristas com a opção de utilização Mobile por padrão", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AcoplarMotoristaAoVeiculoAoSelecionarNaCarga = PropertyEntity({ text: "Vincular motorista ao veículo ao selecioná-lo no pedido/carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirVariacaoNegativaContratoFreteTerceiro = PropertyEntity({ text: "Exibir variação negativa na impressão do contrato de frete", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteEmitirCargaDiferentesOrigensParcialmente = PropertyEntity({ text: "Permite emitir parcialmente cargas com origens diferentes", getType: typesKnockout.bool, val: ko.observable(false) });
    this.OrdenarCargasMobileCrescente = PropertyEntity({ text: "Retornar as cargas para o APP em ordem crescente (da mais antiga para a mais recente)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.InformarPercentualAdiantamentoCarga = PropertyEntity({ text: "Informar percentual de adiantamento do contrato de frete na carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirInformacoesBovinos = PropertyEntity({ text: "Exibir Informações de Bovinos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.CentroResultadoPedidoObrigatorio = PropertyEntity({ text: "As contas contabeis e o centro de resultado são obrigatórios no pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizaMoedaEstrangeira = PropertyEntity({ text: "Utiliza moeda estrangeira para os lançamentos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.FiltrarCargasSemDocumentosParaChamados = PropertyEntity({ text: "Filtrar cargas sem documentos para abertura de chamados", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirAssumirChamadoDeOutroResponsavel = PropertyEntity({ text: "Permitir assumir chamado de outro responsável", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizaPgtoCanhoto = PropertyEntity({ text: "Utilizar situação Pagamento nos Canhotos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoCadastrarProdutoAutomaticamenteDocumentoEntrada = PropertyEntity({ text: "Não cadastrar produto automaticamente no Documento de Entrada", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PreencherUltimoKMEntradaGuaritaTMS = PropertyEntity({ text: "Preencher automaticamente último KM de Entrada na Guarita de Saída", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ConfirmarPagamentoMotoristaAutomaticamente = PropertyEntity({ text: "Confirmar pagamento do motorista de forma automática", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarPagamentoBloqueado = PropertyEntity({ text: "Gerar Pagamentos Bloqueados", getType: typesKnockout.bool, val: ko.observable(false) });
    this.LiberarPagamentoAoConfirmarEntrega = PropertyEntity({ text: "Liberar Pagamentos Bloqueados ao confirmar a entrega", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarSomenteDocumentosDesbloqueados = PropertyEntity({ text: "Gerar Somente lote de Documentos Liberados?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.HabilitarMultiplaSelecaoEmpresaNFSManual = PropertyEntity({ text: "Habilitar multipla seleção de filial na tela de NFS Manual", getType: typesKnockout.bool, val: ko.observable(false) });
    this.InformarDataViagemExecutadaPedido = PropertyEntity({ text: "Habilitar a edição da data de viagem executada no pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarTituloFolhaPagamento = PropertyEntity({ text: "Gerar título a pagar para a importação da folha de pagamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoValidarGrupoPessoaNaIntegracao = PropertyEntity({ text: "Não validar grupo de pessoas na integração dos pedidos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.RetornarCargasAgrupadasCarregamento = PropertyEntity({ text: "Retornar cargas agrupadas no carregamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoRetornarCarregamentosSemData = PropertyEntity({ text: "Não retornar carregamento sem data de carregamento informada", getType: typesKnockout.bool, val: ko.observable(false) });
    this.EnviarEmailAnalistasChamado = PropertyEntity({ text: "Enviar e-mail para os analistas do chamado", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirPagamentoMotoristaSemCarga = PropertyEntity({ text: "Permitir gerar pagamento ao motorista sem carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarProprietarioVeiculoMovimentacaoPlaca = PropertyEntity({ text: "Validar o proprietário do veículo na movimentação de placa", getType: typesKnockout.bool, val: ko.observable(false) });
    this.BloquearFechamentoAbastecimentoSemplaca = PropertyEntity({ text: "Bloquear fechamento de abastecimento sem placa", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AgruparIntegracaoCargaComTipoOperacaoDiferente = PropertyEntity({ text: "Agrupar cargas integradas com tipo de operação diferente", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoGerarCargaDePedidoSemTipoOperacao = PropertyEntity({ text: "Não gerar carga de pedido sem tipo de operação", getType: typesKnockout.bool, val: ko.observable(false) });
    this.IniciarCadastroFuncionarioMotoristaSempreInativo = PropertyEntity({ text: "Iniciar cadastro de funcionário/motorista sempre inativo", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoDescontarValorSaldoMotorista = PropertyEntity({ text: "Não descontar o valor do saldo do motorista?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarCargoConsultaFuncionario = PropertyEntity({ text: "Validar cargo nas consultas de funcionário?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UsaPermissaoControladorRelatorios = PropertyEntity({ text: "Habilitar o uso de permissão por usuário/perfil para o controle de relatórios?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExigirDataAutorizacaoParaPagamento = PropertyEntity({ text: "Exigir data de autorização para pagamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.IndicarIntegracaoNFe = PropertyEntity({ text: "Indicar Integração de NF-e", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoProcessarTrocaAlvoViaMonitoramento = PropertyEntity({ text: "Não executar ações de troca de alvo via monitoramento (fica exclusivo por mobile)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarDataEmissaoContratoParaMovimentoFinanceiro = PropertyEntity({ text: "Utilizar a data de emissão do contrato de frete para gerar a movimentação financeira (o padrão é a data de emissão do CT-e)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoFinalizarCargasAutomaticamente = PropertyEntity({ text: "Não finalizar cargas automaticamente (necessário utilizar a tela de encerramento de cargas)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarDadosBancariosDaEmpresa = PropertyEntity({ text: "Utilizar dados bancários informados na empresa para a geração da fatura", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoCalcularDIFALParaCSTNaoTributavel = PropertyEntity({ text: "Não calcular o difal para os CST não tributárveis (40, 41 e 51)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarParticipantesDaCargaPeloPedido = PropertyEntity({ text: "Utilizar participantes da carga pelo pedido e não pelo tipo de operação", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizaNumeroDeFrotaParaPesquisaDeVeiculo = PropertyEntity({ text: "Utilizar o número de frota para a pesquisa de veículos como padrão", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarFluxoPatioPorCargaAgrupada = PropertyEntity({ text: "Gerar fluxo de pátio por carga agrupada (gera um fluxo para cada carga que foi agrupada)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarFluxoPatioAoFecharCarga = PropertyEntity({ text: "Gerar fluxo de pátio ao fechar a carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarFluxoPatioDestino = PropertyEntity({ text: "Gerar fluxo de pátio de destino", getType: typesKnockout.bool, val: ko.observable(false) });
    this.BuscarPorCargaPedidoCargasPendentesIntegracao = PropertyEntity({ text: "Retornar e confirmar a integração de cargas pendentes por carga e pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirObservacaoAprovadorAutorizacaoOcorrencia = PropertyEntity({ text: "Exibir Observação Aprovador na Autorização", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AtualizarRotasQuandoAlterarLocalizacaoCliente = PropertyEntity({ text: "Atualizar rotas quando alterar localização do cliente", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarMonitoramentoParaCargaRetornoVazio = PropertyEntity({ text: "Gerar monitoramento para cargas de retorno vazio", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AgruparCTesDiferentesPedidosMesmoDestinatario = PropertyEntity({ text: "Ratear os conhecimentos com diferentes IE's de remetente e destinatário", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarCodificacaoUTF8ConversaoPDF = PropertyEntity({ text: "Utilizar Codificação UTF8 na conversão do PDF", getType: typesKnockout.bool, val: ko.observable(false) });
    this.SolicitarConfirmacaoPedidoSemMotoristaVeiculo = PropertyEntity({ text: "Solicitar confirmação para salvar pedido sem Motorista/Veículo/Veículo reboque sem tração", getType: typesKnockout.bool, val: ko.observable(false) });
    this.SolicitarConfirmacaoPedidoDuplicado = PropertyEntity({ text: "Solicitar confirmação para salvar pedido duplicado", getType: typesKnockout.bool, val: ko.observable(false) });
    this.SolicitarConfirmacaoMovimentoFinanceiroDuplicado = PropertyEntity({ text: "Solicitar confirmação para salvar Movimento Financeiro duplicado", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AtualizarVinculoVeiculoMotoristaIntegracaoCarga = PropertyEntity({ text: "Atualizar vínculo entre veículo e motorista na integração de carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AtualizarEnderecoMotoristaIntegracaoCarga = PropertyEntity({ text: "Atualizar endereço motorista na integração da carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AtualizarDadosVeiculoIntegracaoCarga = PropertyEntity({ text: "Atualizar dados veículo na integração da carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AdicionarVeiculoTipoReboqueComoReboqueAoAdicionarCarga = PropertyEntity({ text: "Adicionar veículo tipo reboque como reboque ao adicionar carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.IgnorarCamposEssenciais = PropertyEntity({ text: "Ignorar campos essenciais?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.RetornarCTeIntulizadoNoFluxoCancelamento = PropertyEntity({ text: "Retornar os CT-es inutilizados no mesmo fluxo dos cancelados", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoObrigarDataSaidaRetornoPedido = PropertyEntity({ text: "Não obrigar o preenchimento de data de saída e retorno da viagem", getType: typesKnockout.bool, val: ko.observable(false) });
    this.MovimentarKMApenasPelaGuarita = PropertyEntity({ text: "Habilitar para movimentar o KM do veículo apenas pela passagem na Guarita", getType: typesKnockout.bool, val: ko.observable(false) });
    this.BloquearEmissaoCargaSemTempoRota = PropertyEntity({ text: "Bloquear emissão se não houver tempo de viagem na rota da carga", getType: typesKnockout.bool, val: ko.observable(false) });
    //this.UtilizarPlanoViagem = PropertyEntity({ text: "Utilizar plano de viagem", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirSelecionarReboquePedido = PropertyEntity({ text: "Permitir selecionar os reboques no pedido (não utilizará mais os reboques do veículo)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoValidarCodigoBarrasBoletoTituloAPagar = PropertyEntity({ text: "Não validar Código de Barras Boleto ao preencher em Titulos a Pagar", getType: typesKnockout.bool, val: ko.observable(false) });
    this.EncerrarMDFeAutomaticamente = PropertyEntity({ text: "Encerrar o MDF-e automáticamente a partir da data de previsão", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarMovimentacaoNaBaixaIndividualmente = PropertyEntity({ text: "Gerar movimentação na baixa de título de forma individual", getType: typesKnockout.bool, val: ko.observable(false) });
    this.RatearMovimentosDescontosAcrescimosBaixaTitulosPagar = PropertyEntity({ text: "Ratear movimentos de descontos e acréscimos na baixa de títulos a pagar", getType: typesKnockout.bool, val: ko.observable(false) });
    this.BloquearAlteracaoVeiculoPortalTransportador = PropertyEntity({ text: "Bloquear alteração veículo no portal do transportador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarCRTAverbacao = PropertyEntity({ text: "Utilizar CRT para Averbação", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarMesmoNumeroCRTCancelamentos = PropertyEntity({ text: "Utilizar mesmo número de CRT para cancelamentos duplicando a carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarMesmoNumeroMICDTACancelamentos = PropertyEntity({ text: "Utilizar mesmo número de Mic/DTA para cancelamentos duplicando a carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.HabilitarEnvioAbastecimentoExterno = PropertyEntity({ text: "Habilitar envio de abastecimento externo", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AgruparUnidadesMedidasPorDescricao = PropertyEntity({ text: "Agrupar as unidades de medidas do CT-e de Subcontratação pela descrição", getType: typesKnockout.bool, val: ko.observable(false) });
    this.HabilitarFSDA = PropertyEntity({ text: "Habilitar FSDA", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoExibirValorFreteCTeComplementarRelatorioCTe = PropertyEntity({ text: "Não exibir valor do frete dos CT-es complementares no relatório de CT-es", getType: typesKnockout.bool, val: ko.observable(false) });
    this.HabilitarHoraFiltroDataInicialFinalRelatorioCargas = PropertyEntity({ text: "Habilitar hora nos filtros de data inicial e final no relatório de cargas", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarExportacaoRelatorioCSV = PropertyEntity({ text: "Exportar relatório em CSV", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirTodasCargasNoRelatorioDeValePedagio = PropertyEntity({ text: "Exibir todas as cargas no relatório de Vale Pedágio, indiferente de possuir agrupamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.RetornarDestinatarioDaNFeQuandoTipoForNFSeNoRelatorioDeCTes = PropertyEntity({ text: "Retornar destinatário da NFe quando tipo for NFSe no relatório de CTes", getType: typesKnockout.bool, val: ko.observable(false) });
    this.InformacaoAdicionalMotoristaOrdemColeta = PropertyEntity({ text: "Utilizar informações adicionais de motorista na Ordem de Coleta", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoExibirDevolucaoPaletesSemNotaFiscal = PropertyEntity({ text: "Não exibir devolução de paletes sem nota fiscal", getType: typesKnockout.bool, val: ko.observable(false) });
    this.VincularNotasParciaisPedidoPorProcesso = PropertyEntity({ text: "Vincular as notas fiscais parciais do pedido por processo", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteEmitirCTeComplementarManualmente = PropertyEntity({ text: "O CT-e complementar pode ser emitido pela tela de CT-e manual", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarValorDescontatoComissaoMotoristaInfracao = PropertyEntity({ text: "Utilizar o valor descontato da comissão do motorista para a sua ficha no lançamento da infração", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoPermiteEmitirCargaSemSeguro = PropertyEntity({ text: "Não permite emitir carga sem seguro", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteInformarChamadosNoLancamentoOcorrencia = PropertyEntity({ text: "Permitir selecionar chamados no lançamento da ocorrências (chamados da carga selecionada)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoFecharAcertoViagemAteReceberCanhotos = PropertyEntity({ text: "Não permitir fechar o acerto de viagem sem todos os canhotos recebidos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoExibirInfosAdicionaisGridPatio = PropertyEntity({ text: "Não exibir dados adicionais do pátio (doca e observação) nas grids de pátio (guarita, expedição, etc)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirColunaCodigosAgrupadosOcorrencia = PropertyEntity({ text: "Exibir coluna de Códigos Agrupados na Autorização", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirColunaValorFreteCargaOcorrencia = PropertyEntity({ text: "Exibir coluna Valor Frete Carga na Autorização", getType: typesKnockout.bool, val: ko.observable(false) });
    this.CriarNotaFiscalTransportePorDocumentoDestinado = PropertyEntity({ text: "Criar nota fiscal de transporte à partir das notas fiscais destinadas ao transporte", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirAliquotaEtapaFreteCarga = PropertyEntity({ text: "Exibir aliquota na etapa frete da carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExigirEmpresaTituloFinanceiro = PropertyEntity({ text: "Exigir empresa no título financeiro", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarRotaFreteInformadoPedido = PropertyEntity({ text: "Utilizar a rota informada no pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ObrigatorioCadastrarRastreadorNosVeiculos = PropertyEntity({ text: "Obrigatório informar rastreador no cadastro de veículos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoAdicionarNumeroPedidoEmbarcadorObservacaoCTe = PropertyEntity({ text: "Não adicionar o número do pedido do embarcador (Número do DT) automaticamente nas observações do documento emitido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.EmitirComplementarRedespachoFilialEmissoraDiferenteUFOrigem = PropertyEntity({ text: "Emitir CT-e complementar nos redespachos quando a filial emissora for diferente da UF de Origem da Carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirJustificativaCancelamentoCarga = PropertyEntity({ text: "Exibir justificativa para cancelamento de carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirImportarOcorrencias = PropertyEntity({ text: "Permitir importar planilha de ocorrências pelo cadastro", getType: typesKnockout.bool, val: ko.observable(false) });
    this.BloquearCamposOcorrenciaImportadosDoAtendimento = PropertyEntity({ text: "Bloquear campos da ocorrência quando importado do Atendimento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AvancarEtapaDocumentosEmissaoAoVincularTodasNotasParciaisCarga = PropertyEntity({ text: "Avançar etapa de documentos para emissão ao vincular todas as notas parciais da carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ArmazenarCentroCustoDestinatario = PropertyEntity({ text: "Armazenar Centro de Custo do Destinatário", getType: typesKnockout.bool, val: ko.observable(false) });
    this.EmitirNFeRemessaNaCarga = PropertyEntity({ text: "Emitir NF-e de Remessa na Carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoPermitirAvancarCargaSemEstoque = PropertyEntity({ text: "Não permitir avançar a carga se os produtos não tiverem estoque suficiente?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarRegraICMSCTeSubcontratacao = PropertyEntity({ text: "Utilizar regra de ICMS para CT-e de subcontratação", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirAlterarDataCarregamentoCargaNoPedido = PropertyEntity({ text: "Permitir alterar a data de carregamento da carga no pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarCargaComAgrupamentoNaMontagemCargaComoCargaDeComplemento = PropertyEntity({ text: "Gerar carga com agrupamento como carga de complemento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteAdicionarNFeRepetidaParaOutroPedidoCarga = PropertyEntity({ text: "Permite adicionar a mesma NF-e em diferentes pedidos na mesma carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarPesoPedidoParaRatearPesoNFeRepetida = PropertyEntity({ text: "Utilizar o peso do pedido para ratear o peso da NF-e em diferentes pedidos na mesma carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.RealizarMovimentacaoPamcardProximoDiaUtil = PropertyEntity({ text: "Realizar movimentações PAMCARD do motorista nos próximos dias uteis", getType: typesKnockout.bool, val: ko.observable(false) });
    this.RealizarMovimentacaPagamentoMotoristaPelaDataPagamento = PropertyEntity({ text: "Realizar movimentações do Pagamento do Motorista pela data de Pagamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UsarReciboPagamentoGeracaoAutorizacaoTitulo = PropertyEntity({ text: "Usar recibo de pagamento na geração da autorização do título", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarPlacaVeiculo = PropertyEntity({ text: "Validar Placa no Cadastro", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarRENAVAMVeiculo = PropertyEntity({ text: "Validar RENAVAM no Cadastro", getType: typesKnockout.bool, val: ko.observable(false) });
    this.LancarOsServicosDaOrdemDeServicoAutomaticamente = PropertyEntity({ text: "Lançar os serviços da ordem de serviço automaticamente", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarOSAutomaticamenteCadastroVeiculoEquipamento = PropertyEntity({ text: "Gerar Ordem Serviço automaticamente no cadastro de Veículo/Equipamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.IdentificarMonitoramentoStatusViagemEmTransito = PropertyEntity({ text: "Identificar monitoramento com status de viagem em trânsito automaticamente pelo comportamento de o avanço contínuo em direção ao destino", getType: typesKnockout.bool, val: ko.observable(false) });
    this.IgnorarStatusViagemMonitoramentoAnterioresTransito = PropertyEntity({ text: "Ignorar status viagem monitoramento anterior (caso não tenha entrado em carregamento)", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(false) });
    this.ExibirOpcaoReenviarNotfisComFalhas = PropertyEntity({ text: "Exibir opção para reenviar notfis com falhas", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirOpcaoDownloadPlanilhaRateioOcorrencia = PropertyEntity({ text: "Exibir opção de Download Planilha de Rateio na Autorização", getType: typesKnockout.bool, val: ko.observable(false) });
    this.RetornarCanhotosViaIntegracaoEmQualquerSituacao = PropertyEntity({ text: "Retornar canhotos via integração sem estar digitalizado", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirNumeroPagerEtapaInicialCarga = PropertyEntity({ text: "Exibir Número Pager Etapa Inicial", getType: typesKnockout.bool, val: ko.observable(false) });
    this.IdentificarVeiculoParado = PropertyEntity({ text: "Identificar veículo parado por permanecer imóvel em posições subsequentes", getType: typesKnockout.bool, val: ko.observable(false) });
    this.IdentificarCarregamentoAoIniciarOuFinalizarMonitoramentosConsecutivos = PropertyEntity({ text: "Identificar carregamento ao Iniciar/Finalizar monitoramentos consecutivos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.IdentificarEntradaEmAlvoComPosicaoUnicaIgnorandoTemposDePermanencia = PropertyEntity({ text: "Identificar Entrada e Saída em Alvo com posição única, ignorando tempos de permanência", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.FinalizarMonitoramentoAoGerarTransbordoCarga = PropertyEntity({ text: "Finalizar Monitoramento ao gerar carga de Transbordo", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.ExigirDataEntregaNotaClienteCanhotos = PropertyEntity({ text: "Exigir Data de Entrega da Nota Fiscal ao Cliente", getType: typesKnockout.bool, val: ko.observable(false) });
    this.HabilitarControleFluxoNFeDevolucaoChamado = PropertyEntity({ text: "Habilitar Controle de Fluxo de NFe de Devolução", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ProcessarFilaDocumentosEmLote = PropertyEntity({ text: "Processar os documentos em outra fila para as cargas em lote", getType: typesKnockout.bool, val: ko.observable(false) });
    this.IncluirBCCompontentesDesconto = PropertyEntity({ text: "Incluir na BC componentes de desconto padrão", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteEncerrarMDFeEmitidoNoEmbarcador = PropertyEntity({ text: "Permitir encerrar MDF-e emitido pelo embarcador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ApresentarCodigoIntegracaoComNomeFantasiaCliente = PropertyEntity({ text: "Apresentar código de integração do cliente com o nome fantasia do cliente", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AjustarDataContratoIgualDataFinalizacaoCarga = PropertyEntity({ text: "A data de emissão do contrato de frete deve ser a data de finalização de emissão da carga (será ajustado ao finalizar a emissão)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirClassificacaoNFe = PropertyEntity({ text: "Exibir Classificação NF-e", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirSenhaCadastroPessoa = PropertyEntity({ text: "Exibir senha de liberação mobile", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExpedidorIgualRemetente = PropertyEntity({ text: "Gerar CT-e com expedidor igual ao remetente", getType: typesKnockout.bool, val: ko.observable(false) });
    this.RecebedorIgualDestinatario = PropertyEntity({ text: "Gerar CT-e com recebedor igual ao destinatário", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarRaizCNPJGrupoPessoa = PropertyEntity({ text: "Validar raiz de CNPJ do grupo ao adicionar/atualizar pessoa", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteOutrosOperadoresAlterarLancamentoProspeccao = PropertyEntity({ text: "Permite que outros operadores alterem o lançamento de Prospecção?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ControlarAgendamentoSKU = PropertyEntity({ text: "Controlar Agendamento por SKU", getType: typesKnockout.bool, val: ko.observable(false) });
    this.RemoverEtapaAgendamentoAgendamentoColeta = PropertyEntity({ text: "Remover Etapa de Agendamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ConsultarSomenteTransportadoresPermitidosCadastro = PropertyEntity({ text: "Consultar somente Transportadores Permitidos para Cadastro", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarAutomaticamenteSenhaPedidosAgendas = PropertyEntity({ text: "Gerar Automaticamente a Senha dos Pedidos Agendas", getType: typesKnockout.bool, val: ko.observable(false) });
    this.CalcularDataDeEntregaPorTempoDeDescargaDaRota = PropertyEntity({ text: "Calcular Data de Entrega por Tempo de Descarga da Rota", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NotificarCargaAgConfirmacaoTransportador = PropertyEntity({ text: "Notificar Carga Ag. Confirmação Transportador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.DeixarAbastecimentosMesmaDataHoraInconsistentes = PropertyEntity({ text: "Deixar abastecimentos com mesma data e hora como Inconsistentes?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.VisualizarTipoOperacaoDoPedidoPorTomador = PropertyEntity({ text: "Visualizar Tipos de Operação por Tomador no Pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UsarGrupoDeTipoDeOperacaoNoMonitoramento = PropertyEntity({ text: "Usar grupo de tipo de operação no monitoramento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UsarGrupoDeTipoDeOperacaoNoMonitoramentoOcultarGrupoStatusViagem = PropertyEntity({ text: "Ocultar grupos de status de viagem", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.JustificarEntregaForaDoRaio = PropertyEntity({ text: "Justificar Entrega Fora do Raio", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PadraoTagValePedagioVeiculos = PropertyEntity({ text: "Por padrão a tag de vale pedágio deve vir marcada no cadastro do veículo", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoBloquearCargaComProblemaIntegracaoGrMotoristaVeiculo = PropertyEntity({ text: "Não bloquear carga com problema na GR Motorista/Veículo", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AvisarMDFeEmitidoEmbarcadorSemSeguroValido = PropertyEntity({ text: "Avisar quando houver MDF-e emitido pelo embarcador sem seguro válido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarCanhotoSempre = PropertyEntity({ text: "Gerar canhoto sempre, independente das demais configurações existentes", getType: typesKnockout.bool, val: ko.observable(false) });
    this.RetornarCanhotoParaPendenteAoReceberUmaNotaJaDigitalizada = PropertyEntity({ text: "Retornar canhoto para pendente ao receber uma nota já digitalizada", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarNFSeImportacaoEmbarcador = PropertyEntity({ text: "Gerar NFS-e na importação de cargas/ocorrências do embarcador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.InformarAjusteManualCargasImportadasEmbarcador = PropertyEntity({ text: "Informar ajuste manual em cargas importadas do embarcador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.FiltrarNotasCompativeisPeloDestinatario = PropertyEntity({ text: "Filtrar notas compatíveis pelo destinatário do pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirFiltrosNotasCompativeisCarga = PropertyEntity({ text: "Exibir filtros de notas compatíveis na carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoExibirPessoasChamado = PropertyEntity({ text: "Não exibir Pessoas nas colunas da Pesquisa", getType: typesKnockout.bool, val: ko.observable(false) });
    this.SalvarAnaliseEmAnexoAoLiberarOcorrenciaChamado = PropertyEntity({ text: "Salvar impressão da análise em anexo ao Liberar Ocorrência", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ResponderAnaliseAoLiberarOcorrenciaChamado = PropertyEntity({ text: "Responder análise ao transportador ao Liberar Ocorrência", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirEstornarAprovacaoChamadoLiberado = PropertyEntity({ text: "Permitir estornar aprovação do Chamado", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizaListaDinamicaDatasChamado = PropertyEntity({ text: "Utiliza lista dinâmica de datas no Chamado", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoFinalizarDocumentoEntradaOSValorDivergente = PropertyEntity({ text: "Não permitir finalizar Documento de Entrada ou O.S. com valores divergentes do orçado", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoFinalizarDocumentoEntradaOrdemCompraValorDivergente = PropertyEntity({ text: "Não permitir finalizar Documento de Entrada ou O.C. com valores divergentes do total", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoFinalizarDocumentoEntradaComAbastecimentoInconsistente = PropertyEntity({ text: "Não permitir finalizar Documento de Entrada com abastecimento inconsistente", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarNotaFiscalExistenteNaImportacaoCTeEmbarcador = PropertyEntity({ text: "Utilizar nota fiscal existente na base para importação de CT-es emitidos pelo embarcador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.BuscarCargaPorNumeroPedido = PropertyEntity({ text: "Buscar Carga Por Número do Pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.BloquearSemRegraAprovacaoOrdemServico = PropertyEntity({ text: "Bloquear se não tiver regra aprovação cadastrada pra Ordem de Serviço", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoExibirRotaJanelaCarregamento = PropertyEntity({ text: "Não Exibir Rota na Janela", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ArmazenarPDFDANFE = PropertyEntity({ text: "Armazenar os PDFs das DANFES?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoExibirLocalCarregamentoJanelaCarregamento = PropertyEntity({ text: "Não Exibir Local de Carregamento na Janela", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ObrigarFotoNaEntrega = PropertyEntity({ text: "Obrigar foto na entrega", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ObrigarFotoNaDevolucao = PropertyEntity({ text: "Obrigar foto na devolução", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteQRCodeMobile = PropertyEntity({ text: "Permite identificar cliente por QR Code", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarOcorrenciaComplementoSubcontratacao = PropertyEntity({ text: "Permite gerar ocorrência de complemento para CT-es de terceiros", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoControlarSituacaoVeiculoOrdemServico = PropertyEntity({ text: "Não controlar situação do veículo pela Ordem de Serviço", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AtualizarHistoricoSituacaoVeiculo = PropertyEntity({ text: "Atualizar Histórico de situações do veículo por rotina", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarPreviewDOCCOBFatura = PropertyEntity({ text: "Permite gerar preview do DOCCOB na fatura", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarLimiteCreditoNoPedido = PropertyEntity({ text: "Validar o limite de crédito na criação do pedido?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirAlterarDataPrevisaoEntregaPedidoNoCarga = PropertyEntity({ text: "Permitir alterar a data de previsão de entrega do pedido na carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NecessarioInformarJustificativaAoAlterarDataSaidaOuPrevisaoEntregaPedidoNaCarga = PropertyEntity({ text: "Necessário informar justificativa ao alterar Data de Saída ou Previsao de Entrega do Pedido na carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoValidarTomadorCTeSubcontratacaoComTomadorPedido = PropertyEntity({ text: "Não validar o tomador do CT-e de subcontratação com o tomador do pedido (etapa de documentos da carga)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.LimitarApenasUmMonitoramentoPorPlaca = PropertyEntity({ text: "Limitar a apenas um monitoramento em andamento por placa de veículo", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ConfirmarEntregaDigitilizacaoCanhoto = PropertyEntity({ text: "Confirmar Entrega ao Digitalizar o canhoto no portal do embarcador?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.RetornarCargaPendenciaEmissao = PropertyEntity({ text: "Retornar cargas com situação Pendência Emissão no método BuscarCTes", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AvisarDivergenciaValoresCTeEmitidoEmbarcador = PropertyEntity({ text: "Avisar divergência de valores para CT-es emitidos no embarcador X tabela de frete", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AdicionarRelatorioRelacaoEntregaDownloadDocumentos = PropertyEntity({ text: "Adicionar o Relatório de Relação de Entrega no Download dos Documentos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirAtualizarInicioViagem = PropertyEntity({ text: "Permitir atualizar o início da viagem", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirQuantidadeVolumesNF = PropertyEntity({ text: "Exibir quantidade de volumes das NFs", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirContatoWhatsApp = PropertyEntity({ text: "Permitir Contato WhatsApp", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirAtualizarPrevisaoControleEntrega = PropertyEntity({ text: "Permitir atualizar a previsão de entrega", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirAtualizarPrevisaoEntregaPedidoControleEntrega = PropertyEntity({ text: "Permitir atualizar a previsão de entrega do pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarPrevisaoEntregaPedidoComoDataPrevista = PropertyEntity({ text: "Utilizar a previsão de entrega do pedido como data prevista", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarMaiorDataColetaPrevistaComoDataPrevistaParaEntregaUnica = PropertyEntity({ text: "Utilizar a maior data de coleta prevista como data prevista para entrega única", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ControlarEstoqueNegativo = PropertyEntity({ text: "Possui controle para não permitir produtos com estoque negativo", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteCadastrarLatLngEntregaLocalidade = PropertyEntity({ text: "Permite cadastrar lat x lng entrega em localidades", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoUtilizarDataTerminoProgramacaoVeiculo = PropertyEntity({ text: "Não Utilizar Data Término na Programação de Veículo", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarLocalidadePrestacaoPedido = PropertyEntity({ text: "Permitir informar a localidade de início e término da prestação no pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.HabilitarWidgetAtendimento = PropertyEntity({ text: "Habilitar Widget de Atendimento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.FiltrarWidgetAtendimentoProFiltro = PropertyEntity({ text: "Filtrar Widget de Atendimento por filtro", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteRemoverReentrega = PropertyEntity({ text: "Permite remover reentrega", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarValidadeServicoPeloGrupoServicoOrdemServico = PropertyEntity({ text: "Utilizar validade serviço pelo Grupo Serviço na Ordem de Serviço", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirLancarAvariasSomenteParaProdutosDaCarga = PropertyEntity({ text: "Permitir informar somente produtos da carga no lançamento da avaria", getType: typesKnockout.bool, val: ko.observable(false) });
    this.OcultarOcorrenciasGeradasAutomaticamente = PropertyEntity({ text: "Ocultar Ocorrências Geradas Automaticamente", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoEncerrarViagemAoEncerrarControleEntrega = PropertyEntity({ text: "Não encerrar viagens ao finalizar o controle de entrega", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoUtilizarRegraEntradaDocumentoGrupoNCM = PropertyEntity({ text: "Não utilizar regra para documentos de entrada por grupo de NCM", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteAlterarRotaEmCargaFinalizada = PropertyEntity({ text: "Permitir alterar a rota de frete em cargas em transporte/finalizadas", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoPreencherSerieCTeManual = PropertyEntity({ text: "Não preencher a série automaticamente no CT-e manual", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoObrigarInformarSegmentoNoAcertoDeViagem = PropertyEntity({ text: "Não obrigar informar o segmento no acerto de viagem", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarReciboDetalhadoAcertoViagem = PropertyEntity({ text: "Gerar o recibo do acerto de viagem de forma detalhada", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoBuscarDataInicioViagemAcerto = PropertyEntity({ text: "Não buscar data de inicio de viagem?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.SolicitarAprovacaoFolgaAcertoViagem = PropertyEntity({ text: "Solicitar a aprovação de folga no acerto de viagem?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarWebServiceRestATM = PropertyEntity({ text: "Utilizar WebService REST na averbação de CT-e para a AT&M", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarIntegracaoAverbacaoBradescoEmbarcador = PropertyEntity({ text: "Utilizar integracão de averbação do Bradesco no embarcador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirJanelaDescargaPorPeriodo = PropertyEntity({ text: "Exibir Janela Descarga por Período", getType: typesKnockout.bool, val: ko.observable(false) });
    this.VisualizarValorNFSeDescontandoISSRetido = PropertyEntity({ text: "Visualizar valor de NFS-e descontando ISS Retido na carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoPreencherMotoristaVeiculoAbastecimento = PropertyEntity({ text: "Não preencher automaticamente o motorista atrelado do veículo no abastecimento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoValidarMediaIdealAbastecimento = PropertyEntity({ text: "Não validar a média ideal no abastecimento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoValidarMediaIdealDeArlaAbastecimento = PropertyEntity({ text: "Não validar a média ideal de ARLA no abastecimento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarMesmoKMComLitrosDiferenteAbastecimento = PropertyEntity({ text: "Validar mesmo KM/Horímetro com Litros diferentes no abastecimento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoDeixarAbastecimentoTerceiroInconsistente = PropertyEntity({ text: "Não deixar abastecimento de terceiro inconsistente", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UsarDataChecklistVeiculo = PropertyEntity({ text: "Usar data checklist veículo", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoUtilizarSerieCargaCTeManual = PropertyEntity({ text: "Não utilizar série da carga para emissão de CT-e manual (utilizará a série configurada na empresa)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizaMultiplosLocaisArmazenamento = PropertyEntity({ text: "Utiliza Múltiplos Locais de Armazenamento de Produtos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizaSuprimentoDeGas = PropertyEntity({ text: "Utilizar suprimento de gás", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteImportarPlanilhaValoresFreteNFSManual = PropertyEntity({ text: "Permitir importar planilha com valores de fretes NFS Manual", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ConsiderarPedagioDescargaVariacaoContratoFreteTerceiro = PropertyEntity({ text: "Considerar pedágio e descarga para cálculo da variação do contrato de frete", getType: typesKnockout.bool, val: ko.observable(false) });
    this.VisualizarDatasRaioNoAtendimento = PropertyEntity({ text: "Visualizar Datas de Entrada/Saída do Raio no Atendimento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExigirClienteResponsavelPeloAtendimento = PropertyEntity({ text: "Exigir o Cliente Responsável pelo Atendimento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.MonitorarPassagensFronteiras = PropertyEntity({ text: "Monitorar passagens por fronteiras", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AtualizarMonitoramentoAoGerarMDFeManual = PropertyEntity({ text: "Atualizar monitoramento ao gerar MDF-e manual", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarDadosSumarizadosDasParadasAoFinalizarOMonitoramento = PropertyEntity({ text: "Gerar dados sumariados das paradas ao finalizar o monitoramento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarMonitoramentoAoFecharCarga = PropertyEntity({ text: "Gerar monitoramento ao fechar carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ConsiderarDataAuditoriaComoDataFimDoMonitoramento = PropertyEntity({ text: "Considerar a data da ação de finalização de viagem (Auditoria) e não a data de confirmação informada manualmente como data de fim do monitoramento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.FinalizarAutomaticamenteAlertasDoMonitoramentoAoFinalizarViagem = PropertyEntity({ text: "Finalizar automaticamente Alertas do Monitoramento ao Finalizar o Monitoramento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarModalAntigoDetalhesMonitoramento = PropertyEntity({ text: "Utilizar Modal Antigo Detalhes Monitoramento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.HabilitarVinculoPermanenciasComHistoricoStatusViagem = PropertyEntity({ text: "Habilitar Vinculo de Permanencias com Historico Status Viagem", getType: typesKnockout.bool, val: ko.observable(false) });
    this.FinalizarAutomaticamenteAlertasDoMonitoramentoPeriodicamente = PropertyEntity({ text: "Finalizar automaticamente Alertas do Monitoramento periodicamente (Dias)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AtualizarStatusViagemMonitoramentoAoIniciarViagem = PropertyEntity({ text: "Atualizar status viagem monitoramento ao iniciar viagem", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ManterMonitoramentosDeCargasCanceladasAoReceberNovaCarga = PropertyEntity({ text: "Manter Monitoramento de carga Cancelada ao receber nova carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoGerarNovoMonitoramentoCarga = PropertyEntity({ text: "Não gerar novo monitoramento quando já existe um monitoramento finalizado para a carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ObrigarSelecaoRotaQuandoExistirMultiplas = PropertyEntity({ text: "Obrigatório selecionar rota quando existir vária", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoDuplicarCargaAoCancelarPorImportacaoXMLCTeCancelado = PropertyEntity({ text: "Não duplicar a carga ao cancelar pela importação de XML de CT-e cancelado", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ModeloVeicularCargaNaoObrigatorioMontagemCarga = PropertyEntity({ text: "Modelo veicular de carga não é obrigatório", getType: typesKnockout.bool, val: ko.observable(false) });
    this.BloquearCancelamentoCargasComDataCarregamentoEDadosTransporteInformados = PropertyEntity({ text: "Bloquear cancelamento de cargas com data de carregamento e dados de transporte informados", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarOcorrenciaParaCargaAgrupada = PropertyEntity({ text: "Gerar ocorrência para cargas agrupadas?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AprovarAutomaticamenteCteEmitidoComValorInferiorAoEsperado = PropertyEntity({ text: "Aprovar automaticamente CT-es emitidos com valores inferiores aos dos pré CT-es?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarExistenciaDeConfiguracaoFaturaDoTomador = PropertyEntity({ text: "Validar existência de configuração da fatura do Tomador na criação do pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarConfiguracaoFaturamentoTomador = PropertyEntity({ text: "Validar existência de configuração da fatura do Tomador na etapa do frete", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExigirNumeroDocumentoTituloFinanceiro = PropertyEntity({ text: "Exigir Número do Documento no título financeiro", getType: typesKnockout.bool, val: ko.observable(false) });
    this.BloquearVeiculoExistenteEmCargaNaoFinalizada = PropertyEntity({ text: "Bloquear veículo que já esteja em uma carga não finalizada", getType: typesKnockout.bool, val: ko.observable(false) });
    this.BloquearLancamentoServicoDuplicadoOrdemServico = PropertyEntity({ text: "Bloquear lançamento de Serviço duplicado na Ordem de Serviço", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirAtualizarModeloVeicularCargaDoVeiculoNoWebService = PropertyEntity({ text: "Permitir atualizar o modelo veicular de carga no web service", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AverbarMDFe = PropertyEntity({ text: "Averbar MDF-e", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirGerarNotaMesmoPedidoCarga = PropertyEntity({ text: "Permitir gerar notas somente para o mesmo pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirGerarNotaMesmaCarga = PropertyEntity({ text: "Permitir gerar notas somente para a mesma carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.MonitoramentoStatusViagemQuandoFicarSemStatusManterUltimo = PropertyEntity({ text: "Quando o monitoramento ficar sem status de viagem, deve manter o último/anterior", getType: typesKnockout.bool, val: ko.observable(false) });
    this.MonitoramentoConsiderarPosicaoTardiaParaAtualizarInicioFimEntregaViagem = PropertyEntity({ text: "Considerar posição tardia para atualizar início/fim da entrega/viagem", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.TelaMonitoramentoPadraoFiltroDataInicialFinal = PropertyEntity({ text: "Padrão no filtro na datas inicial com D-2 e final com D+2", getType: typesKnockout.bool, val: ko.observable(false) });
    this.TelaMonitoramentoFiltroFilialDaCarga = PropertyEntity({ text: "Padrão no filtro da filial, Somente filial da Carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.TelaMonitoramentoAtualizarGridAoReceberAtualizacoesOnTime = PropertyEntity({ text: "Atualizar Grid/Consulta de Monitoramento ao receber atualizações on-time", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AoInativarMotoristaTransformarEmFuncionario = PropertyEntity({ text: "Ao inativar o motorista transformar o mesmo em funcionário", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoGerarCTesComValoresZerados = PropertyEntity({ text: "Não emitir documentos de pedidos com valores zerados", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarDataPrevisaoSaidaPedidoMenorDataAtual = PropertyEntity({ text: "A data de previsão de saída dos pedidos não pode ser menor que a data atual", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarDataPrevisaoEntregaPedidoMenorDataAtual = PropertyEntity({ text: "A data de previsão de entrega/retorno dos pedidos não pode ser menor que a data atual", getType: typesKnockout.bool, val: ko.observable(false) });
    this.BloquearVeiculosComMdfeEmAberto = PropertyEntity({ text: "Bloquear veículos com MDF-e em aberto", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirOpcaoLiberarParaTransportador = PropertyEntity({ text: "Exibir opção liberar para transportador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirOpcaoMultiModalAgendamentoColeta = PropertyEntity({ text: "Exibir opções Multimodal no agendamento de coleta", getType: typesKnockout.bool, val: ko.observable(false) });
    this.SugerirDataEntregaAgendamentoColeta = PropertyEntity({ text: "Sugerir a data de entrega no agendamento de coleta", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarFluxoPatioCargaComExpedidor = PropertyEntity({ text: "Gerar fluxo de pátio para cargas com expedidor", getType: typesKnockout.bool, val: ko.observable(false) });
    this.BuscarMotoristaDaCargaLancamentoAbastecimentoAutomatico = PropertyEntity({ text: "Buscar motorista da carga no lançamento de abastecimento automático?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.CalcularFreteCliente = PropertyEntity({ text: "Calcular o Frete do Cliente", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirVencimentoRetroativoFatura = PropertyEntity({ text: "Permitir data de vencimento retroativa na fatura (menor que a data de emissão)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarObservacaoRegraICMSAposObservacaoCTe = PropertyEntity({ text: "Gerar a Observação da Regra de ICMS após as demais observações do CT-e", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarComponentesDeFreteComImpostoIncluso = PropertyEntity({ text: "Gerar componentes de frete com imposto incluso (requer configurações adicionais)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ConsultarRegraICMSGeracaoCTeSubstitutoAutomaticamente = PropertyEntity({ text: "Consultar regra de ICMS para geração de CT-e substituto automaticamente", getType: typesKnockout.bool, val: ko.observable(false) });
    this.EnviarRegraExclusivaCodigoImpostoLayoutINTNC = PropertyEntity({ text: "Enviar o código de imposto 090 para CST 090 e CFOP 5932 ou 6932 no arquivo EDI com layout INTNC", getType: typesKnockout.bool, val: ko.observable(false) });
    this.DesconsiderarSobraRateioParaBaseCalculoIBSCBS = PropertyEntity({ text: "Desconsiderar sobra do rateio para base cálculo IBS/CBS", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarICMSTelaCotacaoPedidosRegraICMS = PropertyEntity({ text: "Validar ICMS na tela de Cotação de Pedidos direto da Regra de ICMS", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirInformarQuilometragemTabelaFreteCliente = PropertyEntity({ text: "Permitir informar KM (Meramente informativo) no cadastro de Tabela de Frete", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirInformarLeadTimeTabelaFreteCliente = PropertyEntity({ text: "Permitir informar lead time no cadastro de Tabela de Frete", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarIntegracaoAlteracaoTabelaFrete = PropertyEntity({ text: "Utilizar integração de alteração da Tabela de Frete", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarVigenciaConfiguracaoDescargaCliente = PropertyEntity({ text: "Utilizar vigência nos valores de descarga do cliente", getType: typesKnockout.bool, val: ko.observable(false) });
    this.MostrarRegistroSomenteComValoresNaAprovacao = PropertyEntity({ text: "Mostrar registros somente com valores na aprovação", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ObrigatorioInformarTransportadorAjusteTabelaFrete = PropertyEntity({ text: "Obrigatório informar o transportador no ajuste da tabela de frete", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ObrigatorioInformarContratoTransporteFreteAjusteTabelaFrete = PropertyEntity({ text: "Obrigatório informar o contrato do transportador no ajuste da tabela de frete", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoBuscarAutomaticamenteVigenciaTabelaFrete = PropertyEntity({ text: "Não buscar automaticamente a vigência da tabela de frete", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ImportarTabelaFreteClienteInformandoOrigensDestinosEmDiferentesColunasNoMesmoArquivo = PropertyEntity({ text: "Importar Tabela de Frete Cliente informando Origens e Destinos em diferentes Colunas no mesmo arquivo", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GravarAuditoriaImportarTabelaFrete = PropertyEntity({ text: "Gravar auditoria ao importar tabela frete", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ArredondarValorDoComponenteDePedagioParaProximoInteiro = PropertyEntity({ text: "Arredondar o valor do componente de pedágio para o próximo número inteiro", getType: typesKnockout.bool, val: ko.observable(false) });
    this.SalvarPlacasVeiculosAoSalvarModelosVeiculos = PropertyEntity({ text: "Salvar todas as placas dos veiculos ao salvar os modelos de veiculos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoPermiteEdicoesEmValoresNaConsultaDeTabelaFrete = PropertyEntity({ text: "Não permite edições em valores por meio da Consulta de Tabelas de Frete", getType: typesKnockout.bool, val: ko.observable(false) });
    this.TabelaFretePrecisaoDinheiroDois = PropertyEntity({ text: "Mostrar apenas duas casas decimais em valores monetários", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoAlterarCentroResultadoMotorista = PropertyEntity({ text: "Não alterar o centro de resultado dos motoristas pela emissão de carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoPermitirRealizarCadastroPlacaBloqueada = PropertyEntity({ text: "Não Permitir criar novos cadastros para placas que possui status de bloqueado", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoConsiderarProdutosSemPesoParaSumarizarVolumes = PropertyEntity({ text: "Não considerar produtos sem peso para sumarizar quantidade de volumes", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PreencherPeriodoFaturaComDataAtual = PropertyEntity({ text: "Preencher período da fatura com a data atual", getType: typesKnockout.bool, val: ko.observable(false) });
    this.BloquearEmissaoTomadorSemEmail = PropertyEntity({ text: "Bloquear emissão caso o tomador/grupo do tomador do pedido não tenha e-mail configurado para envio da documentação", getType: typesKnockout.bool, val: ko.observable(false) });
    this.InformarDataCancelamentoCancelamentoFatura = PropertyEntity({ text: "Informar a data de cancelamento ao cancelar a fatura", getType: typesKnockout.bool, val: ko.observable(false) });
    this.DisponbilizarProvisaoContraPartidaParaCancelamento = PropertyEntity({ text: "Disponibilizar provisão de contrapartida para cancelamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarNumeracaoFaturaAnual = PropertyEntity({ text: "Gerar numeração da fatura anualmente?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoEmitirDocumentosEmCargasDeReentrega = PropertyEntity({ text: "Não emitir documentos para pedidos de reentrega nas cargas", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoPermitirInativarFuncionarioComSaldo = PropertyEntity({ text: "Não permitir inativar funcionário/motorista com saldo", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoPermitirTransportadoAlterarDataValidadeSeguradora = PropertyEntity({ text: "Não permitir transportador alterar a data de validade da seguradora", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarPesoLiquidoNFeParaCTeMDFe = PropertyEntity({ text: "Peso Padrão das NFes para emissão de CTe e MDFe", val: ko.observable(false), options: _liquidoBrutoBool });
    this.NaoSolicitarAtuorizacaoAbastecimento = PropertyEntity({ text: "Não solicitar autorização dos abastecimentos no acerto de viagem?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PreencherDataProgramadaComAtualCheckList = PropertyEntity({ text: "Preencher Data Programada com a atual na manutenção do Check List?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.SomenteAutorizadoresPodemDelegarOcorrencia = PropertyEntity({ text: "Somente autorizadores podem delegar ocorrência?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExigirMotivoOcorrencia = PropertyEntity({ text: "Exigir motivo na ocorrência?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoRetornarNotasEmDocumentoComplementar = PropertyEntity({ text: "Não retornar notas no WS em documentos complementares?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarEtiquetaDetalhadaWMS = PropertyEntity({ text: "Utilizar etiqueta detalhada no WMS?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarSeExisteVeiculoCadastradoComMesmoNrDeFrota = PropertyEntity({ text: "Validar se já existe veículo com mesmo numero de frota", getType: typesKnockout.bool, val: ko.observable(false) });
    this.HabilitarAlertaCargasParadas = PropertyEntity({ text: "Habilitar alerta de cargas paradas?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteInformarModeloVeicularCargaOrigem = PropertyEntity({ text: "Permite informar modelo veicular da carga de origem", getType: typesKnockout.bool, val: ko.observable(false) });
    this.RetornarTitulosNaoGerados = PropertyEntity({ text: "Retornar todos os CTes (mesmo sem título gerado) na consulta por periodo de títulos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoExigirMotivoAprovacaoCTeInconsistente = PropertyEntity({ text: "Não exigir motivo para aprovação na tela de aprovação de CT-e com inconsistência", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExigirTipoMovimentoLancamentoMovimentoFinanceiroManual = PropertyEntity({ text: "Exigir tipo de movimento para lançar movimento financeiro manual", getType: typesKnockout.bool, val: ko.observable(false) });
    this.FinalizarMonitoramentoEmAndamentoDoVeiculoAoIniciar = PropertyEntity({ text: "Finalizar monitoramento em andamento do veículo ao iniciar", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirFiltroEColunaCodigoPedidoClienteGestaoDocumentos = PropertyEntity({ text: "Exibir filtro/coluna Número Pedido Cliente em Gestão Documentos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirNumeroPedidoEmbarcadorGestaoDocumentos = PropertyEntity({ text: "Exibir N° do Pedido Embarcador na Gestão de Documentos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirDeixarDocumentoEmTratativa = PropertyEntity({ text: "Permitir deixar documento em tratativa", getType: typesKnockout.bool, val: ko.observable(false) });
    this.Despachante = PropertyEntity({ text: "Despachante:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.UtilizarCentroResultadoNoRateioDespesaVeiculo = PropertyEntity({ text: "Utilizar Centro de Resultado no Rateio de Despesa de Veículo", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirDarBaixaFaturasCTe = PropertyEntity({ text: "Permitir dar Baixa manual nas faturas do CTe", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PedidoOcorrenciaColetaEntregaIntegracaoNova = PropertyEntity({ text: "Nova integração de ocorrência de coleta/entrega de pedidos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.HabilitarEstadoPassouRaioSemConfirmar = PropertyEntity({ text: "Habilitar estado \"Passou pelo raio sem confirmar\" para entregas e coletas", getType: typesKnockout.bool, val: ko.observable(false) });
    this.HabilitarIconeEntregaAtrasada = PropertyEntity({ text: "Habilitar ícone de entrega atrasada", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ApagarCampoRotaAoDuplicarPedido = PropertyEntity({ text: "Apagar o campo rota ao duplicar pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.RegistrarEntregasApenasAposAtenderTodasColetas = PropertyEntity({ text: "Registrar entregas apenas após atender todas as coletas", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(false) });
    this.HabilitarDescontoGestaoDocumento = PropertyEntity({ text: "Habilitar descontos na gestão de documentos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.CopiarDataTerminoCarregamentoCargaParaPrevisaoEntregaPedidos = PropertyEntity({ text: "Copiar a data de Término de Carregamento da carga para a Previsao de Entrega dos pedidos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.LiberarIntegracaoTransportadorDeCargaImportarDocumentoManual = PropertyEntity({ text: "Liberar integração transportador em carga com importação de documento manual", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoValidarTabelaFreteMesmaIncidenciaImportacao = PropertyEntity({ text: "Não validar tabela de frete com a mesma incidência na importação da tabela por arquivo", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UsarAlcadaAprovacaoGestaoDocumentos = PropertyEntity({ text: "Utilizar alçada de aprovações de documentos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PessoasNaoObrigatorioProdutoEmbarcador = PropertyEntity({ text: "Pessoas/Grupo Pessoas não obrigatórios no cadastro do Produto Embarcador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirPesoCargaEPesoCubadoGestaoDocumentos = PropertyEntity({ text: "Exibir Peso Carga e Peso Cubado na Gestão de Documentos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoApagarCamposDatasAoDuplicarPedido = PropertyEntity({ text: "Não apagar campo datas ao duplicar o pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ConcatenarNumeroPreCargaNoPedido = PropertyEntity({ text: "Concatenar número pré-carga no pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirInformarAcrescimoDescontoNoPedido = PropertyEntity({ text: "Permitir informar acréscimo/desconto no pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirMudarStatusPedidoParaCanceladoAposVinculoCarga = PropertyEntity({ text: "Permitir mudar status do pedido para cancelado após vínculo com a carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoPermitirImportarPedidosExistentes = PropertyEntity({ text: "Não permitir importar pedidos existentes", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarDadosTransporteCargaCancelada = PropertyEntity({ text: "Utilizar dados transporte carga cancelada", getType: typesKnockout.bool, val: ko.observable(false) });
    this.VisualizarUltimoUsuarioDelegadoOcorrencia = PropertyEntity({ text: "Visualizar último usuário delegado da ocorrência", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteInformarCentroResultadoAprovacaoOcorrencia = PropertyEntity({ text: "Permite informar Centro de Resultado na aprovação da ocorrência", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarNumeroOutroDocumento = PropertyEntity({ text: "Utilizar Número Outro Documento nas notas da carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirConsultaDeValoresPedagio = PropertyEntity({ text: "Permitir consulta de valores para vale pedágio", getType: typesKnockout.bool, val: ko.observable(false) });
    this.BloquearFinalizacaoComFluxoCompraAberto = PropertyEntity({ text: "Bloquear finalização do documento com a Ordem de Compra em Fluxo de Compra aberto", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirSelecionarOSFinalizadaDocumentoEntrada = PropertyEntity({ text: "Permitir selecionar O.S. finalizada no Documento de Entrada", getType: typesKnockout.bool, val: ko.observable(false) });
    this.BloquearCadastroProdutoComMesmoCodigo = PropertyEntity({ text: "Bloquear cadastro de produto com o mesmo código", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirEnviarEmailAutorizacaoEmbarque = PropertyEntity({ text: "Permitir enviar e-mail autorização de embarque", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteGerarOcorrenciaCargaAnulada = PropertyEntity({ text: "Permitir gerar ocorrência de cargas anuladas", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExigirAnexosNoCadastroDoTransportador = PropertyEntity({ text: "Exigir anexos no cadastro do transportador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarRaioCliente = PropertyEntity({ text: "Validar raio do cliente ao solicitar atendimento (App GPA)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.RetornarMultiplasCargasApp = PropertyEntity({ text: "Retornar múltiplas cargas para o app", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizaAppTrizy = PropertyEntity({ text: "Utiliza App Trizy", getType: typesKnockout.bool, val: ko.observable(false) });
    this.RegistrarChegadaAppEmMetodoDiferenteDoConfirmar = PropertyEntity({ text: "Registrar chegada do App em método separado do Confirmar", getType: typesKnockout.bool, val: ko.observable(false) });
    this.MenuCarga = PropertyEntity({ text: "Cargas", getType: typesKnockout.bool, val: ko.observable(false) });
    this.MenuServicos = PropertyEntity({ text: "Serviços", getType: typesKnockout.bool, val: ko.observable(false) });
    this.MenuOcorrencias = PropertyEntity({ text: "Ocorrências", getType: typesKnockout.bool, val: ko.observable(false) });
    this.MenuExtratoViagem = PropertyEntity({ text: "Extrato de viagem", getType: typesKnockout.bool, val: ko.observable(false) });
    this.MenuPontosParada = PropertyEntity({ text: "Pontos de Parada", getType: typesKnockout.bool, val: ko.observable(false) });
    this.MenuServicosViagem = PropertyEntity({ text: "Serviços de Viagem", getType: typesKnockout.bool, val: ko.observable(false) });
    this.MenuRH = PropertyEntity({ text: "RH", getType: typesKnockout.bool, val: ko.observable(false) });
    this.VisualizarVeiculosPropriosETerceiros = PropertyEntity({ text: "Visualizar veículos próprios e terceiros na movimentação de placas", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AceitarPedidosComPendenciasDeProdutos = PropertyEntity({ text: "Aceitar pedidos com pendências de produtos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoPermitirNFSComMultiplosCentrosResultado = PropertyEntity({ text: "Não permitir geração de NFS Manual com múltiplos centros de resultado", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirDestinatarioOcorrencia = PropertyEntity({ text: "Exibir Destinatário na Ocorrência", getType: typesKnockout.bool, val: ko.observable(false) });
    this.EnviarXMLDANFEClienteOcorrenciaPedido = PropertyEntity({ text: "Enviar XML e DANFE para cliente da ocorrência de faturamento do pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ObrigatorioSegmentoVeiculo = PropertyEntity({ text: "Obrigatório Segmento do Veículo", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirBuscarValoresTabelaFrete = PropertyEntity({ text: "Permitir buscar valores da tabela da frete no Pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoRetornarDocumentosAnteriores = PropertyEntity({ text: "Não retornar documentos anteriores via Web Service", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirAlterarInformacoesAgrupamentoCarga = PropertyEntity({ text: "Permitir alterar informações no agrupamento de carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.BloquearEmissaoCargaTerceirosSemValePedagio = PropertyEntity({ text: "Bloquear emissão de carga de terceiros sem vale pedágio", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AtivarEnvioDocumentacaoFinalizacaoCarga = PropertyEntity({ text: "Ativar o envio da documentação ao finalizar a carga?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarDuplicidadeTituloSemData = PropertyEntity({ text: "Validar lançamento duplicado de título manual sem data de vencimento (Pessoa, Tipo, Sequência, Número e Tipo do Documento)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.VisualizarApenasVeiculosAtivos = PropertyEntity({ text: "Visualizar apenas veículos ativos nas consultas", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ObrigatorioOperadorResponsavelCancelamentoCarga = PropertyEntity({ text: "Obrigatório Operador Responsável no Cancelamento de Carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AtivarControleDespesas = PropertyEntity({ text: "Ativar o Controle de Despesas com Tipo de Despesas e Centro de Resultado", getType: typesKnockout.bool, val: ko.observable(false) });
    this.CalcularPautaFiscal = PropertyEntity({ text: "Calcular Pauta ICMS", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoPermitirRemoverUltimoPedidoCarga = PropertyEntity({ text: "Não permitir remover último pedido da carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.BloquearCamposMotoristaLGPD = PropertyEntity({ text: "Bloquear campos do motorista referente ao LGPD", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteDownloadCompactadoArquivoOcorrencia = PropertyEntity({ text: "Permite o download compactado dos arquivos das ocorrências", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoImprimirTipoOcorrenciaNaObservacaoCTeComplementar = PropertyEntity({ text: "Não imprimir tipo ocorrência na observação do CT-e complementar", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirCampoInformativoPagadorAutorizacaoOcorrencia = PropertyEntity({ text: "Exibir campo informativo pagador autorização ocorrência", getType: typesKnockout.bool, val: ko.observable(false) });
    this.SalvarDocumentosDoCteAnteriorAoImportarCTeComplementar = PropertyEntity({ text: "Salvar documentos do CTe anterior ao Importar CTe Complementar", getType: typesKnockout.bool, val: ko.observable(false) });
    this.InduzirTransportadorSelecionarApenasUmComplementoSolicitacaoComplementos = PropertyEntity({ text: "Induzir o transportador a selecionar apenas um complemento na solicitação de complementos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarBonificacaoParaTransportadoresViaOcorrencia = PropertyEntity({ text: "Utilizar Bonificação para transportadores via ocorrência", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirDefinirCSTnoTipoDeOcorrencia = PropertyEntity({ text: "Permitir definir CST no tipo de ocorrência", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteAdicionarMaisOcorrenciaMesmoEvento = PropertyEntity({ text: "Permite adicionar mais de uma ocorrência para o mesmo evento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.TrazerCentroResultadoOcorrencia = PropertyEntity({ text: "Trazer Centro de Resultado nas Ocorrências", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizaUsuarioPadraoParaGeracaoOcorrenciaPorEDI = PropertyEntity({ text: "Utiliza usuário padrão para geração de ocorrências por EDI", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirVinculoAutomaticoEntreOcorreciaEAtendimento = PropertyEntity({ text: "Permitir vinculo automático entre ocorrência e atendimento para mesma NF", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarNumeroTentativasTempoIntervaloIntegracaoOcorrenciaPersonalizado = PropertyEntity({ text: "Utilizar Número de tentativas e Tempo de ntervalo de integração da ocorrência personalizado", getType: typesKnockout.bool, val: ko.observable(false) });
    this.IgnorarSituacaoDasNotasAoGerarOcorrencia = PropertyEntity({ text: "Ignorar situação das Notas ao gerar ocorrencia", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirIncluirOcorrenciaPorSelecaoNotasFiscaisCTe = PropertyEntity({ text: "Permitir incluir ocorrência por seleção de notas fiscais do CT-e", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirTodosCTesDaCargaNaAutorizacaoDeOcorrencia = PropertyEntity({ text: "Exibir todos CT-es da Carga na Autorização de Ocorrência", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirCamposSuspensaoMotorista = PropertyEntity({ text: "Exibir campos de suspensão no cadastro do motorista", getType: typesKnockout.bool, val: ko.observable(false) });
    this.BloquearChecklistMotoristaSemLicencaVinculada = PropertyEntity({ text: "Bloquear checklist de motoristas sem licença vinculada", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoPermitirInativarMotoristaComSaldoNoExtrato = PropertyEntity({ text: "Não permitir inativar motorista com saldo no extrato", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirCadastrarMotoristaEstrangeiro = PropertyEntity({ text: "Permitir cadastrar motorista estrangeiro", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoValidarHoraNoPagamentoMotorista = PropertyEntity({ text: "Não validar hora no pagamento ao motorista", getType: typesKnockout.bool, val: ko.observable(false) });
    this.MotoristaUsarFotoDoApp = PropertyEntity({ text: "Definir a foto do motorista como a mesma do app", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirConfiguracoesPortalTransportador = PropertyEntity({ text: "Exibir configurações no portal do transportador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoPermitirRealizarCadastroMotoristaBloqueado = PropertyEntity({ text: "Não permite realizar novos cadastros de motorista quando existir um cadastro bloqueado", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteDuplicarCadastroMotorista = PropertyEntity({ text: "Permite duplicar o cadastro do motorista", getType: typesKnockout.bool, val: ko.observable(false) });
    this.VisualizarLegendaCargaAcordoTipoOperacao = PropertyEntity({ text: "Visualizar legenda da carga de acordo com o tipo de operação", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoEncerrarMDFeDeFormaAutomaticaAoConfirmarDadosDeTransporte = PropertyEntity({ text: "Não encerrar MDF-e de forma automática ao confirmar Dados de Transporte (Placa de Veículo) em outra carga.", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirLiberarCargaParaTransportadoresTerceiros = PropertyEntity({ text: "Permitir liberar carga para transportadores terceiros", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarJanelaDeCarregamento = PropertyEntity({ text: "Gerar janela de carregamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarCentroDescarregamentoPorTipoCarga = PropertyEntity({ text: "Utilizar Centro de Descarregamento por Tipo de Carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirDetalhesAgendamentoJanelaTransportador = PropertyEntity({ text: "Exibir Detalhes Agendamento na Janela do Transportador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarPeriodoDescarregamentoExclusivo = PropertyEntity({ text: "Utilizar período de descarregamento exclusivo", getType: typesKnockout.bool, val: ko.observable(false) });
    this.DisponibilizarCargaParaTransportadoresPorPrioridade = PropertyEntity({ text: "Disponibilizar carga para transportadores por prioridade", getType: typesKnockout.bool, val: ko.observable(false) });
    this.BloquearAberturaChamadoRetencaoQuandoPossuirReentrega = PropertyEntity({ text: "Bloquear abertura de Chamado do tipo Retenção quando possuir do tipo Reentrega para a entrega", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarNovoLayoutPagamentoAgregado = PropertyEntity({ text: "Utilizar novo layout para o pagamento de agregado", getType: typesKnockout.bool, val: ko.observable(false) });
    this.BuscarEmpresaPeloProprietarioDoVeiculo = PropertyEntity({ text: "Buscar empresa pelo proprietario do veiculo na importação de pedidos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoComprarValePedagio = PropertyEntity({ text: "Não comprar vale pedágio", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoPermitirAcessarDocumentosAntesCargaEmTransporte = PropertyEntity({ text: "Não permitir acessar os documentos antes da carga estar em transporte", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ObrigatorioInformarAnoFabricacao = PropertyEntity({ text: "Obrigatório informar Ano de Fabricação do veículo", getType: typesKnockout.bool, val: ko.observable(true) });
    this.ObrigatorioInformarReboqueParaVeiculosDoTipoRodadoCavalo = PropertyEntity({ text: "Obrigatório informar veículo vinculado reboque para veículo com tipo de rodado tração", getType: typesKnockout.bool, val: ko.observable(true) });
    this.ManterVinculoMotoristaEmFolga = PropertyEntity({ text: "Manter o vínculo do motorista em folga?", getType: typesKnockout.bool, val: ko.observable(true) });
    this.NaoPermitirAlterarKMVeiculoEquipamentoPneuPelaOrdemServico = PropertyEntity({ text: "Não permitir alterar o KM do Veículo/Equipamento/Pneu pela Ordem de Serviço", getType: typesKnockout.bool, val: ko.observable(false) });
    this.BloquearAlteracaoCentroResultadoNaMovimentacaoPlaca = PropertyEntity({ text: "Não alterar centro de resultado automaticamente na movimentação de placa", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoPermitirQueTransportadorInativeVeiculo = PropertyEntity({ text: "Não permitir que o transportador inative o veículo", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirRemoverCargasAgrupamentoCarga = PropertyEntity({ text: "Permitir remover cargas do agrupamento de carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AssumirSempreTipoOperacaoDoPedido = PropertyEntity({ text: "Portal multiclifor assumir sempre tipo de operação do pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizaControleDeEntregaManual = PropertyEntity({ text: "Utiliza o Controle de Entrega de Forma Manual", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExigirDefinicaoTipoCarregamentoPedido = PropertyEntity({ text: "Exigir definição do tipo de carregamento do pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AtualizarInformacoesPedidosPorCarga = PropertyEntity({ text: "Atualizar as informações dos pedidos por carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ApresentaOpcaoRemoverCancelarPedidos = PropertyEntity({ text: "Apresenta opção Remover e Cancelar pedidos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ApresentaOpcaoCancelarReserva = PropertyEntity({ text: "Apresenta opção Cancelar Reserva", getType: typesKnockout.bool, val: ko.observable(false) });
    this.FiltroPeriodoVazioAoIniciar = PropertyEntity({ text: "Filtro de período vazio ao iniciar", getType: typesKnockout.bool, val: ko.observable(false) });
    this.DataAtualNovoCarregamento = PropertyEntity({ text: "Utilizar data atual em novo carregamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.OcultarBipagem = PropertyEntity({ text: "Ocultar opções/informações Bipagem", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarDataPrevisaoSaidaVeiculo = PropertyEntity({ text: "Utilizar a data de previsão de saída do veículo", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirCriarPedidoApenasMotoristaSituacaoTrabalhando = PropertyEntity({ text: "Permitir criar pedido apenas com motorista na situação Trabalhando", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ControlarValoresComponentesCTe = PropertyEntity({ text: "Controlar Valores Componentes CT-e", getType: typesKnockout.bool, val: ko.observable(false) });
    this.BloquearLancamentoDocumentosTipoEntrada = PropertyEntity({ text: "Bloquear lançamento de documentos do tipo entrada", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.SalvarDocumentosRecebidosEmailDestinados = PropertyEntity({ text: "Salvar Documentos recebidos por e-mail em Documentos Destinados", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.VincularCteNaOcorrenciaApartirDaObservacao = PropertyEntity({ text: "Vincular Cte na Ocorrencia a partir da observação", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoSalvarXmlApenasNaFalha = PropertyEntity({ text: "Não salvar xml apenas na falha", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.ExibirPedidosImpressaoContratoFrete = PropertyEntity({ text: "Exibir pedidos na impressão do contrato de frete", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoRetornarImagemCanhoto = PropertyEntity({ text: "Não retornar a imagem do canhoto", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RetornarEntregasRejeitadas = PropertyEntity({ text: "Retornar Reentregas e Entregas Rejeitadas nos métodos de integração de entregas pendentes", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.SelecionarRotaFreteAoAdicionarPedido = PropertyEntity({ text: "Selecionar a rota frete ao adicionar o pedido", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoValidarLicencaVeiculoParaCargaRedespacho = PropertyEntity({ text: "Não validar as licenças de veículo para cargas de redespacho", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.UtilizarPesoProdutoParaCalcularPesoCarga = PropertyEntity({ text: "Utilizar peso do produto para calcular o peso da carga", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.UtilizarControlePaletesModeloVeicular = PropertyEntity({ text: "Utilizar controle de paletes por modelo veicular", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoConsiderarDescontoCalculoImpostosContratoFreteTerceiro = PropertyEntity({ text: "Não considerar descontos para o cálculo de impostos", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ObrigatorioInformarFreetimeCadastroRotas = PropertyEntity({ text: "Obrigatório informar Freetime no cadastro de rotas (Coleta/Entrega/Fronteiras)", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoCarregarPlanoEntradaSaidaTipoPagamento = PropertyEntity({ text: "Não carregar plano de entrada e plano de saída ao selecionar um Tipo de Pagamento", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AtivarConsultaSegregacaoPorEmpresa = PropertyEntity({ text: "Ativar a consulta segregada por empresa", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NotificarCanhotosPendentesTodosOsDias = PropertyEntity({ text: "Notificar canhotos pendentes todos os dias", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.DisponibilizarOpcaoDeCanhotoExtraviado = PropertyEntity({ text: "Disponibilizar Opcao De Canhoto Extraviado", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.MovimentacaoFinanceiraParaTitulosDeProvisao = PropertyEntity({ text: "Gerar movimentação financeira para títulos de provisão", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoPermitirReceberCanhotosNaoDigitalizados = PropertyEntity({ text: "Não permitir receber fisicamente canhotos sem imagem digitalizada ou justificada ", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NotificarTransportadorCanhotosQueEstaoComDigitalizacaoRejeitada = PropertyEntity({ text: "Notificar ao Transportador Canhotos pendentes de digitalização e Rec. Físico", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoPermitirEmissaoComMesmaOrigemEDestino = PropertyEntity({ text: "Bloquear emissões com a mesma localidade de origem e destino", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.GerarCargaTerceiroApenasProvedorPedido = PropertyEntity({ text: "Gerar carga de terceiro apenas com o Provedor do Pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ControlarEstoqueReserva = PropertyEntity({ text: "Controlar reserva de estoque no fechamento da OS", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RealizarValidacaoComEstoqueDePosicaoAoFecharOrdemDeServico = PropertyEntity({ text: "Realizar validação com o estoque de posição ao fechar a ordem de serviço", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.SalvarProdutosDaNotaFiscal = PropertyEntity({ text: "Salvar Produtos Da NotaFiscal (NF-e)", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ExibirOpcaoAjustarEntregaOnTime = PropertyEntity({ text: "Exibir opção ajustar entrega OnTime", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ExigirProtocoloLiberacaoSemIntegracaoGR = PropertyEntity({ text: "Exigir Protocolo e Anexo para Liberação Sem Integração GR", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ExigirQueApolicePropriaTransportadorEstejaValida = PropertyEntity({ text: "Exigir que se o transportador possuir apólice própria a mesma esteja valida ao informar os dados do transporte", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ExibirCamposRecebimentoPedidoIntegracao = PropertyEntity({ text: "Exibir campos do recebimento de pedidos via integração", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.HabilitarLayoutFaturaPagamentoAgregado = PropertyEntity({ text: "Habilitar layout de fatura no pagamento de agregado", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirCadastrarTransportadorInformacoesMinimas = PropertyEntity({ text: "Permitir cadastrar transportador com informações mínimas", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.EnviarAlertasMonitoramentoEmail = PropertyEntity({ text: "Enviar alertas de problemas monitoramento por email", getType: typesKnockout.bool, val: ko.observable(false) });
    this.EmailsAlertaMonitoramento = PropertyEntity({ text: "E-mail(s) para envio de alerta de monitoramento:", val: ko.observable(""), maxlength: 1000, getType: typesKnockout.multiplesEmails });
    this.NaoPermitirAlterarCentroResultadoPedido = PropertyEntity({ text: "Não permitir alterar Centro de Resultado no Pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoPermitirInformarExpedidorNoPedido = PropertyEntity({ text: "Não permitir informar Expedidor no Pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AlertarTransportadorCancelamentoCarga = PropertyEntity({ text: "Alertar transportador ao cancelar a carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.OrdenarLocalidades = PropertyEntity({ text: "Ordenar localidades", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ColetasSempreInicioRotaOrdenadaCliente = PropertyEntity({ text: "Coletas sempre no início mesmo em rotas ordenadas pelo cliente", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoEnviarEmailDocumentoEmitidoProprietarioVeiculo = PropertyEntity({ text: "Não enviar e-mail de emissão de documentos para o proprietário do veículo", getType: typesKnockout.bool, val: ko.observable(false) });
    this.TrocarFilialQuandoExpedidorForUmaFilial = PropertyEntity({ text: "Trocar a filial quando o expedidor for uma filial", getType: typesKnockout.bool, val: ko.observable(false) });
    this.DisponibilizarCargaParaTransportadoresPorModeloVeicularCarga = PropertyEntity({ text: "Disponibilizar carga para transportadores por modelo veicular de carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.BloquearInsercaoNotaComEmitenteDiferenteRemetentePedido = PropertyEntity({ text: "Bloquear inserção da nota com Emitente diferente do Remetente do pedido na carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirPacotesOcorrenciaControleEntrega = PropertyEntity({ text: "Exibir pacotes nas ocorrências do controle de entrega", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirReordenarEntregasAoAddPedido = PropertyEntity({ text: "Permitir reordenar entregas no controle de entregas", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AtualizarDataInicialColetaAoAlterarHorarioCarregamento = PropertyEntity({ text: "Atualizar a data inicial de coleta ao alterar o horário de carregamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.BloquearGeracaoJanelaParaCargaRedespacho = PropertyEntity({ text: "Bloquear geração de janela para carga de redespacho", getType: typesKnockout.bool, val: ko.observable(false) });
    this.BloquearDuplicarPedido = PropertyEntity({ text: "Bloquear a opção duplicar pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.BloquearPedidoAoIntegrar = PropertyEntity({ text: "Bloquear automaticamente pedido ao integrar", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarEnderecoExpedidorRecebedorPedido = PropertyEntity({ text: "Utilizar o endereço do Expedidor/Recebedor no Pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ManterTransportadorUnicoEmCargasAgrupadas = PropertyEntity({ text: "Manter transportador único em cargas agrupadas", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoValidarMesmaViagemEMesmoContainer = PropertyEntity({ text: "Não validar mesma viagem e mesmo container", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AtivarControleCarregamentoNavio = PropertyEntity({ text: "Ativar controle de carregamento do Navio", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExigirRetencaoISSQuandoMunicipioPrestacaoForDiferenteTransportador = PropertyEntity({ text: "Exigir Retenção do ISS quando o munícipio da Prestação do Serviço for diferente do Transportador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirTransportadorRetornarEtapaNFe = PropertyEntity({ text: "Permitir retornar etapa NF-e", getType: typesKnockout.bool, val: ko.observable(false) });
    this.EnviarEmailDocumentoRejeitadoAuditoriaFrete = PropertyEntity({ text: "Enviar e-mail de documento rejeitado na auditoria de frete", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoAtualizarNomeFantasiaClienteAlterarDadosTransportador = PropertyEntity({ text: "Não alterar nome fantasia do cliente ao atualizar dados do transportador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoGerarAutomaticamenteUsuarioAcessoPortalTransportador = PropertyEntity({ text: "Não gerar automaticamente usuário de acesso ao portal do transportador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AtivarValidacaoCriacaoCarga = PropertyEntity({ text: "Ativar validação na criação de carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirInformarEmpresaFavorecidaNosDadosBancarios = PropertyEntity({ text: "Permitir informar empresa favorecida nos dados bancários da empresa", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ExisteTransportadorPadraoContratacao = PropertyEntity({ text: "Existe Transportadora Padrão Contratação", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.UtilizarProgramacaoCarga = PropertyEntity({ text: "Utilizar programação de carga", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirVincularVeiculoMotoristaViaPlanilha = PropertyEntity({ text: "Permitir vincular dados de transporte via planilha", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirCriacaoDiretaMalotes = PropertyEntity({ text: "Permitir criação direta de malotes", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirAgrupamentoDeCargasOrdenavel = PropertyEntity({ text: "Permitir agrupamento de cargas Ordenável", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirGerarRegistroDeDesembarqueNoCIOT = PropertyEntity({ text: "Permitir gerar registro de desembarque na eFrete", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirAuditarCanhotosFinalizados = PropertyEntity({ text: "Permitir auditar os registros de canhotos já finalizados", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ObrigatorioInformarNotaNaDevolucaoParcialChamado = PropertyEntity({ text: "Obrigatório informar a nota na devolução parcial", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.HabilitarArvoreDecisaoEscalationList = PropertyEntity({ text: "Habilitar Árvore de Decisão e Escalation List", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoPreencherRotaFreteAutomaticamente = PropertyEntity({ text: "Não preencher a rota do frete automaticamente", getType: typesKnockout.bool, val: ko.observable(false) });
    this.LiquidarPalletAutomaticamente = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) })
    this.HabilitarAlertaMotorista = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) })
    this.UtilizarValorDesproporcionalRateioDespesaVeiculo = PropertyEntity({ text: "Utilizar valor desproporcional no rateio de despesas do veículo", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AtualizarVinculoVeiculoMotoristaIntegracao = PropertyEntity({ text: "Atualizar vínculo entre veículo e motorista via integração", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoPermitirGerarRedespachoDeCargasDeRedespacho = PropertyEntity({ text: "Não permitir gerar Redespacho de Cargas de Redespacho", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoRoteirizarRotaNovamente = PropertyEntity({ text: "Não roteirizar a rota novamente (rotas já cadastradas no sistema)", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoPermitirPedidosTomadoresDiferentesMesmoCarregamento = PropertyEntity({ text: "Não permitir adicionar pedidos com tomadores diferentes no mesmo carregamento", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.GerarUnicoBlocoPorRecebedor = PropertyEntity({ text: "Gerar único bloco por recebedor", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ConsiderarSomentePesoOuCubagemAoGerarBloco = PropertyEntity({ text: "Considerar somente peso ou cubagem ao gerar bloco", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirGerarCarregamentoPedidoBloqueado = PropertyEntity({ text: "Permitir gerar carregamento pedido bloqueado", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AtivarTratativaDuplicidadeEmissaoCargasFeeder = PropertyEntity({ text: "Ativar tratativa de duplicidade de emissão para cargas Feeder", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoExibirPedidosDoDiaAgendamentoPedidos = PropertyEntity({ text: "Não exibir pedidos do dia no agendamento de pedidos", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirTransportadorCadastrarAgendamentoColeta = PropertyEntity({ text: "Permitir transportador cadastrar agendamento coleta ", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.SempreUtilizarTomadorEnviadoNoPedido = PropertyEntity({ text: "Sempre utilizar o tomador enviado no pedido", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TransformarJanelaDeDescarregamentoEmMultiplaSelecao = PropertyEntity({ text: "Transformar Janela de Descarregamento em Múltipla Seleção", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.HabilitarFuncionalidadeProjetoNFTP = PropertyEntity({ text: "Habilitar funcionalidades do projeto [NFTP] - SAP Hanna", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ImportarOcorrenciasDePedidosPorPlanilhas = PropertyEntity({ text: "Importar Ocorrências de Pedidos por planilha", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoSelecionarModeloVeicularAutomaticamente = PropertyEntity({ text: "Não selecionar o modelo veicular automaticamente no pedido", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ControlarOrganizacaoProdutos = PropertyEntity({ text: "Controlar organização dos produtos", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirProvisionamentoDeNotasCTesNaTelaProvisao = PropertyEntity({ text: "Permitir provisionamento de notas e CT-es na tela de Provisão", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.GerarMovimentoPelaDataVencimentoContratoFinanceiro = PropertyEntity({ text: "Gerar movimento pela data de vencimento no Contrato Financeiro", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AutomatizarGeracaoLoteProvisao = PropertyEntity({ text: "Automatizar Geração Lote de Provisao", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.GerarLotesAposEmissaoDaCarga = PropertyEntity({ text: "Gerar lote de pagamento para os documentos logo após a emissão da carga", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.GerarLotePagamentoAposDigitalizacaoDoCanhoto = PropertyEntity({ text: "Gerar lote de pagamento após a digitalização do canhoto", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.GerarLotesProvisaoAposEmissaoDaCarga = PropertyEntity({ text: "Gerar lote de provisão para os documentos logo após a emissão da carga", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoGerarLoteProvisaoParaCargaAguardandoImportarCTeOuLancarNFS = PropertyEntity({ text: "Não gerar lote de provisão para carga aguardando importação de CT-e ou lançamento de NFS", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoGerarLoteProvisaoParaOcorrencia = PropertyEntity({ text: "Não gerar lote de provisão para Ocorrência", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.DesbloquearPagamentoPorCanhoto = PropertyEntity({ text: "Desbloquear Pagamento por Canhoto", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.UtilizarCodigosDeCadastroComoEnredecoSecundario = PropertyEntity({ text: "Utilizar códigos de cadastro cliente como endereço secundário", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TornarCampoINSSeReterImpostoTrazerComoSim = PropertyEntity({ text: "Tornar o campo INSS (Obrigatório) e Reter Imposto trazer como (SIM)", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.FiltrarPorCodigoDeIntegracaoNaPesquisaPorNomePessoa = PropertyEntity({ text: "Filtrar por Código de Integração na Pesquisa Por Nome na tela de Pessoa", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.CadastroAutomaticoPessoaExterior = PropertyEntity({ text: "Realizar o cadastro automático de pessoa do exterior", getType: typesKnockout.bool, visible: true, val: ko.observable(false), def: false });
    //this.NaoPermiteEnviarXMLPorEmailQuandoTipoServicoForSubcontracao = PropertyEntity({ text: "Não permite enviar xml por e-mail quando o tipo de serviço for de Subcontratação", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirCadastroDeTelefoneInternacional = PropertyEntity({ text: "Permitir cadastro de telefones internacionais", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ExigeQueSuasEntregasSejamAgendadas = PropertyEntity({ text: "Exige que suas entregas sejam agendadas", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AtivarValidacaoDosProdutosNoAdicionarCarga = PropertyEntity({ text: "Ativar validações dos produtos no adicionar carga e salvar produto", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoSubstituirEmpresaNaGeracaoCarga = PropertyEntity({ text: "Não substituir a empresa informada no pedido na geração da carga", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoCalcularTempoDeViagemAutomatico = PropertyEntity({ text: "Não calcular tempo de viagem automático", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ValidarCadastroContainerPelaFormulaGlobal = PropertyEntity({ text: "Validar cadastro de container pela formula global", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ValidarSituacaoDigitalizacaoCanhotosAoSumarizarDocumentoFaturamento = PropertyEntity({ text: "Validar apenas a situação digitalização dos canhotos ao sumarizar o documento para faturamento", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.MensagemRodapeEmailCanhotosPendentes = PropertyEntity({ text: "Mensagem rodapé e-mail canhotos pendentes", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.GerarCanhotoParaNotasTipoPallet = PropertyEntity({ text: "Gerar canhoto para notas do tipo pallet", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.LiberarParaPagamentoAposDigitalizacaCanhoto = PropertyEntity({ text: "Disponibilizar arquivos após digitalização dos canhotos e liberar pagamento apos confirmação da digitalização", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirBloquearDocumentoManualmente = PropertyEntity({ text: "Permitir bloquear documento manualmente", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirMultiplaSelecaoLancamentoLotePagamento = PropertyEntity({ text: "Permitir múltipla seleção no lançamento do lote de pagamento", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoPermitirUtilizarVeiculoEmManutencao = PropertyEntity({ text: "Não permitir utilizar veículo em manutenção em Pedidos/Montagem/Cargas", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AverbarMDFeSomenteEmCargasComCIOT = PropertyEntity({ text: "Averbar MDF-e somente em cargas que possuirem CIOT", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirCancelarDocumentosCargaPeloCancelamentoCarga = PropertyEntity({ text: "Permitir cancelar os documentos da Carga pelo Cancelamento de Carga", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ExigirAliquotaNoMunicipioDePrestacaoParaCalculoDeFreteEmFretesMunicipais = PropertyEntity({ text: "Exigir alíquota no município de prestação para Cálculo de Frete em Fretes Municipais", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ObrigarAnexosContratoTransportadorFrete = PropertyEntity({ text: "Obrigar anexos no contrato transportador frete", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirUsarDescricaoFaixaTemperatura = PropertyEntity({ text: "Permite usar descrição para informar faixa de temperatura", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RetornarApenasCarregamentosPendentesComTransportadora = PropertyEntity({ text: "Retornar Apenas Carregamentos Pendentes com Transportadora", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoSobrePorInformacoesViaIntegracao = PropertyEntity({ text: "Não sobrepor informações dos clientes via integração", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.BloquearInclusaoCargaComMesmoNumeroPedidoEmbarcador = PropertyEntity({ text: "Bloquear inclusão de Carga com o mesmo número de Pedido do Embarcador", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RetornarCarregamentosSomenteCargasEmAgNF = PropertyEntity({ text: "Retornar Carregamentos somente de Cargas que estão aguardando Nota Fiscal", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.UtilizarDataVencimentoTituloMovimentoContrato = PropertyEntity({ text: "Utilizar data de vencimento do título para movimento de acréscimo/desconto do contrato", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.NaoIncluirICMSBaseCalculoPisCofins = PropertyEntity({ text: "Não utilizar o valor do ICMS na base de calculo do Pis e Cofins", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.NaoObrigarTipoOperacaoFatura = PropertyEntity({ text: "Não obrigar Tipo de Operação na Fatura", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.HabilitarOpcaoGerarFaturasApenasCanhotosAprovados = PropertyEntity({ text: "Habilitar opção de gerar Faturas apenas com Canhotos Aprovados", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.AtivarColunaCSTConsultaDocumentosFatura = PropertyEntity({ text: "Ativar coluna CST de ICMS dos CTes na Consulta de Documentos para Fatura", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.AtivarColunaNumeroContainerConsultaDocumentosFatura = PropertyEntity({ text: "Ativar coluna número do Container na Consulta de Documentos para Fatura", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.ExigirInformarFilialEmissaoFaturas = PropertyEntity({ text: "Exigir Informar Filial para Emissao de Faturas", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.EscalarAutomaticamenteNivelExcederTempo = PropertyEntity({ text: "Escalar automaticamente o nível se exceder o tempo", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.FinalizarEntregaQuandoDevolucaoParcial = PropertyEntity({ text: "Finalizar entrega quando devolução parcial ao Salvar Tratativa", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ObrigatorioInformarNotaFiscalParaAberturaChamado = PropertyEntity({ text: "Obrigatório informar nota fiscal para abertura de Chamado", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.CalcularValorDasDevolucoes = PropertyEntity({ text: "Calcular Valor das Devoluções", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirRegistrarObservacoesSemVisualizacaoTransportadora = PropertyEntity({ text: "Permitir registrar observações sem visualização da transportadora", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoUsarPesoNotasPallet = PropertyEntity({ text: "Não usar Peso de Notas Pallet.", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.HabilitarEnvioDocumentacaoCargaPorEmail = PropertyEntity({ text: "Habilitar envio da documentação da carga por e-mail", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.SelecionarSomenteOperacoesDeRedespachoNaTelaDeRedespacho = PropertyEntity({ text: "Selecionar somente Operações de Resdespacho na Tela de Redespacho", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NotificarNovaCargaAposConfirmacaoDocumentos = PropertyEntity({ text: "Notificar Nova Carga após confirmacão dos documentos (NF-e)", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.GerarNumeroContratoTransportadorFreteSequencial = PropertyEntity({ text: "Gerar o número do contrato transportador frete sequencialmente", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoSubtrairValePedagioDoContrato = PropertyEntity({ text: "Não subtrair o vale pedágio do contrato de frete", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.UtilizarFechamentoDeAgregado = PropertyEntity({ text: "Utilizar fechamento de agregado", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermiteAlterarDadosContratoIndependenteSituacao = PropertyEntity({ text: "Permite alterar dados do contrato independente da situação", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermiteAlterarAgendamentoDaEntregaNoAcompanhamentoDeCargas = PropertyEntity({ text: "Permite alterar Agendamento da Entrega no Acompanhamento de Cargas", getType: typesKnockout.bool, val: ko.observable(false) });
    this.EnviarEmailTransportadorEntregaEmAtraso = PropertyEntity({ text: "Enviar email para Transportador com entrega em atraso", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirAbrirAtendimentoViaControleEntrega = PropertyEntity({ text: "Permitir abrir atendimento via controle de entrega", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ExibirDataEntregaNotaControleEntrega = PropertyEntity({ text: "Exibir Data Entrega Nota no Controle de Entrega", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermiteExibirCargaCancelada = PropertyEntity({ text: "Permite exibir cargas canceladas", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoPermitirConfirmacaoEntregaPortalTransportadorSemDigitalizacaoCanhotos = PropertyEntity({ text: "Não permitir a confirmação de entrega no portal do transportador sem a digitalização dos canhotos", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirAlterarDataAgendamentoEntregaTransportador = PropertyEntity({ text: "Permitir alterar a Data de Agendamento de Entrega do Transportador", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ConsiderarCargaOrigemParaEntregasTransbordadas = PropertyEntity({ text: "Considerar a carga de origem para entrega nas entregas transbordadas", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RejeitarEntregaNotaFiscalAoRejeitarCanhoto = PropertyEntity({ text: "Rejeitar entrega e nota fiscal ao rejeitar o canhoto", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ConsiderarMediaDeVelocidadeDasUltimasCincoPosicoes = PropertyEntity({ text: "Considerar media de velocidade das últimas 5 posições", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.BloquearInicioeFimDeViagemPeloTransportadorEmCargaNaoEmitida = PropertyEntity({ text: "Bloquear Início e Fim de Viagem pelo Transportador em Cargas não emitidas.", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarLeadTimeDaTabelaDeFreteParaCalculoDaPrevisaoDeEntrega = PropertyEntity({ text: "Utilizar LeadTime da Tabela de Frete para calculo da previsao de entrega", getType: typesKnockout.bool, val: ko.observable(false) });
    this.CalcularDataAgendamentoAutomaticamenteDataFaturamento = PropertyEntity({ text: "Calcular Data Agendamento entrega/coleta automaticamente após data faturamento", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirEnvioCanhotosPeloPortalTransportadorControleEntregas = PropertyEntity({ text: "Permitir envio de canhotos pelo portal do transportador no controle de entregas", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirConsultaMassivaDePedidos = PropertyEntity({ text: "Permitir consulta massiva de pedidos", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermiteInformarPedidoDeSubstituicao = PropertyEntity({ text: "Permite informar que é um pedido de substituição", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.IgnorarValidacoesDatasPrevisaoAoEditarPedido = PropertyEntity({ text: "Ignorar validações das datas de previsão ao editar o pedido", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.HerdarNotasImportadasPedido = PropertyEntity({ text: "Herdar notas importadas automaticamente no pedido", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RecalcularPrevisaoAoIniciarViagem = PropertyEntity({ text: "Recalcular previsão ao Iniciar Viagem", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.LancamentoServicoManualNaOSMarcadadoPorDefault = PropertyEntity({ text: "Lançamento de Serviço Manual na OS marcado por default", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoPermitirVincularPneuVeiculoAbastecimentoAberto = PropertyEntity({ text: "Não permitir vincular Pneu ao Veículo se o mesmo conter Abastecimento em aberto", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.CriarVinculoFrotaCargaForaDoPlanejamentoFrota = PropertyEntity({ text: "Criar vinculo Frota com a carga fora do Planejamento de Frota", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoAlterarCentroResultadoVeiculosEmissaoCargas = PropertyEntity({ text: "Não alterar o centro de resultado dos veículos à partir da emissão de cargas", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoPermitirRealizarFechamentoOrdemServicoCustoZerado = PropertyEntity({ text: "Não permitir realizar fechamento da Ordem de Serviço se o custo estiver zerado", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoPermitirCadastrarVeiculoSemRastreador = PropertyEntity({ text: "Não permitir Cadastrar Veículo sem Rastreador", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.InformarCentroResultadoNaEtapaUmDaCarga = PropertyEntity({ text: "Informar centro de resultado na etapa 1 da carga", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.FinalizarCargaAutomaticamenteAposEncerramentoMDFe = PropertyEntity({ text: "Finalizar carga automaticamente após encerramento de MDF-e", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.UtilizaRegrasDeAprovacaoParaCancelamentoDaCarga = PropertyEntity({ text: "Utiliza Regra de Aprovação para Cancelamento da Carga", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ConsiderarApenasUmaVezKMParaPedidosComMesmoDestinoOrigemCarga = PropertyEntity({ text: "Considerar apenas uma vez o KM enviado via integração para os pedidos com mesma Origem e Destino na mesma Carga", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirAlterarEmpresaNoCTeManual = PropertyEntity({ text: "Permitir alterar a empresa no CT-e manual", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ObrigarJustificativaCancelamentoCarga = PropertyEntity({ text: "Obrigar justificativa para cancelamento de carga", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.DesconsiderarSabadosCalculoPrevisao = PropertyEntity({ text: "Desconsiderar Sabados", getType: typesKnockout.bool, val: ko.observable(false) });
    this.DesconsiderarDomingosCalculoPrevisao = PropertyEntity({ text: "Desconsiderar Domingos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.DesconsiderarFeriadosCalculoPrevisao = PropertyEntity({ text: "Desconsiderar Feriados", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ConsiderarJornadaMotorita = PropertyEntity({ text: "Considerar intervalos do Motorista", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirCancelamentoDaCargaSomenteComDocumentosEmitidos = PropertyEntity({ text: "Permitir o Cancelamento da Carga somente quando houver Documentos Emitidos.", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.GerarCargaAoConfirmarIntegracaoCarregamento = PropertyEntity({ text: "Gerar carga ao confirmar integração de carregamento", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RoteirizarAutomaticamenteAposRoteirizadoAoAdicionarRemoverPedido = PropertyEntity({ text: "Roteirizar automaticamente quando roteirizado ao adicionar ou remover pedidos", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ExibirAlertaRestricaoEntregaClienteCardCarregamento = PropertyEntity({ text: "Exibir alerta restrição entrega cliente card carregamento", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AtualizarDataEmissaoParaDataAtualQuandoReemitirCTeRejeitado = PropertyEntity({ text: "Atualizar data de emissão para data atual quando reemitir CT-e Rejeitado", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoRetornarIntegracaoCarregamentoSeSomenteDadosTransporteForemAlterados = PropertyEntity({ text: "Não retornar Integração de Carregamento se somente Dados de Transporte forem alterados", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AtivarMontagemCargaPorNFe = PropertyEntity({ text: "Ativar Montagem de Carga por NF-e ", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RetornarPedidosInseridosManualmenteAoGerarCarga = PropertyEntity({ text: "Retornar pedidos inseridos manualmente ao gerar a carga", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AoCancelarCargaManterPedidosEmAberto = PropertyEntity({ text: "Ao cancelar a Carga manter os Pedidos em Aberto", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AgruparRelatorioOrdemColetaGuaritaPorDestinatario = PropertyEntity({ text: "Agrupar relatório de Ordem de Coleta da Guarita por destinatário", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ValidarTransportadorNaoInformadoNaImportacaoDocumento = PropertyEntity({ text: "Validar transportador não informado na importação de documento", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirAgendamentoPedidosSemCarga = PropertyEntity({ text: "Habilitar agendamento conforme regra do cadastro do cliente", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoPermitirDesabilitarCompraValePedagioVeiculo = PropertyEntity({ text: "Não permitir desabilitar compra de vale pedágio", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirAdicionarAnexosCheckListGestaoPatio = PropertyEntity({ text: "Permitir anexar documentos ao preencher checklist Gestão Pátio", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirBaixarRomaneioNaEtapaFimCarregamento = PropertyEntity({ text: "Permitir baixar romaneio na etapa fim carregamento do fluxo pátio", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ValidarPesoCargaComPesagemVeiculo = PropertyEntity({ text: "Validar peso da carga com pesagem do veículo", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermiteAlocarVeiculoSemConjuntoCarga = PropertyEntity({ text: "Permite alocar veículo sem conjunto a carga", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegistrarPosicaoVeiculoSubareaAoReceberEvento = PropertyEntity({ text: "Registrar posição do veiculo na Subarea ao receber evento (EventosFluxoPatio)", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ImprimeOrdemServiçoCNPJMatriz = PropertyEntity({ text: "Imprime na Ordem de Serviço o CNPJ da Matriz", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.HabilitarEnvioPorSMSDeDocumentos = PropertyEntity({ text: "Habilitar envio por SMS de Documentos (CT-e e MDF-e)", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.MostrarNoAcompanhamentoDePedidosDeslocamentoVazio = PropertyEntity({ text: "Mostar no Acompanhamento de Pedidos Deslocamento Vazio", getType: typesKnockout.bool, val: ko.observable(false), def: false, issue: 69532 });
    this.RealizarIntegracaoDadosCancelamentoCarga = PropertyEntity({ text: "Realizar integrações dos dados de cancelamento da cargas", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirConfirmarDocumentosFaturaApenasComCtesEscriturados = PropertyEntity({ text: "Permitir confirmar documentos na fatura apenas com Ctes escriturados", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.GerarCargaAutomaticamenteNoPedido = PropertyEntity({ text: "Gerar pedidos por padrão sem carga quando lançados manualmente na tela de pedido", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.UtilizarBloqueioPessoasGrupoApenasParaTomadorDoPedido = PropertyEntity({ text: "Utilizar bloqueio de pessoas/grupo de pessoas apenas para o tomador do pedido", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoValidarCondicaoPagamentoFechamentoLotePagamento = PropertyEntity({ text: "Não validar condição de pagamento no fechamento do lote de pagamento", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ValidarDataPrevisaoPagamentoEDataPagamentoNoCancelamentoDosCTes = PropertyEntity({ text: "Validar Data de Previsão de Pagamento e Data de Pagamento no cancelamento dos CT-es", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.BaixaTitulosRenegociacaoGerarNovoTituloPorDocumento = PropertyEntity({ text: "Baixa títulos renegociação gerar novo título por documento", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.GerarLotesPagamentoIndividuaisPorDocumento = PropertyEntity({ text: "Gerar Lotes de Pagamento Individuais por Documento", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.GerarLotePagamentoSomenteParaCTe = PropertyEntity({ text: "Gerar Lote de Pagamento somente para CT-e (Modelo 57)", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.SomarValorISSNoTotalReceberGeracaoLoteProvisao = PropertyEntity({ text: "Somar o valor de ISS no total a receber na geração do lote de provisão", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.UtilizarConfiguracaoTipoOperacaoGeracaoCargaPorPedido = PropertyEntity({ text: "Utilizar configuração do tipo de operação para geração de cargas por pedido", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PadraoVisualizacaoOperadorLogistico = PropertyEntity({ text: "Padrão de visualização Operador Logistico", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoVincularAutomaticamenteDocumentosEmitidosEmbarcador = PropertyEntity({ text: "Não vincular automaticamente os Documentos que foram emitidos pelo MultiEmbarcador na Carga", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirAvancarCargasEmitidasEmbarcadorPorTipoOperacao = PropertyEntity({ text: "Permitir avançar cargas emitidas no embarcador por tipo de operação", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AtualizarDadosDosPedidosComDadosDaCarga = PropertyEntity({ text: "Atualizar dados dos pedidos com dados da carga", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AjustarValorFreteAposAprovacaoPreCTe = PropertyEntity({ text: "Ajustar valor do frete após aprovação do pré-CTe", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AlterarModeloDocumentoNFSManual = PropertyEntity({ text: "Alterar o modelo de documento da NFS Manual", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.EnviarEmailPreviaCustoParaTransportadores = PropertyEntity({ text: "Enviar Email Previa Custo Para Transportadores", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoDescontarValorDescontoItemAosAbastecimentosGeradosDocumentoEntrada = PropertyEntity({ text: "Não descontar o valor de desconto do item aos abastecimentos gerados no documento de entrada", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoPermitirSolicitarCancelamentoCargaViaIntegracaoViagemIniciada = PropertyEntity({ text: "Não permitir solicitar o cancelamento de cargas via integração se a viagem foi iniciada", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.EnviarApenasEmailDiarioTaxasDescargaPendenteAprovacao = PropertyEntity({ text: "Enviar apenas e-mail diário de taxas de descarga pendentes de aprovação", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoGerarAutomaticamenteLotesCancelados = PropertyEntity({ text: "Não gerar automaticamente novos lotes para pagamentos cancelados", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.GerarEstornoProvisaoAutomaticoAposEscrituracao = PropertyEntity({ text: "Gerar estorno provisão automático após escrituração", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.GerarEstornoProvisaoAutomaticoAposLiberacaoPagamento = PropertyEntity({ text: "Gerar estorno provisão automático após liberação do pagamento", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.UtilizarEstornoProvisaoDeFormaAutomatizada = PropertyEntity({ text: "Utilizar estorno de provisão de forma automatizada", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RateioProvisaoPorGrupoProduto = PropertyEntity({ text: "Rateio de provisão por grupo de produto", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.GerarLoteProvisaoIndividualNfe = PropertyEntity({ text: "Gerar lotes de provisão individual por NFE", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.UtilizarFechamentoAutomaticoProvisao = PropertyEntity({ text: "Utilizar fechamento automático provisão", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoPermitirProvisionarSemCalculoFrete = PropertyEntity({ text: "Não permitir provisionar sem calculo de frete", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.HabilitaIntervaloTempoLiberaDocumentoEmitidoEscrituracao = PropertyEntity({ text: "Habilitar intervalo de tempo para liberar doumentos emitidos para escrituração", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.GerarDoumentoProvisaoAoReceberNotaFiscal = PropertyEntity({ text: "Gerar documento provisão ao receber nota fiscal (integração ou manual)", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AtivarAlertaChamadosMais48hAberto = PropertyEntity({ text: "Ativar alertas de chamados com mais de 48h em aberto", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoRetornarNFSeVinculadaNFSManualMetodoBuscarNFSs = PropertyEntity({ text: "Não retornar NFS-e vinculada a NFS Manual no método BuscarNFSs", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermiteReceberDataCriacaoPedidoERP = PropertyEntity({ text: "Permite receber data de criação de pedido do ERP", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoRecalcularFreteAoAdicionarRemoverPedido = PropertyEntity({ text: "Não recalcular frete ao adicionar ou remover pedido", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AtualizarNumeroPedidoVinculado = PropertyEntity({ text: "Atualizar Número Pedido Vinculado", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AtualizarTodosCadastrosMotoristasMesmoCodigoIntegracao = PropertyEntity({ text: "Atualizar todos os cadastros de motoristas com mesmo código de integração (método SalvarMotorista)", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermiteSelecionarMultiplasCargasParaRedespacho = PropertyEntity({ text: "Permite selecionar multiplas cargas para redespacho", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.GerarRedespachoDeCargasAgrupadas = PropertyEntity({ text: "Gerar redespacho de cargas agrupadas", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermiteExcluirAgendamentoDaCargaJanelaDescarga = PropertyEntity({ text: "Permite excluir agendamento da carga na janela de descarga", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirAbrirChamadoParaEntregaJaRealizada = PropertyEntity({ text: "Permitir abrir atendimento para entrega já realizada", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirSelecionarMultiplasCargasParaAgruparNoTransbordo = PropertyEntity({ text: "Permitir selecionar múltiplas cargas no transbordo para ao final realizar o agrupamento delas", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.VisualizarGNRESemValidacaoDocumentos = PropertyEntity({ text: "Visualizar GNRE sem validação de documentos", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoPermitirFinalizarViagemDetalhesFimViagem = PropertyEntity({ text: "Não permitir finalizar viagem no detalhes do fim da viagem no portal do transportador", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermiteFinalizarAtendimentoComOcorrenciaRejeitada = PropertyEntity({ text: "Permite finalizar atendimento com Ocorrência Rejeitada", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.VincularPrimeiroPedidoDoClienteAoAbrirChamado = PropertyEntity({ text: "Vincular primeiro pedido do cliente ao abrir chamado", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirSelecionarCteApenasComNfeVinculadaOcorrencia = PropertyEntity({ text: "Permitir selecionar apenas CT-e cuja NF-e está vinculada ao chamado, na geração de Ocorrência", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirGerarAtendimentoPorPedido = PropertyEntity({ text: "Permitir gerar Atendimento por Pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.FazerGestaoCriticidade = PropertyEntity({ text: "Fazer gestão de criticidade", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirAtualizarChamadoStatus = PropertyEntity({ text: "Permitir atualizar chamado (Aberto, Em tratativa e Sem Regra)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.OcultarTomadorNoAtendimento = PropertyEntity({ text: "Ocultar o Tomador na tela de Atendimento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.BloquearEstornoAtendimentosFinalizadosPortalTransportador = PropertyEntity({ text: "Bloquear estorno de atendimentos finalizados no Portal Transportador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoPermitirEncerrarCIOTEncerrarCarga = PropertyEntity({ text: "Não permitir encerrar CIOT ao encerrar a carga", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.DesabilitarUtilizacaoCreditoOperadores = PropertyEntity({ text: "Desabilitar utilização de crédito dos operadores", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirAdicionarMotoristaCargaMDFeManual = PropertyEntity({ text: "Permitir adicionar motorista na carga à partir do MDF-e manual", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.HabilitarCadastroArmazem = PropertyEntity({ text: "Habilitar cadastro de armazém", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoInutilizarCTEsFiscalmenteApenasGerencialmente = PropertyEntity({ text: "Não Inutilizar CTEs Fiscalmente Apenas Gerencialmente", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoReutilizarNumeracaoAposAnularGerencialmente = PropertyEntity({ text: "Não Reutilizar Numeração após Anular Gerencialmente", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.NaoBloquearEmissaoNFSeManualSemDANFSE = PropertyEntity({ text: "Não bloquear emissão NFSe manual sem DANFSE", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ObrigatorioInformarModeloVeicularCargaNoWebService = PropertyEntity({ text: "Obrigatório informar modelo veicular da Carga no Web Service", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirAlterarImagemCanhotoDigitalizada = PropertyEntity({ text: "Permitir alterar a imagem do canhoto digitalizada", getType: typesKnockout.bool, val: ko.observable(false) });
    this.RejeitarCanhotosNaoValidadosPeloOCR = PropertyEntity({ text: "Rejeitar Canhotos não validados pelo OCR", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirCanhotosSemVinculoComCarga = PropertyEntity({ text: "Exibir canhotos sem vínculo com carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AtualizarCamposPedidoPorPlanilha = PropertyEntity({ text: "Atualizar campos Pedido por planilha", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoPermitirInformarVeiculoDuplicadoPedidoCargaAberta = PropertyEntity({ text: "Não permitir informar veículo duplicado no planejamento de frota se o mesmo estiver em um pedido ou carga em aberto", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ConverterXMLNotaFiscalParaByteArrayAoImportarNaCarga = PropertyEntity({ text: "Converter XML Nota Fiscal para ByteArray ao importar na Carga", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.MostrarTipoDeOperacaoNoPortalMultiEmbarcadorAgendamentoColeta = PropertyEntity({ text: "Mostrar o Tipo de Operação no MultiEmbarcador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarRegistrosReceberGNREParaCTesComCST90 = PropertyEntity({ text: "Gerar Registros para Receber as GNRE para CTes com CST 90", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ProcessarXMLNotasFiscaisAssincrono = PropertyEntity({ text: "Processar XML Notas Fiscais Assincrono (ao receber via WEBService)", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.VisualizarPermitirAlterarDataEntregaNaConfirmacaoCanhoto = PropertyEntity({ text: "Visualizar e permitir alterar data de entrega na confirmação do canhoto", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RemoverVinculoNotaPedidoAbertoAoCancelarCarga = PropertyEntity({ text: "Manter Pedido em Aberto e remover vínculo da nota ao cancelar a Carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarFiliaisHabilitadasTransportarMontagemCargaMapa = PropertyEntity({ text: " Utilizar Filiais habilitadas a transportar na Montagem Carga Mapa", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ExibirHoraCarregamentoEmPedidosDeColetaECodigosIntegracao = PropertyEntity({ text: "Em Detalhes dos Pedidos, exibir hora na Data de Carregamento em Pedidos de Coleta e Códigos de Integração", getType: typesKnockout.bool, val: ko.observable(false) });
    this.HabilitarBIDTransportePedido = PropertyEntity({ text: "Habilitar BID de Transporte no Pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UsarFatorConversaoProdutoEmPedidoPaletizado = PropertyEntity({ text: "Usar Fator de Conversão Produto em Pedido Paletizado", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirSelecionarCentroDeCarregamentoNoPedido = PropertyEntity({ text: "Permitir selecionar centro de carregamento no pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarCampoDeMotivoDePedido = PropertyEntity({ text: "Utilizar campo de motivo de pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AjustarParticipantesPedidoCTeEmitidoEmbarcador = PropertyEntity({ text: "Ajustar participantes do pedido de acordo com o CT-e emitido no embarcador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoLevarNumeroCotacaoParaPedidoGerado = PropertyEntity({ text: "Não levar o número da cotação para o pedido gerado.", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ImportarParalelizando = PropertyEntity({ text: "Importar Paralelizando.", getType: typesKnockout.bool, val: ko.observable(false) });
    this.SempreConsiderarDestinatarioInformadoNoPedido = PropertyEntity({ text: "Sempre considerar destinatário informado no pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteInformarMaisDeUmaOcorrenciaPorNFe = PropertyEntity({ text: "Permite informar mais de uma Ocorrência por NF-e", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.GerarOutrosDocumentosNaImportacaoDeCTeComplementar = PropertyEntity({ text: "Gerar Outros Documentos na Importação de CT-e Complementar", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.HabilitarUsoCentroResultadoComissaoMotorista = PropertyEntity({ text: "Habilitar uso do centro de resultado na comissão do motorista", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarObservacaoSubstitutoSomenteNumeroCTeAnterior = PropertyEntity({ text: "Gerar observação do substituto somente com o número do CT-e anterior", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.UtilizarLocalidadeTomadorNFSManual = PropertyEntity({ text: "Utilizar localidade do tomador na NFS Manual", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoPermitirGerarNFSeComMesmaNumeracao = PropertyEntity({ text: "Não permitir gerar NFSe com mesma numeração", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.SalvarRegiaoNoClienteParaPreencherRegiaoDestinoDosPedidos = PropertyEntity({ text: "Salvar região no cliente para preencher na Região Destino dos pedidos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoGerarOcorrenciaCTeImportadosEmailEmbarcador = PropertyEntity({ text: "Não gerar ocorrências de CT-es importados do embarcador (e-mail)", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoPermitirCancelamentoNFSManualSeHouverIntegracao = PropertyEntity({ text: "Não permitir cancelamento da NFS manual se houver registro de integração", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ValidarValorLimiteApoliceComValorNFe = PropertyEntity({ text: "Validar valor limite da apólice com valor NFe na carga", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.FiltrarPedidosOndeRecebedorTransportadorNoPortalDoTransportador = PropertyEntity({ text: "Filtrar Pedidos onde o Recebedor é o Transportador no Portal do Transportador", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.VencedorSimuladorFreteEmpresaPedido = PropertyEntity({ text: "Vencedor Simulador Frete Transportador Pedido", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoComprarValePedagioViaIntegracaoSeInformadoManualmenteNaCarga = PropertyEntity({ text: "Não comprar vale pedágio via integração se informado manualmente na carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.SempreSeguirConfiguracaoOcorrenciaQuandoAdicionadaPeloMetodoAdicionarOcorrencia = PropertyEntity({ text: "Sempre seguir a configuração da ocorrencia quando adicionada pelo método AdicionarOcorrencia", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.HabilitarFluxoPedidoEcommerce = PropertyEntity({ text: "Habilitar Fluxo Pedido E-commerce", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.GerarNumerodeCargaAlfanumerico = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.FiltrarPedidosPorRemetenteRetiradaProduto = PropertyEntity({ text: "Filtrar pedidos por Remetente na Retirada Produto", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.HabilitarAcessoTodosClientes = PropertyEntity({ text: "Ativar acesso ao portal para todos os clientes", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.DesabilitarIconeNotificacao = PropertyEntity({ text: "Desabilitar Ícone de Notificação", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.DesabilitarFiltrosBI = PropertyEntity({ text: "Desabilitar filtros do BI", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.SenhaPadraoAcessoPortal = PropertyEntity({ text: "Senha padrão de acesso ao portal", getType: typesKnockout.string, val: ko.observable(""), required: ko.observable(false), visible: ko.observable(false) });
    this.CodigoReportMenuBI = PropertyEntity({ text: "Código Report Menu BI:", val: ko.observable(0), def: 0, getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.ExigirDataEntregaNotaClienteCanhotosReceberFisicamente = PropertyEntity({ text: "Exigir Data de Entrega da Nota Fiscal ao Cliente ao receber Fisicamente", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirImportarCanhotoNFFaturada = PropertyEntity({ text: "Permitir importar canhoto de NF faturada", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirAtualizarSituacaoCanhotoPorImportacao = PropertyEntity({ text: "Permitir atualizar Situação da Digitalização do canhoto por Importação", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirAtualizarSituacaoCanhotoAvulsoPorImportacao = PropertyEntity({ text: "Permitir atualizar Situação da Digitalização do canhoto avulso por Importação", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirImportarDocumentosFiltroSemChaveNFe = PropertyEntity({ text: "Permitir importar documentos para filtro sem chave da NF-e", getType: typesKnockout.bool, val: ko.observable(false) });
    this.IntegrarCanhotosComValidadorIAComprovei = PropertyEntity({ text: "Integrar os canhotos com IA Comprovei", getType: typesKnockout.bool, val: ko.observable(false) });
    this.HabilitarFluxoAnaliseCanhotoRejeitadoIA = PropertyEntity({ text: "Ativar fluxo de análise de canhotos rejeitados pela IA", getType: typesKnockout.bool, val: ko.observable(false) });
    this.EfetuarIntegracaoApenasCanhotosDigitalizados = PropertyEntity({ text: "Efetua Integração de Canhotos apenas com o status Digitalizado", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoIntegrarIAComproveiCanhotosDeNotasDevolvidas = PropertyEntity({ text: "Não integrar canhoto de notas devolvidas", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ReenviarUmaVezIntegracaoCasoRetornarFalhaNaValidacaoDoNumeroDoCanhotoEOuFormatoDoCanhoto = PropertyEntity({ text: "Reenviar uma vez integração caso retornar falha na validação do número do canhoto e/ou formato do canhoto", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirEnviarImagemParaMultiplosCanhotos = PropertyEntity({ text: "Permitir enviar imagem para Multiplos Canhotos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoAtualizarTelaCanhotosAposAprovacaoRejeicao = PropertyEntity({ text: "Não atualizar a tela de canhotos após aprovação ou rejeição", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AprovarAutomaticamenteADigitalizacaoDosCanhotosCasoAValidacaoDaIAComproveiSejaCompleta = PropertyEntity({ text: "Aprovar automaticamente a digitalização dos canhotos, caso a validação da IA Comprovei seja completa", getType: typesKnockout.bool, val: ko.observable(false) });
    this.SempreUtilizarRotaParaBuscarPracasPedagio = PropertyEntity({ text: "Sempre utilizar rota para buscar praças de pedágio", getType: typesKnockout.bool, val: ko.observable(false) });
    this.IgnorarOutroEnderecoPedidoComRecebedor = PropertyEntity({ text: "Roteirizar por Recebedor ignorando outro endereço no pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.RetornarDadosRedespachoTransbordoComInformacoesCargaOrigemConsultada = PropertyEntity({ text: "Retornar dados de Redespacho e Transbordo com as informações da carga de origem consultada", getType: typesKnockout.bool, val: ko.observable(false) });
    this.RemoverAutomaticamenteRequisicaoAbastecimentoAbertaPorPeriodo = PropertyEntity({ text: "Remover Automaticamente Requisição Aberta", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarNumeroSerieInformadoTelaQuandoEmitidoModeloDocumentoNaoFiscal = PropertyEntity({ text: "Utilizar número e série informado em tela quando for emitido um modelo de documento não fiscal", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarLocalidadePrestacaoTransportadorConfiguracaoNFSe = PropertyEntity({ text: "Validar localidade de prestão cadastrada na tela de transportador para que seja informado a alíquota automática", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarExistenciaParaInserirNFSe = PropertyEntity({ text: "Validar existência antes de inserir NFS-e Manual", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoExibirCodigoIntegracaoDoDestinatarioResumoCarga = PropertyEntity({ text: "Não exibir código de integração do destinatário no resumo da carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.HabilitarImportacaoOcorrenciaViaNOTFIS = PropertyEntity({ text: "Habilitar importação de ocorrências via NOTFIS", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoPermitirAlterarDadosCargaQuandoTiverIntegracaoIntegrada = PropertyEntity({ text: "Não permitir alterar dados da carga quando tiver integração gerada e integrada", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AtribuirValorMercadoriaCTeNotasFiscaisDocumentosEmitidosEmbarcador = PropertyEntity({ text: "Atribuir o valor da mercadoria do CT-e nas notas fiscais dos documentos emitidos pelo embarcador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirEncaixarPedidosComReentregaSolicitada = PropertyEntity({ text: "Permitir Encaixar Pedidos Com Reentrega Solicitada", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarContratanteOrigemVPIntegracaoPamcard = PropertyEntity({ text: "Validar contratante e Origem do VP na integração Pamcard", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.CadastrarVeiculoAoInformarDadosTransporteCarga = PropertyEntity({ text: "Cadastrar veículo no método InformarDadosTransporteCarga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoRetornarCargasCanceladasMetodoBuscarPendetesNotasFiscais = PropertyEntity({ text: "Não retornar Cargas Canceladas no método BuscarPendetesNotasFiscais", getType: typesKnockout.bool, val: ko.observable(false) });
    this.RetornarApenasOcorrenciasFinalizadasMetodoBuscarOcorrenciasPendentesIntegracao = PropertyEntity({ text: "Retornar apenas ocorrências finalizadas no método BuscarOcorrenciasPendentesIntegracao", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirRemoverDataPrevisaoDataPagamentoMetodoInformarPrevisaoPagamentoCTe = PropertyEntity({ text: "Permitir remover a data de previsão e data de pagamento no método InformarPrevisaoPagamentoCTe", getType: typesKnockout.bool, val: ko.observable(false) });
    this.DesvincularPreenchimentoDasDatasNosMetodosInformarPrevisaoPagamentoCTeConfirmarPagamentoCTe = PropertyEntity({ text: "Desvincular prenchimento das datas nos métodos InformarPrevisaoPagamentoCTe e ConfirmarPagamentoCTe", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarLogMetodosREST = PropertyEntity({ text: "Gerar Log dos métodos REST", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirAlterarNumeroCargaQuandoForCarga = PropertyEntity({ text: "Permitir AlterarNumeroCarga quando for carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AoSalvarDocumentoTransporteValidarSituacaoCarga = PropertyEntity({ text: "Ao salvar dados do transporte validar situação da carga antes da nota fiscal", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoFiltrarSequencialCargaNoMetodoAdicionarCargaPedido = PropertyEntity({ text: "Não filtrar sequencial carga no método adicionar carga/pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoVincularReboqueNaTracaoAoAcionarMetodoGerarCarregamento = PropertyEntity({ text: "Não vincular reboque na tração ao acionar o método GerarCarregamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoPermitirConfirmarEntregaSituacaoCargaEmAndamento = PropertyEntity({ text: "Não permitir confirmar entrega antes do avanço do frete", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoPermitirAvançarEtapaUmCargaComTransportadorSemApoliceVigente = PropertyEntity({ text: "Não permitir avançar etapa 1 da carga, com transportador sem apólice vigente", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ConsiderarDataEmissaoCTECalculoEmbarquePrevisaoEntrega = PropertyEntity({ text: "Considerar data de emissão do CTe para calculo de Embarque e Previsão Entrega", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirImpressaoDAMDFEContingencia = PropertyEntity({ text: "Permitir impressão DAMDFE contingência", getType: typesKnockout.bool, val: ko.observable(false) });
    this.HabilitarFuncionalidadesProjetoGollum = PropertyEntity({ text: "Habilitar funcionalidades do projeto Gollum", getType: typesKnockout.bool, val: ko.observable(false) });
    this.EnviarCTeApenasParaTomador = PropertyEntity({ text: "Enviar CT-e apenas para o tomado", getType: typesKnockout.bool, val: ko.observable(false) });
    this.HabilitarControleSituacaoColaboradorParaMotoristasTerceiros = PropertyEntity({ text: "Habilitar controle de Situação Colaborador para Motoristas terceiros?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarCargaComFluxoFilialEmissoraComExpedidor = PropertyEntity({ text: "Gerar carga com fluxo filial emissora com expedidor", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirAbaDeEixosNoModeloVeicular = PropertyEntity({ text: "Exibir Aba de Eixos no Modelo Veicular", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoMostrarMotivoBloqueio = PropertyEntity({ text: "Não mostrar o motivo do veículo estar bloqueado", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirReabrirOcorrenciaEmCasoDeRejeicao = PropertyEntity({ text: "Permitir reabrir ocorrência em caso de rejeição", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirAprovarDigitalizacaoDeCanhotoRejeitado = PropertyEntity({ text: "Permitir aprovar digitalização de canhotos rejeitados", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarSituacaoEntregaAoEnviarImagemCanhotoManualmente = PropertyEntity({ text: "Validar situação da entrega ao enviar imagem do canhoto manualmente", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ObterNumeroNotaFiscalPorObjetoOcr = PropertyEntity({ text: "Obter número de nota fiscal por objeto (OCR)", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RetornarMetodoBuscarEntregasRealizadasPendentesIntegracaoSomenteCanhotoDigitalizado = PropertyEntity({ text: "Retornar no método BuscarEntregasRealizadasPendentesIntegracao somentes se o canhoto estiver digitalizado", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirRetornarStatusCanhotoNaAPIDigitalizacao = PropertyEntity({ text: "Permitir retornar status do canhoto na API de digitalização", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ValidarSituacaoEntregaAoEnviarImagemCanhotoManualmente = PropertyEntity({ text: "Validar situação da entrega ao enviar imagem do canhoto manualmente", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UsaFluxoSubstituicaoFaseada = PropertyEntity({ text: "Usa Fluxo de Substituicao Faseada", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.EfetuarCancelamentoDePagamentoAoCancelarCarga = PropertyEntity({ text: "Efetuar cancelamento de pagamento ao cancelar carga", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.BloqueioEnvioIntegracoesCargasAnuladaseCanceladas = PropertyEntity({ text: "Bloquear reenvio de Integrações caso contenha cargas anuladas/canceladas", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.MotivoCancelamentoPagamentoPadrao = PropertyEntity({ text: "*Motivo de Cancelamento do Pagamento Padrão:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: ko.observable(false) });
    this.ObrigarANTTVeiculoValidarSalvarDadosTransporte = PropertyEntity({ text: "Obrigar ANTT no veículo e validar na carga ao salvar dados de transporte", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.CadastrarVeiculoMotoristaBRK = PropertyEntity({ text: "Cadastrar veículo/motorista na BRK", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ValidarTAGDigitalCom = PropertyEntity({ text: "Validar TAG na DigitalCom", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.InformarKmMovimentacaoPlaca = PropertyEntity({ text: "Informar KM na Movimentação de Placa", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ControleComissaoPorTipoOperacao = PropertyEntity({ text: "Controle de comissão por tipo de operação", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirRemoverVeiculoNoMetodoInformarDadosTransporteCarga = PropertyEntity({ text: "Permitir remover veículo no método InformarDadosTransporteCarga", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoCancelarCargaAoAplicarStatusFinalizadorJanelaDescarregamento = PropertyEntity({ text: "Não cancelar carga ao aplicar status finalizador na janela de descarregamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoHabilitarDetalhesCarga = PropertyEntity({ text: "Não habilitar detalhes da carga.", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NotificarTransportadorProcessoShareRotas = PropertyEntity({ text: "Notificar transportador no processo de SHARE (Rotas).", getType: typesKnockout.bool, val: ko.observable(false) });
    this.HabilitarSpotCargaAposLimiteHoras = PropertyEntity({ text: "Habilitar spot aberto de oferta de carga após o tempo limite definido no share", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarMesmoGestorParaTodaComposicao = PropertyEntity({ text: "Gestor de frota: Utilizar mesmo gestor para toda composição", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ExportarCNPJEChaveDeAcessoFormatado = PropertyEntity({ text: "Exportar CNPJ e chave de acesso formatados", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.CompactarImagemCanhotoIaComproveiCasoTamanhoUltrapasseUmMB = PropertyEntity({ text: "Compactar imagem do canhoto antes de enviar para IA da Comprovei, caso passe de 1MB", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RetornarSomenteCanhotoComNFeEntregueEmBuscarCanhotosNotasFiscaisDigitalizados = PropertyEntity({ text: "Retornar no método BuscarCanhotosNotasFiscaisDigitalizados somente se o canhoto estiver digitalizado e com situação da NF-e Entregue", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.IsExportarCNPJVisible = ko.computed(() => { return this.UtilizarExportacaoRelatorioCSV.val(); });
    this.PermitirTransportadorInformarPlacasEMotoristaAoDeclararInteresseCarga = PropertyEntity({ text: "Permitir Transportador Informar Placas e Motorista ao declarar interesse na Carga", getType: typesKnockout.bool, val: ko.observable(false) });

    this.FinalizarAutomaticamenteMonitoramentosEmAndamento = PropertyEntity({ text: "Finalizar automaticamente Monitoramentos em andamento por dias em aberto (Dias)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.FinalizarAutomaticamenteMonitoramentosPrevisaoUltimaEntrega = PropertyEntity({ text: "Finalizar automaticamente Monitoramentos em andamento pela previsão da ultima entrega (Dias)", getType: typesKnockout.bool, val: ko.observable(false) });

    this.ExibirPedidosFormatoGrid = PropertyEntity({ text: "Exibir pedidos em formato de grid", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirListagemNotasFiscais = PropertyEntity({ text: "Exibir listagem das notas fiscais", getType: typesKnockout.bool, val: ko.observable(false) });

    this.ambiente_LimparMotoristaIntegracaoVeiculo = PropertyEntity({ text: "Limpar Motorista Integracao Veiculo", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_LoginAD = PropertyEntity({ text: "Login AD", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_RegerarDACTEOracle = PropertyEntity({ text: "Regerar DACTEOracle", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_ReenviarErroIntegracaoCTe = PropertyEntity({ text: "Reenviar Erro Integracao CTe", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_AtualizarTipoEmpresa = PropertyEntity({ text: "Atualizar Tipo Empresa", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_ValidarNFeJaImportada = PropertyEntity({ text: "Validar NFe Ja Importada", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_UtilizaOptanteSimplesNacionalDaIntegracao = PropertyEntity({ text: "Utiliza Optante Simples Nacional Da Integracao", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_RecalcularICMSNaEmissaoCTe = PropertyEntity({ text: "Recalcular ICMSNa Emissao CTe", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_NaoCalcularDIFALParaCSTNaoTributavel = PropertyEntity({ text: "Nao Calcular DIFALPara CSTNao Tributavel", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_ReenviarErroIntegracaoMDFe = PropertyEntity({ text: "Reenviar Erro Integracao MDFe", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_EncerraMDFeAutomaticoComMesmaData = PropertyEntity({ text: "Encerra MDFe Automatico Com Mesma Data", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_EncerraMDFeAntesDaEmissao = PropertyEntity({ text: "Encerra MDFe Antes Da Emissao", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_EncerraMDFeAutomaticoOutrosSistemas = PropertyEntity({ text: "Encerra MDFe Automatico Outros Sistemas", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_EnviarEmailMDFeClientes = PropertyEntity({ text: "Enviar Email MDFe Clientes", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_UtilizarDocaDoComplementoFilial = PropertyEntity({ text: "Utilizar Doca Do Complemento Filial", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_RetornarModeloVeiculo = PropertyEntity({ text: "Retornar Modelo Veiculo", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_MDFeUtilizaDadosVeiculoCadastro = PropertyEntity({ text: "MDFe Utiliza Dados Veiculo Cadastro", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_MDFeUtilizaVeiculoReboqueComoTracao = PropertyEntity({ text: "MDFe Utiliza Veiculo Reboque Como Tracao", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_CTeUtilizaProprietarioCadastro = PropertyEntity({ text: "CTe Utiliza Proprietario Cadastro", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_GerarCTeDasNFSeAutorizadas = PropertyEntity({ text: "Gerar CTe Das NFSe Autorizadas", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_IncluirISSNFSeLocalidadeTomadorDiferentePrestador = PropertyEntity({ text: "Incluir ISSNFSe Localidade Tomador Diferente Prestador", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_IntegracaoNFSeUtilizaAliquotaMultiCTeQuandoTransportadorSimples = PropertyEntity({ text: "Integracao NFSe Utiliza Aliquota Multi CTe Quando Transportador Simples", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_AtualizarValorFrete_AtualizarICMS = PropertyEntity({ text: "Atualizar Valor Frete_Atualizar ICMS", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_ConsultarDuplicidadeOracle = PropertyEntity({ text: "Consultar Duplicidade Oracle", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_EnviarIntegracaoMagalogNoRetorno = PropertyEntity({ text: "Enviar Integracao Magalog No Retorno", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_EnviarIntegracaoErroMDFeMagalog = PropertyEntity({ text: "Enviar Integracao Erro MDFe Magalog", getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.ambiente_AmbienteProducao = PropertyEntity({ text: "Ambiente Producao", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_AmbienteSeguro = PropertyEntity({ text: "Ambiente Seguro", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_AplicarValorICMSNoComplemento = PropertyEntity({ text: "Aplicar Valor ICMSNo Complemento", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_AdicionarCTesFilaConsulta = PropertyEntity({ text: "Adicionar CTes Fila Consulta", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_NaoUtilizarColetaNaBuscaRotaFrete = PropertyEntity({ text: "Nao Utilizar Coleta Na Busca Rota Frete", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_OcultarConteudoColog = PropertyEntity({ text: "Ocultar Conteudo Colog", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_ConsultarPeloCustoDaRota = PropertyEntity({ text: "Consultar Pelo Custo Da Rota", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_CalcularHorarioDoCarregamento = PropertyEntity({ text: "Calcular Horario Do Carregamento", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_EnviarTodasNotificacoesPorEmail = PropertyEntity({ text: "Enviar Todas Notificacoes Por Email", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_CalcularFreteFechamento = PropertyEntity({ text: "Calcular Frete Fechamento", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_GerarDocumentoFechamento = PropertyEntity({ text: "Gerar Documento Fechamento", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_NovoLayoutPortalFornecedor = PropertyEntity({ text: "Novo Layout Portal Fornecedor", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_NovoLayoutCabotagem = PropertyEntity({ text: "Novo Layout Cabotagem", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_UtilizarIntegracaoSaintGobainNova = PropertyEntity({ text: "Utilizar Integracao Saint Gobain Nova", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_FiltrarCargasPorProprietario = PropertyEntity({ text: "Filtrar Cargas Por Proprietario", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_CargaControleEntrega_Habilitar_ImportacaoCargaFluvial = PropertyEntity({ text: "Carga Controle Entrega_Habilitar_Importacao Carga Fluvial", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_FTPPassivo = PropertyEntity({ text: "FTPPassivo", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_UtilizaSFTP = PropertyEntity({ text: "Utiliza SFTP", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_GerarNotFisPorNota = PropertyEntity({ text: "Gerar Not Fis Por Nota", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_UtilizarMetodoImportacaoTabelaFretePorServico = PropertyEntity({ text: "Utilizar Metodo Importacao Tabela Frete Por Servico", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_UtilizarLayoutImportacaoTabelaFreteGPA = PropertyEntity({ text: "Utilizar Layout Importacao Tabela Frete GPA", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_ExibirSituacaoIntegracaoXMLGPA = PropertyEntity({ text: "Exibir Situacao Integracao XMLGPA", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_ProcessarCTeMultiCTe = PropertyEntity({ text: "Processar CTe Multi CTe", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_NaoUtilizarCNPJTransportador = PropertyEntity({ text: "Nao Utilizar CNPJTransportador", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_BuscarFilialPorCNPJRemetenteDestinatarioGerarCargaCTe = PropertyEntity({ text: "Buscar Filial Por CNPJRemetente Destinatario Gerar Carga CTe", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_SempreUsarAtividadeCliente = PropertyEntity({ text: "Sempre Usar Atividade Cliente", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_AtualizarFantasiaClienteIntegracaoCTe = PropertyEntity({ text: "Atualizar Fantasia Cliente Integracao CTe", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_CadastrarMotoristaIntegracaoCTe = PropertyEntity({ text: "Cadastrar Motorista Integracao CTe", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_CTeCarregarVinculosVeiculosCadastro = PropertyEntity({ text: "CTe Carregar Vinculos Veiculos Cadastro", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_CTeAtualizaTipoVeiculo = PropertyEntity({ text: "CTe Atualiza Tipo Veiculo", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_NaoAtualizarCadastroVeiculo = PropertyEntity({ text: "Nao Atualizar Cadastro Veiculo", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_AgruparQuantidadesImportacaoCTe = PropertyEntity({ text: "Agrupar Quantidades Importacao CTe", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_EncerraMDFeAutomatico = PropertyEntity({ text: "Encerra MDFe Automatico", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_EnviaContingenciaMDFeAutomatico = PropertyEntity({ text: "Envia Contingencia MDFe Automatico", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_EnviarCertificadoOracle = PropertyEntity({ text: "Enviar Certificado Oracle", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_EnviarCertificadoKeyVault = PropertyEntity({ text: "Enviar Certificado Key Vault", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_PermitirInformarCapacidadeMaximaParaUploadArquivos = PropertyEntity({ text: "Permitir informar capacidade máxima para upload de arquivos", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ambiente_DesabilitarPopUpsDeNotificacao = PropertyEntity({ text: "Desabilitar Pop-ups de notificação", getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.ConsiderarConfiguracaoNoTipoDeOperacaoParaParticipantesDosDocumentosAoGerarCargaEspelho = PropertyEntity({ text: "Considerar Configuração no Tipo de Operação para Participantes dos Documentos ao gerar Carga espelho.", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ProcessarDadosTransporteAoFecharCarga = PropertyEntity({ text: "Processar dados transportes ao fechar carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExigirRotaRoteirizadaNoPedido = PropertyEntity({ text: "Exigir rota roteirizada no pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirAuditoriaPedidos = PropertyEntity({ text: "Exigir auditoria de pedidos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarParametrosBuscaAutomaticaClienteImportacao = PropertyEntity({ text: "Utilizar parâmetros de busca automática de Cliente compatível na importação", getType: typesKnockout.bool, val: ko.observable(false) });
    this.RemoverObservacoesDeEntregaAoRemoverPedidoCarga = PropertyEntity({ text: "Remover observações de entrega ao remover o pedido da carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AtualizarCargaAoImportarPlanilha = PropertyEntity({ text: "Atualizar carga ao importar planilha", getType: typesKnockout.bool, val: ko.observable(false) });
    this.HabilitarOpcoesDeDuplicacaoDoPedidoParaDevolucaoTotalParcial = PropertyEntity({ text: "Habilitar opções de duplicação do pedido para (Devolução Total, Devolução Parcial)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarEmpresaFilialEmissoraNoArquivoEDI = PropertyEntity({ text: "Utilizar empresa filial emissora no arquivo EDI", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteReceberNotaFiscalViaIntegracaoNasEtapasFreteETransportador = PropertyEntity({ text: "Permite receber nota fiscal via integração nas etapas frete e transportador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.RecalcularFreteAoDuplicarCargaCancelamentoDocumento = PropertyEntity({ text: "Recalcular Frete Ao Duplicar Carga no Cancelamento de Documentos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteInformarFreteOperadorFilialEmissora = PropertyEntity({ text: "Permite informar frete operador para filial emissora", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoConsiderarRecebedorAoCalcularNumeroEntregasEmissaoPorPedido = PropertyEntity({ text: "Não Considerar Recebedor Ao Calcular Número de Entregas na Emissao Por Pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteHabilitarContingenciaEPECAutomaticamente = PropertyEntity({ text: "Permitir habilitar contingência EPEC automaticamente", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteReceberNotaFiscalViaIntegracaoNasEtapasFreteETransportador = PropertyEntity({ text: "Permite receber nota fiscal via integração nas etapas frete e transportador", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteRealizarConsultaVPUtilizandoModeloVeicularCarga = PropertyEntity({ text: "Permite realizar a consulta do VP utilizando o modelo veicular da carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteSelecionarPlacaPorTipoVeiculoTransbordo = PropertyEntity({ text: "Permite selecionar placa por tipo de veículo na tela de transbordo", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoGerarPagamentoParaMotoristaTerceiro = PropertyEntity({ text: "Não gerar pagamento para motorista terceiro", getType: typesKnockout.bool, val: ko.observable(false) });
    this.EfetuarVinculoCentroResultadoCTeSubstituto = PropertyEntity({ text: "Efetuar vínculo do Centro do Resultado no CTe Substituto", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirInformarPercentual100AdiantamentoCarga = PropertyEntity({ text: "Permitir informar 100% de adiantamento na carga", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.UtilizarControlePaletesPorCliente = PropertyEntity({ text: "Utilizar controle de pallets por Cliente", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.LimiteDiasParaDevolucaoDePallet = PropertyEntity({ text: "Limite Dias para Devolução de Pallet", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TravarFluxoCompraCasoFornecedorDivergenteNaOrdemCompra = PropertyEntity({ text: "Travar fluxo de compra na etapa ordem de compra caso o fornecedor seja diferente da cotação e validar os dados da ordem de compra no documento de entrada", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AgruparProvisoesPorNotaFiscalFechamentoMensal = PropertyEntity({ text: "Agrupar provisões por nota fiscal no fechamento mensal", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.NaoPermitirGerarLotesPagamentosDocumentosBloqueados = PropertyEntity({ text: "Não permitir gerar lotes de pagamentos com documentos bloqueados", getType: typesKnockout.bool, val: ko.observable(false), issue: 79785 });
    this.NaoPermitirReenviarIntegracoesPagamentoSeCancelado = PropertyEntity({ text: "Não permitir o reenvio de integrações de pagamento para pagamentos cancelados.", getType: typesKnockout.bool, val: ko.observable(false) });
    this.SetarCargaComoBloqueadaEnquantoNaoReceberDesbloqueioViaIntegracaoOuManual = PropertyEntity({ text: "Setar Carga Como Bloqueada Enquanto Não Receber Desbloqueio Via Integracao ou Manual", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirDesvincularGerarCopiaCTeRejeitadoCarga = PropertyEntity({ text: "Permitir desvincular e gerar cópia de CT-e rejeitado na carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarDistanciaRoteirizacaoNaCarga = PropertyEntity({ text: "Utilizar Distancia Roteirizacao Na Carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoEnviarXMLCTEPorEmailParaTipoServico = PropertyEntity({ text: "Não enviar XML do CTe por email por tipos de serviço", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TipoServicoCTeEmail = PropertyEntity({ text: "Tipo de Serviço CTe", val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumTipoServicoCTe.obterOpcoes(), required: ko.observable(false), visible: ko.observable(false) });
    this.PermitirSalvarApenasTransportadorEtapaUmCarga = PropertyEntity({ text: "Permitir salvar apenas transportador na etapa 1 da carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExigirConfirmacaoEtapaFreteNoFluxoNotaAposFrete = PropertyEntity({ text: "Exigir confirmação da etapa de frete no fluxo de nota após o frete, se configurada no tipo de operação", getType: typesKnockout.bool, val: ko.observable(false) });
    this.TornarFinalizacaoDeEntregasAssincrona = PropertyEntity({ text: "Tornar a Finalização de Coleta/Entrega assíncrona (Utiliza thread para alto volume)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarIntegracaoContabilizacaoCtesApos = PropertyEntity({ text: "Gerar integração de Escrituração de CTes após", getType: typesKnockout.bool, val: ko.observable(false) });
    this.DelayIntegracaoContabilizacaoCtes = PropertyEntity({ text: "Minutos para integração Escrituração de CTes: ", getType: typesKnockout.int, val: ko.observable(0), visible: ko.observable(false), required: ko.observable(false) });
    this.UtilizarConfiguracoesTransportadorParaFatura = PropertyEntity({ text: "Utilizar as configurações do Transportador para a Fatura", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirEnvioNovasOcorrenciasComMesmoCadastroTipoOcorrencia = PropertyEntity({ text: "Permitir envio de novas ocorrências com mesmo cadastro de Tipo de Ocorrência", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirBuscarCargasAgrupadasAoPesquisarNumero = PropertyEntity({ text: "Permitir buscar cargas com outros numeros ao pesquisar por Numero Carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.EncerrarMDFeAutomaticamenteAoFinalizarEntregas = PropertyEntity({ text: "Encerrar MDFe conforme as entregas do controle de entregas forem finalizadas.", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirAjustarEntregasEtapasAnterioresIntegracao = PropertyEntity({ text: "Permitir ajustar entregas em etapas anteriores a integração", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirReverterAnulacaoGerencialTelaCancelamento = PropertyEntity({ text: "Permitir reverter anulação gerencial e enviar cancelamento na tela de cancelamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.AtivarCancelamentoDeFaturaETituloAoFluxoDeCancelamentoNaCarga = PropertyEntity({ text: "Ativar Cancelamento de Fatura e Título vinculado ao fluxo de Cancelamento na Carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoPermitirAtribuirVeiculoCargaSeExistirMonitoramentoAtivoParaPlaca = PropertyEntity({ text: "Não permitir atribuir veículo na carga se existir monitoramento ativo para a placa", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ConsiderarFilialDaTransportadoraParaCompraDoValePedagioQuandoForEFrete = PropertyEntity({ text: "Considerar filial da transportadora para compra do VP quando for e-Frete", getType: typesKnockout.bool, val: ko.observable(false) });
    this.InformarDocaNaEtapaUmDaCarga = PropertyEntity({ text: "Informar doca na etapa 1 da carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.CancelarValePedagioQuandoGerarCargaTransbordo = PropertyEntity({ text: "Cancelar vale pedágio quando gerar carga de transbordo", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarModeloVeicularVeiculoCargaEtapaFrete = PropertyEntity({ text: "Validar modelo veicular do veículo X carga na etapa 3 (frete)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ObrigatoriedadeCIOTEmissaoMDFe = PropertyEntity({ text: "Obrigatoriedade de CIOT para emitir MDF-e", getType: typesKnockout.bool, val: ko.observable(false), visible: true });

    this.PermitirInformarRecebedorAoCriarUmRedespachoManual = PropertyEntity({ text: "Permitir informar Recebedor ao criar um Redespacho manual", getType: typesKnockout.bool, val: ko.observable(false) });

    this.PermitirFiltrarCargasNaEmissaoManualCteSemTerCtesImportados = PropertyEntity({ text: "Permitir filtrar Cargas na Emissão Manual de CT-e sem ter os CT-es importados", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirRemoverMultiplosPedidosCarga = PropertyEntity({ text: "Permitir remover múltiplos Pedidos da Carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.SolicitarJustificativaAoRemoverPedidoCarga = PropertyEntity({ text: "Solicitar justificativa ao remover Pedido da Carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirBloqueioFinalizacaoEntrega = PropertyEntity({ text: "Bloquear finalização de entrega com atendimentos em aberto ou em tratativa", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PossuiNotaCobertura = PropertyEntity({ text: "Habilitar ícones de nota cobertura", getType: typesKnockout.bool, val: ko.observable(false) });

    //#region configuracao envio email cobranca
    this.AvisoVencimetoEnvarEmail = PropertyEntity({ text: "Enviar e-mail", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AvisoVencimetoQunatidadeDias = PropertyEntity({ text: "Quantidades de dias antes do vencimento para envio:", getType: typesKnockout.int, val: ko.observable(0), visible: ko.observable(true), enable: ko.observable(true) });
    this.AvisoVencimetoEnviarDiariamente = PropertyEntity({ text: "Enviar diariamente até o vencimento:", getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.AvisoVencimetoAssunto = PropertyEntity({ text: "Assunto:", val: ko.observable(""), maxlength: 1000 });
    this.AvisoVencimetoAssuntoCodTitulo = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.AvisoVencimetoAssunto.id, "#CodigoTitulo"), type: types.event, text: "Cód. Titulo", enable: ko.observable(true) });
    this.AvisoVencimetoAssuntoDataVencimento = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.AvisoVencimetoAssunto.id, "#DataVencimento"), type: types.event, text: "Data Vencimento", enable: ko.observable(true) });
    this.AvisoVencimetoAssuntoDataEmissao = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.AvisoVencimetoAssunto.id, "#DataEmissao"), type: types.event, text: "Data Emissão", enable: ko.observable(true) });
    this.AvisoVencimetoAssuntoFatura = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.AvisoVencimetoAssunto.id, "#Fatura"), type: types.event, text: "Fatura", enable: ko.observable(true) });
    this.AvisoVencimetoAssuntoEmpresaRazao = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.AvisoVencimetoAssunto.id, "#EmpresaRazao"), type: types.event, text: "Empresa Razão Social", enable: ko.observable(true) });
    this.AvisoVencimetoAssuntoEmpresaCNPJ = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.AvisoVencimetoAssunto.id, "#EmpresaCNPJ"), type: types.event, text: "Empresa CNPJ", enable: ko.observable(true) });
    this.AvisoVencimetoAssuntoPessoaRazao = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.AvisoVencimetoAssunto.id, "#PessoaRazao"), type: types.event, text: "Pessoa Razão Social", enable: ko.observable(true) });
    this.AvisoVencimetoAssuntoPessoaCNPJ = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.AvisoVencimetoAssunto.id, "#PessoaCNPJ"), type: types.event, text: "Pessoa CPF/CNPJ", enable: ko.observable(true) });
    this.AvisoVencimetoAssuntoDocumentos = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.AvisoVencimetoAssunto.id, "#Documentos"), type: types.event, text: "Documento(s)", enable: ko.observable(true) });

    this.AvisoVencimetoMensagem = PropertyEntity({ text: "Conteúdo:", required: false, visible: ko.observable(true), def: ko.observable(""), val: ko.observable(""), enable: ko.observable(true), maxlength: 3000 });

    this.AvisoVencimetoMensagemCodTitulo = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.AvisoVencimetoMensagem.id, "#CodigoTitulo"), type: types.event, text: "Cód. Titulo", enable: ko.observable(true) });
    this.AvisoVencimetoMensagemDataVencimento = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.AvisoVencimetoMensagem.id, "#DataVencimento"), type: types.event, text: "Data Vencimento", enable: ko.observable(true) });
    this.AvisoVencimetoMensagemDataEmissao = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.AvisoVencimetoMensagem.id, "#DataEmissao"), type: types.event, text: "Data Emissão", enable: ko.observable(true) });
    this.AvisoVencimetoMensagemFatura = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.AvisoVencimetoMensagem.id, "#Fatura"), type: types.event, text: "Fatura", enable: ko.observable(true) });
    this.AvisoVencimetoMensagemSituacao = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.AvisoVencimetoMensagem.id, "#Situacao"), type: types.event, text: "Situação", enable: ko.observable(true) });
    this.AvisoVencimetoMensagemValorOriginal = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.AvisoVencimetoMensagem.id, "#ValorOriginal"), type: types.event, text: "Valor Original", enable: ko.observable(true) });
    this.AvisoVencimetoMensagemValorPendente = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.AvisoVencimetoMensagem.id, "#ValorPendente"), type: types.event, text: "Valor Pendente", enable: ko.observable(true) });
    this.AvisoVencimetoMensagemValorPago = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.AvisoVencimetoMensagem.id, "#ValorPago"), type: types.event, text: "Valor Pago", enable: ko.observable(true) });
    this.AvisoVencimetoMensagemDesconto = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.AvisoVencimetoMensagem.id, "#Desconto"), type: types.event, text: "Desconto", enable: ko.observable(true) });
    this.AvisoVencimetoMensagemAcrescimo = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.AvisoVencimetoMensagem.id, "#Acrescimo"), type: types.event, text: "Acréscimo", enable: ko.observable(true) });
    this.AvisoVencimetoMensagemSaldo = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.AvisoVencimetoMensagem.id, "#Saldo"), type: types.event, text: "Saldo", enable: ko.observable(true) });
    this.AvisoVencimetoMensagemEmpresaRazao = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.AvisoVencimetoMensagem.id, "#EmpresaRazao"), type: types.event, text: "Empresa Razão Social", enable: ko.observable(true) });
    this.AvisoVencimetoMensagemEmpresaCNPJ = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.AvisoVencimetoMensagem.id, "#EmpresaCNPJ"), type: types.event, text: "Empresa CNPJ", enable: ko.observable(true) });
    this.AvisoVencimetoMensagemPessoaRazao = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.AvisoVencimetoMensagem.id, "#PessoaRazao"), type: types.event, text: "Pessoa Razão Social", enable: ko.observable(true) });
    this.AvisoVencimetoMensagemPessoaCNPJ = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.AvisoVencimetoMensagem.id, "#PessoaCNPJ"), type: types.event, text: "Pessoa CPF/CNPJ", enable: ko.observable(true) });
    this.AvisoVencimetoMensagemPessoaFormaTitulo = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.AvisoVencimetoMensagem.id, "#FormaTitulo"), type: types.event, text: "Forma do Título", enable: ko.observable(true) });
    this.AvisoVencimetoMensagemDocumentos = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.AvisoVencimetoMensagem.id, "#Documentos"), type: types.event, text: "Documento(s)", enable: ko.observable(true) });
    this.AvisoVencimetoMensagemObservacaoFatura = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.AvisoVencimetoMensagem.id, "#ObservacaoFatura"), type: types.event, text: "Observação Fatura", enable: ko.observable(true) });
    this.AvisoVencimetoMensagemTabela = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.AvisoVencimetoMensagem.id, "#Tabela"), type: types.event, text: "Tabela Documento", enable: ko.observable(true) });
    this.AvisoVencimetoMensagemQuebraLinha = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.AvisoVencimetoMensagem.id, "#QuebraLinha"), type: types.event, text: "Quebra de Linha", enable: ko.observable(true) });

    this.CobrancaEnvarEmail = PropertyEntity({ text: "Enviar e-mail", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.CobrancaQunatidadeDias = PropertyEntity({ text: "Quantidades de dias após o vencimento para envio:", getType: typesKnockout.int, val: ko.observable(0), visible: ko.observable(true), enable: ko.observable(true) });
    this.CobrancaAssunto = PropertyEntity({ text: "Assunto:", val: ko.observable(""), maxlength: 1000 });
    this.CobrancaAssuntoCodTitulo = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.CobrancaAssunto.id, "#CodigoTitulo"), type: types.event, text: "Cód. Titulo", enable: ko.observable(true) });
    this.CobrancaAssuntoDataVencimento = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.CobrancaAssunto.id, "#DataVencimento"), type: types.event, text: "Data Vencimento", enable: ko.observable(true) });
    this.CobrancaAssuntoDataEmissao = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.CobrancaAssunto.id, "#DataEmissao"), type: types.event, text: "Data Emissão", enable: ko.observable(true) });
    this.CobrancaAssuntoFatura = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.CobrancaAssunto.id, "#Fatura"), type: types.event, text: "Fatura", enable: ko.observable(true) });
    this.CobrancaAssuntoEmpresaRazao = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.CobrancaAssunto.id, "#EmpresaRazao"), type: types.event, text: "Empresa Razão Social", enable: ko.observable(true) });
    this.CobrancaAssuntoEmpresaCNPJ = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.CobrancaAssunto.id, "#EmpresaCNPJ"), type: types.event, text: "Empresa CNPJ", enable: ko.observable(true) });
    this.CobrancaAssuntoPessoaRazao = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.CobrancaAssunto.id, "#PessoaRazao"), type: types.event, text: "Pessoa Razão Social", enable: ko.observable(true) });
    this.CobrancaAssuntoPessoaCNPJ = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.CobrancaAssunto.id, "#PessoaCNPJ"), type: types.event, text: "Pessoa CPF/CNPJ", enable: ko.observable(true) });
    this.CobrancaAssuntoDocumentos = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.CobrancaAssunto.id, "#Documentos"), type: types.event, text: "Documento(s)", enable: ko.observable(true) });
    //#endregion configuracao envio email cobranca

    this.CobrancaMensagem = PropertyEntity({ text: "Conteúdo:", required: false, visible: ko.observable(true), def: ko.observable(""), val: ko.observable(""), enable: ko.observable(true), maxlength: 3000 });
    this.CobrancaMensagemCodTitulo = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.CobrancaMensagem.id, "#CodigoTitulo"), type: types.event, text: "Cód. Titulo", enable: ko.observable(true) });
    this.CobrancaMensagemDataVencimento = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.CobrancaMensagem.id, "#DataVencimento"), type: types.event, text: "Data Vencimento", enable: ko.observable(true) });
    this.CobrancaMensagemDataEmissao = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.CobrancaMensagem.id, "#DataEmissao"), type: types.event, text: "Data Emissão", enable: ko.observable(true) });
    this.CobrancaMensagemFatura = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.CobrancaMensagem.id, "#Fatura"), type: types.event, text: "Fatura", enable: ko.observable(true) });
    this.CobrancaMensagemSituacao = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.CobrancaMensagem.id, "#Situacao"), type: types.event, text: "Situação", enable: ko.observable(true) });
    this.CobrancaMensagemValorOriginal = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.CobrancaMensagem.id, "#ValorOriginal"), type: types.event, text: "Valor Original", enable: ko.observable(true) });
    this.CobrancaMensagemValorPendente = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.CobrancaMensagem.id, "#ValorPendente"), type: types.event, text: "Valor Pendente", enable: ko.observable(true) });
    this.CobrancaMensagemValorPago = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.CobrancaMensagem.id, "#ValorPago"), type: types.event, text: "Valor Pago", enable: ko.observable(true) });
    this.CobrancaMensagemDesconto = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.CobrancaMensagem.id, "#Desconto"), type: types.event, text: "Desconto", enable: ko.observable(true) });
    this.CobrancaMensagemAcrescimo = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.CobrancaMensagem.id, "#Acrescimo"), type: types.event, text: "Acréscimo", enable: ko.observable(true) });
    this.CobrancaMensagemSaldo = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.CobrancaMensagem.id, "#Saldo"), type: types.event, text: "Saldo", enable: ko.observable(true) });
    this.CobrancaMensagemEmpresaRazao = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.CobrancaMensagem.id, "#EmpresaRazao"), type: types.event, text: "Empresa Razão Social", enable: ko.observable(true) });
    this.CobrancaMensagemEmpresaCNPJ = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.CobrancaMensagem.id, "#EmpresaCNPJ"), type: types.event, text: "Empresa CNPJ", enable: ko.observable(true) });
    this.CobrancaMensagemPessoaRazao = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.CobrancaMensagem.id, "#PessoaRazao"), type: types.event, text: "Pessoa Razão Social", enable: ko.observable(true) });
    this.CobrancaMensagemPessoaCNPJ = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.CobrancaMensagem.id, "#PessoaCNPJ"), type: types.event, text: "Pessoa CPF/CNPJ", enable: ko.observable(true) });
    this.CobrancaMensagemPessoaFormaTitulo = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.CobrancaMensagem.id, "#FormaTitulo"), type: types.event, text: "Forma do Título", enable: ko.observable(true) });
    this.CobrancaMensagemDocumentos = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.CobrancaMensagem.id, "#Documentos"), type: types.event, text: "Documento(s)", enable: ko.observable(true) });
    this.CobrancaMensagemObservacaoFatura = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.CobrancaMensagem.id, "#ObservacaoFatura"), type: types.event, text: "Observação Fatura", enable: ko.observable(true) });
    this.CobrancaMensagemTabela = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.CobrancaMensagem.id, "#Tabela"), type: types.event, text: "Tabela Documento", enable: ko.observable(true) });
    this.CobrancaMensagemQuebraLinha = PropertyEntity({ eventClick: (e) => InserirTag(_configuracaoEmbarcador.CobrancaMensagem.id, "#QuebraLinha"), type: types.event, text: "Quebra de Linha", enable: ko.observable(true) });

    this.NaoGerarAtendimentoDuplicadoParaMesmaOcorrencia = PropertyEntity({ text: "Não gerar atendimento duplicado para mesma ocorrência", getType: typesKnockout.bool, val: ko.observable(false) });
    this.EnviarEmailDeNotificacaoAutomaticamenteAoTransportadorDaCarga = PropertyEntity({ text: "Enviar email de notificação automaticamente ao transportador da carga", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizaRazaoSocialNaVisaoDoAgendamento = PropertyEntity({ text: "Utiliza Razão social na visão do agendamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarPreenchimentoTomadorFaturaConfiguracao = PropertyEntity({ text: "Utilizar o preenchimento do Tomador da Fatura para ler as configurações de Faturamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ManterValorMoedaConfirmarDocumentosFatura = PropertyEntity({ text: "Manter valor moeda ao confirmar os documentos da Fatura", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarEmpresaFilialImpressaoReciboPagamentoMotorista = PropertyEntity({ text: "Utilizar empresa/filial na impressão do recibo de pagamento ao motorista", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteInformarDataDeAgendamentoEReagendamentoRetroativamente = PropertyEntity({ text: "Permite informar data de agendamento e reagendamento retroativamente.", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarDataCarregamentoAoCriarCargaViaIntegracao = PropertyEntity({ text: "Utilizar data carregamento ao criar a carga via integração.", getType: typesKnockout.bool, val: ko.observable(false) });
    this.IniciarConfirmacaoDocumentosFiscaisCargaPorThread = PropertyEntity({ text: "Iniciar confirmação dos documentos fiscais da carga por thread", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirInformarValorFreteOperadorMesmoComFreteConfirmadoPeloTransportador = PropertyEntity({ text: "Permitir informar Valor de Frete Operador na Carga mesmo com o Frete confirmado pelo Transportador via Janela de Carregamento", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoAplicarICMSMetodoAtualizarFrete = PropertyEntity({ text: "Não aplicar ICMS ao utilizar o método AtualizarValorFrete", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PararCargaQuandoNaoInformadoCIOT = PropertyEntity({ text: "Parar carga quando não informado CIOT", getType: typesKnockout.bool, val: ko.observable(false) });

    //#endregion Booleanos

    //#region MotoristaIgnorado
    this.NomeMotoristaIgnorado = PropertyEntity({ text: "Nome Motorista Ignorado na Integração de carga:", val: ko.observable("") });
    this.GridMotoristasIgnorados = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), visible: ko.observable(true) });
    this.AdicionarMotoristaIgnorado = PropertyEntity({ eventClick: AdicionarMotoristaIgnoradoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.NaoGerarPreTripMotoristasIgnorados = PropertyEntity({ text: "Não gerar pré-trip para esses motoristas", getType: typesKnockout.bool, val: ko.observable(false) });
    //#endregion MotoristaIgnorado

    // #region Strings
    this.ExpressaoLacreContainer = PropertyEntity({ text: "Expressão para o Lacre do Container:", val: ko.observable("") });
    this.DescricaoProdutoPredominatePadrao = PropertyEntity({ text: "Descrição produto predominante padrão:", val: ko.observable("") });
    this.EmailsRetornoProblemaGerarCargaEmail = PropertyEntity({ text: "Emails retorno problema ao gerar carga:", val: ko.observable("") });
    this.EmailsAvisoVencimentoCotratoFrete = PropertyEntity({ text: "Emails aviso de vencimento de Contrato de Frete:", val: ko.observable("") });
    this.ObservacaoCTePadraoEmbarcador = PropertyEntity({ text: "Observação CT-e Padrão Embarcador:", val: ko.observable(""), maxlength: 500 });
    this.ObservacaoMDFePadraoEmbarcador = PropertyEntity({ text: "Observação MDF-e Padrão Embarcador:", val: ko.observable(""), maxlength: 200 });
    this.CSTCTeSubcontratacao = PropertyEntity({ text: "CST padrão para CT-e de subcontratação:", val: ko.observable(""), maxlength: 2 });
    this.MensagemPadraoInformarDadosTransporteJanelaCarregamentoTransportador = PropertyEntity({ text: "Mensagem padrão ao informar os dados de transporte na janela de carregamento do transportador:", val: ko.observable(""), maxlength: 1000 });
    this.ObservacaoGeralPedido = PropertyEntity({ text: "Observação geral para a impressão do pedido:", val: ko.observable(""), maxlength: 2000 });
    this.CampoObsContribuinteCTeCargaRedespacho = PropertyEntity({ text: "xCampo Obs. Cont. CT-e Carga Redespacho:", val: ko.observable(""), maxlength: 20 });
    this.TextoObsContribuinteCTeCargaRedespacho = PropertyEntity({ text: "xTexto Obs. Cont. CT-e Carga Redespacho:", val: ko.observable(""), maxlength: 60 });
    this.TokenSMS = PropertyEntity({ text: "Token SMS:", val: ko.observable("") });
    this.SenderSMS = PropertyEntity({ text: "Sender SMS:", val: ko.observable("") });
    this.LinkVideoMobile = PropertyEntity({ text: "Link Vídeo Mobile:", val: ko.observable(""), maxlength: 500 });
    this.DocumentoImpressaoPadraoCarga = PropertyEntity({ text: "Documento Impressão Padrão Carga:", val: ko.observable(""), maxlength: 100 });
    this.EmailsAlertaCargasParadas = PropertyEntity({ text: "E-mail(s) para envio de alerta de cargas paradas:", val: ko.observable(""), maxlength: 1000, getType: typesKnockout.multiplesEmails });
    this.ClienteRedMine = PropertyEntity({ text: "Identificação do cliente no Redmine:", required: false, maxlength: 200, visible: ko.observable(true), issue: 0 });
    this.TextoRecibo = PropertyEntity({ text: "Texto para recibo:", required: false, maxlength: 3000, visible: ko.observable(true), issue: 0 });
    this.PrefixoParaCargasGeradasViaCarregamento = PropertyEntity({ text: "Prefixo para cargas geradas via carregamento:", maxlength: 100, visible: ko.observable(true) });
    this.FormulaCustoPadrao = PropertyEntity({ text: "Fórmula para o custo do produto padrão: *Informe os complementos para o cálculo: ((Valor Total do Item) + ......... )", required: false, maxlength: 300, visible: ko.observable(true), def: ko.observable(""), val: ko.observable(""), enable: ko.observable(true) });
    this.TagValorUnitario = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.FormulaCustoPadrao.id, "#ValorUnitario"); }, type: types.event, text: "Valor Unitário" });
    this.TagQuantidade = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.FormulaCustoPadrao.id, "#Quantidade"); }, type: types.event, text: "Quantidade" });
    this.TagValorICMS = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.FormulaCustoPadrao.id, "#ValorICMS"); }, type: types.event, text: "Valor ICMS" });
    this.TagValorDiferencial = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.FormulaCustoPadrao.id, "#ValorDiferencial"); }, type: types.event, text: "Valor Diferencial" });
    this.TagValorICMSST = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.FormulaCustoPadrao.id, "#ValorICMSST"); }, type: types.event, text: "Valor ICMS ST" });
    this.TagValorIPI = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.FormulaCustoPadrao.id, "#ValorIPI"); }, type: types.event, text: "Valor IPI" });
    this.TagValorFrete = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.FormulaCustoPadrao.id, "#ValorFrete"); }, type: types.event, text: "Valor Frete" });
    this.TagValorSeguro = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.FormulaCustoPadrao.id, "#ValorSeguro"); }, type: types.event, text: "Valor Seguro" });
    this.TagValorOutras = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.FormulaCustoPadrao.id, "#ValorOutras"); }, type: types.event, text: "Valor Outras Despesas" });
    this.TagValorDesconto = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.FormulaCustoPadrao.id, "#ValorDesconto"); }, type: types.event, text: "Valor Desconto" });
    this.TagValorDescontoFora = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.FormulaCustoPadrao.id, "#ValorDescontoFora"); }, type: types.event, text: "Valor Desconto Fora" });
    this.TagValorImpostoFora = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.FormulaCustoPadrao.id, "#ValorImpostoFora"); }, type: types.event, text: "Valor Impostos Fora" });
    this.TagValorOutrasFora = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.FormulaCustoPadrao.id, "#ValorOutrasFora"); }, type: types.event, text: "Valor Outras Despesas Fora" });
    this.TagValorFreteFora = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.FormulaCustoPadrao.id, "#ValorFreteFora"); }, type: types.event, text: "Valor Frete Fora" });
    this.TagValorICMSFreteFora = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.FormulaCustoPadrao.id, "#ValorICMSFreteFora"); }, type: types.event, text: "Valor ICMS Frete Fora" });
    this.TagValorDiferencialFreteFora = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.FormulaCustoPadrao.id, "#ValorDiferencialFreteFora"); }, type: types.event, text: "Valor Diferencial do Frete Fora" });
    this.TagValorPIS = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.FormulaCustoPadrao.id, "#ValorPIS"); }, type: types.event, text: "Valor PIS" });
    this.TagValorCOFINS = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.FormulaCustoPadrao.id, "#ValorCOFINS"); }, type: types.event, text: "Valor COFINS" });
    this.TagValorCreditoPresumido = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.FormulaCustoPadrao.id, "#ValorCreditoPresumido"); }, type: types.event, text: "Valor Crédito Presumido" });
    this.TagAbreParenteses = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.FormulaCustoPadrao.id, "#("); }, type: types.event, text: "(", visible: ko.observable(false) });
    this.TagFechaParenteses = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.FormulaCustoPadrao.id, "#)"); }, type: types.event, text: ")", visible: ko.observable(false) });
    this.TagMenos = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.FormulaCustoPadrao.id, "#-"); }, type: types.event, text: "Subtrair (-)" });
    this.TagMais = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.FormulaCustoPadrao.id, "#+"); }, type: types.event, text: "Somar (+)" });
    this.TagVezes = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.FormulaCustoPadrao.id, "#*"); }, type: types.event, text: "Multiplicar (*)", visible: ko.observable(false) });
    this.TagDivisao = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.FormulaCustoPadrao.id, "#/"); }, type: types.event, text: "Dividir (/)", visible: ko.observable(false) });
    this.PadraoGeracaoNumeroCarga = PropertyEntity({ text: "Padrão para Geração do Número da Carga", required: false, maxlength: 300, visible: ko.observable(true), def: ko.observable(""), val: ko.observable(""), enable: ko.observable(true) });
    this.TagPadraoGeracaoNumeroCargaAno = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.PadraoGeracaoNumeroCarga.id, "#Ano"); }, type: types.event, text: "Ano" });
    this.TagPadraoGeracaoNumeroCargaMes = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.PadraoGeracaoNumeroCarga.id, "#Mes"); }, type: types.event, text: "Mês" });
    this.TagPadraoGeracaoNumeroCargaCodigoIntegracaoEmpresa = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.PadraoGeracaoNumeroCarga.id, "#CodigoIntegracaoEmpresa"); }, type: types.event, text: "Código de Integração do Transportador" });
    this.TagPadraoGeracaoNumeroCargaCodigoAlfanumericoEmpresa = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.PadraoGeracaoNumeroCarga.id, "#CodigoAlfanumericoEmpresa"); }, type: types.event, text: "Código Alfanumérico do Transportador" });


    this.arquivo_CaminhoRelatorios = PropertyEntity({ text: "Caminho Relatorios:", val: ko.observable("") });
    this.arquivo_CaminhoTempArquivosImportacao = PropertyEntity({ text: "Caminho Temp Arquivos Importacao:", val: ko.observable("") });
    this.arquivo_CaminhoCanhotos = PropertyEntity({ text: "Caminho Canhotos:", val: ko.observable("") });
    this.arquivo_CaminhoCanhotosAvulsos = PropertyEntity({ text: "Caminho Canhotos Avulsos:", val: ko.observable("") });
    this.arquivo_CaminhoXMLNotaFiscalComprovanteEntrega = PropertyEntity({ text: "Caminho XMLNota Fiscal Comprovante Entrega:", val: ko.observable("") });
    this.arquivo_CaminhoArquivosIntegracao = PropertyEntity({ text: "Caminho Arquivos Integracao:", val: ko.observable("") });
    this.arquivo_CaminhoRelatoriosEmbarcador = PropertyEntity({ text: "Caminho Relatorios Embarcador:", val: ko.observable("") });
    this.arquivo_CaminhoLogoEmbarcador = PropertyEntity({ text: "Caminho Logo Embarcador:", val: ko.observable("") });
    this.arquivo_CaminhoDocumentosFiscaisEmbarcador = PropertyEntity({ text: "Caminho Documentos Fiscais Embarcador:", val: ko.observable("") });
    this.arquivo_Anexos = PropertyEntity({ text: "Anexos:", val: ko.observable("") });
    this.arquivo_CaminhoGeradorRelatorios = PropertyEntity({ text: "Caminho Gerador Relatorios:", val: ko.observable("") });
    this.arquivo_CaminhoArquivosEmpresas = PropertyEntity({ text: "Caminho Arquivos Empresas:", val: ko.observable("") });
    this.arquivo_CaminhoRelatoriosCrystal = PropertyEntity({ text: "Caminho Relatorios Crystal:", val: ko.observable("") });
    this.arquivo_CaminhoRetornoXMLIntegrador = PropertyEntity({ text: "Caminho Retorno XMLIntegrador:", val: ko.observable("") });
    this.arquivo_CaminhoArquivos = PropertyEntity({ text: "Caminho Arquivos:", val: ko.observable("") });
    this.arquivo_CaminhoArquivosIntegracaoEDI = PropertyEntity({ text: "Caminho Arquivos Integracao EDI:", val: ko.observable("") });
    this.arquivo_CaminhoArquivosImportacaoBoleto = PropertyEntity({ text: "Caminho Arquivos Importacao Boleto:", val: ko.observable("") });
    this.arquivo_CaminhoOcorrencias = PropertyEntity({ text: "Caminho Ocorrencias:", val: ko.observable("") });
    this.arquivo_CaminhoOcorrenciasMobiles = PropertyEntity({ text: "Caminho Ocorrencias Mobiles:", val: ko.observable("") });
    this.arquivo_CaminhoArquivosImportacaoXMLNotaFiscal = PropertyEntity({ text: "Caminho Arquivos Importacao XMLNota Fiscal:", val: ko.observable("") });
    this.arquivo_CaminhoDestinoXML = PropertyEntity({ text: "Caminho Destino XML:", val: ko.observable("") });
    this.arquivo_CaminhoCanhotosAntigos = PropertyEntity({ text: "Caminho Canhotos Antigos:", val: ko.observable("") });
    this.arquivo_CaminhoRaiz = PropertyEntity({ text: "Caminho Raiz:", val: ko.observable("") });
    this.arquivo_CaminhoGuia = PropertyEntity({ text: "Caminho Guia:", val: ko.observable("") });
    this.arquivo_CaminhoDanfeSMS = PropertyEntity({ text: "Caminho Danfe SMS:", val: ko.observable("") });
    this.arquivo_CaminhoRaizFTP = PropertyEntity({ text: "Caminho Raiz FTP:", val: ko.observable("") });
    this.ambiente_WebServiceConsultaCTe = PropertyEntity({ text: "Web Service Consulta CTe:", val: ko.observable("") });
    this.ambiente_CodigoLocalidadeNaoCadastrada = PropertyEntity({ text: "Codigo Localidade Nao Cadastrada:", val: ko.observable("") });
    this.ambiente_EmpresasUsuariosMultiCTe = PropertyEntity({ text: "Empresas Usuarios Multi CTe:", val: ko.observable("") });

    this.ambiente_IdentificacaoAmbiente = PropertyEntity({ text: "Identificacao Ambiente:", val: ko.observable("") });
    this.ambiente_CodificacaoEDI = PropertyEntity({ text: "Codificacao EDI:", val: ko.observable("") });
    this.ambiente_LinkCotacaoCompra = PropertyEntity({ text: "Link Cotacao Compra:", val: ko.observable("") });
    this.ambiente_LogoPersonalizadaFornecedor = PropertyEntity({ text: "Logo Personalizada Fornecedor:", val: ko.observable("") });
    this.ambiente_LayoutPersonalizadoFornecedor = PropertyEntity({ text: "Layout Personalizado Fornecedor:", val: ko.observable("") });
    this.ambiente_ConcessionariasComDescontos = PropertyEntity({ text: "Concessionarias Com Descontos:", val: ko.observable("") });
    this.ambiente_PercentualDescontoConcessionarias = PropertyEntity({ text: "Percentual Desconto Concessionarias:", val: ko.observable("") });
    this.ambiente_PlacaPadraoConsultaValorPedagio = PropertyEntity({ text: "Placa Padrao Consulta Valor Pedagio:", val: ko.observable("") });
    this.ambiente_APIOCRLink = PropertyEntity({ text: "APIOCRLink:", val: ko.observable("") });
    this.ambiente_APIOCRKey = PropertyEntity({ text: "APIOCRKey:", val: ko.observable("") });
    this.ambiente_QuantidadeSelecaoAgrupamentoCargaAutomatico = PropertyEntity({ text: "Quantidade Selecao Agrupamento Carga Automatico:", val: ko.observable("") });
    this.ambiente_QuantidadeCargasAgrupamentoCargaAutomatico = PropertyEntity({ text: "Quantidade Cargas Agrupamento Carga Automatico:", val: ko.observable("") });
    this.ambiente_HorarioExecucaoThreadDiaria = PropertyEntity({ text: "Horario Execucao Thread Diaria:", val: ko.observable("") });
    this.ambiente_FornecedorTMS = PropertyEntity({ text: "Fornecedor TMS:", val: ko.observable("") });
    this.ambiente_TipoArmazenamento = PropertyEntity({ text: "Tipo Armazenamento:", val: ko.observable("") });
    this.ambiente_TipoArmazenamentoLeitorOCR = PropertyEntity({ text: "Tipo Armazenamento Leitor OCR:", val: ko.observable("") });
    this.ambiente_EnderecoFTP = PropertyEntity({ text: "Endereco FTP:", val: ko.observable("") });
    this.ambiente_UsuarioFTP = PropertyEntity({ text: "Usuario FTP:", val: ko.observable("") });
    this.ambiente_SenhaFTP = PropertyEntity({ text: "Senha FTP:", val: ko.observable("") });
    this.ambiente_PortaFTP = PropertyEntity({ text: "Porta FTP:", val: ko.observable("") });
    this.ambiente_PrefixosFTP = PropertyEntity({ text: "Prefixos FTP:", val: ko.observable("") });
    this.ambiente_EmailsFTP = PropertyEntity({ text: "Emails FTP:", val: ko.observable("") });
    this.ambiente_CodigoEmpresaMultisoftware = PropertyEntity({ text: "Codigo Empresa Multisoftware:", val: ko.observable("") });
    this.ambiente_MinutosParaConsultaNatura = PropertyEntity({ text: "Minutos Para Consulta Natura:", val: ko.observable("") });
    this.ambiente_FiliaisNatura = PropertyEntity({ text: "Filiais Natura:", val: ko.observable("") });
    this.ambiente_TipoProtocolo = PropertyEntity({ val: ko.observable(EnumTipoProtocolo.Padrao), options: EnumTipoProtocolo.obterOpcoes(), def: EnumTipoProtocolo.Padrao, text: "Tipo de Protocolo: " });
    this.MensagemPersonalizadaMotoristaBloqueado = PropertyEntity({ text: "Mensagem personalizada para motoristas bloqueados:", val: ko.observable("") });

    // #endregion Strings

    // #region Int
    this.QuantidadeCargasEmAberto = PropertyEntity({ text: "Quantidade de cargas que pode ficar em aberto:", getType: typesKnockout.int, val: ko.observable(0) });
    this.QuantidadeDiasLimiteVencimentoFaturaManual = PropertyEntity({ text: "Quantidade dias limite para vencimento no faturamento manual:", getType: typesKnockout.int, val: ko.observable(0) });
    this.QuantidadeDiasAbertoEstornoProvisao = PropertyEntity({ text: ko.observable("Quantidade dias em aberto para estorno provisão:"), getType: typesKnockout.int, val: ko.observable(0) });
    this.DiasDeFechamentoParaGeracaoPagamentoEscrituracaoAutomatico = PropertyEntity({ text: "Dias de fechamento para geracao Pagamento e Escrituração (dias separados por vírgula):", val: ko.observable(0), maxlength: 20 });
    this.TempoSegundosParaInicioEmissaoDocumentos = PropertyEntity({ text: "Tempo em segundos para início da emissão de documentos:", getType: typesKnockout.int, val: ko.observable(0) });
    this.TempoMinutosParaEnvioProgramadoIntegracao = PropertyEntity({ text: "Tempo em minutos para envio das integrações programadas:", getType: typesKnockout.int, val: ko.observable(0) });
    this.DiasAvisoVencimentoCotratoFrete = PropertyEntity({ text: "Dias aviso de vencimento Contrato de Frete:", getType: typesKnockout.int, val: ko.observable(0) });
    this.PrazoSolicitacaoOcorrencia = PropertyEntity({ text: "Prazo solicitação de Ocorrência:", getType: typesKnockout.int, val: ko.observable(0) });
    this.NumeroTentativasIntegracao = PropertyEntity({ text: "*Número de tentativas de integração:", getType: typesKnockout.int, visible: ko.observable(false), required: ko.observable(false), val: ko.observable("") });
    this.IntervaloMinutosEntreIntegracoes = PropertyEntity({ text: "*Intervalo em minutos entre as integrações:", getType: typesKnockout.int, visible: ko.observable(false), required: ko.observable(false), val: ko.observable("") });
    this.MinutosToleranciaPrevisaoChegadaDocaCarregamento = PropertyEntity({ text: "Minutos Tolerância Previsão Chegada Doca de Carregamento:", getType: typesKnockout.int, val: ko.observable(0) });
    this.LimiteLinhasArquivoEDI = PropertyEntity({ text: "Limite de Linhas Arquivo EDI (INTPFAR):", getType: typesKnockout.int, val: ko.observable(0) });
    this.LinhasNecessariasOutrasInformacoes = PropertyEntity({ text: "Linhas Necessárias para Outras Informações (INTPFAR):", getType: typesKnockout.int, val: ko.observable(0) });
    this.KMLimiteEntreAbastecimentos = PropertyEntity({ text: "KM de limite entre os abastecimentos:", getType: typesKnockout.int, val: ko.observable(0) });
    this.KMLimiteAberturaOrdemServico = PropertyEntity({ text: "KM de limite na abertura de ordem de serviço:", getType: typesKnockout.int, val: ko.observable(0) });
    this.KMLimiteEntreAbastecimentosARLA = PropertyEntity({ text: "KM de limite entre os abastecimentos de Arla:", getType: typesKnockout.int, val: ko.observable(0) });
    this.HorimetroLimiteEntreAbastecimentos = PropertyEntity({ text: "Horímetro de limite entre os abastecimentos:", getType: typesKnockout.int, val: ko.observable(0) });
    this.DesabilitarVeiculosInutilizadosDias = PropertyEntity({ text: "Desabilitar veículos não utilizados (dias):", getType: typesKnockout.int, val: ko.observable(0) });
    this.NumeroLinhasFeradasPorCTe = PropertyEntity({ text: "Número de Linhas Geradas Por CT-e (INTPFAR):", getType: typesKnockout.int, val: ko.observable(0) });
    this.QuantidadeRegistrosGridDocumentoEntrada = PropertyEntity({ text: "Quantidade Registros Grid Documento Entrada:", getType: typesKnockout.int, val: ko.observable(0) });
    this.TempoMinutosParaReenviarCancelamento = PropertyEntity({ text: "Tempo em min. para reenvio do cancelamento de CTe vedado por MDFe:", getType: typesKnockout.int, val: ko.observable(0) });
    this.MaxDownloadsPorVez = PropertyEntity({ text: "Número máximo de itens para download por vez:", getType: typesKnockout.int, val: ko.observable(0) });
    this.NumeroTentativasReenvioCteRejeitado = PropertyEntity({ text: "Número de Tentativas de Reenvio de CT-e Rejeitado:", getType: typesKnockout.int, val: ko.observable(0), maxlength: 2, configInt: { precision: 0, allowZero: true } });
    this.NumeroTentativasReenvioRotaFrete = PropertyEntity({ text: "Número de Tentativas de Reenvio da Rota Frete:", getType: typesKnockout.int, val: ko.observable(0), maxlength: 2, configInt: { precision: 0, allowZero: true } });
    this.NumeroDiasParaConsultaPracaPedagio = PropertyEntity({ text: "Número de dias para consulta de Praça pedagio", getType: typesKnockout.int, val: ko.observable(false), maxlength: 2, configInt: { precision: 0, allowZero: true } });
    this.QuantidadeMaximaDiasRelatorios = PropertyEntity({ text: "Qtd. máx. dias relatórios:", getType: typesKnockout.int, val: ko.observable(0), maxlength: 3, configInt: { precision: 0, allowZero: true } });
    this.QuantidadeCargaPedidoProcessamentoLote = PropertyEntity({ text: "Qtd. Carga Pedido para processar carga em lote:", getType: typesKnockout.int, val: ko.observable(0), maxlength: 3, configInt: { precision: 0, allowZero: true } });
    this.TempoMinutosPermanenciaCliente = PropertyEntity({ text: "Tempo de permanência no cliente para considerar entrada: (minutos)", getType: typesKnockout.int, val: ko.observable(0), maxlength: 3, configInt: { precision: 0, allowZero: true } });
    this.TempoHorasParaRetornoCTeAposFinalizacaoEmissao = PropertyEntity({ text: "Tempo para disponibilizar os documentos para retorno via WS após emissão da Carga: (Horas)", getType: typesKnockout.int, val: ko.observable(0), maxlength: 3, configInt: { precision: 0, allowZero: true } });
    this.TempoMinutosPermanenciaSubareaCliente = PropertyEntity({ text: "Tempo de permanência na subárea do cliente para considerar entrada: (minutos)", getType: typesKnockout.int, val: ko.observable(0), maxlength: 3, configInt: { precision: 0, allowZero: true } });
    this.VelocidadeMaximaExtremaEntrePosicoes = PropertyEntity({ text: "Velocidade máxima extrema entre as posições subsequentes: (Km/h)", getType: typesKnockout.int, val: ko.observable(0), maxlength: 4, configInt: { precision: 0, allowZero: true } });
    this.QuantidadeMaximaRegistrosRelatorios = PropertyEntity({ text: "Qtd. máx. registros relatórios:", getType: typesKnockout.int, val: ko.observable(0), maxlength: 11 });
    this.RaioPadrao = PropertyEntity({ text: "Raio padrão: (metros)", getType: typesKnockout.int, val: ko.observable(0), maxlength: 11 });
    this.TempoPadraoTerminoCarregamentoParaValidarDisponibilidadeDescarregamento = PropertyEntity({ text: "Tempo padrão para término do carregamento para validar a disponibilidade de descarregamento: (horas)", getType: typesKnockout.int, val: ko.observable(0), maxlength: 3 });
    this.IdentificarMonitoramentoStatusViagemEmTransitoKM = PropertyEntity({ text: "Distância mínima percorrida em direção ao destino: (Km) *", getType: typesKnockout.int, val: ko.observable(0), maxlength: 6, visible: ko.observable(true), required: ko.observable(false) });
    this.IdentificarMonitoramentoStatusViagemEmTransitoMinutos = PropertyEntity({ text: "Tempo de viagem mínimo em direção ao destino: (minutos) *", getType: typesKnockout.int, val: ko.observable(0), maxlength: 4, visible: ko.observable(true), required: ko.observable(false) });
    this.IdentificarVeiculoParadoDistancia = PropertyEntity({ text: "Distância máxima percorrida em posições subsequentes: (metros) *", getType: typesKnockout.int, val: ko.observable(0), maxlength: 6, visible: ko.observable(true), required: ko.observable(false) });
    this.IdentificarVeiculoParadoTempo = PropertyEntity({ text: "Tempo mínimo desprendido em posições subsequentes: (minutos) *", getType: typesKnockout.int, val: ko.observable(0), maxlength: 4, visible: ko.observable(true), required: ko.observable(false) });
    this.NumeroSerieNotaDebitoPadrao = PropertyEntity({ text: "Número Série Nota Débito Padrão (ND):", getType: typesKnockout.int, val: ko.observable(0), maxlength: 3 });
    this.NumeroSerieNotaCreditoPadrao = PropertyEntity({ text: "Número Série Nota Crédito Padrão (NC):", getType: typesKnockout.int, val: ko.observable(0), maxlength: 3 });
    this.PrevisaoEntregaTempoUtilDiarioMinutos = PropertyEntity({ text: "Tempo total útil diário: (minutos)", getType: typesKnockout.int, val: ko.observable(0), maxlength: 3 });
    this.PrevisaoEntregaVelocidadeMediaVazio = PropertyEntity({ text: "Velocidade média do veículo quando vazio: (Km/h)", getType: typesKnockout.int, val: ko.observable(0), maxlength: 3 });
    this.PrevisaoEntregaVelocidadeMediaCarregado = PropertyEntity({ text: "Velocidade média do veículo quando carregado: (Km/h)", getType: typesKnockout.int, val: ko.observable(0), maxlength: 3 });
    this.FatorMetroCubicoProdutoEmbarcadorIntegracao = PropertyEntity({ text: "Fator metro cúbico produto embarcador importação:", getType: typesKnockout.int, val: ko.observable(1), maxlength: 10 });
    this.DistanciaMinimaPercorridaParaSaidaDoAlvo = PropertyEntity({ text: "Distância mínima percorrida fora do alvo para considerar saída: (Km)", getType: typesKnockout.int, val: ko.observable(0), maxlength: 3 });
    this.TempoSemPosicaoParaVeiculoPerderSinal = PropertyEntity({ text: "Tempo sem posição para veículo perder sinal: (minutos)", getType: typesKnockout.int, val: ko.observable(0), maxlength: 4 });
    this.TempoPadraoDeEntrega = PropertyEntity({ text: "Tempo padrão da entrega: (minutos)", getType: typesKnockout.int, val: ko.observable(0) });
    this.TempoPadraoDeColetaParaCalcularPrevisao = PropertyEntity({ text: "Tempo padrão da coleta para calcular previsão: (minutos)", getType: typesKnockout.int, val: ko.observable(0) });
    this.TempoPadraoDeEntregaParaCalcularPrevisao = PropertyEntity({ text: "Tempo padrão da entrega para calcular previsão: (minutos)", getType: typesKnockout.int, val: ko.observable(0) });
    this.MinutosIntervalo = PropertyEntity({ text: "Tempo Intervalo Almoço ", val: ko.observable(0), getType: typesKnockout.int });
    this.IntervaloInicial = PropertyEntity({ text: "Retonar cargas entre o intervalo de ", val: ko.observable(0), getType: typesKnockout.int });
    this.IntervaloFinal = PropertyEntity({ text: "Até ", val: ko.observable(0), getType: typesKnockout.int });
    this.NumeroTentativasConsultarCargasErroRoteirizacao = PropertyEntity({ text: "Número de tentativas de consultar cargas com erro na roteirização:", getType: typesKnockout.int, maxlength: 2 });
    this.TempoMinutosAguardarReconsultarCargasErroRoteirizacao = PropertyEntity({ text: "Tempo aguardar para reconsultar cargas com erro na roteirizacao: (minutos)", getType: typesKnockout.int, maxlength: 3 });
    this.TempoPadraoDeDescargaMinutos = PropertyEntity({ text: "Tempo Padrão de Descarga (em Minutos)", getType: typesKnockout.int, maxlength: 5 });
    this.RaioMaximoGeoLocalidadeGeoCliente = PropertyEntity({ text: "Raio máximo geo cliente com relação geo localidade: (km)", getType: typesKnockout.int, maxlength: 3 });
    this.MaximoThreadsMontagemCarga = PropertyEntity({ text: "Limite Paralelismo roteirização:", getType: typesKnockout.int, maxlength: 2 });
    this.DiasAnterioresPesquisaCarga = PropertyEntity({ text: "Dias Anterior Para Pesquisa Carga: ", getType: typesKnockout.int, val: ko.observable(0) });
    this.HorasCargaExibidaNoApp = PropertyEntity({ text: "Tempo em que a carga ficará disponível para ser iniciada no aplicativo: (Horas)", getType: typesKnockout.int, val: ko.observable(0), maxlength: 3, configInt: { precision: 0, allowZero: true } });
    this.TempoMinutosAlertaCargasParadas = PropertyEntity({ text: "Tempo em minutos para alerta de cargas paradas:", getType: typesKnockout.int, val: ko.observable(0) });
    this.TempoMinimoAcionarPassouRaioSemConfirmar = PropertyEntity({ text: "Tempo mínimo no raio para acionar o estado \"Passou pelo raio sem confirmar\": (minutos)", getType: typesKnockout.int, val: ko.observable(0) });
    this.DiasEncerramentoAutomaticoMDFE = PropertyEntity({ text: "Quantidade Dias para Encerramento Automático do MDF-e:", getType: typesKnockout.int, val: ko.observable(0) });
    this.QuantidadeHorasPreencherDataCarregamentoCarga = PropertyEntity({ text: "Quantidade de Horas para acrescentar na Data De Carregamento da Carga a partir da hora de integração:", getType: typesKnockout.int, val: ko.observable(0) });
    this.DiasFiltrarDataProgramada = PropertyEntity({ text: "Dias para filtrar a previsão de chegada:", getType: typesKnockout.int, val: ko.observable(0), def: 0, maxlength: 2 });
    this.CodigoUsuarioDestino = PropertyEntity({ text: "Código usuário atribuido tarefa Redmine:", getType: typesKnockout.int, val: ko.observable(0) });
    this.TempoInicioViagemAposEmissaoDoc = PropertyEntity({ text: "Tempo para inicio viagem após emissão docs: (minutos)", getType: typesKnockout.int, val: ko.observable(0) });
    this.TempoInicioViagemAposFinalizacaoFluxoPatio = PropertyEntity({ text: "Tempo para inicio viagem após encerramento fluxo de pátio: (minutos)", getType: typesKnockout.int, val: ko.observable(0) });
    this.PrazoDiasAposDataEmissao = PropertyEntity({ text: "Prazo após data de emissão (dias):", getType: typesKnockout.int, val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.QuantidadeDiasLimiteVencimentoTitulo = PropertyEntity({ text: "Quantidade dias limite para vencimento do Título:", getType: typesKnockout.int, val: ko.observable(0), def: 0, configInt: { precision: 0, allowZero: true, thousands: "" }, maxlength: 5 });
    this.PrazoParaReverterDigitalizacaoAposAprovacao = PropertyEntity({ text: "Prazo para reverter digitalização após aprovação (dias):", getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.TamanhoMaximoDaImagemDoCanhoto = PropertyEntity({ text: "Tamanho máximo da imagem do canhoto (kb):", getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.QteDiasParaLiquidarPallet = PropertyEntity({ text: "Liquidar Pallet Automaticamente? Quantidade de Dias: ", val: ko.observable(""), getType: typesKnockout.int, def: 0, visible: ko.observable(true), required: ko.observable(false) });
    this.MinutosNotificarMotoristaAlertaViagem = PropertyEntity({ text: "Notificar motorista do início da viajem (minutos): ", val: ko.observable(""), getType: typesKnockout.int, def: 0, visible: ko.observable(true), required: ko.observable(false) });
    this.DiasParaPermitirInformarHorarioCarregamento = PropertyEntity({ text: "Dias para permitir informar o horário de carregamento: ", val: ko.observable(""), getType: typesKnockout.int, def: "", maxlength: 2 });
    this.DiasLimiteRetornarMultiplasCargas = PropertyEntity({ text: "Dias limite para retornar múltiplas cargas: ", getType: typesKnockout.int, val: ko.observable(0), def: 0, maxlength: 3 });
    this.DiasAntecidenciaParaComunicarMotoristaVencimentoLicenca = PropertyEntity({ text: "Dias de antecedência para comunicar o motorista sobre o vencimento das licenças:", getType: typesKnockout.int, val: ko.observable(0) });
    this.HorasLiberacaoDocumentoPagamentoTransportadorComCertificado = PropertyEntity({ text: "Liberar documento para pagamento do Transportador com Certificado após (Horas):", getType: typesKnockout.int, val: ko.observable(0) });
    this.HorasLiberacaoDocumentoPagamentoTransportadorSemCertificado = PropertyEntity({ text: "Liberar documento para pagamento do Transportador sem Certificado após (Horas):", getType: typesKnockout.int, val: ko.observable(0) });
    this.RemoverAutomaticamenteRequisicaoAbastecimentoAbertaPorPeriodoDias = PropertyEntity({ text: "Remover Automaticamente Requisição Aberta após (Dias retroativos)", val: ko.observable(""), getType: typesKnockout.int, def: 0, visible: ko.observable(true), required: ko.observable(false) });
    this.DiasParaFinalizarAutomaticamenteAlertasDoMonitoramentoPeriodicamente = PropertyEntity({ val: ko.observable(30), getType: typesKnockout.int, def: 30, visible: ko.observable(true), required: ko.observable(false) });
    this.DiasFinalizarAutomaticamenteMonitoramentoEmAndamento = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int, def: 0, visible: ko.observable(true), required: ko.observable(false) });
    this.DiasFinalizarMonitoramentoPrevisaoUltimaEntrega = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int, def: 0, visible: ko.observable(true), required: ko.observable(false) });
    this.DelayFaturamentoAutomatico = PropertyEntity({ text: "Delay para faturamento automático: ", getType: typesKnockout.int, val: ko.observable(0) });
    this.QuantidadeDocumentosLotePagamentoGeradoAutomatico = PropertyEntity({ text: "Quantidade de Documentos em cada Lote Pagamento Gerado Automático: ", getType: typesKnockout.int, val: ko.observable(0) });
    this.QuantidadeMinimaDocumentosLotePagamentoGeradoAutomatico = PropertyEntity({ text: "Quantidade Minima de Documentos em cada Lote Pagamento Gerado Automático: ", getType: typesKnockout.int, val: ko.observable(0) });
    this.InformePorcentagemAceitacaoInd = PropertyEntity({ text: ko.observable("Informe a % de Aceitação Ind.:"), getType: typesKnockout.int, val: ko.observable(0), maxlength: 2, required: ko.observable(false), visible: ko.observable(false) });
    this.TempoPermitirReagendamentoHoras = PropertyEntity({ text: "Tempo permitido para Reagendamento: (horas)", getType: typesKnockout.int, val: ko.observable(0), maxlength: 3, visible: ko.observable(false) });
    this.ambiente_CapacidadeMaximaParaUploadArquivos = PropertyEntity({ text: ko.observable("Capacidade máxima para upload de arquivos"), getType: typesKnockout.int, val: ko.observable(0), visible: ko.observable(false), required: ko.observable(false), maxlength: 2 });
    this.DistanciaMaximaRotaCurta = PropertyEntity({ text: "Distância máxima para considerar rota curta: (Km)", getType: typesKnockout.int, val: ko.observable(0), maxlength: 6 });
    this.TempoPermitidoPermanenciaEmCarregamento = PropertyEntity({ text: "Tempo máximo para permanência em carregamento (minutos):", getType: typesKnockout.int, val: ko.observable(0), maxlength: 6 });
    this.TempoPermitidoPermanenciaNoCliente = PropertyEntity({ text: "Tempo máximo para permanência no cliente (minutos):", getType: typesKnockout.int, val: ko.observable(0), maxlength: 6 });
    this.QuantidadeDiasDataColeta = PropertyEntity({ text: "Quantidade de dias para validação da data de coleta:", getType: typesKnockout.int, val: ko.observable(0) });
    this.TempoReprocessarCargaEntregasSemNotas = PropertyEntity({ text: "Dias retroativos para reprocessar entregas sem nota fiscal:", getType: typesKnockout.int, val: ko.observable(0), maxlength: 3 });
    // #endregion Int

    // #region Select
    this.SituacaoCargaAposConfirmacaoImpressao = PropertyEntity({ text: "Situação da carga após confirmação de impressão: ", val: ko.observable(0), options: _situacaoCarga });
    this.SituacaoCargaAposEmissaoDocumentos = PropertyEntity({ text: "Situação da carga após emissão de documentos: ", val: ko.observable(0), options: _situacaoCarga });
    this.SituacaoCargaAposIntegracao = PropertyEntity({ text: "Situação da carga após integração: ", val: ko.observable(0), options: _situacaoCarga });
    this.SituacaoCargaAposFinalizacaoDaCarga = PropertyEntity({ text: "Situação da carga após finalização da carga: ", val: ko.observable(0), options: _situacaoCarga });
    this.SistemaIntegracaoPadraoCarga = PropertyEntity({ text: "Sistema de integração padrão da carga: ", val: ko.observable(0), options: EnumTipoIntegracao.obterOpcoes(true) });
    this.TipoIntegracaoCancelamentoPadrao = PropertyEntity({ text: "Tipo de Integração Cancelamento Padrão: ", val: ko.observable(0), options: EnumTipoIntegracao.obterOpcoesPadraoCancelamentoCarga(true) });
    this.TipoEmissaoIntramunicipal = PropertyEntity({ text: "Tipo de emissão intra municipal: ", val: ko.observable(0), options: _tipoEmissaoIntramunicipal });
    this.ObrigatorioRegrasOcorrencia = PropertyEntity({ text: "Obrigatório regras de ocorrência: ", val: ko.observable(0), options: _simNao });
    this.TipoContratoFreteTerceiro = PropertyEntity({ text: "Tipo de Contrato de Frete Terceiro: ", val: ko.observable(0), options: _tipoContratoFreteTerceiro });
    this.TipoFechamentoFrete = PropertyEntity({ text: "Tipo de Fechamento de Frete: ", val: ko.observable(0), options: EnumTipoFechamentoFrete.obterOpcoes() });
    this.TipoChamado = PropertyEntity({ text: "Tipo do chamado: ", val: ko.observable(0), options: _tipoChamado });
    this.TipoGeracaoTituloFatura = PropertyEntity({ text: "Tipo da geração de título da fatura: ", val: ko.observable(EnumTipoMontagemCarga.Todos), def: EnumTipoMontagemCarga.Todos, options: _tipoGeracaoTituloFatura });
    this.TipoMontagemCargaPadrao = PropertyEntity({ text: "Tipo de montagem de carga: ", val: ko.observable(0), options: EnumTipoMontagemCarga.obterOpcoesPesquisa() });
    this.TipoControleSaldoPedido = PropertyEntity({ text: "Tipo controle saldo pedido: ", val: ko.observable(0), options: EnumTipoControleSaldoPedido.obterOpcoes() });
    this.TipoFiltroDataMontagemCarga = PropertyEntity({ text: "Filtrar data pedido em: ", val: ko.observable(0), options: EnumTiposFiltroDataMontagemCarga });
    this.DiaSemanaNotificarCanhotosPendentes = PropertyEntity({ text: "Dia da Semana para Notificar Canhotos Pendentes: ", val: ko.observable(EnumDiaSemana.Segunda), options: EnumDiaSemana.obterOpcoes() });
    this.DataCompetenciaDocumentoEntrada = PropertyEntity({ text: "Data de competência para Documento de Entrada: ", val: ko.observable(EnumDataCompetenciaDocumentoEntrada.DataEntrada), options: _dataCompetenciaDocumentoEntrada });
    this.DataEntradaDocumentoEntrada = PropertyEntity({ text: "Data de entrada para Documento de Entrada: ", val: ko.observable(EnumDataEntradaDocumentoEntrada.DataLancamento), options: _dataEntradaDocumentoEntrada });
    this.TipoUltimoPontoRoteirizacao = PropertyEntity({ val: ko.observable(EnumTipoUltimoPontoRoteirizacao.AteOrigem), options: EnumTipoUltimoPontoRoteirizacao.obterOpcoes(), def: EnumTipoUltimoPontoRoteirizacao.AteOrigem, text: "Tipo do último Ponto da roteirização: ", issue: 1292, required: true });
    this.NumeroCasasDecimaisQuantidadeProduto = PropertyEntity({ val: ko.observable(2), options: _numeroCasasDecimaisQuantidadeProduto, def: 2, text: "Numero de casas decimais na quantidade do produto: " });
    this.TipoRomaneio = PropertyEntity({ val: ko.observable(EnumTipoRomaneio.Padrao), options: EnumTipoRomaneio.obterOpcoes(), def: EnumTipoRomaneio.Padrao, text: "Tipo de romaneio: " });
    this.TipoImpressaoPedido = PropertyEntity({ text: "Tipo Impressão do Pedido: ", val: ko.observable(EnumTipoImpressaoPedido.AutorizacaoCarregamento), options: EnumTipoImpressaoPedido.obterOpcoes(), def: EnumTipoImpressaoPedido.AutorizacaoCarregamento, required: true });
    this.TipoImpressaoPedidoPrestacaoServico = PropertyEntity({ text: "Tipo Impressão da Prestação do Pedido: ", val: ko.observable(EnumTipoImpressaoPedidoPrestacaoServico.PrestacaoComViaCliente), options: EnumTipoImpressaoPedidoPrestacaoServico.obterOpcoes(), def: EnumTipoImpressaoPedidoPrestacaoServico.PrestacaoComViaCliente });
    this.SituacaoJanelaCarregamentoPadraoPesquisa = PropertyEntity({ text: "Situação da carga padrão para pesquisa na janela de carregamento:", val: ko.observable(EnumSituacaoCargaJanelaCarregamento.Todas), def: EnumSituacaoCargaJanelaCarregamento.Todas, options: [{ text: "Todas", value: this.Todas }].concat(EnumSituacaoCargaJanelaCarregamento.obterOpcoesPesquisaJanelaCarregamento()) });
    this.TipoRestricaoPalletModeloVeicularCarga = PropertyEntity({ text: "Tipo de restrição de número de pallets por modelo veicular de carga:", val: ko.observable(EnumTipoRestricaoPalletModeloVeicularCarga.BloquearSomenteNumeroSuperior), def: EnumTipoRestricaoPalletModeloVeicularCarga.BloquearSomenteNumeroSuperior, options: EnumTipoRestricaoPalletModeloVeicularCarga.obterOpcoes() });
    this.SituacaoCargaAcertoViagem = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, url: "Configuracao/ObterSituacaoesCarga", text: "Situações da Carga para o Acerto de Viagem: ", options: ko.observable(new Array()), visible: ko.observable(true) });
    this.TipoMovimentoPagamentoMotorista = PropertyEntity({ text: "Tipo de movimento para o Pagamento ao Motorista: ", val: ko.observable(EnumTipoMovimentoEntidade.Entrada), options: _situacaoPagamentoAdiantamentoMotorista });
    this.TipoMovimentoReversaoPagamentoMotorista = PropertyEntity({ text: "Tipo de movimento para a reversão do Pagamento ao Motorista: ", val: ko.observable(EnumTipoMovimentoEntidade.Saida), options: _situacaoPagamentoAdiantamentoMotorista });
    this.MonitorarPosicaoAtualVeiculo = PropertyEntity({ text: "Monitorar posição atual dos veículos: ", val: ko.observable(EnumMonitorarPosicaoAtualVeiculo.Todos), options: EnumMonitorarPosicaoAtualVeiculo.ObterOpcoes() });
    this.DataBaseCalculoPrevisaoControleEntrega = PropertyEntity({ text: "Data base para cálculo das previsões no controle de entrega (antes de iniciar viagem): ", val: ko.observable(EnumDataBaseCalculoPrevisaoControleEntrega.DataPrevisaoTerminoCarga), options: EnumDataBaseCalculoPrevisaoControleEntrega.obterOpcoes() });
    this.AtualizarRotaRealizadaDoMonitoramento = PropertyEntity({ text: "Atualizar rota realizada do monitoramento: ", val: ko.observable(EnumQuandoProcessarMonitoramento.AoIniciarMonitoramento), options: EnumQuandoProcessarMonitoramento.obterOpcoes() });
    this.Pais = PropertyEntity({ text: "País: ", val: ko.observable(EnumPaises.Brasil), options: EnumPaises.obterOpcoes() });
    this.TipoRecibo = PropertyEntity({ text: "Tipo de Recibo: ", val: ko.observable(EnumTipoRecibo.Padrao), options: EnumTipoRecibo.obterOpcoes() });
    this.TelaMonitoramentoApresentarCargasQuando = PropertyEntity({ text: "Passar a apresentar cargas quando: ", val: ko.observable(EnumQuandoProcessarMonitoramento.AoIniciarMonitoramento), options: EnumQuandoProcessarMonitoramento.obterOpcoes() });
    this.MetroCubicoPorUnidadePedidoProdutoIntegracao = PropertyEntity({ text: "Metro cúbico por item importação: ", val: ko.observable(false), options: _simNaoBool });
    this.CubagemPorCaixa = PropertyEntity({ text: "Metro cúbico por caixa: ", val: ko.observable(false), options: _simNaoBool });
    this.FormaPreenchimentoCentroResultadoPedido = PropertyEntity({ text: "Forma preenchimento do Centro de Resultado: ", val: ko.observable(EnumFormaPreenchimentoCentroResultadoPedido.TipoOperacao), options: EnumFormaPreenchimentoCentroResultadoPedido.obterOpcoes() });
    this.TipoImpressaoFatura = PropertyEntity({ text: "Tipo Impressão da Fatura: ", val: ko.observable(EnumTipoImpressaoFatura.Padrao), options: EnumTipoImpressaoFatura.obterOpcoes(), def: EnumTipoImpressaoFatura.Padrao });
    this.TipoCalculoPercentualViagem = PropertyEntity({ text: "Tipo de cálculo do percentual de viagem do monitoramento: ", val: ko.observable(EnumTipoCalculoPercentualViagem.DistanciaRotaRestanteAteDestino), options: EnumTipoCalculoPercentualViagem.obterOpcoes(), def: EnumTipoCalculoPercentualViagem.DistanciaRotaRestanteAteDestino });
    this.MonitoramentoStatusViagemTipoRegraParaCalcularPercentualViagem = PropertyEntity({ text: "Tipo de regra do status de viagem do monitoramento para calcular percentual de viagem (passado por): ", val: ko.observable(EnumMonitoramentoStatusViagemTipoRegra.Nenhum), options: EnumMonitoramentoStatusViagemTipoRegra.obterOpcoesCadastroOpcional(), def: EnumMonitoramentoStatusViagemTipoRegra.Nenhum });
    this.QuandoIniciarViagemViaMonitoramento = PropertyEntity({ text: "Quando iniciar viagem via monitoramento: ", val: ko.observable(EnumQuandoIniciarViagemViaMonitoramento.AoChegarNaOrigem), options: EnumQuandoIniciarViagemViaMonitoramento.obterOpcoes(), def: EnumQuandoIniciarViagemViaMonitoramento.AoChegarNaOrigem, visible: ko.observable(true) });
    this.QuandoIniciarMonitoramento = PropertyEntity({ text: "Quando iniciar monitoramento: ", val: ko.observable(EnumQuandoIniciarMonitoramento.AoGerarCarga), options: EnumQuandoIniciarMonitoramento.obterOpcoes(), def: EnumQuandoIniciarMonitoramento.AoGerarCarga, visible: ko.observable(true) });
    this.TipoComprovanteSaida = PropertyEntity({ text: "Tipo de Comprovante de Saída: ", val: ko.observable(EnumTipoComprovanteSaida.ComprovanteSaida), options: EnumTipoComprovanteSaida.obterOpcoes(), def: EnumTipoComprovanteSaida.ComprovanteSaida, visible: ko.observable(true) });
    this.AcaoAoFinalizarMonitoramento = PropertyEntity({ text: "Ação ao finalizar um monitoramento: ", val: ko.observable(EnumAcaoAoFinalizarMonitoramento.Nenhuma), options: EnumAcaoAoFinalizarMonitoramento.obterOpcoes(), def: EnumAcaoAoFinalizarMonitoramento.Nenhuma, visible: ko.observable(true) });
    this.FormatoDataCarregamentoImportacaoPedido = PropertyEntity({ text: "Formato data carregamento na importação: ", val: ko.observable(EnumFormatoData.NaoDefinido), options: EnumFormatoData.obterOpcoes(), def: EnumFormatoData.NaoDefinido, visible: ko.observable(true) });
    this.FormatoHoraCarregamentoImportacaoPedido = PropertyEntity({ text: "Formato hora carregamento na importação: ", val: ko.observable(EnumFormatoHora.NaoDefinido), options: EnumFormatoHora.obterOpcoes(), def: EnumFormatoHora.NaoDefinido, visible: ko.observable(true) });
    this.TipoPagamentoContratoFrete = PropertyEntity({ text: "Tipo de cálculo: ", val: ko.observable(EnumTipoPagamentoContratoFreteTerceiro.SobreFreteCarga), options: EnumTipoPagamentoContratoFreteTerceiro.obterOpcoes(), def: EnumTipoPagamentoContratoFreteTerceiro.SobreFreteCarga, visible: ko.observable(true) });
    this.ModeloEmailAgendamentoPedido = PropertyEntity({ text: "Modelo de e-mail do agendamento de pedido: ", options: EnumModeloEmailAgendamentoPedido.obterOpcoes(), val: ko.observable(EnumModeloEmailAgendamentoPedido.Modelo1), def: EnumModeloEmailAgendamentoPedido.Modelo1 });
    this.LayoutEtiquetaProduto = PropertyEntity({ text: "Layout Etiqueta Produto: ", options: EnumLayoutEtiquetaProduto.obterOpcoes(), val: ko.observable(EnumLayoutEtiquetaProduto.QrCode), def: EnumLayoutEtiquetaProduto.QrCode });
    this.DataReferenciaBusca = PropertyEntity({ text: "Data Referência para Busca: ", val: ko.observable(0), options: EnumDataReferenciaBusca.obterOpcoes(), val: ko.observable(EnumDataReferenciaBusca.DataCarregamentoCarga), def: EnumDataReferenciaBusca.DataCarregamentoCarga });
    this.TamanhoNumerodeCargaAlfanumerico = PropertyEntity({ text: "Gerar número da Carga Alfanumérico: ", options: EnumTamanhoNumerodeCargaAlfanumerico.obterOpcoes(), val: ko.observable(EnumTamanhoNumerodeCargaAlfanumerico.Nenhum), def: EnumTamanhoNumerodeCargaAlfanumerico.Nenhum, enable: ko.observable(false) });
    this.FrequenciaCapturaPosicoesAppTrizy = PropertyEntity({ text: "Frequência de captura de posições via App Trizy: ", options: EnumFrequenciaTrackingAppTrizy.obterOpcoes(), val: ko.observable(EnumFrequenciaTrackingAppTrizy.High), def: EnumFrequenciaTrackingAppTrizy.High, enable: ko.observable(true), visible: ko.observable(true) });
    this.DiaSemanaNotificarPaletesPendentes = PropertyEntity({ text: "Notificar Paletes Pendentes? Dia da Semana: ", val: ko.observable(EnumDiaSemana.Segunda), options: EnumDiaSemana.obterOpcoes() });
    //#endregion Select

    //#region Entidades
    this.GrupoPessoasDocumentosDestinados = PropertyEntity({ text: "Grupo de pessoas documentos destinados:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.ComponenteFreteComplementoFechamento = PropertyEntity({ text: "Componente de frete complemento fechamento:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.ComponenteFreteDescontoSeguro = PropertyEntity({ text: "Componente de frete para desconto de seguro:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.ComponenteFreteDescontoFilial = PropertyEntity({ text: "Componente de frete para desconto por filial:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.RemetentePadraoImportacaoPlanilhaPedido = PropertyEntity({ text: "Remetente padrão importação planilha pedido:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.ClienteContratoAditivo = PropertyEntity({ text: "Cliente do Contrato Aditivo:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.DestinatarioPadraoImportacaoPlanilhaPedido = PropertyEntity({ text: "Destinatário padrão importação planilha pedido:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.ModeloVeicularCargaPadraoImportacaoPedido = PropertyEntity({ text: "Modelo padrão importação pedido:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.FilialPadraoImportacaoPedido = PropertyEntity({ text: "Filial padrão importação pedido:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.TipoOperacaoPadraoCargaDistribuidor = PropertyEntity({ text: "Tipo Operação padrão Carga distribuidor:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.TipoDeOcorrenciaCriacaoPedido = PropertyEntity({ text: "Tipo de ocorrência para a criação do pedido:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.TipoDeOcorrenciaReentrega = PropertyEntity({ text: "Tipo de ocorrência para a Reentrega do pedido:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.TipoDeOcorrenciaRecebimentoMercadoria = PropertyEntity({ text: "Tipo de ocorrência para o recebimento de mercadoria:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.PostoPadrao = PropertyEntity({ text: "Posto Padrão:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.CombustivelPadrao = PropertyEntity({ text: "Combustível Padrão:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.LocalManutencaoPadraoCheckList = PropertyEntity({ text: "Local de Manutenção Padrão pro Check List:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.PlanoContaAdiantamentoFornecedor = PropertyEntity({ text: "Plano de contas de adiantamento de fornecedor:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.PlanoContaAdiantamentoCliente = PropertyEntity({ text: "Plano de contas de adiantamento de cliente:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.TransportadorPadraoContratacao = PropertyEntity({ text: "Transportadora Padrão Contratação:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(false) });
    this.TipoOcorrenciaPadraoPallet = PropertyEntity({ text: "Tipo de Ocorrência Padrão:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(false) });
    this.ModeloEmailNotificacaoAutomaticaTransportador = PropertyEntity({ text: "Modelo email notificação automatica ao tranpostador:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(false) });
    this.UsuarioPadraoParaGeracaoOcorrenciaPorEDI = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Usuário: "), idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(false) });
    //#endregion Entidades

    //#region Decimal
    this.PercentualImpostoFederal = PropertyEntity({ text: "Percentual Imposto Federal:", getType: typesKnockout.decimal, val: ko.observable(0) });
    this.VelocidadeMinimaAceitaDasTecnologias = PropertyEntity({ text: "Velocidade mínima aceita das tecnologias:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true, allowNegative: true }, maxlength: 7, val: ko.observable(0) });
    this.ValorMinimoMonitoramentoCritico = PropertyEntity({ text: "Valor minimo para monitoramento critico automatico:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true, allowNegative: false }, maxlength: 10, val: ko.observable(0) });
    this.VelocidadeMaximaAceitaDasTecnologias = PropertyEntity({ text: "Velocidade máxima aceita das tecnologias:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true, allowNegative: true }, maxlength: 7, val: ko.observable(0) });
    this.TemperaturaMinimaAceitaDasTecnologias = PropertyEntity({ text: "Temperatura mínima aceita das tecnologias:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true, allowNegative: true }, maxlength: 7, val: ko.observable(0) });
    this.TemperaturaMaximaAceitaDasTecnologias = PropertyEntity({ text: "Temperatura máxima aceita das tecnologias:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true, allowNegative: true }, maxlength: 7, val: ko.observable(0) });
    this.PercentualAdiantamentoTerceiroPadrao = PropertyEntity({ text: "% Adiantamento Padrão no Pedido:", getType: typesKnockout.decimal, val: ko.observable(0) });
    this.PercentualMinimoAdiantamentoTerceiroPadrao = PropertyEntity({ text: "% Mínimo Adiantamento no Pedido:", getType: typesKnockout.decimal, val: ko.observable(0) });
    this.PercentualMaximoAdiantamentoTerceiroPadrao = PropertyEntity({ text: "% Máximo Adiantamento no Pedido:", getType: typesKnockout.decimal, val: ko.observable(0) });
    this.ValorMaximoCalculoFrete = PropertyEntity({ text: "Valor Máximo Cálculo Frete:", getType: typesKnockout.decimal, val: ko.observable("") });
    this.PercentualAdiantamentoFreteTerceiros = PropertyEntity({ text: "% Adiantamento Frete Terceiros:", getType: typesKnockout.decimal, val: ko.observable(0) });
    this.LimitePesoDocumentoCarga = PropertyEntity({ text: "Limite do peso no documento da carga", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true, allowNegative: false }, maxlength: 10, val: ko.observable(0) });
    this.LimiteValorDocumentoCarga = PropertyEntity({ text: "Limite do valor no documento da carga", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true, allowNegative: false }, maxlength: 14, val: ko.observable(0) });
    this.LimiteTaraPedidosCarga = PropertyEntity({ text: "Limite da tara nos pedidos da carga", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true, allowNegative: false }, maxlength: 10, val: ko.observable(0) });
    this.ambiente_PesoMaximoIntegracaoCarga = PropertyEntity({ text: "Peso Maximo Integracao Carga:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true, allowNegative: false }, maxlength: 10, val: ko.observable(0) });
    this.FlexibilidadeParaValidacaoNaIAComprovei = PropertyEntity({ text: "Flexibilidade para Validacao na IA Comprovei (De 0,00 até 1,00):", getType: typesKnockout.decimal, configDecimal: { min: 0, max: 1, precision: 2, allowZero: true, allowNegative: false }, maxlength: 4, val: ko.observable(0) });
    this.ValorParaConsiderarComoValido = PropertyEntity({ text: "Valores para considerar como válidos na IA Comprovei (De 0,00 até 1,00):", getType: typesKnockout.decimal, configDecimal: { min: 0, max: 1, precision: 2, allowZero: true, allowNegative: false }, maxlength: 4, val: ko.observable(0) });

    this.ValidacaoNumero = PropertyEntity({ text: "Validação Número:", getType: typesKnockout.string, maxlength: 4, val: ko.observable("") });
    this.ValidacaoAssinatura = PropertyEntity({ text: "Validação Assinatura:", getType: typesKnockout.string, maxlength: 4, val: ko.observable("") });
    this.ValidacaoEncontrouData = PropertyEntity({ text: "Validação Data:", getType: typesKnockout.string, maxlength: 4, val: ko.observable("") });
    this.ValidacaoCanhoto = PropertyEntity({ text: "Validação Canhoto:", getType: typesKnockout.string, maxlength: 4, val: ko.observable("") });

    aplicarValidacaoDecimal(this.ValidacaoNumero);
    aplicarValidacaoDecimal(this.ValidacaoAssinatura);
    aplicarValidacaoDecimal(this.ValidacaoEncontrouData);
    aplicarValidacaoDecimal(this.ValidacaoCanhoto);

    //#endregion Decimal

    // #region Time
    this.HoraGeracaoCargaDeCTEsNaoVinculadosACargas = PropertyEntity({ text: "Horário da geração de cargas de CT-es automaticamente:", val: ko.observable(""), def: "", getType: typesKnockout.time });
    this.PrevisaoEntregaPeriodoUtilHorarioInicial = PropertyEntity({ text: "Horário inicial do período útil de viagem:", val: ko.observable(""), def: "", getType: typesKnockout.time });
    this.PrevisaoEntregaPeriodoUtilHorarioFinal = PropertyEntity({ text: "Horário final do período útil de viagem:", val: ko.observable(""), def: "", getType: typesKnockout.time });
    this.HoraCorteRecalcularPrazoEntregaAposEmissaoDocumentos = PropertyEntity({ text: "Hora de corte para recalcular o prazo de entrega após emissão dos documentos:", val: ko.observable(""), def: "", getType: typesKnockout.time });
    this.HoraFimPadraoEntrega = PropertyEntity({ text: "Hora fim padrão para entrega:", val: ko.observable(""), def: "", getType: typesKnockout.time });
    this.HorarioInicialAlmoco = PropertyEntity({ text: "Horário inicial do período Almoço:", val: ko.observable(""), def: "", getType: typesKnockout.time });
    this.MinutosAguardarGeracaoLotePagamento = PropertyEntity({ text: "Intervalo em horas e minutos:", val: ko.observable(""), getType: typesKnockout.mask, mask: '999:99' });
    this.MinutosAguardarGeracaoLotePagamentoUltimoDiaMes = PropertyEntity({ text: "Intervalo em horas e minutos no último dia do mês:", val: ko.observable(""), getType: typesKnockout.mask, mask: '999:99' });
    //#endregion Time

    // #region Regra para montar o número do pedido no web service
    this.RegraMontarNumeroPedidoEmbarcadorWebService = PropertyEntity({ text: "Regra para Montar o Número do Pedido:", maxlength: 200 });
    this.TagRegraMontarNumeroPedidoEmbarcadorWebServiceAno = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.RegraMontarNumeroPedidoEmbarcadorWebService.id, "#Ano#"); }, type: types.event, text: "Ano" });
    this.TagRegraMontarNumeroPedidoEmbarcadorWebServiceDia = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.RegraMontarNumeroPedidoEmbarcadorWebService.id, "#Dia#"); }, type: types.event, text: "Dia" });
    this.TagRegraMontarNumeroPedidoEmbarcadorWebServiceHoras = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.RegraMontarNumeroPedidoEmbarcadorWebService.id, "#Horas#"); }, type: types.event, text: "Horas" });
    this.TagRegraMontarNumeroPedidoEmbarcadorWebServiceMes = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.RegraMontarNumeroPedidoEmbarcadorWebService.id, "#Mes#"); }, type: types.event, text: "Mês" });
    this.TagRegraMontarNumeroPedidoEmbarcadorWebServiceMinutos = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.RegraMontarNumeroPedidoEmbarcadorWebService.id, "#Minutos#"); }, type: types.event, text: "Minutos" });
    this.TagRegraMontarNumeroPedidoEmbarcadorWebServiceNumeroPedido = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.RegraMontarNumeroPedidoEmbarcadorWebService.id, "#NumeroPedido#"); }, type: types.event, text: "Número do Pedido" });
    this.TagRegraMontarNumeroPedidoEmbarcadorWebServiceSegundos = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.RegraMontarNumeroPedidoEmbarcadorWebService.id, "#Segundos#"); }, type: types.event, text: "Segundos" });
    // #endregion Regra para montar o número do pedido no web service

    //#region Anexo
    this.ArquivoPoliticaPrivacidadeMobile = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Anexo Política de Privacidade pro Mobile:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.EnviarArquivoPoliticaPrivacidadeMobile = PropertyEntity({ eventClick: enviarArquivoPoliticaPrivacidadeMobileClick, type: types.event, text: "Enviar Arquivo", visible: ko.observable(true) });
    //#endregion Anexo

    //#region Contrato de Frete para Terceiros
    this.DiasVencimentoAdiantamentoContratoFreteTerceiro = PropertyEntity({ text: "Dias Vencto. Adiantamento:", getType: typesKnockout.int, visible: ko.observable(true), issue: 0 });
    this.DiasVencimentoSaldoContratoFreteTerceiro = PropertyEntity({ text: "Dias Vencto. Saldo:", getType: typesKnockout.int, visible: ko.observable(true), issue: 0 });
    this.ReterImpostosContratoFreteTerceiro = PropertyEntity({ text: "Reter os impostos no contrato de frete", issue: 0, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.RetencaoPorRaizCNPJ = PropertyEntity({ text: "Retenção por raiz de CNPJ", issue: 0, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.TextoAdicionalContratoFreteTerceiro = PropertyEntity({ text: "Texto adicional para o contrato de frete:", required: false, maxlength: 100000, visible: ko.observable(true), issue: 0 });
    this.ObservacaoContratoFreteTerceiro = PropertyEntity({ text: "Observação para o contrato de frete:", required: false, maxlength: 400, visible: ko.observable(true), issue: 0 });
    this.CalcularAdiantamentoComPedagioContratoFreteTerceiro = PropertyEntity({ text: "O valor total do pedágio deve ser somado ao adiantamento", issue: 0, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.UtilizarTaxaPagamentoContratoFreteTerceiro = PropertyEntity({ text: "Utilizar taxa para pagamento de terceiro no contrato de frete", getType: typesKnockout.bool, val: ko.observable(false) });
    this.TagContratoFreteValorTotal = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.ObservacaoContratoFreteTerceiro.id, "#ValorTotal"); }, type: types.event, text: "Valor Total" });
    this.TagContratoFretePercentualAdiantamento = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.ObservacaoContratoFreteTerceiro.id, "#PercentualAdiantamento"); }, type: types.event, text: "% Adiantamento" });
    this.TagContratoFreteValorAdiantamento = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.ObservacaoContratoFreteTerceiro.id, "#ValorAdiantamento"); }, type: types.event, text: "Valor Adiantamento" });
    this.TagContratoFretePercentualAbastecimento = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.ObservacaoContratoFreteTerceiro.id, "#PercentualAbastecimento"); }, type: types.event, text: "% Abastecimento" });
    this.TagContratoFreteValorAbastecimento = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.ObservacaoContratoFreteTerceiro.id, "#ValorAbastecimento"); }, type: types.event, text: "Valor Abastecimento" });
    this.TagContratoFreteSaldoReceber = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.ObservacaoContratoFreteTerceiro.id, "#SaldoReceber"); }, type: types.event, text: "Saldo à Receber" });
    this.TagContratoFreteVencimentoAdiantamento = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.ObservacaoContratoFreteTerceiro.id, "#VencimentoAdiantamento"); }, type: types.event, text: "Venc. Adiantamento" });
    this.TagContratoFreteVencimentoSaldo = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.ObservacaoContratoFreteTerceiro.id, "#VencimentoSaldo"); }, type: types.event, text: "Venc. Saldo" });
    this.TagContratoFreteOperadoraValePedagio = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.ObservacaoContratoFreteTerceiro.id, "#OperadoraValePedagio"); }, type: types.event, text: "Operadora Vale Pedágio" });
    this.TagContratoFreteCartaoAbastecimento = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.ObservacaoContratoFreteTerceiro.id, "#CartaoAbastecimento"); }, type: types.event, text: "Cartão Abastecimento" });
    //#endregion Contrato de Frete para Terceiros

    //#region comissao de motorista
    this.UtilizaControlePercentualExecucao = PropertyEntity({ text: "Utiliza controle de % de execução", getType: typesKnockout.bool, val: ko.observable(false) });
    this.BloquearAlteracaoValorFreteLiquido = PropertyEntity({ text: "Bloquear alteração do frete líquido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PercentualComissaoMotorista = PropertyEntity({ text: "Percentual", getType: typesKnockout.decimal, val: ko.observable(0), visible: ko.observable(true) });
    this.PercentualBasecalculoComissaoMotorista = PropertyEntity({ text: "Percentual da base de cálculo", getType: typesKnockout.decimal, val: ko.observable(0), visible: ko.observable(true) });
    this.DataBaseComissaoMotorista = PropertyEntity({ text: "Data Base: ", val: ko.observable(EnumDataBaseComissaoMotorista.DataEmissao), options: EnumDataBaseComissaoMotorista.obterOpcoes(), def: EnumDataBaseComissaoMotorista.DataEmissao, visible: ko.observable(true) });
    //#endregion Comissao de motorista

    //#region Infracoes
    this.FormulaInfracaoPadrao = PropertyEntity({ text: "Infrações: *Informe os complementos para configuração de infração ", required: false, maxlength: 3000, visible: ko.observable(true), def: ko.observable(""), val: ko.observable(""), enable: ko.observable(true) });

    //#region bidding
    this.CalcularKMMedioRotaPorOrigemDestino = PropertyEntity({ text: "Calcular o KM médio da Rota por Origem/Destino", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteAdicionarRotaSemInformarKMMedio = PropertyEntity({ text: "Permite Adicionar Rota sem informar o KM Médio", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteSelecionarMaisDeUmaOfertaPorBidding = PropertyEntity({ text: "Permite selecionar mais de uma Oferta por Bidding", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteRemoverObrigatoriedadeDatas = PropertyEntity({ text: "Permite remover obrigatoriedade de datas", getType: typesKnockout.bool, val: ko.observable(false) });
    this.TransportadorUtilizaProcessoAutomatizadoAvancoEtapasBidding = PropertyEntity({ text: "Transportador utiliza processo automatizado de avanço de etapas no bidding", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteOfertarQuandoAceitacaoIndForMenorCemPorcento = PropertyEntity({ text: "Permite ofertar quando Aceitação Ind. for menor que 100%", getType: typesKnockout.bool, val: ko.observable(false) });

    //#region abastecimento
    this.GerarRequisicaoAutomaticaParaVeiculosVinculados = PropertyEntity({ text: "Gerar requisição automática para veículos vinculados", getType: typesKnockout.bool, val: ko.observable(false) });
    this.MotivoCompraAbastecimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Selecionar motivo para geração de ordem de compra:", idBtnSearch: guid() });
    this.UtilizarCustoMedioParaLancamentoAbastecimentos = PropertyEntity({ text: "Utilizar o custo médio para o lançamento dos abastecimentos", getType: typesKnockout.bool, val: ko.observable(false) });

    this.TagMotoristaInfracao = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.FormulaInfracaoPadrao.id, "#MotoristaInfracao"); }, type: types.event, text: "Motorista" });
    this.TagCpfInfracao = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.FormulaInfracaoPadrao.id, "#CpfInfracao"); }, type: types.event, text: "CPF" });
    this.TagCnhInfracao = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.FormulaInfracaoPadrao.id, "#CnhInfracao"); }, type: types.event, text: "CNH" });
    this.TagDataEmissaoInfracao = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.FormulaInfracaoPadrao.id, "#DataEmissaoInfracao "); }, type: types.event, text: "Data da Emissão" });
    this.TagDataInfracao = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.FormulaInfracaoPadrao.id, "#DataInfracao"); }, type: types.event, text: "Data da Infração" });
    this.TagNumeroInfracao = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.FormulaInfracaoPadrao.id, "#NumeroInfracao"); }, type: types.event, text: "N° da Infração" });
    this.TagNumeroAutuacao = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.FormulaInfracaoPadrao.id, "#NumeroAutuacao "); }, type: types.event, text: "N° da Autuação" });
    this.TagTipoInfracao = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.FormulaInfracaoPadrao.id, "#TipoInfracao"); }, type: types.event, text: "Tipo da Infração" });
    this.TagLocalInfracao = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.FormulaInfracaoPadrao.id, "#LocalInfracao"); }, type: types.event, text: "Local da Infração" });
    this.TagCidadeInfracao = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.FormulaInfracaoPadrao.id, "#CidadeInfracao"); }, type: types.event, text: "Cidade da Infração" });
    this.TagVeiculoInfracao = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.FormulaInfracaoPadrao.id, "#VeiculoInfracao"); }, type: types.event, text: "Veículo da Infração" });
    this.TagObservacaoInfracao = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoEmbarcador.FormulaInfracaoPadrao.id, "#ObservacaoInfracao"); }, type: types.event, text: "Observação da Infração" });

    // #endregion Infracoes

    // #region Configuração Mongo

    this.BancoMongo = PropertyEntity({ text: "Banco:", val: ko.observable("") });
    this.ServidorMongo = PropertyEntity({ text: "Servidor:", val: ko.observable("") });
    this.UsaTlsMongo = PropertyEntity({ getType: typesKnockout.bool, text: "Usar SSL", val: ko.observable("") });
    this.UtilizaCosmosDbMongo = PropertyEntity({ getType: typesKnockout.bool, text: "Utilizar Cosmos DB", val: ko.observable("") });
    this.TimeoutMongo = PropertyEntity({ getType: typesKnockout.int, def: 0, getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, text: "Timeout:", val: ko.observable("") });
    this.PortaMongo = PropertyEntity({ getType: typesKnockout.int, getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, text: "Porta:", val: ko.observable("") });
    this.UsuarioHangfireMongo = PropertyEntity({ text: "Usuário Hangfire:", val: ko.observable("") });
    this.SenhaHangfireMongo = PropertyEntity({ text: "Senha Hangfire:", val: ko.observable("") });
    this.SenhaMongo = PropertyEntity({ text: "Senha Mongo:", val: ko.observable("") });
    this.UsuarioMongo = PropertyEntity({ text: "Usuário Mongo:", val: ko.observable("") });

    // #endregion Configuração Mongo

    // #region Configuração SSO

    this.AtivoSSo = PropertyEntity({ getType: typesKnockout.bool, text: "Ativa", val: ko.observable("") });
    this.TipoSSo = PropertyEntity({ text: "Tipo SSO: ", val: ko.observable(1), options: _tipoSSo });
    this.DisplaySSo = PropertyEntity({ text: "Display:", val: ko.observable(""), maxlength: 50 });
    this.ClientIdSSo = PropertyEntity({ text: "Client Id:", val: ko.observable(""), maxlength: 200 });
    this.ClientSecretSSo = PropertyEntity({ text: "Client Secret:", val: ko.observable(""), maxlength: 200, visible: ko.observable(true) });
    this.UrlAutenticacaoSSo = PropertyEntity({ text: "Url Autenticacao:", val: ko.observable(""), maxlength: 200 });
    this.UrlAccessTokenSSo = PropertyEntity({ text: "Access Token:", val: ko.observable(""), maxlength: 200, visible: ko.observable(true) });
    this.UrlRefreshTokenSSo = PropertyEntity({ text: "Refresh Token:", val: ko.observable(""), maxlength: 200, visible: ko.observable(true) });
    this.UrlRevokeTokenSSo = PropertyEntity({ text: "Revoke Token:", val: ko.observable(""), maxlength: 200, visible: ko.observable(true) });
    this.UrlDominioSSo = PropertyEntity({ text: "URL Sistema:", val: ko.observable(""), maxlength: 200, visible: ko.observable(false) });

    this.ArquivoCertificadoSSo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Anexo Certificado SSO ou Xml Federation Metadata:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(false) });
    this.EnviarArquivoCertificadoSSo = PropertyEntity({ eventClick: enviarArquivoCertificadoSSoClick, type: types.event, text: "Enviar Arquivo", visible: ko.observable(false) });

    this.TipoSSo.val.subscribe(function (newValue) {
        if (_configuracaoEmbarcador) {
            var oauth2 = (newValue == 1)
            _configuracaoEmbarcador.ClientSecretSSo.visible(oauth2);
            _configuracaoEmbarcador.UrlAccessTokenSSo.visible(oauth2);
            _configuracaoEmbarcador.UrlRefreshTokenSSo.visible(oauth2);
            _configuracaoEmbarcador.UrlRevokeTokenSSo.visible(oauth2);

            var saml2 = (newValue == 2);
            _configuracaoEmbarcador.ArquivoCertificadoSSo.visible(saml2);
            _configuracaoEmbarcador.EnviarArquivoCertificadoSSo.visible(saml2);
            _configuracaoEmbarcador.UrlDominioSSo.visible(saml2);
        }
    });

    // #endregion Configuração SSO

    // #region CIOT
    this.GerarCIOTParaTodasAsCargas = PropertyEntity({ text: "Gerar CIOT para todas as cargas", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarExisteCargaMesmoNumeroCIOT = PropertyEntity({ text: "Validar se já existe carga com o mesmo número de CIOT", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ExigirConfiguracaoTerceiroParaGerarCIOTParaTodos = PropertyEntity({ text: "Exigir configuração no Terceiro para gerar CIOT quando for CIOT para Todos", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RemoverNumeroCIOTEncerrado = PropertyEntity({ text: "Remover o número do CIOT do cadastro do Veículo quando CIOT Encerrado", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.CancelarCIOTAutomaticamenteFluxoCancelamentoCarga = PropertyEntity({ text: "Cancelar CIOT automaticamente no fluxo de cancelamento de cargas", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.SalvarTransportadorTerceiroComoGerarCIOT = PropertyEntity({ text: "Salvar Transportador terceiro para gerar CIOT (Integração)", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoPermitirEncerrarCIOTEncerrarCarga = PropertyEntity({ text: "Não permitir encerrar CIOT ao encerrar a carga", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirAutorizarPagamentoCIOTComCanhotosRecebidos = PropertyEntity({ text: "Permitir autorizar o pagamento do CIOT apenas com canhotos recebidos fisicamente e digitalizados", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.GerarCIOTMarcadoAoCadastrarTransportadorTerceiro = PropertyEntity({ text: "Gerar CIOT Marcado ao cadastrar Transportador terceiro", getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.TipoGeracaoCIOT = PropertyEntity({ text: "Tipo de CIOT", options: EnumTipoGeracaoCIOT.ObterOpcoes(), val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });
    this.TipoFavorecidoCIOT = PropertyEntity({ text: "Tipo do Favorecido", options: EnumTipoFavorecidoCIOT.ObterOpcoes(), val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });
    this.TipoQuitacaoCIOT = PropertyEntity({ text: "Tipo de Quitação CIOT", options: EnumTipoQuitacaoCIOT.ObterOpcoes(), val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });
    this.TipoAdiantamentoCIOT = PropertyEntity({ text: "Tipo de Adiantamento CIOT", options: EnumTipoQuitacaoCIOT.ObterOpcoes(), val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });

    this.TiposPagamentoCIOTOperadora = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), text: "", idGrid: guid() });

    this.TipoPagamentoCIOTOperadora = PropertyEntity({ text: "Tipo do Pagamento", options: ko.observable(EnumTipoPagamentoCIOT.ObterOpcoes()), val: ko.observable(EnumTipoPagamentoCIOT.SemPgto), def: "", issue: 0, visible: ko.observable(true), codEntity: ko.observable("") });
    this.OperadoraTipoPagamentoCIOTOperadora = PropertyEntity({ text: "Operadora de CIOT", options: EnumOperadoraCIOT.ObterOpcoes(), val: ko.observable(EnumOperadoraCIOT.eFrete), def: EnumOperadoraCIOT.eFrete, visible: ko.observable(true), codEntity: ko.observable("") });

    this.OperadoraTipoPagamentoCIOTOperadora.val.subscribe(function (operadora) {
        _configuracaoEmbarcador.TipoPagamentoCIOTOperadora.options(EnumTipoPagamentoCIOT.obterOpcoesPorOperadora(operadora));

    });

    this.AdicionarTipoPagamentoCIOT = PropertyEntity({ eventClick: adicionarTipoPagamentoCIOTClick, type: types.event, text: "Adicionar Tipo de Pagamento CIOT", visible: ko.observable(true) });
    // #endregion CIOT

    // #region Cotacao

    this.GravarNumeroCotacaoObservacaoInternaAoCriarPedido = PropertyEntity({ text: "Gravar número da cotação na observação interna ao criar Pedido", getType: typesKnockout.bool, val: ko.observable(false) });

    // #endregion Cotacao

    // #region Configuração Agendamento Entrega
    this.VisualizarTelaDeAgendamentoPorEntrega = PropertyEntity({ text: "Visualizar Tela de Agendamento por Entrega", getType: typesKnockout.bool, val: ko.observable(false) });
    // #endregion Configuração Agendamento Entrega

    // #region Configurações Geração de Atendimento Automático Divergência de valores CTe Emitidos Embarcador
    this.GerarAtendimentoDivergenciaValoresCTeEmitidoEmbarcador = PropertyEntity({ text: "Gerar atendimento divergência de valores para CT-es emitidos no embarcador X tabela de frete", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(false) });
    this.MotivoAtendimentoDivergenciaValoresCTeEmitidoEmbarcador = PropertyEntity({ text: "Motivo atendimento divergência de valores para CT-es emitidos no embarcador X tabela de frete", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(false) });
    // #endregion Configurações Geração de Atendimento Automático Divergência de valores CTe Emitidos Embarcador
    this.UsarApiDeConexaoComGeradorExcelKMM = PropertyEntity({ text: "Usar API de conexão com o gerador de Excel KMM", getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirBaixarArquivosConembOcorenManualmente = PropertyEntity({ text: "Permitir baixar arquivos CONEMB e OCOREN manualmente", getType: typesKnockout.bool, val: ko.observable(false) });
    this.HabilitarLayoutFaturaNFSManual = PropertyEntity({ text: "Habilitar layout da fatura para NFS Manual", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });

    // #region Configurações de paginação
    this.GridConfiguracoesPaginacaoInterfaces = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), text: "", idGrid: guid() });

    this.ConfiguracoesPaginacaoInterfaces = PropertyEntity({ text: "*Interfaces:", options: ko.observable(EnumConfiguracaoPaginacaoInterfaces.ObterOpcoes()), val: ko.observable(EnumConfiguracaoPaginacaoInterfaces.Cargas_Carga), def: EnumOperadoraCIOT.eFrete, issue: 0, visible: ko.observable(true), codEntity: ko.observable("") });
    this.ConfiguracoesPaginacaoDias = PropertyEntity({ text: "*Consultar registros a partir de:", getType: typesKnockout.int, getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, val: ko.observable("Quantidade de dias"), visible: ko.observable(true) });

    this.AdicionarConfiguracaoPaginacao = PropertyEntity({ eventClick: adicionarConfiguracaoPaginacaoClick, type: types.event, text: "Adicionar configuração de paginação", visible: ko.observable(true) });



    // #endregion Configurações de paginação

    // #region Funções Subscribe

    this.EfetuarCancelamentoDePagamentoAoCancelarCarga.val.subscribe((valor) => {
        _configuracaoEmbarcador.MotivoCancelamentoPagamentoPadrao.required(valor);
    });

    this.LiquidarPalletAutomaticamente.val.subscribe((valor) => {
        _configuracaoEmbarcador.TipoOcorrenciaPadraoPallet.required(valor);
        _configuracaoEmbarcador.QteDiasParaLiquidarPallet.required(valor);
    });

    this.HabilitarAlertaMotorista.val.subscribe((valor) => {
        _configuracaoEmbarcador.MinutosNotificarMotoristaAlertaViagem.required(valor);
    });

    this.ExisteTransportadorPadraoContratacao.val.subscribe(function (val) {
        _configuracaoEmbarcador.TransportadorPadraoContratacao.visible(val);
    });

    this.ReterImpostosContratoFreteTerceiro.val.subscribe((valor) => {
        _configuracaoEmbarcador.RetencaoPorRaizCNPJ.visible(valor);
    });

    this.NumeroCargaSequencialUnico.val.subscribe(function (valor) {
        if (!valor)
            _configuracaoEmbarcador.UtilizarNumeroSequencialCargaNoCarregamento.val(false);

        _configuracaoEmbarcador.UtilizarNumeroSequencialCargaNoCarregamento.visible(valor);
    });

    this.NaoInutilizarCTEsFiscalmenteApenasGerencialmente.val.subscribe((valor) => {
        if (valor) {
            _configuracaoEmbarcador.NaoReutilizarNumeracaoAposAnularGerencialmente.visible(true);
        }
        else {
            _configuracaoEmbarcador.NaoReutilizarNumeracaoAposAnularGerencialmente.val(false);
            _configuracaoEmbarcador.NaoReutilizarNumeracaoAposAnularGerencialmente.visible(false);
        }
    });

    this.HabilitaIntervaloTempoLiberaDocumentoEmitidoEscrituracao.val.subscribe((visible) => visible ? $("#container-configuracao-intervalo-tempo-libera-documento-emitido-escrituracao").show() : $("#container-configuracao-intervalo-tempo-libera-documento-emitido-escrituracao").hide())

    this.GerarNumerodeCargaAlfanumerico.val.subscribe(function () { ClickGerarNumerodeCargaAlfanumerico(); });
    this.NaoProcessarTrocaAlvoViaMonitoramento.val.subscribe(function (val) { ClickNaoProcessarTrocaAlvoViaMonitoramento(val); });
    this.IdentificarMonitoramentoStatusViagemEmTransito.val.subscribe(function (val) { ClickIdentificarMonitoramentoStatusViagemEmTransito(val); });
    this.IdentificarVeiculoParado.val.subscribe(function (val) { ClickIdentificarVeiculoParado(val); });
    this.UsarGrupoDeTipoDeOperacaoNoMonitoramento.val.subscribe(function (val) { ClickUsarGrupoDeTipoDeOperacaoNoMonitoramento(val); });
    this.UtilizarNumeroTentativasTempoIntervaloIntegracaoOcorrenciaPersonalizado.val.subscribe(function (novoValor) {
        if (novoValor) {
            _configuracaoEmbarcador.NumeroTentativasIntegracao.val("");
            _configuracaoEmbarcador.IntervaloMinutosEntreIntegracoes.val("");
            _configuracaoEmbarcador.NumeroTentativasIntegracao.visible(true);
            _configuracaoEmbarcador.NumeroTentativasIntegracao.required(true);
            _configuracaoEmbarcador.IntervaloMinutosEntreIntegracoes.visible(true);
            _configuracaoEmbarcador.IntervaloMinutosEntreIntegracoes.required(true);
        } else {
            _configuracaoEmbarcador.NumeroTentativasIntegracao.visible(false);
            _configuracaoEmbarcador.NumeroTentativasIntegracao.required(false);
            _configuracaoEmbarcador.IntervaloMinutosEntreIntegracoes.visible(false);
            _configuracaoEmbarcador.IntervaloMinutosEntreIntegracoes.required(false);
        }
    });

    this.FinalizarAutomaticamenteAlertasDoMonitoramentoPeriodicamente.val.subscribe(function (val) {
        _configuracaoEmbarcador.DiasParaFinalizarAutomaticamenteAlertasDoMonitoramentoPeriodicamente.required(val);
    })

    this.FinalizarAutomaticamenteMonitoramentosEmAndamento.val.subscribe(function (val) {
        _configuracaoEmbarcador.DiasFinalizarAutomaticamenteMonitoramentoEmAndamento.required(val);
    })

    this.FinalizarAutomaticamenteMonitoramentosPrevisaoUltimaEntrega.val.subscribe(function (val) {
        _configuracaoEmbarcador.DiasFinalizarMonitoramentoPrevisaoUltimaEntrega.required(val);
    })

    this.FlexibilidadeParaValidacaoNaIAComprovei.val.subscribe((valor) => {
        let novoValor = parseFloat(valor.replace(',', '.'));
        if (novoValor > 1) novoValor = 1;
        else if (novoValor < 0) novoValor = 0;

        _configuracaoEmbarcador.FlexibilidadeParaValidacaoNaIAComprovei.val(novoValor.toFixed(2).replace('.', ','));
    });

    this.ValorParaConsiderarComoValido.val.subscribe((valor) => {
        let novoValor = parseFloat(valor.replace(',', '.'));
        if (novoValor > 1) novoValor = 1;
        else if (novoValor < 0) novoValor = 0;

        _configuracaoEmbarcador.ValorParaConsiderarComoValido.val(novoValor.toFixed(2).replace('.', ','));
    });

    this.EnviarEmailDeNotificacaoAutomaticamenteAoTransportadorDaCarga.val.subscribe((valor) => {
        _configuracaoEmbarcador.ModeloEmailNotificacaoAutomaticaTransportador.required(valor);
    });

    this.UtilizaUsuarioPadraoParaGeracaoOcorrenciaPorEDI.val.subscribe(function (novoValor) {
        if (novoValor) {
            _configuracaoEmbarcador.UsuarioPadraoParaGeracaoOcorrenciaPorEDI.required(true);
            _configuracaoEmbarcador.UsuarioPadraoParaGeracaoOcorrenciaPorEDI.text("*Usuário: ");
        } else {
            _configuracaoEmbarcador.UsuarioPadraoParaGeracaoOcorrenciaPorEDI.required(false);
            _configuracaoEmbarcador.UsuarioPadraoParaGeracaoOcorrenciaPorEDI.text("Usuário: ");
            _configuracaoEmbarcador.UsuarioPadraoParaGeracaoOcorrenciaPorEDI.codEntity(0);
            _configuracaoEmbarcador.UsuarioPadraoParaGeracaoOcorrenciaPorEDI.val("");
        }
    });
    this.PermiteOfertarQuandoAceitacaoIndForMenorCemPorcento.val.subscribe(function (novoValor) {
        if (novoValor) {
            _configuracaoEmbarcador.InformePorcentagemAceitacaoInd.required(true);
            _configuracaoEmbarcador.InformePorcentagemAceitacaoInd.text("*Informe a % de Aceitação Ind.:");
            _configuracaoEmbarcador.InformePorcentagemAceitacaoInd.visible(true);
        } else {
            _configuracaoEmbarcador.InformePorcentagemAceitacaoInd.required(false);
            _configuracaoEmbarcador.InformePorcentagemAceitacaoInd.text("Informe a % de Aceitação Ind.:");
            _configuracaoEmbarcador.InformePorcentagemAceitacaoInd.visible(false);
            _configuracaoEmbarcador.InformePorcentagemAceitacaoInd.codEntity(0);
            _configuracaoEmbarcador.InformePorcentagemAceitacaoInd.val("");
        }
    });

    this.NaoEnviarXMLCTEPorEmailParaTipoServico.val.subscribe(function (novoValor) {
        if (novoValor) {
            _configuracaoEmbarcador.TipoServicoCTeEmail.visible(novoValor);
            _configuracaoEmbarcador.TipoServicoCTeEmail.required(novoValor);
        } else {
            _configuracaoEmbarcador.TipoServicoCTeEmail.visible(false);
            _configuracaoEmbarcador.TipoServicoCTeEmail.val(new Array());
        }
    });

    this.NaoCancelarCargaAoAplicarStatusFinalizadorJanelaDescarregamento.val.subscribe(function (novoValor) {
        _configuracaoEmbarcador.TempoPermitirReagendamentoHoras.visible(novoValor);
    });

    this.GerarIntegracaoContabilizacaoCtesApos.val.subscribe(function (valor) {
        _configuracaoEmbarcador.DelayIntegracaoContabilizacaoCtes.visible(false);
        _configuracaoEmbarcador.DelayIntegracaoContabilizacaoCtes.required(false);
        if (valor) {
            _configuracaoEmbarcador.DelayIntegracaoContabilizacaoCtes.visible(true);
            _configuracaoEmbarcador.DelayIntegracaoContabilizacaoCtes.required(true);
        }
    });

    this.ambiente_PermitirInformarCapacidadeMaximaParaUploadArquivos.val.subscribe(function (novoValor) {
        if (novoValor) {
            _configuracaoEmbarcador.ambiente_CapacidadeMaximaParaUploadArquivos.required(true);
            _configuracaoEmbarcador.ambiente_CapacidadeMaximaParaUploadArquivos.visible(true);
            _configuracaoEmbarcador.ambiente_CapacidadeMaximaParaUploadArquivos.text("*Capacidade máxima para upload de arquivos:");
        } else {
            _configuracaoEmbarcador.ambiente_CapacidadeMaximaParaUploadArquivos.required(false);
            _configuracaoEmbarcador.ambiente_CapacidadeMaximaParaUploadArquivos.visible(false);
            _configuracaoEmbarcador.ambiente_CapacidadeMaximaParaUploadArquivos.val(0);
        }
    });

    this.AvisarDivergenciaValoresCTeEmitidoEmbarcador.val.subscribe(function (val) {
        _configuracaoEmbarcador.GerarAtendimentoDivergenciaValoresCTeEmitidoEmbarcador.visible(val);
    });

    this.GerarAtendimentoDivergenciaValoresCTeEmitidoEmbarcador.val.subscribe(function (val) {
        limpaCampoMotivoAtendimentoDivergenciaValoresCTeEmitidoEmbarcador();
        _configuracaoEmbarcador.MotivoAtendimentoDivergenciaValoresCTeEmitidoEmbarcador.visible(val);
    });

    this.HabilitarAcessoTodosClientes.val.subscribe(function (val) {
        _configuracaoEmbarcador.SenhaPadraoAcessoPortal.required(val);
        _configuracaoEmbarcador.SenhaPadraoAcessoPortal.visible(val);
    });

    this.HabilitarSpotCargaAposLimiteHoras.val.subscribe(function (val) {
        clarity("set", "HabilitarSpotCargaAposLimiteHoras", `${val}`);
    });

    //#endregion Funções Subscribe
};

var CRUDConfiguracaoEmbarcador = function () {
    this.Salvar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Salvar", icon: "fa fa-save", visible: ko.observable(true) });
    this.ProcessarReceitasDespesasVeiculo = PropertyEntity({ eventClick: ProcessarReceitasDespesasVeiculosClick, type: types.event, text: "Processar Receitas e Despesas Veículos", icon: "fa fa-truck", visible: ko.observable(true) });
    this.GerarCanhotosRetroativos = PropertyEntity({ eventClick: GerarCanhotosRetroativosClick, type: types.event, text: "Processar Canhotos Retroativos", icon: "fa fa-truck", visible: ko.observable(true) });
    this.ReciclarIntegradores = PropertyEntity({ eventClick: ReciclarIntegradoresClick, type: types.event, text: "Reiniciar Integradores", icon: "fa fa-refresh", visible: ko.observable(false) });
    this.ConfiguracaoLogs = PropertyEntity({ eventClick: ConfiguracaoLogClick, type: types.event, text: "Configurações Log", icon: "fa fa-truck", visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadConfiguracao() {
    _configuracaoEmbarcador = new ConfiguracaoEmbarcador();
    KoBindings(_configuracaoEmbarcador, "knockoutConfiguracaoEmbarcador");

    _CRUDConfiguracaoEmbarcador = new CRUDConfiguracaoEmbarcador();
    KoBindings(_CRUDConfiguracaoEmbarcador, "knockoutCRUDConfiguracaoEmbarcador");

    HeaderAuditoria("ConfiguracaoTMS", _configuracaoEmbarcador);

    BuscarGruposPessoas(_configuracaoEmbarcador.GrupoPessoasDocumentosDestinados);
    BuscarComponentesDeFrete(_configuracaoEmbarcador.ComponenteFreteComplementoFechamento);
    BuscarComponentesDeFrete(_configuracaoEmbarcador.ComponenteFreteDescontoSeguro);
    BuscarComponentesDeFrete(_configuracaoEmbarcador.ComponenteFreteDescontoFilial);
    BuscarClientes(_configuracaoEmbarcador.RemetentePadraoImportacaoPlanilhaPedido);
    BuscarClientes(_configuracaoEmbarcador.DestinatarioPadraoImportacaoPlanilhaPedido);
    BuscarClientes(_configuracaoEmbarcador.ClienteContratoAditivo);
    BuscarModelosVeicularesCarga(_configuracaoEmbarcador.ModeloVeicularCargaPadraoImportacaoPedido);
    BuscarFilial(_configuracaoEmbarcador.FilialPadraoImportacaoPedido);
    BuscarTiposOperacao(_configuracaoEmbarcador.TipoOperacaoPadraoCargaDistribuidor);
    BuscarTipoOcorrencia(_configuracaoEmbarcador.TipoDeOcorrenciaRecebimentoMercadoria);
    BuscarTipoOcorrencia(_configuracaoEmbarcador.TipoDeOcorrenciaCriacaoPedido);
    BuscarTipoOcorrencia(_configuracaoEmbarcador.TipoDeOcorrenciaReentrega);
    BuscarClientes(_configuracaoEmbarcador.PostoPadrao);
    BuscarClientes(_configuracaoEmbarcador.Despachante);
    BuscarProdutoTMS(_configuracaoEmbarcador.CombustivelPadrao);
    BuscarClientes(_configuracaoEmbarcador.LocalManutencaoPadraoCheckList);
    BuscarPlanoConta(_configuracaoEmbarcador.PlanoContaAdiantamentoCliente, "Selecione a Conta Analítica", "Contas Analíticas", null, EnumAnaliticoSintetico.Analitico);
    BuscarPlanoConta(_configuracaoEmbarcador.PlanoContaAdiantamentoFornecedor, "Selecione a Conta Analítica", "Contas Analíticas", null, EnumAnaliticoSintetico.Analitico);
    BuscarEmpresa(_configuracaoEmbarcador.TransportadorPadraoContratacao);
    BuscarTipoOcorrencia(_configuracaoEmbarcador.TipoOcorrenciaPadraoPallet);
    BuscarMotivoCancelamentoPagamento(_configuracaoEmbarcador.MotivoCancelamentoPagamentoPadrao);
    BuscarConfiguracaoModeloEmail(_configuracaoEmbarcador.ModeloEmailNotificacaoAutomaticaTransportador);
    BuscarFuncionario(_configuracaoEmbarcador.UsuarioPadraoParaGeracaoOcorrenciaPorEDI);
    BuscarMotivoCompra(_configuracaoEmbarcador.MotivoCompraAbastecimento);

    loadTransportadorTerceiro();
    loadConfiguracaoMotorista();
    loadConfiguracaoVeiculo();
    loadConfiguracaoMercosul();
    loadConfiguracaoIntervaloTempoLiberaDocumentoEmitidoEscrituracao();
    loadConfiguracaoMinutosAguardarGeracaoLotePagamento();
    loadConfiguracaoAtendimentoAutomatico();
    loadConfiguracaoPaginacao();
    buscarConfiguracao();
    LoadConfiguracaoLogs();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function loadConfiguracaoMinutosAguardarGeracaoLotePagamento() {
    $("#" + _configuracaoEmbarcador.MinutosAguardarGeracaoLotePagamento.id).on('blur', function () {
        minutosGeracaoLotePagamentoBlur($(this));
    });

    $("#" + _configuracaoEmbarcador.MinutosAguardarGeracaoLotePagamentoUltimoDiaMes.id).on('blur', function () {
        minutosGeracaoLotePagamentoBlur($(this));
    });
}

function loadConfiguracaoAtendimentoAutomatico() {
    BuscarMotivoChamado(_configuracaoEmbarcador.MotivoAtendimentoDivergenciaValoresCTeEmitidoEmbarcador);
}

function minutosGeracaoLotePagamentoBlur(campo) {
    var value = campo.val();
    var parts = value.split(':');
    var hours = parseInt(parts[0], 10);
    var minutes = parseInt(parts[1], 10);

    if (hours < 0 || minutes >= 60 || value.length == 1) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Informe um intervalo válido no formato HH:mm.");
        campo.val('');
    }
}

function atualizarClick() {
    if (ValidarCamposObrigatorios(_configuracaoEmbarcador)) {
        requestAtualizar(true);
    }
}

function GerarCanhotosRetroativosClick(e, sender) {
    executarReST("Configuracao/GerarCanhotosRetroativos", null, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data)
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Canhotos processado com sucesso");
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function ReciclarIntegradoresClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja reiniciar os integradores?", function () {
        executarReST("Configuracao/ReiniciarIntegradores", null, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data)
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Integradores Reiniciados");
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function ProcessarReceitasDespesasVeiculosClick() {
    executarReST("Configuracao/ProcessarReceitasDespesasVeiculo", obterConfiguracaoSalvar(), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data)
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function ConfiguracaoLogClick() {
    AbrirModalConfiguracaoLogs();
}

function enviarArquivoCertificadoSSoClick() {
    var arquivo = document.getElementById(_configuracaoEmbarcador.ArquivoCertificadoSSo.id);

    if (arquivo.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");

    arquivo = arquivo.files[0];
    var formData = new FormData();
    formData.append("upload", arquivo);

    enviarArquivo("Configuracao/EnviarArquivoCertificadoSSo?callback=?", null, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Certificado de autenticação SSo enviado com sucesso!");
                _configuracaoEmbarcador.ArquivoPoliticaPrivacidadeMobile.val("");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function enviarArquivoPoliticaPrivacidadeMobileClick() {
    var arquivo = document.getElementById(_configuracaoEmbarcador.ArquivoPoliticaPrivacidadeMobile.id);

    if (arquivo.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");

    arquivo = arquivo.files[0];
    var formData = new FormData();
    formData.append("upload", arquivo);

    enviarArquivo("Configuracao/EnviarArquivoPoliticaPrivacidadeMobile?callback=?", null, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Documento enviado com sucesso");
                _configuracaoEmbarcador.ArquivoPoliticaPrivacidadeMobile.val("");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

/*
 * Declaração das Funções
 */

function requestAtualizar(mostrarConfirmacao) {
    let requestObj = obterConfiguracaoSalvar();
    requestObj["ExibirConfirmacao"] = mostrarConfirmacao;
    executarReST("Configuracao/Atualizar", requestObj, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (retorno.Data.msg) {
                    exibirConfirmacao("Confirmação", retorno.Data.msg, () => requestAtualizar(false));
                }
                else
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}


function buscarConfiguracao() {
    executarReST("Configuracao/ObterConfiguracao", {}, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                var dados = retorno.Data;
                var objetos = [
                    null,
                    'ConfiguracaoCargaEmissaoDocumento',
                    'ConfiguracaoContratoFreteTerceiro',
                    'ConfiguracaoFatura',
                    'ConfiguracaoPedido',
                    'ConfiguracaoOcorrencia',
                    'ConfiguracaoWebService',
                    'ConfiguracaoTabelaFrete',
                    'ConfiguracaoJanelaCarregamento',
                    'ConfiguracaoDocumentoEntrada',
                    'ConfiguracaoCargaDadosTransporte',
                    'ConfiguracaoMobile',
                    'ConfiguracaoAcertoViagem',
                    'ConfiguracaoAprovacao',
                    'ConfiguracaoAgendamentoColeta',
                    'ConfiguracaoCargaIntegracao',
                    'ConfiguracaoAbaVeiculo',
                    'ConfiguracaoAbaMotorista',
                    'ConfiguracaoGeralCarga',
                    'ConfiguracaoEncerramentoMDFeAutomatico',
                    'ConfiguracaoFinanceiro',
                    'ConfiguracaoChamado',
                    'ConfiguracaoControleEntrega',
                    'ConfiguracaoCargaCalculoFrete',
                    'ConfiguracaoMontagemCarga',
                    'ConfiguracaoRedMine',
                    'ConfiguracaoDocumentosDestinados',
                    'ConfiguracaoPaletes',
                    'ConfiguracaoGeral',
                    'ConfiguracaoCanhoto',
                    'ConfiguracaoMonitoramento',
                    'ConfiguracaoTransportador',
                    'ConfiguracaoRoteirizacao',
                    'ConfiguracaoFilaCarregamento',
                    'ConfiguracaoProduto',
                    'ConfiguracaoCalculoPrevisao',
                    'ConfiguracaoPortalMultiClifor',
                    'ConfiguracaoPortalFluxoPatio',
                    'ConfiguracaoPortalNFSeManual',
                    'ConfiguracaoComissaoMotorista',
                    'ConfiguracaoEnvioEmailCobranca',
                    'ConfiguracaoInfracoes',
                    'ConfiguracaoBidding',
                    'ConfiguracaoAbastecimento',
                    'ConfiguracaoArquivo',
                    'ConfiguracaoAmbiente',
                    'ConfiguracaoPessoa',
                    'ConfiguracaoRelatorio',
                    'ConfiguracaoMongo',
                    'ConfiguracaoSSo',
                    'ConfiguracaoMercosul',
                    'ConfiguracaoPaisMercosul',
                    'ConfiguracaoAgendamentoEntrega',
                    'ConfiguracaoAtendimentoAutomaticoDivergenciaValorFreteCTeEmitidosEmbarcador',
                    'ConfiguracaoAPIDeConexaoComGeradorExcelKMM',
                    'ConfiguracaoCotacao',
                    'ConfiguracaoGeralCIOT',
                    'ConfiguracaoGeralTipoPagamentoCIOT',
                    'ConfiguracaoGeralDownloadArquivos',
                    'ConfiguracaoPaginacaoInterfaces',
                    'ConfiguracaosAgendamentoEntrega'
                ];

                for (var objeto of objetos) {
                    var dadosObjeto = objeto == null ? dados : dados[objeto];
                    PreencherObjetoKnout(_configuracaoEmbarcador, { Data: dadosObjeto });
                }

                preencherConfiguracaoMotorista(retorno.Data.ConfiguracaoMotorista);
                preencherConfiguracaoMotoristaIgnorados(retorno.Data.ConfiguracaoAbaMotorista);
                preencherConfiguracaoVeiculo(retorno.Data.ConfiguracaoVeiculo);
                preencherConfiguracaoPaisesMercosul(retorno.Data.ConfiguracaoPaisMercosul);
                preencherConfiguracaoMonitoramento(retorno.Data);
                preencherUsarGrupoDeTipoDeOperacaoNoMonitoramento(retorno.Data);
                preencherNaoProcessarTrocaAlvoViaMonitoramento(retorno.Data);
                preencheGridIntervaloTempoLiberaDocumentoEmitidoEscrituracao(retorno.Data.ConfiguracaoFinanceiro.ConfiguracaoIntervaloTempoLiberaDocumentoEmitidoEscrituracao);
                preencherConfiguracaoAtendimentoAutomaticoDivergenciaValorFreteCTeEmitidosEmbarcador(retorno.Data.ConfiguracaoAtendimentoAutomaticoDivergenciaValorFreteCTeEmitidosEmbarcador);
                preencherConfiguracaoAPIDeConexaoComGeradorExcelKMM(retorno.Data.ConfiguracaoAPIDeConexaoComGeradorExcelKMM);
                preencherConfiguracaoTiposPagamentoCIOT(retorno.Data.ConfiguracaoGeralTipoPagamentoCIOT);
                preencherConfiguracaoDownloadArquivos(retorno.Data.ConfiguracaoGeralDownloadArquivos)
                preencherConfiguracaoPaginacaoInterfaces(retorno.Data.ConfiguracaoPaginacaoInterfaces);
                $("#divContent").removeClass("hidden");
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function obterConfiguracaoSalvar() {
    var configuracao = RetornarObjetoPesquisa(_configuracaoEmbarcador);

    preencherConfiguracaoMotoristaSalvar(configuracao);
    preencherConfiguracaoVeiculoSalvar(configuracao);
    preencherConfiguracaoPaisesMercosulSalvar(configuracao);
    obterListaIntervaloTempoLiberaDocumentoEmitidoEscrituracaoSalvar(configuracao);

    return configuracao;
}

function ClickIdentificarMonitoramentoStatusViagemEmTransito(val) {
    _configuracaoEmbarcador.IdentificarMonitoramentoStatusViagemEmTransitoKM.visible(val);
    _configuracaoEmbarcador.IdentificarMonitoramentoStatusViagemEmTransitoMinutos.visible(val);
    _configuracaoEmbarcador.IdentificarMonitoramentoStatusViagemEmTransitoKM.required(val);
    _configuracaoEmbarcador.IdentificarMonitoramentoStatusViagemEmTransitoMinutos.required(val);
    _configuracaoEmbarcador.IgnorarStatusViagemMonitoramentoAnterioresTransito.visible(val);
}

function ClickIdentificarVeiculoParado(val) {
    _configuracaoEmbarcador.IdentificarVeiculoParadoDistancia.visible(val);
    _configuracaoEmbarcador.IdentificarVeiculoParadoTempo.visible(val);
    _configuracaoEmbarcador.IdentificarVeiculoParadoDistancia.required(val);
    _configuracaoEmbarcador.IdentificarVeiculoParadoTempo.required(val);
}

function ClickUsarGrupoDeTipoDeOperacaoNoMonitoramento(val) {
    _configuracaoEmbarcador.UsarGrupoDeTipoDeOperacaoNoMonitoramentoOcultarGrupoStatusViagem.visible(val);
    if (!val) _configuracaoEmbarcador.UsarGrupoDeTipoDeOperacaoNoMonitoramentoOcultarGrupoStatusViagem.val(false);
}

function ClickNaoProcessarTrocaAlvoViaMonitoramento(val) {
    _configuracaoEmbarcador.MonitoramentoConsiderarPosicaoTardiaParaAtualizarInicioFimEntregaViagem.visible(!val);
    _configuracaoEmbarcador.QuandoIniciarViagemViaMonitoramento.visible(!val);
    _configuracaoEmbarcador.RegistrarEntregasApenasAposAtenderTodasColetas.visible(!val);
    if (val) {
        _configuracaoEmbarcador.MonitoramentoConsiderarPosicaoTardiaParaAtualizarInicioFimEntregaViagem.val(false);
        _configuracaoEmbarcador.RegistrarEntregasApenasAposAtenderTodasColetas.val(false);
    }
}

function preencherConfiguracaoMonitoramento(dados) {
    ClickIdentificarMonitoramentoStatusViagemEmTransito(dados.IdentificarMonitoramentoStatusViagemEmTransito);
    ClickIdentificarVeiculoParado(dados.IdentificarVeiculoParado);
}

function preencherUsarGrupoDeTipoDeOperacaoNoMonitoramento(dados) {
    ClickUsarGrupoDeTipoDeOperacaoNoMonitoramento(dados.NaoProcessarTrocaAlvoViaMonitoramento);
}

function preencherNaoProcessarTrocaAlvoViaMonitoramento(dados) {
    ClickNaoProcessarTrocaAlvoViaMonitoramento(dados.NaoProcessarTrocaAlvoViaMonitoramento);
}

function preencherConfiguracaoAtendimentoAutomaticoDivergenciaValorFreteCTeEmitidosEmbarcador(dados) {
    _configuracaoEmbarcador.GerarAtendimentoDivergenciaValoresCTeEmitidoEmbarcador.val(dados.GerarAtendimentoDivergenciaValorTabelaCTeEmitidoEmbarcador);
    if (_configuracaoEmbarcador.GerarAtendimentoDivergenciaValoresCTeEmitidoEmbarcador.val() == true) {
        _configuracaoEmbarcador.MotivoAtendimentoDivergenciaValoresCTeEmitidoEmbarcador.codEntity(dados.MotivoChamado.Codigo);
        _configuracaoEmbarcador.MotivoAtendimentoDivergenciaValoresCTeEmitidoEmbarcador.val(dados.MotivoChamado.Descricao);
    }
}

function preencherConfiguracaoAPIDeConexaoComGeradorExcelKMM(dados) {
    _configuracaoEmbarcador.UsarApiDeConexaoComGeradorExcelKMM.val(dados.UsarApiDeConexaoComGeradorExcelKMM);
}

function preencherConfiguracaoDownloadArquivos(dados) {
    _configuracaoEmbarcador.PermitirBaixarArquivosConembOcorenManualmente.val(dados.PermitirBaixarArquivosConembOcorenManualmente);
}

function preencherConfiguracaoTiposPagamentoCIOT(dados) {
    _configuracaoEmbarcador.TiposPagamentoCIOTOperadora.val(dados.TiposPagamentoCIOTOperadora);
    recarregarGridTiposPagamentoCIOT();
}

function preencherConfiguracaoPaginacaoInterfaces(dados) {
    _configuracaoEmbarcador.GridConfiguracoesPaginacaoInterfaces.val(dados.GridConfiguracoesPaginacaoInterfaces);
    recarregarGridConfiguracoesPaginacaoInterfaces();
}

function ClickGerarNumerodeCargaAlfanumerico() {
    if (_configuracaoEmbarcador.GerarNumerodeCargaAlfanumerico.val()) {
        _configuracaoEmbarcador.TamanhoNumerodeCargaAlfanumerico.enable(true);

        if (_configuracaoEmbarcador.TamanhoNumerodeCargaAlfanumerico.val() == EnumTamanhoNumerodeCargaAlfanumerico.Nenhum)
            _configuracaoEmbarcador.TamanhoNumerodeCargaAlfanumerico.val(EnumTamanhoNumerodeCargaAlfanumerico.Dois);
    }
    else {
        _configuracaoEmbarcador.TamanhoNumerodeCargaAlfanumerico.enable(false);
        _configuracaoEmbarcador.TamanhoNumerodeCargaAlfanumerico.val(EnumTamanhoNumerodeCargaAlfanumerico.Nenhum);
    }
}

function limpaCampoMotivoAtendimentoDivergenciaValoresCTeEmitidoEmbarcador() {
    _configuracaoEmbarcador.MotivoAtendimentoDivergenciaValoresCTeEmitidoEmbarcador.codEntity(0);
    _configuracaoEmbarcador.MotivoAtendimentoDivergenciaValoresCTeEmitidoEmbarcador.val("");
}
function aplicarValidacaoDecimal(prop) {
    let inicializado = false;

    prop.val.subscribe(function (value) {
        if (!inicializado) {
            inicializado = true;
            return;
        }

        if (value === null || value === undefined) {
            prop.val("");
            return;
        }

        let str = String(value).trim();
        if (str === "") {
            prop.val("");
            return;
        }

        let num = parseFloat(str.replace(',', '.'));
        if (isNaN(num)) {
            prop.val("");
            return;
        }

        if (num > 1) num = 1;
        if (num < 0) num = 0;

        prop.val(num.toFixed(2).replace('.', ','));
    });
}
