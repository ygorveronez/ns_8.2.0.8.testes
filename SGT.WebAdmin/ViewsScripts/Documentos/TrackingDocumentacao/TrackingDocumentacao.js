/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/TipoTerminalImportacao.js" />
/// <reference path="../../Consultas/PedidoViagemNavio.js" />
/// <reference path="../../Consultas/Porto.js" />
/// <reference path="../../Enumeradores/EnumTipoIMO.js" />
/// <reference path="../../Enumeradores/EnumSituacaoTrackingDocumentacao.js" />
/// <reference path="../../Enumeradores/EnumTipoTrackingDocumentacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _trackingDocumentacao;
var _pesquisaTrackingDocumentacao;
var _gridTrackingDocumentacao;
var _gridSelecaoRegistro;

var _TipoIMO = [
    { text: "Apenas IMO", value: EnumTipoIMO.ApenasIMO },
    { text: "Todos os Documentos", value: EnumTipoIMO.TodosDocumentos }
];

var _SituacaoTrackingDocumentacao = [
    { text: "Sem Registros", value: EnumSituacaoTrackingDocumentacao.SemRegistros },
    { text: "Todos Registos", value: EnumSituacaoTrackingDocumentacao.TodosRegistros }
];

var _TipoTrackingDocumentacao = [
    { text: "Cabotagem", value: EnumTipoTrackingDocumentacao.Cabotagem },
    { text: "Feeder", value: EnumTipoTrackingDocumentacao.Feeder }
];

var _PesquisaTipoTrackingDocumentacao = [
    { text: "Todos", value: EnumTipoTrackingDocumentacao.Todos },
    { text: "Cabotagem", value: EnumTipoTrackingDocumentacao.Cabotagem },
    { text: "Feeder", value: EnumTipoTrackingDocumentacao.Feeder }
];

var _PesquisaTipoIMO = [
    { text: "Todos", value: EnumTipoIMO.Todos },
    { text: "Apenas IMO", value: EnumTipoIMO.ApenasIMO },
    { text: "Todos os Documentos", value: EnumTipoIMO.TodosDocumentos }
];

