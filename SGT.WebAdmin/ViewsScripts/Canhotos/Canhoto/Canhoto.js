/// <reference path="../../Consultas/CTe.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/MotivoInconsistenciaDigitacao.js" />
/// <reference path="../../Enumeradores/EnumPermissaoPersonalizada.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCanhoto.js" />
/// <reference path="../../Enumeradores/EnumTipoCanhoto.js" />
/// <reference path="../../Enumeradores/EnumTipoNotaFiscalIntegrada.js" />
/// <reference path="../../Enumeradores/EnumSituacaoIntegracao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoDigitalizacaoCanhoto.js" />
/// <reference path="../../Enumeradores/EnumSituacaoPgtoCanhoto.js" />
/// <reference path="../../Enumeradores/EnumTipoPessoaGrupo.js" />
/// <reference path="../../Enumeradores/EnumTipoLocalPrestacao.js" />
/// <reference path="../../Enumeradores/EnumCodigoControleImportacao.js" />
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../Enumeradores/EnumSituacaoNotaFiscal.js" />
/// <reference path="../../Enumeradores/EnumPaises.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCargaMercante.js" />
/// <reference path="../../Enumeradores/EnumTipoRejeicaoPelaIA.js" />
/// <reference path="Conferencia.js" />
/// <reference path="CanhotoImagemInteira.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _canhotoImagemEnviar;
var _knoutArquivo;
var _knouJustificativa;
var _knouAceitarDigitalizacaoCanhotoReprovado;
var _knoutPesquisar;
var _knoutInformarDataEntrega;
var _gridCanhotos;
var _gridConhecimentos;
var _gridImagensInteiras;
var _opcoesSituacaoDigitalizacaoPOD;
var _configuracoesParaCanhoto;
var _editarImagemCanhoto;
var _knoutDisponibilidadeConsutaAPI;
var _KnoutCanhotoValidadoIA;

var opcoesDigitalizacaoIntegrada = [
    { text: "Todos", value: null },
    { text: "Não", value: false },
    { text: "Sim", value: true },
];

var opcoesCanhotoValidadoIA = [
    { text: "Todos", value: null },
    { text: "Não", value: false },
    { text: "Sim", value: true },
];

var _situacaoCanhotoIA = [
    { value: "", text: "Todas" },
    { value: EnumSituacaoIntegracao.AgIntegracao, text: "Aguardando Integração" },
    { value: EnumSituacaoIntegracao.AgRetorno, text: "Aguardando Retorno" },
    { value: EnumSituacaoIntegracao.Integrado, text: "Integrado" },
    { value: EnumSituacaoIntegracao.ProblemaIntegracao, text: "Falha na Integração" },
];

