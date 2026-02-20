/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCancelamentoOcorrencia.js" />
/// <reference path="EtapasCancelamento.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCancelamentoOcorrencia;
var _cancelamentoOcorrencia;
var _CRUDCancelamentoOcorrencia;
var _pesquisaCancelamentoOcorrencia;

var CancelamentoOcorrencia = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoCancelamentoOcorrencia.Todas), def: EnumSituacaoCancelamentoOcorrencia.Todas, text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Situacao });
}

var CRUDCancelamentoOcorrencia = function () {
    this.Limpar = PropertyEntity({ eventClick: limparCancelamentoClick, type: types.event, text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.LimparGerarNovoCancelamento.getFieldDescription(), idGrid: guid(), visible: ko.observable(false) });
}

var PesquisaCancelamentoOcorrencia = function () {
    this.NumeroOcorrencia = PropertyEntity({ text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.NumeroOcorrencia.getFieldDescription(), getType: typesKnockout.int });
    this.NumeroCTe = PropertyEntity({ text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.NumeroCTe.getFieldDescription(), getType: typesKnockout.int });
    this.DataInicial = PropertyEntity({ text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.DataInicio.getFieldDescription(), getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.DataLimite.getFieldDescription(), dateRangeInit: this.DataInicial, getType: typesKnockout.date });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicio;
    this.Carga = PropertyEntity({ text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Carga.getFieldDescription() , getType: typesKnockout.string });
    this.Operador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Operador.getFieldDescription() , idBtnSearch: guid() });
    this.Tipo = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoCancelamentoOcorrencia.obterOpcoeCancelamentoOcorrenciaPesquisa(), def: "", text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Tipo.getFieldDescription() });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoCancelamentoOcorrencia.Todas), options: EnumSituacaoCancelamentoOcorrencia.obterOpcoesPesquisa(), def: EnumSituacaoCancelamentoOcorrencia.Todas, text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Situacao.getFieldDescription() });
    this.NumeroControle = PropertyEntity({ text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.NumeroControle.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(false), getType: typesKnockout.string });
    this.NumeroBooking = PropertyEntity({ text:"Booking:", val: ko.observable(""), def: "", visible: ko.observable(false), getType: typesKnockout.string });
    this.NumeroOS = PropertyEntity({ text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.NumeroOS.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(false), getType: typesKnockout.string });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCancelamentoOcorrencia.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

//*******EVENTOS*******

function loadCancelamentoOcorrencia() {
    _cancelamentoOcorrencia = new CancelamentoOcorrencia();

    _CRUDCancelamentoOcorrencia = new CRUDCancelamentoOcorrencia();
    KoBindings(_CRUDCancelamentoOcorrencia, "knockoutCRUD");

    _pesquisaCancelamentoOcorrencia = new PesquisaCancelamentoOcorrencia();
    KoBindings(_pesquisaCancelamentoOcorrencia, "knockoutPesquisaCancelamentoOcorrencia", false, _pesquisaCancelamentoOcorrencia.Pesquisar.id);

    HeaderAuditoria("OcorrenciaCancelamento", _cancelamentoOcorrencia);

    loadEtapasCancelamentoOcorrencia();
    loadDadosCancelamento();
    loadDocumentos();
    loadResumo();
    loadIntegracao();
    loadIntegracaoCTe();
    loadConexaoSignalRCancelamentoOcorrencia();
    
    // Inicia as buscas
    new BuscarFuncionario(_pesquisaCancelamentoOcorrencia.Operador);

    LimparCamposCancelamento();
    BuscarCancelamentoOcorrencia();
    SetarCamposMultiModal();
}

function limparCancelamentoClick(e, sender) {
    LimparCamposCancelamento();
}

//*******MÉTODOS*******

function BuscarCancelamentoOcorrencia() {
    var editar = { descricao: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Editar, id: "clasEditar", evento: "onclick", metodo: editarCancelamentoOcorrencia, tamanho: "15", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridCancelamentoOcorrencia = new GridView(_pesquisaCancelamentoOcorrencia.Pesquisar.idGrid, "OcorrenciaCancelamento/Pesquisa", _pesquisaCancelamentoOcorrencia, menuOpcoes);
    _gridCancelamentoOcorrencia.CarregarGrid();
}

function editarCancelamentoOcorrencia(itemGrid) {
    // Limpa os campos
    LimparCamposCancelamento();

    // Esconde filtros
    _pesquisaCancelamentoOcorrencia.ExibirFiltros.visibleFade(false);

    // Busca dados
    BuscarCancelamentoPorCodigo(itemGrid.Codigo);
}

function BuscarCancelamentoPorCodigo(codigo, cb) {
    cb = !$.isFunction(cb) ? function () { } : cb;

    executarReST("OcorrenciaCancelamento/BuscarPorCodigo", { Codigo: codigo }, function (arg) {
        cb();
        if (arg.Data != null) {
            EditarCancelamentoOcorrencia(arg.Data);
            EditarDadosCancelamento(arg.Data);
            EditarDocumentos(arg.Data);
            PreencherResumo(arg.Data);
            SetarEtapasCancelamento();
            preencherIntegracaoOcorrenciaCancelamento(arg.Data);
            preencherIntegracaoCTeOcorrenciaCancelamento(arg.Data);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
    }, null);
}

function EditarCancelamentoOcorrencia(data) {
    _cancelamentoOcorrencia.Codigo.val(data.Codigo);
    _cancelamentoOcorrencia.Situacao.val(data.Situacao);
    _CRUDCancelamentoOcorrencia.Limpar.visible(true);
    if (data.permiteLiberarSemInutilizacao && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.OcorrenciaCancelamento_LiberarCancelamentoComCTeNaoInutilizado, _PermissoesPersonalizadasOcorrenciaCancelamento))
        _documentos.LiberarCancelamentoComCTeNaoInutilizado.visible(true);
    else
        _documentos.LiberarCancelamentoComCTeNaoInutilizado.visible(false);
        
}

function LimparCamposCancelamento() {
    LimparCampos(_cancelamentoOcorrencia);
    _CRUDCancelamentoOcorrencia.Limpar.visible(false);
    SetarEtapaInicioCancelamento();

    LimparCamposDadosCancelamento();
    LimparCamposResumo();
    limparIntegracaoOcorrenciaCancelamento();
    limparIntegracaoCTeOcorrenciaCancelamento();
}

function SetarCamposMultiModal() {
    if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) {
        _pesquisaCancelamentoOcorrencia.NumeroBooking.visible(true);
        _pesquisaCancelamentoOcorrencia.NumeroControle.visible(true);
        _pesquisaCancelamentoOcorrencia.NumeroOS.visible(true);
    }
}