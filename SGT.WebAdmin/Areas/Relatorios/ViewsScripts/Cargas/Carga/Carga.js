/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../js/Global/Globais.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Container.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Porto.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/PedidoViagemNavio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CentroResultado.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoSeparacao.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CentroCarregamento.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Carregamento.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CentroCustoViagem.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumStatusCTe.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoCTe.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoPropostaMultimodal.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoServicoMultimodal.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumProblemasCarga.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoCargaMercante.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCarga, _pesquisaCarga, _CRUDRelatorio, _relatorioCarga, _CRUDFiltrosRelatorio;

var PesquisaCarga = function () {
    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Cargas.Carga.TipoDoRelatorio.getFieldDescription(), issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });
    this.FlagCargaPercentualExecucao = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.Carga.FlagCargaPercentualPendente, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });

    this.DataCarregamentoInicio = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.Carga.DataCarregamentoInicio.getFieldDescription(), getType: typesKnockout.date });
    this.DataCarregamentoFim = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.Carga.DataCarregamentoFim.getFieldDescription(), dateRangeInit: this.DataInicio, getType: typesKnockout.date });

    var dataHoraAtual = _CONFIGURACAO_TMS.HabilitarHoraFiltroDataInicialFinalRelatorioCargas ? Global.DataHoraAtual() : Global.DataAtual();
    this.DataInicial = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.Carga.DataInicial.getFieldDescription(), issue: 2, getType: _CONFIGURACAO_TMS.HabilitarHoraFiltroDataInicialFinalRelatorioCargas ? typesKnockout.dateTime : typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.DataFinal = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.Carga.DataFinal.getFieldDescription(), issue: 2, getType: _CONFIGURACAO_TMS.HabilitarHoraFiltroDataInicialFinalRelatorioCargas ? typesKnockout.dateTime : typesKnockout.date, val: ko.observable(dataHoraAtual), def: dataHoraAtual });

    this.Carregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Relatorios.Cargas.Carga.Carregamento.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(false) });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Relatorios.Cargas.Carga.Transportador.getFieldDescription()), issue: 69, idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Cargas.Carga.Filial.getFieldDescription(), issue: 63, idBtnSearch: guid(), visible: ko.observable(true) });
    this.CentroCarregamento = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Cargas.Carga.CentroDeCarregamento.getFieldDescription(), issue: 320, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Cargas.Carga.Remetente.getFieldDescription(), issue: 52, idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Cargas.Carga.Destinatario.getFieldDescription(), issue: 52, idBtnSearch: guid() });
    this.Origem = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Cargas.Carga.Origem.getFieldDescription(), issue: 16, idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Cargas.Carga.Destino.getFieldDescription(), issue: 16, idBtnSearch: guid() });
    this.GrupoPessoas = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Cargas.Carga.GrupoDePessoas.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.TipoCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Cargas.Carga.TipoDeCarga.getFieldDescription(), issue: 53, idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Cargas.Carga.TipoDeOperacao.getFieldDescription(), issue: 121, idBtnSearch: guid(), visible: ko.observable(true) });
    this.ModeloVeiculo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Cargas.Carga.ModeloDeVeiculo.getFieldDescription(), issue: 144, idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Cargas.Carga.Veiculo.getFieldDescription(), issue: 143, idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Cargas.Carga.Motorista.getFieldDescription(), issue: 145, idBtnSearch: guid() });
    this.Operador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Cargas.Carga.Operador.getFieldDescription(), issue: 145, idBtnSearch: guid() });
    this.Rota = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Cargas.Carga.Rota.getFieldDescription(), issue: 830, idBtnSearch: guid(), visible: ko.observable(false) });
    this.TipoSeparacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Cargas.Carga.TipoDeSeparacao.getFieldDescription(), idBtnSearch: guid() });
    this.CentroResultado = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Cargas.Carga.CentroResultado.getFieldDescription(), idBtnSearch: guid() });
    this.Expedidor = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Cargas.Carga.Expedidor.getFieldDescription(), idBtnSearch: guid() });
    this.Recebedor = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Cargas.Carga.Recebedor.getFieldDescription(), idBtnSearch: guid() });
    this.LocRecebedor = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Cargas.Carga.LocRecebedor.getFieldDescription(), idBtnSearch: guid() });
    this.CanalEntrega = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Canal Entrega:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.FilialVenda = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Cargas.Carga.FilialVenda.getFieldDescription(), issue: 64, idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataFaturamentoInicial = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.Carga.DataFaturamentoInicial.getFieldDescription(), getType: typesKnockout.date });
    this.DataFaturamentoFinal = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.Carga.DataFaturamentoFinal.getFieldDescription(), getType: typesKnockout.date });

    this.TipoLocalPrestacao = PropertyEntity({ val: ko.observable(EnumTipoLocalPrestacao.todos), options: EnumTipoLocalPrestacao.obterOpcoesPesquisa(), def: EnumTipoLocalPrestacao.todos, text: Localization.Resources.Relatorios.Cargas.Carga.TipoDePrestacao.getRequiredFieldDescription() });
    this.SomenteDescontoOperador = PropertyEntity({ type: types.bool, val: ko.observable(false), def: false, text: Localization.Resources.Relatorios.Cargas.Carga.SomenteCargasOndeOperadorObteveUmDescontoNoFrete, visible: ko.observable(false) });
    this.ExibirCargasAgrupadas = PropertyEntity({ type: types.bool, val: ko.observable(false), def: false, text: Localization.Resources.Relatorios.Cargas.Carga.VisualizarAsCargasAgrupadasApenasParaCargasQueForamAgrupadas, visible: ko.observable(true) });
    this.SomenteTerceiros = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.Carga.FreteDeTerceiro.getFieldDescription(), options: Global.ObterOpcoesPesquisaBooleano(Localization.Resources.Enumeradores.SimNao.Sim, Localization.Resources.Enumeradores.SimNao.Nao), val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });
    this.Transbordo = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.Carga.Transbordo.getFieldDescription(), options: Global.ObterOpcoesPesquisaBooleano(Localization.Resources.Enumeradores.SimNao.Sim, Localization.Resources.Enumeradores.SimNao.Nao), val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });
    this.InformacoesCargaPreCarga = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.Carga.InformacoesCargaPreCarga.getFieldDescription(), options: EnumInformacaoCargaPreCarga.ObterOpcoesPesquisa(), val: ko.observable(EnumInformacaoCargaPreCarga.SomenteCargas), def: EnumInformacaoCargaPreCarga.SomenteCargas, issue: 0, visible: ko.observable(true) });
    this.Problemas = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.Carga.Problemas.getFieldDescription(), options: EnumProblemasCarga.obterOpcoesPesquisa(), val: ko.observable(0), def: 1, issue: 0, visible: ko.observable(true) });
    this.TabelasFrete = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Relatorios.Cargas.Carga.TabelasDeFrete.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(true) });
    this.CodigoCargaEmbarcador = PropertyEntity({ type: types.map, text: Localization.Resources.Relatorios.Cargas.Carga.DescricaoCarga.getFieldDescription(), visible: ko.observable(true) });
    this.PreCarga = PropertyEntity({ type: types.map, text: Localization.Resources.Relatorios.Cargas.Carga.PreCarga.getFieldDescription(), visible: ko.observable(true) });
    this.Pedido = PropertyEntity({ type: types.map, text: Localization.Resources.Relatorios.Cargas.Carga.Pedido.getFieldDescription(), visible: ko.observable(true) });
    this.SomenteComReserva = PropertyEntity({ type: types.bool, val: ko.observable(false), def: false, text: Localization.Resources.Relatorios.Cargas.Carga.SomenteCargasComReserva });
    this.CargasSemPacote = PropertyEntity({ type: types.bool, val: ko.observable(false), def: false, text: "Cargas sem pacote", visible: ko.observable(false) });
    this.DataAnulacaoInicial = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.Carga.DataAnulacaoInicial.getFieldDescription(), getType: typesKnockout.date });
    this.DataAnulacaoFinal = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.Carga.DataAnulacaoFinal.getFieldDescription(), getType: typesKnockout.date });
    this.DataInicialInicioEmissaoDocumentos = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.Carga.DataInicialInicioEmissaoDocumentos.getFieldDescription(), getType: typesKnockout.date });
    this.DataFinalInicioEmissaoDocumentos = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.Carga.DataFinalInicioEmissaoDocumentos.getFieldDescription(), getType: typesKnockout.date });
    this.DataInicialFimEmissaoDocumentos = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.Carga.DataInicialFimEmissaoDocumentos.getFieldDescription(), getType: typesKnockout.date });
    this.DataFinalFimEmissaoDocumentos = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.Carga.DataFinalFimEmissaoDocumentos.getFieldDescription(), getType: typesKnockout.date });
    this.DeliveryTerm = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.Carga.DeliveryTerm.getFieldDescription(), maxlength: 150 });
    this.IdAutorizacao = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.Carga.IdDeAutorizacao.getFieldDescription(), maxlength: 150 });
    this.DataInclusaoBookingInicial = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.Carga.InclusaoBookingInicio.getFieldDescription(), getType: typesKnockout.date });
    this.DataInclusaoBookingLimite = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.Carga.InclusaoBookingLimite.getFieldDescription(), dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataInclusaoPCPInicial = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.Carga.InclusaoPCPInicio.getFieldDescription(), getType: typesKnockout.date });
    this.DataInclusaoPCPLimite = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.Carga.InclusaoPCPLimite.getFieldDescription(), dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.NumeroPedidoCliente = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.Carga.NumeroDoPedidoDoCliente.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.DataEncerramentoInicial = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.Carga.DataInicialEncerramento.getFieldDescription(), getType: typesKnockout.date });
    this.DataEncerramentoFinal = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.Carga.DataFinalEncerramento.getFieldDescription(), getType: typesKnockout.date });
    this.DataConfirmacaoDocumentosInicial = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.Carga.DataInicialConfirmacaoDocumento.getFieldDescription(), getType: typesKnockout.date });
    this.DataConfirmacaoDocumentosFinal = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.Carga.DataFinalConfirmacaoDocumento.getFieldDescription(), getType: typesKnockout.date });
    this.NaoComparecimento = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.Carga.NaoComparecimento.getFieldDescription(), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), val: ko.observable(EnumSimNaoPesquisa.Todos), def: EnumSimNaoPesquisa.Todos, visible: ko.observable(true) });
    this.CargaTrechos = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.Carga.CargaTrechos.getFieldDescription(), options: EnumCargaTrechos.obterOpcoesPesquisa(), val: ko.observable(EnumCargaTrechos.Todos), def: EnumCargaTrechos.Todos, visible: ko.observable(false) });
    this.CargaTrechoSumarizada = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.Carga.CargaDeTrecho.getFieldDescription(), options: EnumCargaTrechoSumarizada.obterOpcoesPesquisa(), val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.NumeroDtNatura = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.Carga.NumeroDtNatura.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;
    this.DataAnulacaoInicial.dateRangeLimit = this.DataAnulacaoFinal;
    this.DataAnulacaoFinal.dateRangeInit = this.DataAnulacaoInicial;
    this.DataInicialInicioEmissaoDocumentos.dateRangeLimit = this.DataFinalInicioEmissaoDocumentos;
    this.DataFinalInicioEmissaoDocumentos.dateRangeInit = this.DataInicialInicioEmissaoDocumentos;
    this.DataInicialFimEmissaoDocumentos.dateRangeLimit = this.DataFinalFimEmissaoDocumentos;
    this.DataFinalFimEmissaoDocumentos.dateRangeInit = this.DataInicialFimEmissaoDocumentos;
    this.DataInclusaoBookingInicial.dateRangeLimit = this.DataInclusaoBookingLimite;
    this.DataInclusaoBookingLimite.dateRangeInit = this.DataInclusaoBookingInicial;
    this.DataInclusaoPCPInicial.dateRangeLimit = this.DataInclusaoPCPLimite;
    this.DataInclusaoPCPLimite.dateRangeInit = this.DataInclusaoPCPInicial;
    this.DataEncerramentoInicial.dateRangeLimit = this.DataEncerramentoFinal;
    this.DataEncerramentoFinal.dateRangeInit = this.DataEncerramentoInicial;
    this.DataConfirmacaoDocumentosInicial.dateRangeLimit = this.DataConfirmacaoDocumentosFinal;
    this.DataConfirmacaoDocumentosFinal.dateRangeInit = this.DataConfirmacaoDocumentosInicial;
    this.DataFaturamentoInicial.dateRangeLimit = this.DataFaturamentoFinal;
    this.DataFaturamentoFinal.dateRangeInit = this.DataFaturamentoInicial;

    this.NumeroMdfe = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.Carga.NumeroDoManifesto.getFieldDescription(), maxlength: 20, getType: typesKnockout.int });

    var situacaoCargaPesquisa = _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador ? EnumSituacoesCarga.obterOpcoesEmbarcador() : EnumSituacoesCarga.obterOpcoesTMS();

    this.Situacao = PropertyEntity({ val: ko.observable(""), options: situacaoCargaPesquisa, def: "", text: Localization.Resources.Gerais.Geral.SituacaoCargaMercante.getFieldDescription(), required: false });

    this.Situacoes = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, params: { Tipo: "", Ativo: situacaoCargaPesquisa.Todos, OpcaoSemGrupo: false }, text: Localization.Resources.Relatorios.Cargas.Carga.Situacaoes.getFieldDescription(), issue: 533, options: ko.observable(situacaoCargaPesquisa), visible: ko.observable(true) });
    this.SituacaoCargaMercante = PropertyEntity({ text: Localization.Resources.Gerais.Geral.SituacaoCargaMercante.getFieldDescription(), getType: typesKnockout.selectMultiple, val: ko.observable([]), options: EnumSituacaoCargaMercante.obterOpcoes(), def: [], visible: ko.observable(false) });

    this.TipoOSConvertido = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, text: "Tipo OS Convertido: ", options: EnumTipoOSConvertido.obterOpcoes(), visible: ko.observable(true) });
    this.TipoOS = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, text: "Tipo OS: ", options: EnumTipoOS.obterOpcoes(), visible: ko.observable(true) });
    this.ProvedorOS = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Provedor OS: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.DirecionamentoCustoExtra = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, text: "Direcionamento Custo Extra: ", options: EnumDirecionamentoCustoExtra.obterOpcoes(), visible: ko.observable(true) });
    this.StatusCustoExtra = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, text: "Status Custo Extra: ", options: EnumStatusCustoExtra.obterOpcoes(), visible: ko.observable(true) });

    this.CentroDeCustoViagemCodigo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Relatorios.Cargas.Carga.CentroDeCustoViagem.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoProduto = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Cargas.Carga.GrupoProduto.getFieldDescription(), idBtnSearch: guid() });

    //Emissão Multimodal
    this.NumeroBooking = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.Carga.NumeroBooking.getFieldDescription() });
    this.NumeroOS = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.Carga.NumeroOrdemServico.getFieldDescription() });
    this.NumeroNota = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.Carga.NumeroNotaFiscal.getFieldDescription(), getType: typesKnockout.int });
    this.NumeroControle = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.Carga.NumeroControle.getFieldDescription() });
    this.SituacaoCTe = PropertyEntity({ val: ko.observable(EnumStatusCTe.TODOS), def: EnumStatusCTe.TODOS, text: Localization.Resources.Relatorios.Cargas.Carga.SituacaoCTe.getFieldDescription(), options: EnumStatusCTe.obterOpcoes() });
    this.PortoOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Cargas.Carga.PortoOrigem.getFieldDescription(), idBtnSearch: guid() });
    this.PortoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Cargas.Carga.PortoDestino.getFieldDescription(), idBtnSearch: guid() });
    this.Viagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Cargas.Carga.Viagem.getFieldDescription(), idBtnSearch: guid() });
    this.Container = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Cargas.Carga.Container.getFieldDescription(), idBtnSearch: guid() });
    this.TipoCTe = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumTipoCTe.ObterOpcoes(), text: Localization.Resources.Relatorios.Cargas.Carga.TipoDoCTe.getFieldDescription() });
    this.TipoPropostaMultimodal = PropertyEntity({ val: ko.observable(EnumTipoPropostaMultimodal.Todos), def: EnumTipoPropostaMultimodal.Todos, text: Localization.Resources.Relatorios.Cargas.Carga.TipoProposta.getFieldDescription(), options: EnumTipoPropostaMultimodal.obterOpcoesSemNumero(), getType: typesKnockout.selectMultiple });
    this.TipoServicoMultimodal = PropertyEntity({ val: ko.observable(EnumTipoServicoMultimodal.Todos), def: EnumTipoServicoMultimodal.Todos, text: Localization.Resources.Relatorios.Cargas.Carga.TipoServico.getFieldDescription(), options: EnumTipoServicoMultimodal.obterOpcoesSemNumero(), getType: typesKnockout.selectMultiple });
    this.VeioPorImportacao = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.Carga.VeioPorImportacao.getFieldDescription(), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), val: ko.observable(EnumSimNaoPesquisa.Todos), def: EnumSimNaoPesquisa.Todos, visible: ko.observable(true) });
    this.SomenteCTeSubstituido = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.Carga.SomenteCTeSubstituido, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });

    this.ValorToneladaSimulado = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Cargas.Carga.ValorToneladaSimulado.getFieldDescription(), idBtnSearch: guid() });
    this.ValorFreteSimulacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Cargas.Carga.FreteSimulado.getFieldDescription(), idBtnSearch: guid() });
    this.SerieNfe = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Cargas.Carga.SerieNFe.getFieldDescription(), idBtnSearch: guid() });
    this.ResponsavelValePedagio = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Cargas.Carga.ResponsavelValePedagio.getFieldDescription(), idBtnSearch: guid() });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            if (ValidarFiltrosObrigatorios())
                _gridCarga.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Preview, idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaCarga.Visible.visibleFade()) {
                _pesquisaCarga.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaCarga.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Avancada, icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: Localization.Resources.Gerais.Geral.GerarPDF });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: Localization.Resources.Gerais.Geral.GerarPlanilhaExcel, idGrid: guid() });
};