var _opcoesRadioAceitarCanhotoRejeitado = [
    { text: "Sim, houve erro", value: true },
    { text: "Não, o canhoto estava incorreto", value: false }
];
var PesquisaCanhoto = function () {
    let dataAtual = moment().add(-2, 'days').format("DD/MM/YYYY");
    this.Numero = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.Numero.getFieldDescription(), getType: typesKnockout.int, maxlength: 9, configInt: { precision: 0, allowZero: false, thousands: "" }, placeholder: Localization.Resources.Canhotos.Canhoto.Numero });
    this.Numeros = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Canhotos.Canhoto.Numeros.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.CodigoCargaEmbarcador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Canhotos.Canhoto.NumeroDaCarga.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-6 col-md-2 col-lg-2"), placeholder: Localization.Resources.Canhotos.Canhoto.NumeroDaCarga, });
    this.Carga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0, visible: ko.observable(false) });
    this.Chave = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.ChaveNFe.getFieldDescription(), maxlength: 44, visible: ko.observable(false) });
    this.NumeroNFe = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.NumeroNFe.getFieldDescription(), maxlength: 50, visible: ko.observable(true), val: ko.observable(0) });

    this.Terceiro = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Canhotos.Canhoto.Terceiro.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Canhotos.Canhoto.Destinatario.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true), placeholder: Localization.Resources.Canhotos.Canhoto.Destinatario });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Canhotos.Canhoto.Motorista.getFieldDescription(), idBtnSearch: guid(), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-4 col-lg-4"), placeholder: Localization.Resources.Canhotos.Canhoto.Motorista, visible: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Canhotos.Canhoto.Filial.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });

    let opcoesSituacaoCarga = _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS ? EnumSituacoesCarga.obterOpcoesTMSSemCancelada() : EnumSituacoesCarga.obterOpcoesEmbarcadorSemCancelada();
    this.SituacaoCanhoto = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.SituacaoCanhotoFisico.getFieldDescription(), getType: typesKnockout.selectMultiple, val: ko.observable([EnumSituacaoCanhoto.Pendente]), options: EnumSituacaoCanhoto.obterOpcoesMultiEmbarcador(), def: [EnumSituacaoCanhoto.Pendente], visible: ko.observable(true) });
    this.SituacoesDigitalizacaoCanhoto = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.SituacaoDigitalizacao.getFieldDescription(), getType: typesKnockout.selectMultiple, val: ko.observable([]), def: [], options: EnumSituacaoDigitalizacaoCanhoto.ObterOpcoes(), visible: ko.observable(true) });
    this.SituacaoPgtoCanhoto = PropertyEntity({ val: ko.observable(EnumSituacaoPgtoCanhoto.Todos), options: EnumSituacaoPgtoCanhoto.obterOpcoesPesquisa(), def: EnumSituacaoPgtoCanhoto.Todas, text: Localization.Resources.Canhotos.Canhoto.SituacaoPagamento.getFieldDescription(), visible: ko.observable(true) });
    this.TipoCanhoto = PropertyEntity({ val: ko.observable(EnumTipoCanhoto.Todos), options: EnumTipoCanhoto.obterOpcoesPesquisaComPlaceHolder(), def: EnumTipoCanhoto.Todos, text: Localization.Resources.Canhotos.Canhoto.Tipo.getFieldDescription(), visible: ko.observable(false) });

    this.TipoLocalPrestacao = PropertyEntity({ val: ko.observable(EnumTipoLocalPrestacao.todos), options: EnumTipoLocalPrestacao.obterOpcoesPesquisa(), def: EnumTipoLocalPrestacao.todos, text: Localization.Resources.Canhotos.Canhoto.TipoDaPrestacao.getFieldDescription(), visible: ko.observable(false) });
    this.SituacaoCarga = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.SituacaoDaCarga.getFieldDescription(), getType: typesKnockout.selectMultiple, val: ko.observable([]), options: opcoesSituacaoCarga, def: [], visible: ko.observable(true) });
    this.SituacaoCargaMercante = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.SituacaoDaCarga.getFieldDescription(), getType: typesKnockout.selectMultiple, val: ko.observable([]), options: EnumSituacaoCargaMercante.obterOpcoes(), def: [], visible: ko.observable(false) });


    this.DataInicio = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.DataEmissaoInicial.getFieldDescription(), val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true), placeholder: Localization.Resources.Canhotos.Canhoto.DataEmissaoInicial });
    this.DataFim = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.DataEmissaoFinal.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true), placeholder: Localization.Resources.Canhotos.Canhoto.DataEmissaoFinal });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.DataInicioDigitalizacao = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.DataDigitalizacaoInicial.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true), placeholder: Localization.Resources.Canhotos.Canhoto.DataDigitalizacaoInicial });
    this.DataFimDigitalizacao = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.DataDigitalizacaoFinal.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true), placeholder: Localization.Resources.Canhotos.Canhoto.DataDigitalizacaoFinal });
    this.DataInicioDigitalizacao.dateRangeLimit = this.DataFimDigitalizacao;
    this.DataFimDigitalizacao.dateRangeInit = this.DataInicioDigitalizacao;

    this.Serie = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.Serie.getFieldDescription(), getType: typesKnockout.int, maxlength: 9, configInt: { precision: 0, allowZero: false, thousands: "" }, placeholder: Localization.Resources.Canhotos.Canhoto.Serie, visible: ko.observable(true) });

    this.Empresa = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Canhotos.Canhoto.Transportador.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Canhotos.Canhoto.TipoDeOperacao.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true), permitirEnviarImagemParaMultiplosCanhotos: ko.observable(false), permitirInformarDataEntregaParaMultiplosCanhotos: ko.observable(false) });
    this.Veiculo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Canhotos.Canhoto.Veiculo.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.Operador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Canhotos.Canhoto.UsuarioOperador.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.ClienteComplementar = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Canhotos.Canhoto.ClienteComplementar.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });

    this.TipoPessoa = PropertyEntity({ val: ko.observable(true), options: EnumTipoPessoaGrupo.obterOpcoes(), def: true, text: Localization.Resources.Canhotos.Canhoto.TipoDeEmitente.getFieldDescription(), issue: 306, required: true, eventChange: TipoPessoaChange, visible: ko.observable(true) });
    this.Emitente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Canhotos.Canhoto.Emitente.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Canhotos.Canhoto.Emitente.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: Localization.Resources.Canhotos.Canhoto.MarcarTodas, visible: ko.observable(true) });
    this.OpcoesMenu = PropertyEntity({ type: types.event, val: ko.observable(false), text: Localization.Resources.Canhotos.Canhoto.Opcoes, visible: ko.observable(false) });

    this.ImagensConferencia = PropertyEntity({ val: ko.observableArray([]), });
    this.ImagensInteirasConferencia = PropertyEntity({ val: ko.observableArray([]) });
    this.RejeitarImagem = PropertyEntity({ visible: ko.observable(), eventClick: exibirModalMotivoInconsistenciaDigitacaoClick, type: types.event, text: Localization.Resources.Canhotos.Canhoto.Rejeitar });
    this.DownloadCanhoto = PropertyEntity({ eventClick: downloadCanhotoClick, type: types.event });
    this.Detalhes = PropertyEntity({ eventClick: detalhesCanhotoClick, type: types.event });
    this.EditarImagem = PropertyEntity({ eventClick: abrirModalEditarCanhotoClick, type: types.event });
    this.AlterarDataConfirmacaoEntrega = PropertyEntity({ eventClick: alterarDataConfirmacaoEntregaClick, type: types.event });
    this.VisualizarPDF = PropertyEntity({ eventClick: exibirModalVisualizarPDFClick, type: types.event });
    this.ValidarImagem = PropertyEntity({ visible: ko.observable(), eventClick: validarImagemCanhotoClick, type: types.event, text: Localization.Resources.Canhotos.Canhoto.Aprovar });
    this.ReverterImagem = PropertyEntity({ visible: ko.observable(false), eventClick: reverterImagemCanhotoClick, type: types.event, text: Localization.Resources.Canhotos.Canhoto.Reverter });
    this.UploadNovaImagem = PropertyEntity({ type: types.event, eventClick: abrirModalEnviarCanhotoClick, text: ko.observable(Localization.Resources.Canhotos.Canhoto.Upload) });
    this.AceitarDigitalizacaoCanhotoReprovado = PropertyEntity({ type: types.event, eventClick: abrirModalAprovarImagemCanhotoRejeitadoImagensAgrupadasClick, text: Localization.Resources.Canhotos.Canhoto.AceitarDigitalizacao, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) });
    this.MarcarTodosCanhotosVisualizacao = PropertyEntity({ val: false, type: types.event, eventClick: marcarTodosCanhotosVisualizacaoClick, text: ko.observable(Localization.Resources.Canhotos.Canhoto.MarcarTodos), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) });

    this.AprovarSelecaoCanhotosVisualizacao = PropertyEntity({ type: types.event, eventClick: aprovarSelecaoCanhotosVisualizacaoClick, text: ko.observable(Localization.Resources.Canhotos.Canhoto.AprovarSelecao), visible: ko.observable(false) });
    this.AceitarDigitalizacaoCanhotoEmMassa = PropertyEntity({ type: types.event, eventClick: abrirModalAprovarImagemCanhotoRejeitadoEmMassaClick, text: Localization.Resources.Canhotos.Canhoto.AceitarDigitalizacao, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) });
    this.RejeitarSelecaoCanhotosVisualizacao = PropertyEntity({ type: types.event, eventClick: rejeitarEnvioEmMassaClick, text: ko.observable(Localization.Resources.Canhotos.Canhoto.RejeitarSelecao), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) });

    this.CanhotoInteiroAtual = ko.observable();

    this.ConhecimentoEletronico = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.ChaveDoDocumento.getFieldDescription(), required: false, idBtnSearch: guid(), eventClick: ConhecimentoEletronicoClick, enable: ko.observable(true), visible: ko.observable(true) });
    this.ListaConhecimentos = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), text: "", idGrid: guid() });
    this.ListaCTes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.CTe = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Canhotos.Canhoto.Documento.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false), placeholder: Localization.Resources.Canhotos.Canhoto.Documento });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Canhotos.Canhoto.Carga.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });

    this.PermissaoReverter = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.TipoServicoMultiCTe = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NumeroDocumentoOriginario = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.DocOrig.getFieldDescription(), visible: ko.observable(true), val: ko.observable("") });
    this.SituacaoNotaFiscal = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.SituacaoNotaFiscal.getFieldDescription(), getType: typesKnockout.selectMultiple, val: ko.observable([]), options: EnumSituacaoNotaFiscal.obterOpcoesTelaCanhoto(), def: [], visible: ko.observable(true) });

    this.SerieMobile = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.Serie.getFieldDescription(), getType: typesKnockout.int, maxlength: 9, configInt: { precision: 0, allowZero: false, thousands: "" }, placeholder: Localization.Resources.Canhotos.Canhoto.Serie, val: this.Serie.val, visible: ko.observable(true) });
    this.NumerosMobile = PropertyEntity({ type: types.multiplesEntities, multiplesEntities: this.Numero.multiplesEntities, val: this.Numeros.val, codEntity: this.Numeros.codEntity, text: Localization.Resources.Canhotos.Canhoto.Numeros.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.CodigoCargaEmbarcadorMobile = PropertyEntity({ type: types.multiplesEntities, multiplesEntities: this.CodigoCargaEmbarcador.multiplesEntities, val: this.CodigoCargaEmbarcador.val, codEntity: this.CodigoCargaEmbarcador.codEntity, text: Localization.Resources.Canhotos.Canhoto.NumeroDaCarga.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-6 col-md-2 col-lg-2"), placeholder: Localization.Resources.Canhotos.Canhoto.NumeroDaCarga, });
    this.ChaveMobile = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.ChaveNFe.getFieldDescription(), maxlength: 44, visible: ko.observable(true), val: this.Chave.val });
    this.NumeroNFeMobile = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.NumeroNFe.getFieldDescription(), maxlength: 50, visible: ko.observable(true), val: this.NumeroNFe.val });
    this.DestinatarioMobile = PropertyEntity({ type: types.entity, codEntity: this.Destinatario.codEntity, val: this.Destinatario.val, text: Localization.Resources.Canhotos.Canhoto.Destinatario.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true), placeholder: Localization.Resources.Canhotos.Canhoto.Destinatario });
    this.MotoristaMobile = PropertyEntity({ type: types.entity, codEntity: this.Motorista.codEntity, val: this.Motorista.val, text: Localization.Resources.Canhotos.Canhoto.Motorista.getFieldDescription(), idBtnSearch: guid(), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-4 col-lg-4"), placeholder: Localization.Resources.Canhotos.Canhoto.Motorista, visible: ko.observable(true) });
    this.FilialMobile = PropertyEntity({ type: types.multiplesEntities, multiplesEntities: this.Filial.multiplesEntities, codEntity: this.Filial.codEntity, val: this.Filial.val, text: Localization.Resources.Canhotos.Canhoto.Filial.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.SituacaoCanhotoMobile = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.SituacaoCanhotoFisico.getFieldDescription(), getType: typesKnockout.selectMultiple, val: this.SituacaoCanhoto.val, options: EnumSituacaoCanhoto.obterOpcoesMultiEmbarcador(), def: [EnumSituacaoCanhoto.Pendente], visible: ko.observable(true) });
    this.SituacoesDigitalizacaoCanhotoMobile = PropertyEntity({ val: this.SituacoesDigitalizacaoCanhoto.val, options: EnumSituacaoDigitalizacaoCanhoto.ObterOpcoesPesquisaComPlaceHolder(true), getType: typesKnockout.selectMultiple, def: [], text: Localization.Resources.Canhotos.Canhoto.SituacaoDigitalizacao.getFieldDescription(), visible: ko.observable(true) });
    this.SituacaoPgtoCanhotoMobile = PropertyEntity({ val: this.SituacaoPgtoCanhoto.val, options: EnumSituacaoPgtoCanhoto.obterOpcoesPesquisa(), def: EnumSituacaoPgtoCanhoto.Todas, text: Localization.Resources.Canhotos.Canhoto.SituacaoPagamento.getFieldDescription(), visible: ko.observable(true) });
    this.TipoCanhotoMobile = PropertyEntity({ val: this.TipoCanhoto.val, options: EnumTipoCanhoto.obterOpcoesPesquisaComPlaceHolder(), def: EnumTipoCanhoto.Todos, text: Localization.Resources.Canhotos.Canhoto.Tipo.getFieldDescription(), visible: ko.observable(true) });
    this.TipoLocalPrestacaoMobile = PropertyEntity({ val: this.TipoLocalPrestacao.val, options: EnumTipoLocalPrestacao.obterOpcoesPesquisa(), def: EnumTipoLocalPrestacao.todos, text: Localization.Resources.Canhotos.Canhoto.TipoDaPrestacao.getFieldDescription(), visible: ko.observable(true) });
    this.SituacaoCargaMobile = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.SituacaoDaCarga.getFieldDescription(), getType: typesKnockout.selectMultiple, val: this.SituacaoCarga.val, options: opcoesSituacaoCarga, def: [], visible: ko.observable(true) });
    this.SituacaoCargaMercanteMobile = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.SituacaoDaCarga.getFieldDescription(), getType: typesKnockout.selectMultiple, val: this.SituacaoCargaMercante.val, options: EnumSituacaoCargaMercante.obterOpcoes(), def: [], visible: ko.observable(false) });
    this.DataInicioDigitalizacaoMobile = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.DataDigitalizacaoInicial.getFieldDescription(), val: this.DataInicioDigitalizacao.val, def: "", getType: typesKnockout.date, visible: ko.observable(true), placeholder: Localization.Resources.Canhotos.Canhoto.DataDigitalizacaoInicial });
    this.DataFimDigitalizacaoMobile = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.DataDigitalizacaoFinal.getFieldDescription(), val: this.DataFimDigitalizacao.val, def: "", getType: typesKnockout.date, visible: ko.observable(true), placeholder: Localization.Resources.Canhotos.Canhoto.DataDigitalizacaoFinal });
    this.EmpresaMobile = PropertyEntity({ type: types.multiplesEntities, codEntity: this.Empresa.codEntity, val: this.Empresa.val, multiplesEntities: this.Empresa.multiplesEntities, text: Localization.Resources.Canhotos.Canhoto.Transportador.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoOperacaoMobile = PropertyEntity({ type: types.multiplesEntities, codEntity: this.TipoOperacao.codEntity, multiplesEntities: this.TipoOperacao.multiplesEntities, val: this.TipoOperacao.val, text: Localization.Resources.Canhotos.Canhoto.TipoDeOperacao.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true), permitirEnviarImagemParaMultiplosCanhotos: ko.observable(false), permitirInformarDataEntregaParaMultiplosCanhotos: ko.observable(false) });
    this.OperadorMobile = PropertyEntity({ type: types.entity, codEntity: this.Operador.codEntity, val: this.Operador.val, text: Localization.Resources.Canhotos.Canhoto.UsuarioOperador.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.NumeroDocumentoOriginarioMobile = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.DocOrig.getFieldDescription(), visible: ko.observable(true), val: this.NumeroDocumentoOriginario.val });
    this.SituacaoNotaFiscalMobile = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.SituacaoNotaFiscal.getFieldDescription(), getType: typesKnockout.selectMultiple, val: this.SituacaoNotaFiscal.val, options: EnumSituacaoNotaFiscal.obterOpcoesTelaCanhoto(), def: [], visible: ko.observable(true) });

    this.PossuiIntegracaoComprovei = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.PossuiIntegracaoComprovei.getFieldDescription(), getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ValidacaoCanhotoComprovei = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.ValidacaoCanhotoComprovei.getFieldDescription(), getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ValidacaoNumeroComprovei = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.ValidacaoNumeroComprovei.getFieldDescription(), getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ValidacaoEncontrouDataComprovei = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.ValidacaoEncontrouDataComprovei.getFieldDescription(), getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ValidacaoAssinaturaComprovei = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.ValidacaoAssinaturaComprovei.getFieldDescription(), getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.DigitalizacaoIntegrada = PropertyEntity({ text: "Canhoto Disponível Consulta API:", val: ko.observable(null), options: opcoesDigitalizacaoIntegrada, visible: ko.observable(false) });
    this.EscritorioVendas = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.EscritorioVendas.getFieldDescription(), maxlength: 50, visible: ko.observable(true) });
    this.Matriz = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.Matriz.getFieldDescription(), maxlength: 50, visible: ko.observable(true) });
    this.ValidacaoCanhoto = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.CanhotoValidadoIA.getFieldDescription(), val: ko.observable(null), options: opcoesCanhotoValidadoIA, visible: ko.observable(true) });

    this.TipoNotaFiscalIntegrada = PropertyEntity({ val: ko.observable(EnumTipoNotaFiscalIntegrada.Todos), options: EnumTipoNotaFiscalIntegrada.obterOpcoesPesquisa(), def: EnumTipoNotaFiscalIntegrada.Todos, text: Localization.Resources.Canhotos.Canhoto.TipoNotaFiscalIntegrada.getFieldDescription(), visible: ko.observable(true) });


    this.TipoSituacaoIA = PropertyEntity({ val: ko.observable(EnumSituacaoIntegracao.Todas), options: _situacaoCanhotoIA, def: EnumSituacaoIntegracao.Todas, text: Localization.Resources.Canhotos.Canhoto.SituacaoIA.getFieldDescription(), visible: ko.observable(true) });

    this.TipoRejeicaoPelaIA = PropertyEntity({ val: ko.observable(EnumTipoRejeicaoPelaIA.Todos), options: EnumTipoRejeicaoPelaIA.obterOpcoes(), getType: typesKnockout.selectMultiple, def: [], text: Localization.Resources.Canhotos.Canhoto.RejeicaoPelaIA.getFieldDescription(), visible: ko.observable(true) });


    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            buscarCanhotos();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Avancada, idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.BuscaAvancadaMobile = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancadaMobile.visibleFade()) {
                e.BuscaAvancadaMobile.visibleFade(false);
                e.BuscaAvancadaMobile.icon("fal fa-plus");
            } else {
                e.BuscaAvancadaMobile.visibleFade(true);
                e.BuscaAvancadaMobile.icon("fal fa-minus");
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Avancada, idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(false)
    });

    this.ImportarPlanilha = PropertyEntity({
        type: types.local,
        text: Localization.Resources.Canhotos.Canhoto.ImportarDocumentosParaFiltro,
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default botaoDentroSmartAdmimForm",
        UrlImportacao: "Canhoto/ObterDadosCTePlanilha",
        UrlConfiguracao: "Canhoto/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O012_FiltroCTeCanhoto,
        CallbackImportacao: function (arg) {
            _knoutPesquisar.ListaConhecimentos.list = arg.Data.Retorno;
            RecarregarGridConhecimentos();
        },
        FecharModalSeSucesso: true
    });

    this.DownloadMassa = PropertyEntity({ eventClick: DownloadMassaClick, type: types.event, text: Localization.Resources.Canhotos.Canhoto.DownloadMassa, visible: ko.observable(true) });
    this.ImportarCancelamentos = PropertyEntity({
        type: types.local,
        text: Localization.Resources.Canhotos.Canhoto.ImportarCanhotosParaDescarte,
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default botaoDentroSmartAdmimForm",
        UrlImportacao: "Canhoto/ImportarDescarteCanhotos",
        UrlConfiguracao: "Canhoto/ConfiguracaoImportacaoDescarte",
        CodigoControleImportacao: EnumCodigoControleImportacao.O018_DescarteCanhotos,
        CallbackImportacao: function (arg) {
            RecarregarGridConhecimentos();
        },
        FecharModalSeSucesso: true
    });
    this.ImportarCanhotos = PropertyEntity({
        type: types.local,
        text: Localization.Resources.Canhotos.Canhoto.ImportarCanhotos,
        visible: ko.observable(_permitirImportarCanhotoNFFaturada),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default botaoDentroSmartAdmimForm",
        UrlImportacao: "Canhoto/ImportarCanhotos",
        UrlConfiguracao: "Canhoto/ConfiguracaoImportacaoCanhotos",
        CodigoControleImportacao: EnumCodigoControleImportacao.O072_ImportacaoCanhotos,
        CallbackImportacao: function (arg) {
            buscarCanhotos();
        },
        FecharModalSeSucesso: true
    });
    this.AtualizarCanhotos = PropertyEntity({
        type: types.local,
        text: "Atualizar Canhotos",
        visible: ko.observable(_permitirAtualizarCanhotosPorImportacao),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default botaoDentroSmartAdmimForm",
        UrlImportacao: "Canhoto/ImportarAtualizacaoCanhotos",
        UrlConfiguracao: "Canhoto/ConfiguracaoImportacaoAtualizarCanhotos",
        CodigoControleImportacao: EnumCodigoControleImportacao.O074_AtualizarSituacaoCanhoto,
        CallbackImportacao: function (arg) {
            buscarCanhotos();
        },
        FecharModalSeSucesso: true
    });

    this.ImportarDataEntrega = PropertyEntity({
        type: types.local,
        text: Localization.Resources.Canhotos.Canhoto.ImportarDatasDeEntregas,
        visible: ko.observable(_obrigatorioInformarDataEnvioCanhoto || _CONFIGURACAO_TMS.ExigirDataEntregaNotaClienteCanhotos),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default botaoDentroSmartAdmimForm",
        UrlImportacao: "Canhoto/ImportarDataEntregaCanhotos",
        UrlConfiguracao: "Canhoto/ConfiguracaoImportacaoDataEntrega",
        CodigoControleImportacao: EnumCodigoControleImportacao.O039_DataEntregaCanhotos,
        CallbackImportacao: function () {
            buscarCanhotos();
        },
        FecharModalSeSucesso: true
    });

    this.AlterarStatusTodos = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: AlterarStatusTodosClick, text: ko.observable(" "), visible: ko.observable(false) });
    this.ConfirmarRecebimentoTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: enviarMultiplosCanhotosFisicoClick, text: ko.observable(Localization.Resources.Canhotos.Canhoto.ConfirmarRecebimentoFisico), visible: ko.observable(false) });
    this.JustificarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), text: Localization.Resources.Canhotos.Canhoto.Justificar, eventClick: abrirModalMultiplasJustificativasClick, visible: ko.observable(false) });
    this.ConfirmarCanhotos = PropertyEntity({ type: types.event, val: ko.observable(false), text: Localization.Resources.Canhotos.Canhoto.AprovarDigitalizacoes, eventClick: ConfirmarCanhotosEmMassaClick, visible: ko.observable(false) });
    this.LiberarPagamentoCanhotos = PropertyEntity({ type: types.event, val: ko.observable(false), text: Localization.Resources.Canhotos.Canhoto.LiberarPagamento, eventClick: liberarPagamentoMultiplosCanhotosClick, visible: ko.observable(false) });
    this.RejeitarPagamentoCanhotos = PropertyEntity({ type: types.event, val: ko.observable(false), text: Localization.Resources.Canhotos.Canhoto.RejeitarPagamento, eventClick: rejeitarPagamentoMultiplosCanhotosClick, visible: ko.observable(false) });
    this.EnviarImagemCanhotos = PropertyEntity({ type: types.event, val: ko.observable(false), text: Localization.Resources.Canhotos.Canhoto.EnviarImagem, eventClick: abrirModalEnviarImagemMultiplosCanhotosClick, visible: ko.observable(false) });
    this.InformarDataEntregaCanhotos = PropertyEntity({ type: types.event, val: ko.observable(false), text: Localization.Resources.Canhotos.Canhoto.InformarDataEntrega, eventClick: abrirModalInformarDataEntregaClick, visible: ko.observable(false) });
    this.AlterarSituacaoCanhotoPendenteConsultaAPI = PropertyEntity({ type: types.event, text: "Alterar a situação dos canhotos disponível consulta API", eventClick: abrirModalDisponibilidadeConsultaAPI, visible: ko.observable(true) });
};