var TrackingDocumentacao = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Numero = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.IntegracaoPendente = PropertyEntity({ val: ko.observable(true), def: true, getType: typesKnockout.bool });

    this.PedidoViagemNavio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: ko.observable("*Viagem/Navio/Direção:"), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.PortoOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: ko.observable("*Porto de Origem:"), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.PortoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("Porto de Destino:"), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });

    this.TipoTrackingDocumentacao = PropertyEntity({ val: ko.observable(EnumTipoTrackingDocumentacao.Cabotagem), options: _TipoTrackingDocumentacao, def: EnumTipoTrackingDocumentacao.Cabotagem, text: "*Tipo: ", required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoIMO = PropertyEntity({ val: ko.observable(EnumTipoIMO.ApenasIMO), options: _TipoIMO, def: EnumTipoIMO.ApenasIMO, text: "*IMO: ", required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.SituacaoTrackingDocumentacao = PropertyEntity({ val: ko.observable(EnumSituacaoTrackingDocumentacao.SemRegistros), options: _SituacaoTrackingDocumentacao, def: EnumSituacaoTrackingDocumentacao.SemRegistros, text: "*Situação: ", required: false, visible: ko.observable(true), enable: ko.observable(true) });
    
    this.Pesquisa = PropertyEntity({ eventClick: PesquisaRegistrosClick, type: types.event, text: "Pesquisar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Registros = PropertyEntity({ idGrid: guid(), enable: ko.observable(true) });

    // CRUD
    this.GerarTracking = PropertyEntity({ eventClick: gerarTrackingClick, type: types.event, text: "Gerar Tracking", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
}

var PesquisaTrackingDocumentacao = function () {

    this.PedidoViagemNavio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("Viagem/Navio/Direção:"), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.PortoOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("Porto de Origem:"), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.PortoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("Porto de Destino:"), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoTrackingDocumentacao = PropertyEntity({ val: ko.observable(EnumTipoTrackingDocumentacao.Todos), options: _PesquisaTipoTrackingDocumentacao, def: EnumTipoTrackingDocumentacao.Todos, text: "Tipo: ", required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoIMO = PropertyEntity({ val: ko.observable(EnumTipoIMO.Todos), options: _PesquisaTipoIMO, def: EnumTipoIMO.Todos, text: "IMO: ", required: false, visible: ko.observable(true), enable: ko.observable(true) });

    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTrackingDocumentacao.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}


//*******EVENTOS*******
function loadTrackingDocumentacao() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaTrackingDocumentacao = new PesquisaTrackingDocumentacao();
    KoBindings(_pesquisaTrackingDocumentacao, "knockoutPesquisaTrackingDocumentacao", false, _pesquisaTrackingDocumentacao.Pesquisar.id);

    // Instancia objeto principal
    _trackingDocumentacao = new TrackingDocumentacao();
    KoBindings(_trackingDocumentacao, "knockoutTrackingDocumentacao");

    new BuscarPorto(_trackingDocumentacao.PortoOrigem);
    new BuscarPorto(_trackingDocumentacao.PortoDestino);
    new BuscarPedidoViagemNavio(_trackingDocumentacao.PedidoViagemNavio);

    new BuscarPorto(_pesquisaTrackingDocumentacao.PortoOrigem);
    new BuscarPorto(_pesquisaTrackingDocumentacao.PortoDestino);
    new BuscarPedidoViagemNavio(_pesquisaTrackingDocumentacao.PedidoViagemNavio);

    HeaderAuditoria("TrackingDocumentacao", _trackingDocumentacao);

    // Inicia busca
    CriarGridConsultaRegistro();
    buscarTrackingDocumentacao();
}

function PesquisaRegistrosClick(e, sender) {
    if (ValidarCamposObrigatorios(_trackingDocumentacao)) 
        buscarRegistrosParaTracking();    
    else
        exibirCamposObrigatorio();
}

function gerarTrackingClick(e, sender) {
    Salvar(_trackingDocumentacao, "TrackingDocumentacao/GerarTracking", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridTrackingDocumentacao.CarregarGrid();
                limparCamposTrackingDocumentacao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function cancelarClick(e) {
    limparCamposTrackingDocumentacao();
}

function editarTrackingDocumentacaoClick(itemGrid) {
    // Limpa os campos
    limparCamposTrackingDocumentacao();

    // Seta o codigo do objeto
    _trackingDocumentacao.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_trackingDocumentacao, "TrackingDocumentacao/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                buscarRegistrosParaTracking();

                _pesquisaTrackingDocumentacao.ExibirFiltros.visibleFade(false);

                SetarEnableCamposKnockout(_trackingDocumentacao, false);
                _trackingDocumentacao.GerarTracking.visible(false);

                // Alternas os campos de CRUD
                _trackingDocumentacao.Cancelar.visible(true);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

//*******MÉTODOS*******

function CriarGridConsultaRegistro() {
    var somenteLeitura = false;

    var auditar = { descricao: "Auditoria", id: guid(), evento: "onclick", metodo: OpcaoAuditoria("TrackingDocumentacaoRegistro", "Codigo"), tamanho: "5", icone: "", visibilidade: VisibilidadeOpcaoAuditoria };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 5, opcoes: [auditar] };

    _gridSelecaoRegistro = new GridView(_trackingDocumentacao.Registros.idGrid, "TrackingDocumentacao/PesquisaRegistro", _trackingDocumentacao, menuOpcoes);
}

function buscarRegistrosParaTracking() {
    _gridSelecaoRegistro.CarregarGrid();
}

function buscarTrackingDocumentacao() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarTrackingDocumentacaoClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridTrackingDocumentacao = new GridView(_pesquisaTrackingDocumentacao.Pesquisar.idGrid, "TrackingDocumentacao/Pesquisa", _pesquisaTrackingDocumentacao, menuOpcoes, null);
    _gridTrackingDocumentacao.CarregarGrid();
}

function limparCamposTrackingDocumentacao() {
    SetarEnableCamposKnockout(_trackingDocumentacao, true);
    _trackingDocumentacao.GerarTracking.visible(true);
    LimparCampos(_trackingDocumentacao);
    buscarRegistrosParaTracking();
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}