//*******EVENTOS*******

function LoadRelatorioCarga() {
    
    _pesquisaCarga = new PesquisaCarga();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridCarga = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "/Relatorios/Carga/Pesquisa", _pesquisaCarga, null, null, 10, null, null, null, null, 20);
    _gridCarga.SetPermitirEdicaoColunas(true);

    _relatorioCarga = new RelatorioGlobal("Relatorios/Carga/BuscarDadosRelatorio", _gridCarga, function () {
        _relatorioCarga.loadRelatorio(function () {
            KoBindings(_pesquisaCarga, "knockoutPesquisaCarga");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaCarga");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaCargas");

            new BuscarCarregamento(_pesquisaCarga.Carregamento);
            new BuscarTransportadores(_pesquisaCarga.Transportador, null, null, true);
            new BuscarVeiculos(_pesquisaCarga.Veiculo);
            new BuscarMotoristas(_pesquisaCarga.Motorista);
            new BuscarTiposdeCarga(_pesquisaCarga.TipoCarga);
            new BuscarModelosVeicularesCarga(_pesquisaCarga.ModeloVeiculo);
            new BuscarFilial(_pesquisaCarga.Filial);
            new BuscarFilial(_pesquisaCarga.FilialVenda);

            new BuscarCentrosCarregamento(_pesquisaCarga.CentroCarregamento);
            new BuscarGruposPessoas(_pesquisaCarga.GrupoPessoas, null, null, null, EnumTipoGrupoPessoas.Clientes);
            new BuscarClientes(_pesquisaCarga.Remetente);
            new BuscarClientes(_pesquisaCarga.Destinatario);
            new BuscarOperador(_pesquisaCarga.Operador);
            new BuscarLocalidades(_pesquisaCarga.Origem, null, null, null, true);
            new BuscarLocalidades(_pesquisaCarga.Destino);
            new BuscarTiposOperacao(_pesquisaCarga.TipoOperacao);
            new BuscarRotasFrete(_pesquisaCarga.Rota);
            new BuscarTabelasDeFrete(_pesquisaCarga.TabelasFrete);
            new BuscarPedidoViagemNavio(_pesquisaCarga.Viagem);
            new BuscarPorto(_pesquisaCarga.PortoOrigem);
            new BuscarPorto(_pesquisaCarga.PortoDestino);
            new BuscarContainers(_pesquisaCarga.Container);
            new BuscarTiposSeparacao(_pesquisaCarga.TipoSeparacao);
            new BuscarCentroResultado(_pesquisaCarga.CentroResultado);
            new BuscarClientes(_pesquisaCarga.Expedidor);
            new BuscarClientes(_pesquisaCarga.Recebedor);
            new BuscarClientes(_pesquisaCarga.ProvedorOS);
            new BuscarCanaisEntrega(_pesquisaCarga.CanalEntrega);
            new BuscarCentroCustoViagem(_pesquisaCarga.CentroDeCustoViagemCodigo);
            new BuscarGruposProdutos(_pesquisaCarga.GrupoProduto);
            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
                _pesquisaCarga.SomenteDescontoOperador.visible(true);
                _pesquisaCarga.Carregamento.visible(true);
            }
            else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
                _pesquisaCarga.CentroCarregamento.visible(false);
                _pesquisaCarga.Filial.visible(false);
                _pesquisaCarga.FilialVenda.visible(false);
                _pesquisaCarga.GrupoPessoas.visible(true);
                _pesquisaCarga.Transportador.text("Empresa/Filial:");
            }

            if (_CONFIGURACAO_TMS.AtivarNovosFiltrosConsultaCarga) {
                _pesquisaCarga.Situacoes.visible(false);
                //_pesquisaCarga.Situacoes.val(EnumSituacoesCarga.Todas);
                //_pesquisaCarga.Situacoes.def = EnumSituacoesCarga.Todas;

                _pesquisaCarga.SituacaoCargaMercante.visible(true);
            }

            if (!_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) {
                _pesquisaCarga.Rota.visible(true);
                $("#liFiltrosEmissaoMultimodal").hide();
            }

            ObterQuantidadeDeStages();
            TipoOperacaoPermiteIntegrarPacotes();

        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaCarga, true);
}