var Canhoto = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int });
    this.XMLNotaFiscal = PropertyEntity({ getType: typesKnockout.int });
    this.NumeroNota = PropertyEntity({ getType: typesKnockout.int });
    this.CodigoCanhoto = PropertyEntity({ getType: typesKnockout.int });
    this.CodigoCargaEntrega = PropertyEntity({ getType: typesKnockout.int });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), required: false });
    this.DataEntregaCliente = PropertyEntity({ getType: typesKnockout.date });

    this.ChamadosEMotivos = PropertyEntity({ val: ko.observable("") });
    this.CancelarAtendimentoAutomaticamente = PropertyEntity({ val: ko.observable(false) });

    this.Observacao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Canhotos.Canhoto.Justificativa.getFieldDescription(), maxlength: 300, required: false });
    this.MotivoRejeicaoDigitalizacao = PropertyEntity({ getType: typesKnockout.string, text: ko.observable(Localization.Resources.Canhotos.Canhoto.MotivoDaRejeicao.getFieldDescription()), maxlength: 300, required: false });
    this.Motivo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Canhotos.Canhoto.Motivo.getFieldDescription(), idBtnSearch: guid(), required: false });
    this.Observacoes = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), def: "", maxlength: 400, text: ko.observable(Localization.Resources.Canhotos.Canhoto.Observacoes.getFieldDescription()), required: false });

    this.Multiplos = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.Justificar = PropertyEntity({ eventClick: enviarJustificativaClick, enable: ko.observable(false), type: types.event, text: Localization.Resources.Canhotos.Canhoto.Justificar, visible: ko.observable(true) });
    this.AceitarCanhotoReprovado = PropertyEntity({ eventClick: enviarAceitarDigitalizacaoCanhotoReprovadoClick, enable: ko.observable(false), type: types.event, text: Localization.Resources.Canhotos.Canhoto.AceitarDigitalizacao, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: fecharModalAprovarImagemCanhotoRejeitadoImagensAgrupadasClick, enable: ko.observable(false), type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(true) });

    this.ValidacaoIAComproveiHabilitada = PropertyEntity({ text: ko.observable("Para aceitar a digitalização manualmente, precisamos entender se houve erro na validação da IA. Esses dados serão utilizados para melhorias na nossa ferramenta."), getType: typesKnockout.bool, val: ko.observable(true) });
    this.HouveFalhaNaValidacao = PropertyEntity({ text: ko.observable("Foi identificado erro na validação da IA?"), val: ko.observable(true), def: true, options: _opcoesRadioAceitarCanhotoRejeitado, getType: typesKnockout.selectMultiple, cssClass: ko.observable('') });
    this.FalhaIA_Comprovante = PropertyEntity({ text: ko.observable("Comprovante"), val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(true) });
    this.FalhaIA_NumeroDocumento = PropertyEntity({ text: ko.observable("Número do Documento"), val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(true) });
    this.FalhaIA_Data = PropertyEntity({ text: ko.observable("Data do Documento"), val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(true) });
    this.FalhaIA_Assinatura = PropertyEntity({ text: ko.observable("Assinatura"), val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(true) });
    this.CodigosCanhotosSelecionados = PropertyEntity({ val: ko.observableArray([]) });

    this.HouveFalhaNaValidacao.val.subscribe(function (novoValor) {
        _knouAceitarDigitalizacaoCanhotoReprovado.HouveFalhaNaValidacao.cssClass('');
        _knouAceitarDigitalizacaoCanhotoReprovado.FalhaIA_Comprovante.enable(novoValor);
        _knouAceitarDigitalizacaoCanhotoReprovado.FalhaIA_NumeroDocumento.enable(novoValor);
        _knouAceitarDigitalizacaoCanhotoReprovado.FalhaIA_Data.enable(novoValor);
        _knouAceitarDigitalizacaoCanhotoReprovado.FalhaIA_Assinatura.enable(novoValor);
        _knouAceitarDigitalizacaoCanhotoReprovado.Observacao.required = novoValor;
        if (!novoValor) {
            _knouAceitarDigitalizacaoCanhotoReprovado.HouveFalhaNaValidacao.cssClass('disabled-div');
            _knouAceitarDigitalizacaoCanhotoReprovado.FalhaIA_Comprovante.val(novoValor);
            _knouAceitarDigitalizacaoCanhotoReprovado.FalhaIA_NumeroDocumento.val(novoValor);
            _knouAceitarDigitalizacaoCanhotoReprovado.FalhaIA_Data.val(novoValor);
            _knouAceitarDigitalizacaoCanhotoReprovado.FalhaIA_Assinatura.val(novoValor);
        }
    });
    this.AceitarCanhotoReprovado.enable = ko.pureComputed(function () {
        if (!_knouAceitarDigitalizacaoCanhotoReprovado.ValidacaoIAComproveiHabilitada.val() || !_knouAceitarDigitalizacaoCanhotoReprovado.HouveFalhaNaValidacao.val())
            return true;

        return (_knouAceitarDigitalizacaoCanhotoReprovado.FalhaIA_Comprovante.val() ||
            _knouAceitarDigitalizacaoCanhotoReprovado.FalhaIA_NumeroDocumento.val() ||
            _knouAceitarDigitalizacaoCanhotoReprovado.FalhaIA_Data.val() ||
            _knouAceitarDigitalizacaoCanhotoReprovado.FalhaIA_Assinatura.val()) && _knouAceitarDigitalizacaoCanhotoReprovado.Observacao.val().length > 0;
    })

    this.JustificarRejeicaoDigitalizacao = PropertyEntity({ eventClick: justificarRejeicaoDigitalizacaoClick, enable: ko.observable(false), type: types.event, text: Localization.Resources.Canhotos.Canhoto.ConfirmarRejeicao, visible: ko.observable(true) });
};

var CanhotoImagemEnviar = function () {
    this.DataEnvioCanhoto = PropertyEntity({ val: ko.observable(""), text: _obrigatorioInformarDataEnvioCanhoto ? Localization.Resources.Canhotos.Canhoto.DataEnvioCanhoto.getRequiredFieldDescription() : Localization.Resources.Canhotos.Canhoto.DataEnvioCanhoto.getFieldDescription(), getType: typesKnockout.dateTime, required: _obrigatorioInformarDataEnvioCanhoto, visible: ko.observable(true) });
    this.DataEntregaNotaCliente = PropertyEntity({ val: ko.observable(""), text: (_CONFIGURACAO_TMS.ExigirDataEntregaNotaClienteCanhotos ? "*" : "") + Localization.Resources.Canhotos.Canhoto.DataEntregaDaNotaAoCliente.getFieldDescription(), getType: typesKnockout.dateTime, required: ko.observable(_CONFIGURACAO_TMS.ExigirDataEntregaNotaClienteCanhotos) });

    this.Enviar = PropertyEntity({ type: types.event, eventClick: enviarImagemCanhotoClick, text: ko.observable(Localization.Resources.Canhotos.Canhoto.Enviar) });
};

var CanhotoInformarDataEntrega = function () {
    this.DataEntregaNotaCliente = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.DataEntregaDaNotaAoCliente.getRequiredFieldDescription(), getType: typesKnockout.dateTime, required: ko.observable(true) });

    this.Enviar = PropertyEntity({ type: types.event, eventClick: informarDataEntregaCanhotoClick, text: ko.observable(Localization.Resources.Canhotos.Canhoto.Enviar) });
};

var DisponibilidadeConsultaAPI = function () {
    this.Disponibilizar = PropertyEntity({ text: "Disponibilizar para consulta API", val: ko.observable(true), def: true, options: opcoesDigitalizacaoIntegrada, visible: ko.observable(true) });

    this.Alterar = PropertyEntity({ type: types.event, text: "Alterar", eventClick: AlterarDisponibilidadeCanhotosAPIEmMassaClick, visible: ko.observable(true) });
}
//*******EVENTOS*******

function SituacoesDigitalizacaoSubscribe() {
    _knoutPesquisar.ConfirmarCanhotos.visible(_knoutPesquisar.SituacoesDigitalizacaoCanhoto.val().includes(EnumSituacaoDigitalizacaoCanhoto.AgAprovocao) && _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe && _gridCanhotos.NumeroRegistros() > 0);
}
function DownloadMassaClick() {

    exibirConfirmacao(Localization.Resources.Canhotos.Canhoto.DownloadMassa, Localization.Resources.Canhotos.Canhoto.TemCertezaQueDesejaBaixarEmMassa, function () {
        _knoutPesquisar.ImagensConferencia.val.removeAll();
        _knoutPesquisar.ImagensInteirasConferencia.val.removeAll();
        executarDownload("Canhoto/DownloadCanhotosEmMassa", RetornarObjetoPesquisa(_knoutPesquisar));
        buscarCanhotos();
    });
}

function loadCanhoto() {
    ObterSituacaoDigitalizacaoPOD().then(function () {
        _knoutArquivo = new Canhoto();
        KoBindings(_knoutArquivo, "knoutEnviarArquivo");

        _canhotoImagemEnviar = new CanhotoImagemEnviar();
        KoBindings(_canhotoImagemEnviar, "KnoutCanhotoImagemEnviar");

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe && !_obrigatorioInformarDataEnvioCanhoto)
            _canhotoImagemEnviar.DataEnvioCanhoto.visible(false);

        $.get("Content/Static/Canhotos/DetalheCanhoto.html?dyn=" + guid(), function (data) {
            $("#divDetalhesCanhoto").html(data.replace(/#KnoutDetalhesCanhoto/g, _knoutArquivo.Codigo.id));
            loadDetalhesCanhoto();
        });

        _knouJustificativa = new Canhoto();
        KoBindings(_knouJustificativa, "knoutJustificarCanhoto");
        _knouJustificativa.Observacao.required = true;

        _knouAceitarDigitalizacaoCanhotoReprovado = new Canhoto();
        KoBindings(_knouAceitarDigitalizacaoCanhotoReprovado, "knoutAceitarDigitalizacaoCanhotoReprovado");
        _knouAceitarDigitalizacaoCanhotoReprovado.Observacao.required = true;

        _knoutMotivoRejeicao = new Canhoto();
        KoBindings(_knoutMotivoRejeicao, "knoutMotivoRejeicaoDigitalizacao");
        _knoutMotivoRejeicao.MotivoRejeicaoDigitalizacao.required = true;
        _knoutMotivoRejeicao.MotivoRejeicaoDigitalizacao.text(Localization.Resources.Canhotos.Canhoto.MotivoDaRejeicao.getRequiredFieldDescription());
        BuscarMotivoInconsistenciaDigitacao(_knoutMotivoRejeicao.Motivo, callbackMotivoInconsistenciaDigitalizacao);

        _knoutInformarDataEntrega = new CanhotoInformarDataEntrega();
        KoBindings(_knoutInformarDataEntrega, "knoutDataEntregaCanhoto");

        _knoutDisponibilidadeConsutaAPI = new DisponibilidadeConsultaAPI();
        KoBindings(_knoutDisponibilidadeConsutaAPI, "knoutDisponibilidadeConsultaAPI");
        $("#" + _knoutArquivo.Arquivo.id).click(function () {
            $(this).val("");
        });

        $("#" + _knoutArquivo.Arquivo.id).on("change", enviarCanhotoClick);
        LoadReverterJustificativa();

        _knoutPesquisar = new PesquisaCanhoto();
        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
            _knoutPesquisar.SituacaoCanhoto.options = EnumSituacaoCanhoto.obterOpcoesMultiTMS();
            KoBindings(_knoutPesquisar, "knockoutPesquisaCanhotos", false, _knoutPesquisar.Pesquisar.id);
        } else {
            KoBindings(_knoutPesquisar, "knockoutPesquisaCanhotos", false, _knoutPesquisar.Pesquisar.idBtnSearch);
        }

        LoadGridCanhotosInteiros();
        ControlarAbasVisualizacaoCanhotos(0, 0);

        BuscarTransportadores(_knoutPesquisar.Empresa);
        BuscarTransportadores(_knoutPesquisar.EmpresaMobile);
        BuscarFilial(_knoutPesquisar.Filial);
        BuscarFilial(_knoutPesquisar.FilialMobile);
        BuscarMotorista(_knoutPesquisar.Motorista, retornoMotorista);
        BuscarMotorista(_knoutPesquisar.MotoristaMobile, retornoMotorista);
        BuscarClientes(_knoutPesquisar.Emitente);
        BuscarClientes(_knoutPesquisar.Destinatario);
        BuscarClientes(_knoutPesquisar.DestinatarioMobile);
        BuscarClientes(_knoutPesquisar.Terceiro);
        BuscarGruposPessoas(_knoutPesquisar.GrupoPessoa, null, null, null, EnumTipoGrupoPessoas.Clientes);
        BuscarCTes(_knoutPesquisar.CTe, null, null, Localization.Resources.Canhotos.Canhoto.BuscarDocumentos, Localization.Resources.Canhotos.Canhoto.Documentos);
        BuscarTiposOperacao(_knoutPesquisar.TipoOperacao, retornoTipoOperacao);
        BuscarTiposOperacao(_knoutPesquisar.TipoOperacaoMobile, retornoTipoOperacao);
        BuscarCargas(_knoutPesquisar.Carga);
        BuscarVeiculos(_knoutPesquisar.Veiculo);
        BuscarFuncionario(_knoutPesquisar.Operador);
        BuscarFuncionario(_knoutPesquisar.OperadorMobile);
        BuscarCargas(_knoutPesquisar.CodigoCargaEmbarcador, null, null, null, null, [EnumSituacoesCarga.Cancelada]);
        BuscarCargas(_knoutPesquisar.CodigoCargaEmbarcadorMobile);
        BuscarCanhotosSemVinculo(_knoutPesquisar.Numeros);
        BuscarCanhotosSemVinculo(_knoutPesquisar.NumerosMobile);
        BuscarClienteComplementar(_knoutPesquisar.ClienteComplementar);

        if (_CONFIGURACAO_TMS.AtivarNovosFiltrosConsultaCarga) {
            _knoutPesquisar.SituacaoCarga.visible(false);
            _knoutPesquisar.SituacaoCargaMobile.visible(false);
            _knoutPesquisar.SituacaoCargaMercante.visible(true);
            _knoutPesquisar.SituacaoCargaMercanteMobile.visible(true);
        }

        _knoutPesquisar.PermissaoReverter.val(VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Canhoto_PermitirReverterImagem, _PermissoesPersonalizadasCanhotos));
        _knoutPesquisar.SituacaoCanhoto.val.subscribe(exibirMultiplasOpcoes);

        configurarLayoutCanhotoPorTipoSistema();

        LoadConferencia();
        loadControleAbas();
        CarregarGridConhecimentos();
        buscarCanhotos();
        LoadCanhotoRecebidoFisicamente();
        loadAnexo();
        LoadObservacaoAlteracaoCanhoto();
        LoadAuditarRegistroCanhoto();
        buscarConfiguracoesParaCanhoto();
        LoadAlterarDataConfirmacaoEntrega();
    });
}

