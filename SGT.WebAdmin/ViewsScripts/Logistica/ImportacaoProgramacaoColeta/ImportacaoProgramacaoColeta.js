/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Enumeradores/EnumSituacaoImportacaoProgramacaoColeta.js" />
/// <reference path="Etapa.js" />
/// <reference path="ImportacaoPedido.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridImportacaoProgramacaoColeta;
var _pesquisaImportacaoProgramacaoColeta;
var _importacaoProgramacaoColeta;
var _CRUDImportacaoProgramacaoColeta;

var PesquisaImportacaoProgramacaoColeta = function () {
    this.NumeroImportacaoProgramacaoColeta = PropertyEntity({ text: "Número:", maxlength: 11 });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid() });
    this.ClienteDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destino:", idBtnSearch: guid() });
    this.ProdutoPadrao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto Padrão:", idBtnSearch: guid() });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoImportacaoProgramacaoColeta.Todos), options: EnumSituacaoImportacaoProgramacaoColeta.obterOpcoesPesquisa(), def: EnumSituacaoImportacaoProgramacaoColeta.Todos });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridImportacaoProgramacaoColeta.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
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
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var ImportacaoProgramacaoColeta = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoImportacaoProgramacaoColeta.Todos), options: EnumSituacaoImportacaoProgramacaoColeta.obterOpcoesPesquisa(), def: EnumSituacaoImportacaoProgramacaoColeta.Todos });

    this.NumeroImportacaoProgramacaoColeta = PropertyEntity({ text: "Número:", maxlength: 11, enable: ko.observable(false), visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo de Operação:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.ProdutoPadrao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Produto Padrão:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.ClienteDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Destino:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.NumeroRepeticoes = PropertyEntity({ getType: typesKnockout.int, text: "*Número de Repetições:", maxlength: 11, required: true, enable: ko.observable(true), configInt: { precision: 0, allowZero: true, thousands: "" } });
    this.IntervaloDiasGeracao = PropertyEntity({ getType: typesKnockout.int, text: "*Intervalo de Dias para Geração:", maxlength: 11, required: true, enable: ko.observable(true), configInt: { precision: 0, allowZero: true, thousands: "" } });
};

var CRUDImportacaoProgramacaoColeta = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Limpar = PropertyEntity({ eventClick: LimparCamposClick, type: types.event, text: "Limpar Campos / Novo", visible: ko.observable(true) });
};

//*******EVENTOS*******

function LoadImportacaoProgramacaoColeta() {
    _importacaoProgramacaoColeta = new ImportacaoProgramacaoColeta();
    KoBindings(_importacaoProgramacaoColeta, "knockoutImportacaoProgramacaoColeta");

    HeaderAuditoria("ImportacaoProgramacaoColeta", _importacaoProgramacaoColeta);

    _CRUDImportacaoProgramacaoColeta = new CRUDImportacaoProgramacaoColeta();
    KoBindings(_CRUDImportacaoProgramacaoColeta, "knockoutCRUDImportacaoProgramacaoColeta");

    _pesquisaImportacaoProgramacaoColeta = new PesquisaImportacaoProgramacaoColeta();
    KoBindings(_pesquisaImportacaoProgramacaoColeta, "knockoutPesquisaImportacaoProgramacaoColeta", false, _pesquisaImportacaoProgramacaoColeta.Pesquisar.id);

    new BuscarProdutos(_importacaoProgramacaoColeta.ProdutoPadrao);
    new BuscarTiposOperacao(_importacaoProgramacaoColeta.TipoOperacao);
    new BuscarClientes(_importacaoProgramacaoColeta.ClienteDestino);

    new BuscarTiposOperacao(_pesquisaImportacaoProgramacaoColeta.TipoOperacao);
    new BuscarClientes(_pesquisaImportacaoProgramacaoColeta.ClienteDestino);

    LoadEtapaImportacaoProgramacaoColeta();
    LoadImportacaoPedido();

    BuscarImportacoesProgramacaoColeta();
}

function AdicionarClick() {
    Salvar(_importacaoProgramacaoColeta, "ImportacaoProgramacaoColeta/Adicionar", function (r) {
        if (r.Success) {
            if (r.Data) {
                _gridImportacaoProgramacaoColeta.CarregarGrid();

                PreencherObjetoKnout(_importacaoProgramacaoColeta, r);
                SetarEnableCamposKnockout(_importacaoProgramacaoColeta, false);

                SetarEtapaImportacaoProgramacaoColeta();

                BuscarImportacaoPedido();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function editarImportacaoProgramacaoColeta(importacaoProgramacaoColetaGrid) {
    LimparCamposImportacaoProgramacaoColeta();
    _importacaoProgramacaoColeta.Codigo.val(importacaoProgramacaoColetaGrid.Codigo);
    BuscarPorCodigo(_importacaoProgramacaoColeta, "ImportacaoProgramacaoColeta/BuscarPorCodigo", function (arg) {
        _pesquisaImportacaoProgramacaoColeta.ExibirFiltros.visibleFade(false);
        _CRUDImportacaoProgramacaoColeta.Adicionar.visible(false);

        SetarEnableCamposKnockout(_importacaoProgramacaoColeta, false);
        SetarEtapaImportacaoProgramacaoColeta();

        BuscarImportacaoPedido();
    }, null);
}

function LimparCamposClick() {
    LimparCamposImportacaoProgramacaoColeta();
}

////*******MÉTODOS*******

function BuscarImportacoesProgramacaoColeta() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarImportacaoProgramacaoColeta, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridImportacaoProgramacaoColeta = new GridView(_pesquisaImportacaoProgramacaoColeta.Pesquisar.idGrid, "ImportacaoProgramacaoColeta/Pesquisa", _pesquisaImportacaoProgramacaoColeta, menuOpcoes);
    _gridImportacaoProgramacaoColeta.CarregarGrid();
}

function LimparCamposImportacaoProgramacaoColeta() {
    _CRUDImportacaoProgramacaoColeta.Adicionar.visible(true);

    SetarEnableCamposKnockout(_importacaoProgramacaoColeta, true);
    _importacaoProgramacaoColeta.NumeroImportacaoProgramacaoColeta.enable(false);

    LimparCampos(_importacaoProgramacaoColeta);

    SetarEtapaInicioImportacaoProgramacaoColeta();
}