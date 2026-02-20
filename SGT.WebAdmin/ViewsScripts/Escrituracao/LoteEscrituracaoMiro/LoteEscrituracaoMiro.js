/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _gridLoteEscrituracaoMiro;
var _CRUDLoteEscrituracaoMiro;
var _pesquisaLoteEscrituracao;

var CRUDLoteEscrituracaoMiro = function () {
    this.Limpar = PropertyEntity({ eventClick: limparLoteEscrituracaoClick, type: types.event, text: "Limpar (Gerar Nova LoteEscrituracao)", idGrid: guid(), visible: ko.observable(false) });
}

var PesquisaLoteEscrituracaoMiro = function () {

    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, visible: true, val: ko.observable() });
    this.DataFim = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date, visible: true });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;
    this.Numero = PropertyEntity({ text: "Número Lote:", maxlength: 12, enable: ko.observable(true), getType: typesKnockout.int });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Transportador:"), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoLoteEscrituracao.Todas), options: [], def: EnumSituacaoLoteEscrituracao.Todas, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridLoteEscrituracaoMiro.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}


//*******EVENTOS*******

function loadLoteEscrituracaoMiro() {
    _CRUDLoteEscrituracaoMiro = new CRUDLoteEscrituracaoMiro();
    KoBindings(_CRUDLoteEscrituracaoMiro, "knockoutCRUDMiro");

    _pesquisaLoteEscrituracaoMiro = new PesquisaLoteEscrituracaoMiro();
    KoBindings(_pesquisaLoteEscrituracaoMiro, "knockoutPesquisaLoteEscrituracaoMiro", false, _pesquisaLoteEscrituracaoMiro.Pesquisar.id);

    loadEtapasLoteEscrituracaoMiro();
    loadSelecaoDocumentosMiro();

    BuscarHTMLINtegracaoLoteEscrituracaoMiro();

    new BuscarTransportadores(_pesquisaLoteEscrituracaoMiro.Empresa);
    new BuscarCargas(_pesquisaLoteEscrituracaoMiro.Carga, null, null, null, null, null, null, null, null, true);

    BuscarLoteEscrituracaoMiro();
}


function limparLoteEscrituracaoClick(e, sender) {
    LimparCamposLoteEscrituracao();
    GridSelecaoDocumentos();
}


//*******MÉTODOS*******
function BuscarLoteEscrituracaoMiro() {
    let editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarLoteEscrituracao, tamanho: "15", icone: "" };
    let menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridLoteEscrituracaoMiro = new GridView(_pesquisaLoteEscrituracaoMiro.Pesquisar.idGrid, "LoteEscrituracaoMiro/Pesquisa", _pesquisaLoteEscrituracaoMiro, menuOpcoes);
    _gridLoteEscrituracaoMiro.CarregarGrid();
}

function editarLoteEscrituracao(itemGrid) {
    // Limpa os campos
    LimparCamposLoteEscrituracao();

    // Esconde filtros
    _pesquisaLoteEscrituracaoMiro.ExibirFiltros.visibleFade(false);

    // Busca dados
    BuscarLoteEscrituracaoPorCodigo(itemGrid.Codigo);
}

function BuscarLoteEscrituracaoPorCodigo(codigo, cb) {
    executarReST("LoteEscrituracaoMiro/BuscarPorCodigo", { Codigo: codigo }, function (arg) {
        if (arg.Data != null) {

            EditarLoteEscrituracao(arg.Data);

            EditarSelecaoDocumentos(arg.Data);
            SetarEtapasLoteEscrituracao();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function EditarLoteEscrituracao(data) {
    _loteEscrituracao.Situacao.val(data.Situacao);
    _loteEscrituracao.SituacaoNoCancelamento.val(data.SituacaoNoCancelamento);
    _CRUDLoteEscrituracaoMiro.Limpar.visible(true);
}

function LimparCamposLoteEscrituracao() {
    LimparCampos(_loteEscrituracao);
    _CRUDLoteEscrituracaoMiro.Limpar.visible(false);
    _loteEscrituracao.Situacao.val(EnumSituacaoLoteEscrituracao.Todas);
    SetarEtapasLoteEscrituracao();
    LimparCamposSelecaoDocumentos();
    $("#" + _etapaLoteEscrituracao.Etapa1.idTab).click();
}