function configurarLayoutCanhotoPorTipoSistema() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        $("#liTabConhecimentos").hide();
        _knoutPesquisar.ConhecimentoEletronico.visible(false);
        _knoutPesquisar.Empresa.visible(true);
        _knoutPesquisar.Filial.visible(true);
        _knoutPesquisar.TipoCanhoto.visible(true);
        _knoutPesquisar.TipoPessoa.visible(false);
        _knoutPesquisar.Emitente.visible(false);
        _knoutPesquisar.GrupoPessoa.visible(false);

        _knoutPesquisar.TipoLocalPrestacao.visible(true);

    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _knoutPesquisar.Emitente.visible(true);
        _knoutPesquisar.Terceiro.visible(true);
        _knoutPesquisar.TipoCanhoto.visible(true);
        _knoutPesquisar.ConhecimentoEletronico.visible(true);
        _knoutPesquisar.CTe.visible(true);
        _knoutPesquisar.Carga.visible(true);
        _knoutPesquisar.CodigoCargaEmbarcador.visible(false);
        _knoutPesquisar.Veiculo.visible(true);
        _knoutPesquisar.Motorista.cssClass("col col-xs-12 col-sm-12 col-md-3 col-lg-3");
        $("#liTabConhecimentos").show();

    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe) {
        _knoutPesquisar.Empresa.visible(false);
        _knoutPesquisar.DataInicioDigitalizacao.visible(false);
        _knoutPesquisar.DataFimDigitalizacao.visible(false);
        _knoutPesquisar.TipoPessoa.visible(false);
        _knoutPesquisar.Emitente.visible(false);
        _knoutPesquisar.Filial.visible(true);
        _knoutPesquisar.TipoCanhoto.visible(true);
        _knoutPesquisar.SelecionarTodos.visible(false);
        _knoutPesquisar.ConhecimentoEletronico.visible(false);
        _knoutPesquisar.ImportarPlanilha.visible(false);
        _knoutPesquisar.DownloadMassa.visible(false);
        _knoutPesquisar.ImportarCancelamentos.visible(false);
        _knoutPesquisar.ImportarCanhotos.visible(false);
        _knoutPesquisar.BuscaAvancada.visible(true);
        _knoutPesquisar.Destinatario.visible(false);
        _knoutPesquisar.Operador.visible(false);
        _knoutPesquisar.NumeroDocumentoOriginario.visible(false);
        _knoutPesquisar.SituacaoCarga.visible(false);
        _knoutPesquisar.SituacaoNotaFiscal.visible(false);

        _knoutPesquisar.TipoServicoMultiCTe.val(true);

        $("#liTabConhecimentos").hide();
    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.Fornecedor) {
        _knoutPesquisar.Empresa.visible(false);
        _knoutPesquisar.TipoPessoa.visible(false);
        _knoutPesquisar.Emitente.visible(false);
        _knoutPesquisar.CTe.visible(true);
        _knoutPesquisar.Filial.visible(false);
        _knoutPesquisar.TipoCanhoto.visible(true);
        _knoutPesquisar.SelecionarTodos.visible(false);
        _knoutPesquisar.ConhecimentoEletronico.visible(false);
        _knoutPesquisar.ImportarPlanilha.visible(false);
        _knoutPesquisar.DownloadMassa.visible(false);
        _knoutPesquisar.ImportarCancelamentos.visible(false);
        _knoutPesquisar.ImportarCanhotos.visible(false);
        _knoutPesquisar.BuscaAvancada.visible(false);
        $("#tabTransportador").show();
        $("#liTabConhecimentos").show();
    }

    if (IsMobile() == true) {
        _knoutPesquisar.DownloadMassa.visible(false);
        _knoutPesquisar.ImportarPlanilha.visible(false);
        _knoutPesquisar.ImportarCancelamentos.visible(false);
        _knoutPesquisar.ImportarCanhotos.visible(false);
        _knoutPesquisar.BuscaAvancadaMobile.visible(true);
        _knoutPesquisar.BuscaAvancada.visible(false);
        _knoutPesquisar.Numeros.visible(false);
        _knoutPesquisar.Serie.visible(false);
        _knoutPesquisar.DataInicioDigitalizacao.visible(false);
        _knoutPesquisar.DataFimDigitalizacao.visible(false);
        _knoutPesquisar.TipoCanhoto.visible(false);
        _knoutPesquisar.SituacaoCanhoto.visible(false);
        _knoutPesquisar.SituacoesDigitalizacaoCanhoto.visible(false);
        _knoutPesquisar.CodigoCargaEmbarcador.visible(false);
        _knoutPesquisar.NumeroNFe.visible(false);
        _knoutPesquisar.Motorista.visible(false);
        _knoutPesquisar.Carga.visible(false);
        _knoutPesquisar.Filial.visible(false);
        _knoutPesquisar.Empresa.visible(false);
        _knoutPesquisar.SituacaoPgtoCanhoto.visible(false);
        _knoutPesquisar.ImportarDataEntrega.visible(false);
    }

    _knoutPesquisar.ConfirmarRecebimentoTodas.visible(false);
    _knoutPesquisar.JustificarTodas.visible(false);
    _knoutPesquisar.AlterarStatusTodos.visible(false);
}

function ConhecimentoEletronicoClick(e, sender) {
    setTimeout(function () {
        if (_knoutPesquisar.ConhecimentoEletronico.val() != "") {
            Salvar(_knoutPesquisar, "Canhoto/ConsultarConhecimentoEletronico", function (arg) {
                _knoutPesquisar.ConhecimentoEletronico.val("");
                if (arg.Success) {
                    if (arg.Data) {
                        if (arg.Data.Codigo > 0)
                            AdicionarConhecimentoNaLista(arg.Data);
                        $("#" + _knoutPesquisar.ConhecimentoEletronico.id).focus();
                    } else {
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                }
            });
        }
    }, 30);
}

function CarregarGridConhecimentos() {
    let excluir = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), evento: "onclick", metodo: ExcluirConhecimentoClick, tamanho: "15", icone: "" };
    let menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(excluir);

    let header = [
        { data: "Tipo", title: Localization.Resources.Canhotos.Canhoto.Tipo, visible: false },
        { data: "Codigo", title: Localization.Resources.Canhotos.Canhoto.Codigo, visible: false },
        { data: "DescricaoTipo", title: Localization.Resources.Canhotos.Canhoto.Tipo, width: "20%" },
        { data: "Chave", title: Localization.Resources.Canhotos.Canhoto.Chave, width: "70%", className: "text-align-left" }
    ];

    _knoutPesquisar.ListaCTes.val("");
    _knoutPesquisar.ListaConhecimentos.list = [];

    _gridConhecimentos = new BasicDataTable(_knoutPesquisar.ListaConhecimentos.idGrid, header, menuOpcoes);
    _gridConhecimentos.CarregarGrid([]);
}

function ExcluirConhecimentoClick(data) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Canhotos.Canhoto.RealmenteDesejaExcluirDocumento.format(data.Chave), function () {

        for (let i = 0; i < _knoutPesquisar.ListaConhecimentos.list.length; i++) {
            let documento = _knoutPesquisar.ListaConhecimentos.list[i];

            if (documento.Codigo == data.Codigo && documento.Tipo == data.Tipo) {
                _knoutPesquisar.ListaConhecimentos.list.splice(i, 1);
                break;
            }
        }

        RecarregarGridConhecimentos();
    });
}

function RecarregarGridConhecimentos() {
    var data = new Array();

    $.each(_knoutPesquisar.ListaConhecimentos.list, function (i, CTe) {
        data.push({
            Codigo: CTe.Codigo,
            Chave: CTe.Chave,
            Tipo: CTe.Tipo,
            DescricaoTipo: CTe.Tipo == EnumTipoDocumentoFiltroPesquisa.CTe ? Localization.Resources.Canhotos.Canhoto.CTe : Localization.Resources.Canhotos.Canhoto.NFe
        });
    });

    _gridConhecimentos.CarregarGrid(data);
}

function AdicionarConhecimentoNaLista(dados) {

    if (!_knoutPesquisar.ListaConhecimentos.list.some(function (o) { return o.Codigo == dados.Codigo && o.Tipo == dados.Tipo; }))
        _knoutPesquisar.ListaConhecimentos.list.push(dados);

    RecarregarGridConhecimentos();
}

function RetornoConhecimentoEletronico(data) {
    _knoutPesquisar.ConhecimentoEletronico.val(data.Chave);
    _knoutPesquisar.ConhecimentoEletronico.codEntity(data.Codigo);
    AdicionarConhecimentoNaLista(data);
}

function retornoMotorista(data) {
    _knoutPesquisar.Motorista.codEntity(data.Codigo);
    _knoutPesquisar.Motorista.val(data.Nome);
}

function retornoTipoOperacao(data) {
    _knoutPesquisar.TipoOperacao.codEntity(data.Codigo);
    _knoutPesquisar.TipoOperacao.val(data.Descricao);
    _knoutPesquisar.TipoOperacao.permitirEnviarImagemParaMultiplosCanhotos(data.PermitirEnviarImagemParaMultiplosCanhotos);
    _knoutPesquisar.TipoOperacao.permitirInformarDataEntregaParaMultiplosCanhotos(data.PermitirInformarDataEntregaParaMultiplosCanhotos);
}

function TipoPessoaChange(e, sender) {
    if (e.TipoPessoa.val() === EnumTipoPessoaGrupo.Pessoa) {
        e.Emitente.required = true;
        e.Emitente.visible(true);
        e.GrupoPessoa.required = false;
        e.GrupoPessoa.visible(false);
    } else if (e.TipoPessoa.val() === EnumTipoPessoaGrupo.GrupoPessoa) {
        e.Emitente.required = false;
        e.Emitente.visible(false);
        e.GrupoPessoa.required = true;
        e.GrupoPessoa.visible(true);
        LimparCampoEntity(e.Emitente);
    }
}