function GerarRelatorioPDFClick(e, sender) {
    if (ValidarFiltrosObrigatorios())
        _relatorioCarga.gerarRelatorio("Relatorios/Carga/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    if (ValidarFiltrosObrigatorios())
        _relatorioCarga.gerarRelatorio("Relatorios/Carga/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}

function ValidarFiltrosObrigatorios() {
    var tudoCerto = true;
    var valido = ValidarCamposObrigatorios(_pesquisaCarga);

    if (!valido) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.FiltrosObrigatorios, Localization.Resources.Gerais.Geral.InformeOsFiltrosObrigatorios);
        tudoCerto = false;
    }

    var totalDias = Global.ObterDiasEntreDatas(_pesquisaCarga.DataInicial.val(), _pesquisaCarga.DataFinal.val());
    if (_CONFIGURACAO_TMS.QuantidadeMaximaDiasRelatorios > 0 && totalDias > _CONFIGURACAO_TMS.QuantidadeMaximaDiasRelatorios) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Relatorios.Cargas.Carga.DatasInvalidas, Localization.Resources.Relatorios.Cargas.Carga.DiferencaDasDatasNaoPodeSerMaiorQueDias.format(_CONFIGURACAO_TMS.QuantidadeMaximaDiasRelatorios));
        tudoCerto = false;
    }

    return tudoCerto;
}

function ObterQuantidadeDeStages() {
    executarReST("Relatorios/Carga/ObterQuantidadeDeStages", {}, function (retorno) {
        if (retorno.Success && retorno.Data) {
            _pesquisaCarga.CargaTrechos.visible(retorno.Data.QuantidadeStages);
            _pesquisaCarga.CargaTrechoSumarizada.visible(retorno.Data.PossuiTipoOperacaoConsolidacao);
        }
    });
}

function TipoOperacaoPermiteIntegrarPacotes() {
    executarReST("Relatorios/Carga/VerificarSeExisteTipoOperacaoCarga", {}, function (retorno) {
        if (retorno.Success && retorno.Data) {
            _pesquisaCarga.CargasSemPacote.visible(retorno.Data.PermitirIntegrarPacotes);
        }
    });
}