function ConfirmarCanhotosEmMassaClick() {
    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Canhoto_PermiteConfirmarRecebimentoCanhotoFisico, _PermissoesPersonalizadasCanhotos)) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Falha, Localization.Resources.Canhotos.Canhoto.UsuarioNaoTemPermissaoParaConfirmarRecebimentoCanhoto, 10000)
        return;
    }

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Canhotos.Canhoto.RealmenteDesejaConfirmarDigitalizacaoDosCanhotos, function () {
        var codigos = obterCodigosCanhotosExibidos();

        executarReST('Canhoto/ConfirmarDigitalizacao', { Codigos: JSON.stringify(codigos) }, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Canhotos.Canhoto.CanhotosConfirmadoComSucesso);
                    buscarCanhotos();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function AlterarDisponibilidadeCanhotosAPIEmMassaClick() {
    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Canhoto_PermitirRetornarStatusCanhotoAPIDigitalizacao, _PermissoesPersonalizadasCanhotos)) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.aviso, "Usuário não tem permissão para alterar a disponibilidade de consulta do canhoto", 10000)
        return;
    }

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, "Realmente deseja alterar a disponibilidade da consulta do canhoto na API", function () {
        const data = {
            Codigos: JSON.stringify(_gridCanhotos.ObterCodigosMultiplosSelecionados()),
            Disponibilizar: _knoutDisponibilidadeConsutaAPI.Disponibilizar.val()
        };

        executarReST('Canhoto/DisponibilizarCanhotoParaConsulta', data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Disponibilidade consulta API alterada");
                    buscarCanhotos();
                    Global.fecharModal("divModalDisponibilidadeConsultaAPI")
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function obterCodigosCanhotosExibidos() {
    var linhas = _gridCanhotos.GridViewTable().rows();
    var dados = linhas.data();
    var codigos = [];

    for (let i = 0; i < linhas[0].length; i++)
        codigos.push(dados[i].Codigo);

    return codigos;
}
function exibirModalCanhotoImagemEnviar() {
    _canhotoImagemEnviar.DataEnvioCanhoto.val(Global.DataHoraAtual());
    _canhotoImagemEnviar.DataEntregaNotaCliente.val(_knoutArquivo.DataEntregaCliente.val());

    Global.abrirModal('divModalCanhotoImagemEnviar');
    $("#divModalCanhotoImagemEnviar").on('hidden.bs.modal', function () {
        LimparCampos(_canhotoImagemEnviar);
    });
}

function fecharModalCanhotoImagemEnviar() {
    Global.fecharModal('divModalCanhotoImagemEnviar');
}

function enviarImagemCanhoto(dataEnvioCanhoto, dataEntregaNotaCliente, callbackSucesso) {
    var chamadosMotivos = _knoutArquivo.ChamadosEMotivos.val();
    var flagCancelarAtendimentoAutomaticamente = _knoutArquivo.CancelarAtendimentoAutomaticamente.val();

    function executarEnvio() {
        var file = document.getElementById(_knoutArquivo.Arquivo.id);
        var formData = new FormData();

        for (var i = 0; i < file.files.length; i++)
            formData.append("upload", file.files[i]);

        var data = {
            Codigo: _knoutArquivo.Codigo.val(),
            DataEnvioCanhoto: dataEnvioCanhoto,
            DataEntregaNotaCliente: dataEntregaNotaCliente
        };

        enviarArquivo("Canhoto/EnviarImagemCanhoto?callback=?", data, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Canhotos.Canhoto.CanhotoEnviadoComSucesso);
                    LimparTodosCampos();
                    buscarCanhotos();
                    file.value = null;

                    if (callbackSucesso instanceof Function)
                        callbackSucesso();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            }
        });
    }

    if (flagCancelarAtendimentoAutomaticamente) {
        executarReST('Canhoto/BuscarSeExisteChamadoAbertoParaOCanhoto', { codigoCanhoto: _knoutArquivo.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data != null) {
                    exibirConfirmacao(
                        Localization.Resources.Gerais.Geral.Confirmacao,
                        `Existem atendimentos em aberto para a NFe:<br/><i>Atendimento(s): </i><b>${arg.Data}.<br/></b>Essa ação irá finalizar os chamados em aberto. Deseja prosseguir?`, function () {
                            executarEnvio()
                        });
                }
                else
                    executarEnvio();
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    } else {
        executarEnvio();
    }
}


function enviarImagemParaMultiplosCanhotos(dataEnvioCanhoto, dataEntregaNotaCliente, callbackSucesso) {
    var file = document.getElementById(_knoutArquivo.Arquivo.id);
    var formData = new FormData();

    formData.append("upload", file.files[0]);

    //Primeiro envia somente a imagem, pois não é possível passar grande quantidade de dados nos parâmetros
    enviarArquivo("Canhoto/EnviarByteImagemParaMultiplosCanhotos?callback=?", null, formData, function (retorno) {
        file.value = null;

        if (retorno.Success) {
            if (retorno.Data) {

                //Envia os parâmetros para a imagem
                var dados = {
                    TokenImagem: retorno.Data,
                    DataEnvioCanhoto: dataEnvioCanhoto,
                    DataEntregaNotaCliente: dataEntregaNotaCliente,
                    SelecionarTodos: _knoutPesquisar.SelecionarTodos.val(),
                    CanhotosSelecionados: JSON.stringify(_gridCanhotos.ObterMultiplosSelecionados()),
                    CanhotosNaoSelecionados: JSON.stringify(_gridCanhotos.ObterMultiplosNaoSelecionados())
                };
                var dadosPesquisa = $.extend({}, RetornarObjetoPesquisa(_knoutPesquisar), dados);

                executarReST("Canhoto/EnviarImagemParaMultiplosCanhotos", dadosPesquisa, function (arg) {
                    if (arg.Success) {
                        if (arg.Data) {

                            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Canhotos.Canhoto.CanhotosEnviadosComSucesso);
                            LimparTodosCampos();
                            buscarCanhotos();

                            if (callbackSucesso instanceof Function)
                                callbackSucesso();
                        }
                        else
                            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                    }
                    else
                        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                });

            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg, 20000);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function enviarImagemCanhotoClick() {
    if (ValidarCamposObrigatorios(_canhotoImagemEnviar)) {
        if (_knoutArquivo.Multiplos.val()) {
            enviarImagemParaMultiplosCanhotos(_canhotoImagemEnviar.DataEnvioCanhoto.val(), _canhotoImagemEnviar.DataEntregaNotaCliente.val(), function () {
                fecharModalCanhotoImagemEnviar();
            });
        } else {
            enviarImagemCanhoto(_canhotoImagemEnviar.DataEnvioCanhoto.val(), _canhotoImagemEnviar.DataEntregaNotaCliente.val(), function () {
                fecharModalCanhotoImagemEnviar();
            });
        }
    }
    else
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
}

function enviarCanhotoClick() {
    if (_obrigatorioInformarDataEnvioCanhoto || _CONFIGURACAO_TMS.ExigirDataEntregaNotaClienteCanhotos)
        exibirModalCanhotoImagemEnviar();
    else {
        var file = document.getElementById(_knoutArquivo.Arquivo.id);

        if (_knoutArquivo.Multiplos.val()) {
            exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Canhotos.Canhoto.RealmenteDesejaEnviarArquivoParaTodosOsCanhotosSelecionados.format(file.files[0].name), function () {
                enviarImagemParaMultiplosCanhotos();
            });
        } else {
            exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Canhotos.Canhoto.RealmenteDesejaEnviarArquivoDeCanhotoDaNotaFiscal.format(file.files[0].name, _knoutArquivo.NumeroNota.val()), function () {
                enviarImagemCanhoto();
            });
        }
    }
}

function enviarMultiplasJustificativa() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Canhotos.Canhoto.VoceRealmenteDesejaConfirmarRecebimentoFisicoDeTodosOsCanhotosSelecionados, function () {
        var dados = {
            Observacao: _knouJustificativa.Observacao.val(),
            SelecionarTodos: _knoutPesquisar.SelecionarTodos.val(),
            CanhotosSelecionados: JSON.stringify(_gridCanhotos.ObterMultiplosSelecionados()),
            CanhotosNaoSelecionados: JSON.stringify(_gridCanhotos.ObterMultiplosNaoSelecionados())
        };

        var dadosPesquisa = $.extend({}, RetornarObjetoPesquisa(_knoutPesquisar), dados);

        executarReST("Canhoto/EnviarMultiplasJustificativas", dadosPesquisa, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    Global.fecharModal('divModalJustificarCanhoto');
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.EnviadoComSucesso);
                    LimparTodosCampos();
                    buscarCanhotos();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        })
    });
}

function enviarJustificativa() {
    var dados = {
        Codigo: _knouJustificativa.Codigo.val(),
        Observacao: _knouJustificativa.Observacao.val(),
        encerrarCargaAutomaticamente: false
    };
    executarReST("Canhoto/Justificar", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Canhotos.Canhoto.JustificativaEnviadoComSucesso);
                Global.fecharModal('divModalJustificarCanhoto');
                LimparTodosCampos();
                buscarCanhotos();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function enviarJustificativaClick() {
    if (ValidarCamposObrigatorios(_knouJustificativa)) {
        if (_knouJustificativa.Multiplos.val()) {
            enviarMultiplasJustificativa();
        } else {
            enviarJustificativa();
        }
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Canhotos.Canhoto.InformeJustificativa);
    }
}

function enviarAceitarDigitalizacaoCanhotoReprovadoClick(e) {
    if (ValidarCamposObrigatorios(_knouAceitarDigitalizacaoCanhotoReprovado))
        aceitarDigitalizacaoCanhotoReprovadoClick(e);
    else
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Canhotos.Canhoto.InformeJustificativa);
}

function rejeitarEnvioClick(e) {
    limparCamposKnoutMotivoInconsistenciaDigitalizacao();
    _knoutMotivoRejeicao.Codigo.val(e.Codigo);
    _knoutMotivoRejeicao.NumeroNota.val(e.Numero);
    _knoutMotivoRejeicao.CodigoCanhoto.val(e.CodigoCanhoto);
    _knoutMotivoRejeicao.MotivoRejeicaoDigitalizacao.val(e.MotivoRejeicaoDigitalizacao);
    Global.abrirModal('divModalMotivoRejeicaoDigitalizacao');
}

function justificarRejeicaoDigitalizacaoClick() {
    if (ValidarCamposObrigatorios(_knoutMotivoRejeicao)) {
        if (_knoutMotivoRejeicao.Observacoes.required == true && _knoutMotivoRejeicao.Observacoes.val().length < 20) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Canhotos.Canhoto.OCampoDeObservacaoDeveTerNoMinimo20Caracteres);
        } else if (_knoutMotivoRejeicao.MotivoRejeicaoDigitalizacao.required == true && _knoutMotivoRejeicao.MotivoRejeicaoDigitalizacao.val().length < 20) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Canhotos.Canhoto.OCampoDeMotivoDaRejeicaoDeveTerNoMinimo20Caracteres);
        } else {

            let codigos = [];
            if (_knoutMotivoRejeicao.Codigo.val() > 0)
                codigos.push(_knoutMotivoRejeicao.Codigo.val());
            if (_knoutMotivoRejeicao.CodigosCanhotosSelecionados.val())
                codigos.push(..._knoutMotivoRejeicao.CodigosCanhotosSelecionados.val());

            let dados = {
                Codigos: JSON.stringify(codigos),
                MotivoRejeicaoDigitalizacao: _knoutMotivoRejeicao.MotivoRejeicaoDigitalizacao.val(),
                Motivo: _knoutMotivoRejeicao.Motivo.codEntity(),
                Observacoes: _knoutMotivoRejeicao.Observacoes.val()
            };

            executarReST("Canhoto/RejeitarEnvio", dados, function (arg) {
                if (arg.Success) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Canhotos.Canhoto.DigitalizacaoDoCanhotoRejeitado);
                    LimparTodosCampos();
                    fecharModalMotivoRejeicaoDigitalizacao();
                    buscarCanhotos();
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                }
            });
        }
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function abrirModalEnviarCanhotoClick(e) {

    _knoutArquivo.Codigo.val(e.Codigo);
    _knoutArquivo.NumeroNota.val(e.Numero);
    _knoutArquivo.CodigoCanhoto.val(e.CodigoCanhoto);
    _knoutArquivo.DataEntregaCliente.val(e.DataEntregaClienteDescricao);
    _knoutArquivo.Multiplos.val(false);
    _knoutArquivo.ChamadosEMotivos.val(e.ChamadosEMotivos)
    _knoutArquivo.CancelarAtendimentoAutomaticamente.val(e.CancelarAtendimentoAutomaticamente)

    $("#" + _knoutArquivo.Arquivo.id).trigger("click");
}

function abrirModalAprovarImagemCanhotoRejeitadoImagensAgrupadasClick(e) {
    _knouAceitarDigitalizacaoCanhotoReprovado.Codigo.val(e.Codigo);
    _knouAceitarDigitalizacaoCanhotoReprovado.Observacao.val("");
    Global.abrirModal('divModalAceitarDigitalizacaoCanhotoReprovado');
}

function marcarTodosCanhotosVisualizacaoClick() {
    let listaImagensCanhoto = _knoutPesquisar.ImagensConferencia.val();
    let marcar = !_knoutPesquisar.MarcarTodosCanhotosVisualizacao.val;

    for (const canhoto of listaImagensCanhoto) {
        if (canhoto.PendenteAprovacao || canhoto.Rejeitado || canhoto.ValidacaoEmbarcador) {
            canhoto.Selecionada = marcar;
            let checkbox = document.getElementById('checkbox-imagem-' + canhoto.Codigo);
            if (checkbox) {
                checkbox.checked = marcar;
            }
        }
    }

    _knoutPesquisar.MarcarTodosCanhotosVisualizacao.val = marcar;
    _knoutPesquisar.MarcarTodosCanhotosVisualizacao.text(marcar ? Localization.Resources.Canhotos.Canhoto.DesmarcarTodos : Localization.Resources.Canhotos.Canhoto.MarcarTodos);
    _knoutPesquisar.ImagensConferencia.val(listaImagensCanhoto);
}


function fecharModalAprovarImagemCanhotoRejeitadoImagensAgrupadasClick(e) {
    LimparCampos(_knouAceitarDigitalizacaoCanhotoReprovado);
    Global.fecharModal('divModalAceitarDigitalizacaoCanhotoReprovado');
}

function abrirModalEnviarImagemMultiplosCanhotosClick(e) {
    _knoutArquivo.Multiplos.val(true);

    $("#" + _knoutArquivo.Arquivo.id).trigger("click");
}

function abrirModalJustificativaClick(e) {
    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Canhoto_PermiteAdicionarJustificativa, _PermissoesPersonalizadasCanhotos)) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Falha, Localization.Resources.Canhotos.Canhoto.UsuarioNaoTemPermissaoParaAdicionarJustificativa, 10000)
        return;
    }
    _knouJustificativa.Multiplos.val(false);
    abrirModalJustificativa(e);
}

function abrirModalMultiplasJustificativasClick(e) {
    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Canhoto_PermiteAdicionarJustificativa, _PermissoesPersonalizadasCanhotos)) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Falha, Localization.Resources.Canhotos.Canhoto.UsuarioNaoTemPermissaoParaAdicionarJustificativa, 10000)
        return;
    }
    _knouJustificativa.Multiplos.val(true);
    abrirModalJustificativa(e);
}

function abrirModalJustificativa(e) {
    _knouJustificativa.Codigo.val(e.Codigo);
    _knouJustificativa.NumeroNota.val(e.Numero);
    _knouJustificativa.CodigoCanhoto.val(e.CodigoCanhoto);
    _knouJustificativa.Observacao.val(e.Observacao);
    Global.abrirModal('divModalJustificarCanhoto');
}

function abrirModalAprovarCanhotoReprovadoClick(e) {
    _knouAceitarDigitalizacaoCanhotoReprovado.Codigo.val(e.Codigo);
    _knouAceitarDigitalizacaoCanhotoReprovado.NumeroNota.val(e.Numero);
    _knouAceitarDigitalizacaoCanhotoReprovado.CodigoCanhoto.val(e.CodigoCanhoto);
    _knouAceitarDigitalizacaoCanhotoReprovado.Observacao.val("");
    _knouAceitarDigitalizacaoCanhotoReprovado.ValidacaoIAComproveiHabilitada.val(_IntegrarCanhotosComValidadorIAComprovei && e.PossuiIntegracaoComprovei)
    Global.abrirModal('divModalAceitarDigitalizacaoCanhotoReprovado');
}

function abrirModalDisponibilidadeConsultaAPI() {
    const canhotosSelecionados = _gridCanhotos.ObterMultiplosSelecionados();
    if (canhotosSelecionados.length <= 0) return;

    const referencia = canhotosSelecionados[0].DigitalizacaoIntegrada;
    const canhotosDiferentes = canhotosSelecionados.filter(function (canhoto) {
        return canhoto.DigitalizacaoIntegrada !== referencia;
    })

    if (canhotosDiferentes > 0)
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "Canhotos com situações diferentes! Favor selecionar apenas canhotos com a mesma situação.");
    else
        Global.abrirModal('divModalDisponibilidadeConsultaAPI');
}
function extraviadoVisivelClick(e) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Canhotos.Canhoto.VoceRealmenteDesejaInformarQueEsteCanhotoFoiExtraviado, function () {
        var dados = {
            Codigo: e.Codigo
        }
        executarReST("Canhoto/InformarCanhotoExtraviado", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Canhotos.Canhoto.ExtravioRegistradoComSucesso);
                    LimparTodosCampos();
                    buscarCanhotos();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        })
    });
}
function naoEntregueVisivelClick(e) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Canhotos.Canhoto.VoceRealmenteDesejaInformarQueEsseCanhotoNaoFoiEntregue, function () {
        var dados = {
            Codigo: e.Codigo
        }
        executarReST("Canhoto/InformarCanhotoNaoEntregue", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Canhotos.Canhoto.CanhotoNaoEntregueRegistradoComSucesso);
                    LimparTodosCampos();
                    buscarCanhotos();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        })
    });
}

function AlterarStatusTodosClick() {
    AbrirModalObservacaoAlteracaoCanhotoClick()
}

function enviarMultiplosCanhotosFisicoClick(e) {
    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Canhoto_PermiteConfirmarRecebimentoCanhotoFisico, _PermissoesPersonalizadasCanhotos)) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Falha, Localization.Resources.Canhotos.Canhoto.UsuarioNaoTemPermissaoParaConfirmarRecebimentoCanhoto, 10000)
        return;
    }
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Canhotos.Canhoto.VoceRealmenteDesejaConfirmarRecebimentoFisicoDeTodosOsCanhotosSelecionados, function () {
        var dados = {
            SelecionarTodos: e.SelecionarTodos.val(),
            CanhotosSelecionados: JSON.stringify(_gridCanhotos.ObterMultiplosSelecionados()),
            CanhotosNaoSelecionados: JSON.stringify(_gridCanhotos.ObterMultiplosNaoSelecionados())
        }

        var dadosPesquisa = $.extend({}, RetornarObjetoPesquisa(_knoutPesquisar), dados);

        executarReST("Canhoto/EnviarMultiplosCanhotosFisico", dadosPesquisa, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.EnviadoComSucesso);
                    LimparTodosCampos();
                    buscarCanhotos();
                    if (arg.Data.Mensagem != "")
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Data.Mensagem, 10000);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        })
    });
}

function abrirModalInformarDataEntregaClick() {
    LimparCampos(_knoutInformarDataEntrega);
    Global.abrirModal('divModalDataEntregaCanhoto');
}

function informarDataEntregaCanhotoClick() {
    if (!ValidarCamposObrigatorios(_knoutInformarDataEntrega)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Canhotos.Canhoto.VoceRealmenteDesejaEnviarDataDeEntregaDeTodosOsCanhotosSelecionados, function () {
        var dados = {
            DataEntregaNotaCliente: _knoutInformarDataEntrega.DataEntregaNotaCliente.val(),
            SelecionarTodos: _knoutPesquisar.SelecionarTodos.val(),
            CanhotosSelecionados: JSON.stringify(_gridCanhotos.ObterMultiplosSelecionados()),
            CanhotosNaoSelecionados: JSON.stringify(_gridCanhotos.ObterMultiplosNaoSelecionados())
        };

        var dadosPesquisa = $.extend({}, RetornarObjetoPesquisa(_knoutPesquisar), dados);

        executarReST("Canhoto/EnviarMultiplasDataEntrega", dadosPesquisa, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    Global.fecharModal('divModalDataEntregaCanhoto');
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.EnviadoComSucesso);
                    LimparTodosCampos();
                    buscarCanhotos();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        })
    });
}

function RemoverArquivoClick(id) {
    uploader.removeFile(uploader.getFile(id));
    $("#" + id).remove();
}

function downloadCanhotoClick(e) {
    if (e.Codigo > 0 && e.GuidNomeArquivo != "") {
        var dados = {
            Codigo: e.Codigo
        }
        executarDownload("Canhoto/DownloadCanhoto", dados);
    } else {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Canhotos.Canhoto.CanhotoNaoEnviado, Localization.Resources.Canhotos.Canhoto.NaoFoiEnviadoCanhotoParaEstaNota);
    }
}

function imprimirCanhotoClick(e) {
    if (e.Codigo > 0) {
        executarDownload("Canhoto/ImprimirCanhoto", { Codigo: e.Codigo })
    } else {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Canhotos.Canhoto.CanhotoNaoEnviado, Localization.Resources.Canhotos.Canhoto.NaoFoiEnviadoCanhotoParaEstaNota)
    }
}

function detalhesCanhotoClick(e) {
    _knoutArquivo.Codigo.val(e.Codigo);
    BuscarDetalhesCanhoto(e.Codigo, function () {
        Global.abrirModal('ModalDivDetalhesCanhoto');
    });
}

function abrirModalEditarCanhotoClick(e) {
    let canvas = document.getElementById('canvas');
    let croppedImage = document.getElementById('croppedImage');
    let imageSrc = 'data:image/png;base64,' + e.Miniatura;

    let cropper = ImageCropper(canvas, croppedImage, imageSrc);

    let cropButton = document.getElementById('cropButton');
    let rotateButton = document.getElementById('rotateButton');

    cropButton.onclick = function () {
        const croppedImageData = cropper.getCroppedImageData();
        const blob = dataURLToBlob(croppedImageData);
        enviarImagemEditadaCanhoto(e.DataDigitalizacao, e.DataEntregaNotaCliente, blob, e.Codigo, function () {
            Global.fecharModal('ModalDivEditarCanhoto');
        });
    };

    rotateButton.onclick = function () {
        cropper.rotateImage();
    };

    Global.abrirModal('ModalDivEditarCanhoto');
}

function alterarDataConfirmacaoEntregaClick(e) {
    _knoutArquivo.Codigo.val(e.Codigo);

    LimparCampos(_canhotoDataEntregaCliente);
    _canhotoDataEntregaCliente.DataEntregaNotaCliente.val(e.DataEntregaClienteDescricao);
    Global.abrirModal('divModalAlterarDataEntrega');
}

function PreencherListaConhecimentos() {
    _knoutPesquisar.ListaCTes.val(JSON.stringify(_knoutPesquisar.ListaConhecimentos.list));
}

function dataURLToBlob(dataURL) {
    const byteString = atob(dataURL.split(',')[1]);
    const mimeString = dataURL.split(',')[0].split(':')[1].split(';')[0];
    const ab = new ArrayBuffer(byteString.length);
    const ia = new Uint8Array(ab);
    for (let i = 0; i < byteString.length; i++) {
        ia[i] = byteString.charCodeAt(i);
    }
    return new Blob([ab], { type: mimeString });
}

function enviarImagemEditadaCanhoto(dataEnvioCanhoto, dataEntregaNotaCliente, imagemBlob, codigo, callbackSucesso) {
    const formData = new FormData();
    formData.append("upload", imagemBlob, 'croppedImage.png'); // Adiciona o Blob ao FormData

    const data = {
        Codigo: codigo,
        DataEnvioCanhoto: dataEnvioCanhoto,
        DataEntregaNotaCliente: dataEntregaNotaCliente,
        EnviouImagemViaEdicao: true
    };
    exibirConfirmacao(Localization.Resources.Canhotos.Canhoto.EditarImagemCanhoto, "Tem certeza que deseja reenviar a imagem editada?", function () {
        enviarArquivo("Canhoto/EnviarImagemCanhoto?callback=?", data, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Canhotos.Canhoto.CanhotoEnviadoComSucesso);
                    LimparTodosCampos();
                    buscarCanhotos();

                    if (callbackSucesso instanceof Function)
                        callbackSucesso();
                }
                else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg, 20000);
                }
            }
            else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            }
        });
    });
}

function CancelarEdicaoImagemClick() {
    Global.fecharModal('ModalDivEditarCanhoto');
}

function buscarCanhotos() {
    _buscouMiniaturas = false;
    _knoutPesquisar.ImagensConferencia.val.removeAll();
    _knoutPesquisar.ImagensInteirasConferencia.val.removeAll();
    _knoutPesquisar.CanhotoInteiroAtual('');

    if (_gridCanhotos != null) {
        _gridCanhotos.Destroy();
        _gridCanhotos = null;
    }

    _knoutPesquisar.SelecionarTodos.val(false);

    let multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _knoutPesquisar.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    };

    let Detalhes = { descricao: Localization.Resources.Gerais.Geral.Detalhes, id: guid(), tamanho: 9, metodo: detalhesCanhotoClick };
    let Download = { descricao: Localization.Resources.Canhotos.Canhoto.DownloadImagem, id: guid(), metodo: downloadCanhotoClick, visibilidade: DownloadVisivel };
    let Imprimir = { descricao: Localization.Resources.Canhotos.Canhoto.ImprimirCanhoto, id: guid(), tamanho: 9, metodo: imprimirCanhotoClick, visibilidade: ImprimirCanhotoVisivel };
    let EnviarArquivo = { descricao: Localization.Resources.Canhotos.Canhoto.EnviarImagem, id: guid(), tamanho: 9, metodo: abrirModalEnviarCanhotoClick, visibilidade: EnviarImagemVisivel };
    let Anexos = { descricao: Localization.Resources.Gerais.Geral.Anexos, id: guid(), tamanho: 9, metodo: exibirAnexos };
    let RejeitarEnvio = { descricao: Localization.Resources.Canhotos.Canhoto.RejeitarDigitalizacao, id: guid(), tamanho: 9, metodo: rejeitarEnvioClick, visibilidade: ConfirmacaoVisivel };
    let AceitarEnvio = { descricao: Localization.Resources.Canhotos.Canhoto.AceitarDigitalizacao, id: guid(), tamanho: 9, metodo: aceitarEnvioClick, visibilidade: ConfirmacaoVisivel };
    let AceitarEnvioCanhotoRejeitado = { descricao: Localization.Resources.Canhotos.Canhoto.AceitarDigitalizacao, id: guid(), tamanho: 9, metodo: abrirModalAprovarCanhotoReprovadoClick, visibilidade: ConfirmacaoCanhotoRejeitadoVisivel };
    let Justificar = { descricao: Localization.Resources.Canhotos.Canhoto.Justificar, id: guid(), tamanho: 9, metodo: abrirModalJustificativaClick, visibilidade: ExtraviadoJustificarVisivel };
    let ReverterJustificativa = { descricao: Localization.Resources.Canhotos.Canhoto.ReverterJustificativa, id: guid(), tamanho: 9, metodo: abrirModalReverterJustificativaClick, visibilidade: VisibilidadeOpcaoReverterJustificativa };
    let Extraviado = { descricao: Localization.Resources.Canhotos.Canhoto.CanhotoExtraviado, id: guid(), tamanho: 9, metodo: extraviadoVisivelClick, visibilidade: ExtraviadoJustificarVisivel };
    let NaoEntregue = { descricao: Localization.Resources.Canhotos.Canhoto.CanhotoNaoEntregue, id: guid(), tamanho: 9, metodo: naoEntregueVisivelClick, visibilidade: NaoEntregueDigitalizadoVisivel };
    let BaixarCanhoto = { descricao: Localization.Resources.Canhotos.Canhoto.RecebidoFisicamente, id: guid(), tamanho: 9, metodo: AbrirModalCanhotoRecebidoFisicamenteClick, visibilidade: EnviarFiscamenteVisivel };
    let RemoverCanhotoLocalArmazenamento = { descricao: Localization.Resources.Canhotos.Canhoto.RemoverDoArmazenamento, id: guid(), tamanho: 9, metodo: removerCanhotoLocalArmazenamentoClick, visibilidade: RemoverCanhotoLocalArmazenamentoVisivel };
    let ReverterBaixaCanhoto = { descricao: Localization.Resources.Canhotos.Canhoto.ReverterBaixa, id: guid(), tamanho: 9, metodo: ReverterBaixaCanhotoClick, visibilidade: ReverterBaixaCanhotoVisivel };
    let LiberarPgto = { descricao: Localization.Resources.Canhotos.Canhoto.LiberarPagamento, id: guid(), tamanho: 9, metodo: liberarPgtoClick, visibilidade: PgtoVisivel };
    let RejeitarPgto = { descricao: Localization.Resources.Canhotos.Canhoto.RejeitarPagamento, id: guid(), tamanho: 9, metodo: rejeitarPgtoClick, visibilidade: PgtoVisivel };
    let AuditarRegistro = { descricao: Localization.Resources.Canhotos.Canhoto.AuditarRegistro, id: guid(), tamanho: 9, metodo: auditarRegistroSituacaoCanhoto, visibilidade: obterVisivilidadePelaSituacaoDaDigitalizacaoCanhoto };
    let DonwloadFotosNotaEntrega = { descricao: Localization.Resources.Canhotos.Canhoto.BaixarFotosNotaEntrega, metodo: baixarFotosNotaEntrega, visibilidade: visibilidadeBaixarFotosNotaEntrega };
    let AlterarImagemCanhotoDigitalizada = { descricao: Localization.Resources.Canhotos.Canhoto.AlterarCanhoto, id: guid(), tamanho: 9, metodo: abrirModalEnviarCanhotoClick, visibilidade: PermiteAlterarImagemCanhotoDigitalizada };
    let AlterarDataEntrega = { descricao: Localization.Resources.Canhotos.Canhoto.AlterarDataEntrega, id: guid(), tamanho: 9, metodo: alterarDataConfirmacaoEntregaClick, visibilidade: alterarDataEntregaVisivel };
    let Integracoes = { descricao: Localization.Resources.Canhotos.Canhoto.Integracoes, id: guid(), tamanho: 9, metodo: IntegracoesClick, visibilidade: IntegracoesVisivel };

    let menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [Detalhes, AceitarEnvio, AceitarEnvioCanhotoRejeitado, RejeitarEnvio, EnviarArquivo, Anexos, Extraviado, NaoEntregue, Justificar, ReverterJustificativa, BaixarCanhoto, Download, Imprimir, RemoverCanhotoLocalArmazenamento, ReverterBaixaCanhoto, LiberarPgto, RejeitarPgto, DonwloadFotosNotaEntrega, AuditarRegistro, AlterarImagemCanhotoDigitalizada, AlterarDataEntrega, Integracoes] };

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.Fornecedor)
        menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [Detalhes, Download] };

    if (IsMobile())
        menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 25, opcoes: [Detalhes, AceitarEnvio, AceitarEnvioCanhotoRejeitado, RejeitarEnvio, EnviarArquivo, Anexos, Extraviado, NaoEntregue, Justificar, ReverterJustificativa, BaixarCanhoto, Download, RemoverCanhotoLocalArmazenamento, ReverterBaixaCanhoto, LiberarPgto, RejeitarPgto, DonwloadFotosNotaEntrega, AuditarRegistro] };

    PreencherListaConhecimentos();

    LoadGridCanhotosInteiros();
    ControlarAbasVisualizacaoCanhotos(0, 0);

    if (_FormularioSomenteLeitura)
        menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [Detalhes, Download] };

    _gridCanhotos = new GridView("grid-canhoto", "Canhoto/Consultar", _knoutPesquisar, menuOpcoes, null, 10, SituacoesDigitalizacaoSubscribe, null, null, multiplaescolha, 50, null, null, null, false);

    _gridCanhotos.SetPermitirEdicaoColunas(true);
    _gridCanhotos.SetSalvarPreferenciasGrid(true);

    _gridCanhotos.onTableCreated(function (table) {
        iniciarControleAbas(table);

        table.on("draw.dt", function () {
            ExibirSliceCanhotos();
        })
    });

    _gridCanhotos.onTableDestroy(function (table) {
        table.off("draw.dt");
    });

    _gridCanhotos.CarregarGrid();

    exibirMultiplasOpcoes();
}

function exibirMultiplasOpcoes() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _knoutPesquisar.ConfirmarRecebimentoTodas.visible(false);
        _knoutPesquisar.JustificarTodas.visible(false);
        _knoutPesquisar.AlterarStatusTodos.visible(false);
        _knoutPesquisar.EnviarImagemCanhotos.visible(true);

        if (_knoutPesquisar.SituacaoCanhoto.val().length === 1) {
            var situacaoCanhoto = _knoutPesquisar.SituacaoCanhoto.val()[0];

            if (situacaoCanhoto == EnumSituacaoCanhoto.Pendente) {
                _knoutPesquisar.AlterarStatusTodos.visible(true);
                _knoutPesquisar.AlterarStatusTodos.text(Localization.Resources.Canhotos.Canhoto.ConfirmarEntregaPeloMotorista);
            }
            else if (situacaoCanhoto == EnumSituacaoCanhoto.EntregueMotorista) {
                _knoutPesquisar.AlterarStatusTodos.visible(true);
                _knoutPesquisar.AlterarStatusTodos.text(Localization.Resources.Canhotos.Canhoto.ConfirmarRecebimentoFisicamente);
            }
            else if (situacaoCanhoto == EnumSituacaoCanhoto.RecebidoFisicamente) {
                _knoutPesquisar.AlterarStatusTodos.visible(true);
                _knoutPesquisar.AlterarStatusTodos.text(Localization.Resources.Canhotos.Canhoto.ConfirmarEnvioAoCliente);
            }
            else if (situacaoCanhoto == EnumSituacaoCanhoto.EnviadoCliente) {
                _knoutPesquisar.AlterarStatusTodos.visible(true);
                _knoutPesquisar.AlterarStatusTodos.text(Localization.Resources.Canhotos.Canhoto.ConfirmarRecebimentoPeloCliente);
            }
        }
    }
    else if (
        _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe &&
        _knoutPesquisar.TipoCanhoto.val() != EnumTipoCanhoto.Todos && (_knoutPesquisar.SituacaoCanhoto.val().length === 1 && _knoutPesquisar.SituacaoCanhoto.val()[0] == EnumSituacaoCanhoto.Pendente) &&
        (_knoutPesquisar.SelecionarTodos.val() || _gridCanhotos.ObterMultiplosSelecionados().length > 0)
    ) {
        _knoutPesquisar.ConfirmarRecebimentoTodas.visible(true);
        _knoutPesquisar.JustificarTodas.visible(true);
        _knoutPesquisar.AlterarStatusTodos.visible(false);
        _knoutPesquisar.EnviarImagemCanhotos.visible(_knoutPesquisar.TipoOperacao.permitirEnviarImagemParaMultiplosCanhotos() && _knoutPesquisar.TipoOperacao.codEntity() > 0);
        _knoutPesquisar.InformarDataEntregaCanhotos.visible(_knoutPesquisar.TipoOperacao.permitirInformarDataEntregaParaMultiplosCanhotos() && _knoutPesquisar.TipoOperacao.codEntity() > 0);
    }
    else {
        _knoutPesquisar.ConfirmarRecebimentoTodas.visible(false);
        _knoutPesquisar.JustificarTodas.visible(false);
        _knoutPesquisar.AlterarStatusTodos.visible(false);
        _knoutPesquisar.EnviarImagemCanhotos.visible(false);
        _knoutPesquisar.TipoOperacao.visible(true);
        _knoutPesquisar.EnviarImagemCanhotos.visible(_knoutPesquisar.TipoOperacao.permitirEnviarImagemParaMultiplosCanhotos() && _knoutPesquisar.TipoOperacao.codEntity() > 0);

        if (_gridCanhotos.ObterMultiplosSelecionados().length > 0)
            _knoutPesquisar.OpcoesMenu.visible(_configuracoesParaCanhoto.PermitirRetornarStatusCanhotoNaAPIDigitalizacao);
    }

    if (
        _CONFIGURACAO_TMS.UtilizaPgtoCanhoto &&
        (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe) &&
        (_knoutPesquisar.SituacoesDigitalizacaoCanhoto.val() == EnumSituacaoDigitalizacaoCanhoto.Digitalizado) &&
        (_knoutPesquisar.SelecionarTodos.val() || (_gridCanhotos.ObterMultiplosSelecionados().length > 0))
    ) {
        _knoutPesquisar.LiberarPagamentoCanhotos.visible(true);
        _knoutPesquisar.RejeitarPagamentoCanhotos.visible(true);
    }
    else {
        _knoutPesquisar.LiberarPagamentoCanhotos.visible(false);
        _knoutPesquisar.RejeitarPagamentoCanhotos.visible(false);
    }
    if (_permitirEnviarImagemParaMultiplosCanhotos)
        _knoutPesquisar.EnviarImagemCanhotos.visible(true);
}

function EnviarImagemVisivel(e) {
    if ((e.SituacaoDigitalizacaoCanhoto != EnumSituacaoDigitalizacaoCanhoto.Digitalizado && e.SituacaoDigitalizacaoCanhoto != EnumSituacaoDigitalizacaoCanhoto.Cancelada && e.SituacaoDigitalizacaoCanhoto != EnumSituacaoDigitalizacaoCanhoto.AgIntegracao) || e.DescricaoSituacaoIA.includes("Falha")) {
        return true;
    } else {
        return false;
    }
}

function alterarDataEntregaVisivel(e) {
    if (e.ExibirOpcaoAlterarDataEntrega && _obrigatorioInformarDataEnvioCanhoto)
        return true;
    else
        return false;
}

function ExtraviadoJustificarVisivel(e) {
    if ((_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe || _CONFIGURACAO_TMS.DisponibilizarOpcaoDeCanhotoExtraviado)
        && _gridCanhotos.ObterMultiplosSelecionados().length == 0 && !_knoutPesquisar.SelecionarTodos.val()
        && (e.SituacaoCanhoto == EnumSituacaoCanhoto.Pendente)) {
        return true;
    } else {
        return false;
    }
}
function NaoEntregueDigitalizadoVisivel(e) {
    return e.SituacaoDigitalizacaoCanhoto == EnumSituacaoDigitalizacaoCanhoto.PendenteDigitalizacao && e.SituacaoNotaFiscal != EnumSituacaoNotaFiscal.NaoEntregue;
}

function PgtoVisivel(e) {
    //&& e.SituacaoPgtoCanhoto == EnumSituacaoPgtoCanhoto.Pendente
    if (_CONFIGURACAO_TMS.UtilizaPgtoCanhoto && _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe && e.SituacaoDigitalizacaoCanhoto == EnumSituacaoDigitalizacaoCanhoto.Digitalizado) {
        return true;
    } else {

        return false;
    }
}

function EnviarFiscamenteVisivel(e) {
    if (
        _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe
        && _gridCanhotos.ObterMultiplosSelecionados().length == 0
        && !_knoutPesquisar.SelecionarTodos.val()
        && e.SituacaoCanhoto != EnumSituacaoCanhoto.RecebidoFisicamente
        && e.SituacaoCanhoto != EnumSituacaoCanhoto.Cancelado
    ) {
        return true;
    } else {
        return false;
    }
}

function DownloadVisivel(e) {
    if (e.SituacaoDigitalizacaoCanhoto == EnumSituacaoDigitalizacaoCanhoto.AgAprovocao || e.SituacaoDigitalizacaoCanhoto == EnumSituacaoDigitalizacaoCanhoto.Digitalizado) {
        return true;
    } else {
        return false;
    }
}

function ConfirmacaoVisivel(e) {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe && e.SituacaoDigitalizacaoCanhoto == EnumSituacaoDigitalizacaoCanhoto.AgAprovocao) {
        return true;
    } else {
        return false;
    }
}

function ConfirmacaoCanhotoRejeitadoVisivel(e) {
    return (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe &&
        e.SituacaoDigitalizacaoCanhoto == EnumSituacaoDigitalizacaoCanhoto.DigitalizacaoRejeitada &&
        _configuracoesParaCanhoto.PermitirAprovarDigitalizacaoDeCanhotoRejeitado);
}

function RemoverCanhotoLocalArmazenamentoVisivel(e) {
    return (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe && e.SituacaoCanhoto != EnumSituacaoCanhoto.Pendente && e.CodigoLocalArmazenamento > 0);
}

function ReverterBaixaCanhotoVisivel(e) {
    return (
        _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe
        && e.SituacaoCanhoto != EnumSituacaoCanhoto.Pendente
        && e.CodigoLocalArmazenamento == 0
        && e.SituacaoCanhoto != EnumSituacaoCanhoto.Cancelado
    );
}

function ImprimirCanhotoVisivel(e) {
    return (_gridCanhotos.ObterMultiplosSelecionados().length == 0 && !_knoutPesquisar.SelecionarTodos.val());
}
function IntegracoesVisivel(e) {
    return (_gridCanhotos.ObterMultiplosSelecionados().length == 0 && !_knoutPesquisar.SelecionarTodos.val() && e.EnvioCanhotoFaturaHabilitado);
}
function aceitarEnvioClick(e) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Canhotos.Canhoto.VoceRealmenteDesejaAceitarEnvioDoCanhotoDigitalmente, function () {
        var dados = {
            Codigo: e.Codigo
        };
        executarReST("Canhoto/AceitarEnvio", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.EnviadoComSucesso);
                    LimparTodosCampos();
                    buscarCanhotos();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function aceitarDigitalizacaoCanhotoReprovadoClick(e) {
    let codigos = [];
    if (_knouAceitarDigitalizacaoCanhotoReprovado.Codigo.val() > 0)
        codigos.push(_knouAceitarDigitalizacaoCanhotoReprovado.Codigo.val());
    if (_knoutMotivoRejeicao.CodigosCanhotosSelecionados.val())
        codigos.push(..._knouAceitarDigitalizacaoCanhotoReprovado.CodigosCanhotosSelecionados.val());

    let dados = {
        Codigos: JSON.stringify(codigos),
        Observacao: _knouAceitarDigitalizacaoCanhotoReprovado.Observacao.val(),
        HouveFalhaNaValidacao: _knouAceitarDigitalizacaoCanhotoReprovado.HouveFalhaNaValidacao.val(),
        FalhaIA_Comprovante: _knouAceitarDigitalizacaoCanhotoReprovado.FalhaIA_Comprovante.val(),
        FalhaIA_NumeroDocumento: _knouAceitarDigitalizacaoCanhotoReprovado.FalhaIA_NumeroDocumento.val(),
        FalhaIA_Data: _knouAceitarDigitalizacaoCanhotoReprovado.FalhaIA_Data.val(),
        FalhaIA_Assinatura: _knouAceitarDigitalizacaoCanhotoReprovado.FalhaIA_Assinatura.val(),
    };
    executarReST("Canhoto/AceitarDigitalizacaoCanhotoReprovado", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.EnviadoComSucesso);
                Global.fecharModal('divModalAceitarDigitalizacaoCanhotoReprovado');
                LimparTodosCampos();
                buscarCanhotos();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function liberarPgtoClick(e) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Canhotos.Canhoto.VoceRealmenteDesejaLiberarPagamento, function () {
        var dados = {
            Codigo: e.Codigo
        };

        executarReST("Canhoto/LiberarPgto", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Canhotos.Canhoto.LiberacaoParaPagamentoRealizadaComSucesso);
                    LimparTodosCampos();
                    buscarCanhotos();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    });
}

function liberarPagamentoMultiplosCanhotosClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Canhotos.Canhoto.VoceRealmenteDesejaLiberarPagamentoDeTodosOsCanhotosSelecionados, function () {
        var dados = {
            SelecionarTodos: _knoutPesquisar.SelecionarTodos.val(),
            CanhotosSelecionados: JSON.stringify(_gridCanhotos.ObterMultiplosSelecionados()),
            CanhotosNaoSelecionados: JSON.stringify(_gridCanhotos.ObterMultiplosNaoSelecionados())
        }

        var dadosPesquisa = $.extend({}, RetornarObjetoPesquisa(_knoutPesquisar), dados);

        executarReST("Canhoto/LiberarMultiplosPagamentos", dadosPesquisa, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    if (retorno.Data.CanhotosLiberadoPagamento > 0) {
                        if (retorno.Data.CanhotosLiberadoPagamento > 1)
                            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Canhotos.Canhoto.LiberacoesParaPagamentosRealizadasComSucesso.format(retorno.Data.CanhotosLiberadoPagamento));
                        else
                            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Canhotos.Canhoto.UmaLiberacaoParaPagamentoRealizadaComSucesso);
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Canhotos.Canhoto.NenhumLiberacaoParaPagamentoFoiRealizada);

                    LimparTodosCampos();
                    buscarCanhotos();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        })
    });
}

function rejeitarPgtoClick(e) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Canhotos.Canhoto.VoceRealmenteDesejaRejeitarPagamentoDisponibilizarCanhotoParaNovaDigitalizacao, function () {
        var dados = {
            Codigo: e.Codigo
        };

        executarReST("Canhoto/RejeitarPgto", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Canhotos.Canhoto.RejeicaoDePagamentoRealizadaComSucesso);
                    LimparTodosCampos();
                    buscarCanhotos();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    });
}


function rejeitarPagamentoMultiplosCanhotosClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Canhotos.Canhoto.VoceRealmenteDesejaRejeitarPagamentoDeTodosOsCanhotosSelecionados, function () {
        var dados = {
            SelecionarTodos: _knoutPesquisar.SelecionarTodos.val(),
            CanhotosSelecionados: JSON.stringify(_gridCanhotos.ObterMultiplosSelecionados()),
            CanhotosNaoSelecionados: JSON.stringify(_gridCanhotos.ObterMultiplosNaoSelecionados())
        }

        var dadosPesquisa = $.extend({}, RetornarObjetoPesquisa(_knoutPesquisar), dados);

        executarReST("Canhoto/RejeitarMultiplosPagamentos", dadosPesquisa, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    if (retorno.Data.CanhotosRejeitadoPagamento > 0) {
                        if (retorno.Data.CanhotosRejeitadoPagamento > 1)
                            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Canhotos.Canhoto.RejeicoesDePagamentosRealizadasComSucesso.format(retorno.Data.CanhotosRejeitadoPagamento));
                        else
                            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Canhotos.Canhoto.UmaRejeicaoDePagamentoRealizadaComSucesso);
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Canhotos.Canhoto.NenhumRejeicaoDePagamentoFoiRealizada);

                    LimparTodosCampos();
                    buscarCanhotos();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        })
    });
}

function removerCanhotoLocalArmazenamentoClick(e) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Canhotos.Canhoto.DesejaRealmenteRemoverCanhotoDoLocalDeArmazenamentoAtual + ' (' + e.DescricaoLocalArmazenamento + ')?<br /><br /><strong class="text-warning">' + Localization.Resources.Canhotos.Canhoto.ProcessoIrreversivelNaoPossivelColocaloNoMesmoLocal + '</strong>', function () {
        executarReST("Canhoto/RemoverCanhotoLocalArmazenamento", { Codigo: e.Codigo }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Canhotos.Canhoto.CanhotoRemovidoDoLocalDeArmazenamentoComSucesso);
                    LimparTodosCampos();
                    buscarCanhotos();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function ReverterBaixaCanhotoClick(e) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Canhotos.Canhoto.DesejaRealmenteReverterBaixaDesteCanhotoSituacaoDoCanhotoSeraSetadaParaPendenteSeraNecessarioRealizarBaixaNovamente, function () {
        executarReST("Canhoto/ReverterBaixaCanhoto", { Codigo: e.Codigo }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Canhotos.Canhoto.BaixaDoCanhotoRevertidaComSucesso);
                    _gridCanhotos.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}


function LimparTodosCampos() {
    _knoutPesquisar.ConfirmarRecebimentoTodas.visible(false);
    _knoutPesquisar.JustificarTodas.visible(false);
    _knoutPesquisar.AlterarStatusTodos.visible(false);
    CarregarGridConhecimentos();
    _knoutPesquisar.ConhecimentoEletronico.val("");
}

function ControlarAbasVisualizacaoCanhotos(canhotosInteiro, canhotosAvulcos) {
    if (canhotosInteiro > 0 && canhotosAvulcos > 0) {
        $("#liImagensAgrupadas").show();
        $("#liImagensIndividuais").show();
        Global.ExibirAba("tabImagensAgrupadas");
        //$("#liImagensAgrupadas a").click();
    } else if (canhotosInteiro > 0) {
        $("#liImagensAgrupadas").hide();
        $("#liImagensIndividuais").show();
        Global.ExibirAba("tabImagensIndividuais");
        //$("#liImagensIndividuais a").click();
    } else {
        $("#liImagensAgrupadas").show();
        $("#liImagensIndividuais").hide();
        Global.ExibirAba("tabImagensAgrupadas");
        //$("#liImagensAgrupadas a").click();
    }
}

function ObterSituacaoDigitalizacaoPOD() {
    var p = new promise.Promise();

    _opcoesSituacaoDigitalizacaoPOD = new Array();

    _opcoesSituacaoDigitalizacaoPOD.push({ value: 0, text: "--" + Localization.Resources.Gerais.Geral.Selecione + "--", exigeObservacao: false });

    executarReST("MotivoInconsistenciaDigitacao/ObterTodos", null, function (r) {
        if (r.Success) {
            for (var i = 0; i < r.Data.length; i++)
                _opcoesSituacaoDigitalizacaoPOD.push({ value: r.Data[i].Codigo, text: r.Data[i].Descricao, exigeObservacao: r.Data[i].ExigeObservacao });
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }

        p.done();
    });

    return p;
};

function obterVisivilidadePelaSituacaoDaDigitalizacaoCanhoto(e) {

    if ((e.SituacaoDigitalizacaoCanhoto == EnumSituacaoDigitalizacaoCanhoto.Digitalizado && e.SituacaoCanhoto == EnumSituacaoCanhoto.RecebidoFisicamente)
        && (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Canhotos_PermitirRegistrarAuditoriaNosCanhotos, _PermissoesPersonalizadasCanhotos) || _CONFIGURACAO_TMS.UsuarioAdministrador)
        && (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe)) {
        return true;
    } else {
        return false;
    }

    return (
        (e.SituacaoDigitalizacaoCanhoto == EnumSituacaoDigitalizacaoCanhoto.Digitalizado || e.SituacaoCanhoto == EnumSituacaoCanhoto.RecebidoFisicamente)
        && (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Canhotos_PermitirRegistrarAuditoriaNosCanhotos, _PermissoesPersonalizadasCanhotos) || _CONFIGURACAO_TMS.UsuarioAdministrador)
        && (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe));
}

function auditarRegistroSituacaoCanhoto(e) {
    AbrirModalAuditarRegistroCanhotoClick(e.Codigo);
}

function fecharModalMotivoRejeicaoDigitalizacao() {
    Global.fecharModal("divModalMotivoRejeicaoDigitalizacao");
}

function callbackMotivoInconsistenciaDigitalizacao(data) {
    _knoutMotivoRejeicao.Motivo.codEntity(data.Codigo);
    _knoutMotivoRejeicao.Motivo.val(data.Descricao);

    if (data.ExigeObservacao) {
        _knoutMotivoRejeicao.Observacoes.required = true;
        _knoutMotivoRejeicao.Observacoes.text(Localization.Resources.Canhotos.Canhoto.Observacoes.getRequiredFieldDescription());
    } else {
        _knoutMotivoRejeicao.Observacoes.required = false;
        _knoutMotivoRejeicao.Observacoes.text(Localization.Resources.Canhotos.Canhoto.Observacoes.getFieldDescription());
    }
}

function limparCamposKnoutMotivoInconsistenciaDigitalizacao() {
    _knoutMotivoRejeicao.Observacoes.required = false;
    _knoutMotivoRejeicao.Observacoes.text(Localization.Resources.Canhotos.Canhoto.Observacoes.getFieldDescription());
    LimparCampos(_knoutMotivoRejeicao);
}

function baixarFotosNotaEntrega(data) {
    if (data != undefined) {
        if (data.Codigo > 0) {
            var dados = {
                Codigo: data.Codigo
            }
            executarDownload("Canhoto/DownloadFotosNotaEntrega", dados);
        } else {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, "Não foi possível realizar o donwload das fotos da nota de entrega.");
        }
    }
}

function visibilidadeBaixarFotosNotaEntrega() {
    return _CONFIGURACAO_TMS.Pais == EnumPaises.Exterior;
}

function buscarConfiguracoesParaCanhoto() {
    executarReST("Canhoto/BuscarConfiguracoesGeraisCanhoto", {}, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _configuracoesParaCanhoto = retorno.Data;

                if (_configuracoesParaCanhoto.PermitirRetornarStatusCanhotoNaAPIDigitalizacao)
                    _knoutPesquisar.DigitalizacaoIntegrada.visible(true);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function PermiteAlterarImagemCanhotoDigitalizada(e) {
    return _configuracoesParaCanhoto.PermitirAlterarImagemCanhotoDigitalizada
}

function aprovarSelecaoCanhotosVisualizacaoClick() {
    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Canhoto_PermiteConfirmarRecebimentoCanhotoFisico, _PermissoesPersonalizadasCanhotos)) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Falha, Localization.Resources.Canhotos.Canhoto.UsuarioNaoTemPermissaoParaConfirmarRecebimentoCanhoto, 10000);
        return;
    }

    let codigos = [];

    let listaCodigoCanhoto = _knoutPesquisar.ImagensConferencia.val();
    for (const canhoto of listaCodigoCanhoto) {
        if (canhoto.Selecionada === true) {
            codigos.push(canhoto.Codigo);
        }
    }

    if (codigos.length <= 0) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, "Selecione pelo menos um canhoto para aprovar seleção.");
        return;
    }

    executarReST('Canhoto/ConfirmarDigitalizacaoSelecaoCanhotos', { Codigos: JSON.stringify(codigos) }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Canhotos.Canhoto.CanhotosConfirmadoComSucessoEmMassa);
                buscarCanhotos();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function rejeitarEnvioEmMassaClick() {
    LimparCampos(_knoutMotivoRejeicao);
    let codigos = [];
    let listaCodigoCanhoto = _knoutPesquisar.ImagensConferencia.val();

    for (const canhoto of listaCodigoCanhoto) {
        if (canhoto.Selecionada) {
            if (canhoto.Rejeitado) {
                exibirMensagem(
                    tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, "O Canhoto já foi rejeitado anteriormente."
                );
                return;
            }

            if (canhoto.PendenteAprovacao || canhoto.ValidacaoEmbarcador) {
                codigos.push(canhoto.Codigo);
            }
        }
    }

    if (codigos.length <= 0) {
        exibirMensagem(
            tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, "Selecione pelo menos um canhoto 'Aguardando Aprovação' para rejeitar seleção."
        );
        return;
    }

    _knoutMotivoRejeicao.CodigosCanhotosSelecionados.val(codigos);
    Global.abrirModal('divModalMotivoRejeicaoDigitalizacao');
}

function abrirModalAprovarImagemCanhotoRejeitadoEmMassaClick(e) {
    LimparCampos(_knouAceitarDigitalizacaoCanhotoReprovado);

    let codigos = [];
    let listaCodigoCanhoto = _knoutPesquisar.ImagensConferencia.val();
    for (const canhoto of listaCodigoCanhoto) {
        if ((canhoto.PendenteAprovacao || canhoto.ValidacaoEmbarcador || canhoto.Rejeitado) && canhoto.Selecionada === true) {
            codigos.push(canhoto.Codigo);
        }
    }

    if (codigos.length <= 0) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, "Selecione pelo menos um canhoto com a 'Aguardando Aprovação' para aprovar seleção.");
        return;
    }

    _knouAceitarDigitalizacaoCanhotoReprovado.ValidacaoIAComproveiHabilitada.val(_IntegrarCanhotosComValidadorIAComprovei);
    _knouAceitarDigitalizacaoCanhotoReprovado.CodigosCanhotosSelecionados.val(codigos);
    _knouAceitarDigitalizacaoCanhotoReprovado.Observacao.val("");
    Global.abrirModal('divModalAceitarDigitalizacaoCanhotoReprovado');
}

function fecharModalAprovarImagemCanhotoRejeitadoEmMassaClick(e) {
    LimparCampos(_knouAceitarDigitalizacaoCanhotoReprovado);
    Global.fecharModal('divModalAceitarDigitalizacaoCanhotoReprovado');
}
