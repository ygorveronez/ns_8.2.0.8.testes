/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="Filial.js" />
/// <reference path="TipoCarga.js" />
/// <reference path="TipoOperacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridOperador;
var _operador;
var _pesquisaOperador;

var PesquisaOperador = function () {
    this.Nome = PropertyEntity({ text: Localization.Resources.Operacional.RestricaoVisualizacaoCanhotos.Nome.getFieldDescription() });
    this.Ativo = PropertyEntity({ val: ko.observable(1), def: 1 });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridOperador.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Operacional.RestricaoVisualizacaoCanhotos.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Operacional.RestricaoVisualizacaoCanhotos.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
};

var Operador = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoOperadorCanhoto = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Filiais = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.TiposOperacao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.TiposCarga = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0) });

    this.RestricaoAtiva = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PossuiFiltroTipoOperacao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.VisualizaCargasSemTipoOperacao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Operacional.RestricaoVisualizacaoCanhotos.Cancelar, visible: ko.observable(false) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Operacional.RestricaoVisualizacaoCanhotos.Salvar, visible: ko.observable(false) });
};

var Restricao = function () {
    this.RestricaoAtiva = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Operacional.RestricaoVisualizacaoCanhotos.RestricaoAtiva });
}

//*******EVENTOS*******

function loadRestricaoVisualizacaoCanhotos() {
    _operador = new Operador();
    KoBindings(_operador, "knockoutRestricaoVisualizacaoCanhotos");

    _pesquisaOperador = new PesquisaOperador();
    KoBindings(_pesquisaOperador, "knockoutPesquisaRestricaoVisualizacaoCanhotos", false, _pesquisaOperador.Pesquisar.id);

    _restricao = new Restricao();
    KoBindings(_restricao, "knockoutRestricao");

    HeaderAuditoria("OperadorCanhoto", _operador, "CodigoOperadorCanhoto");
    buscarOperadores();

    loadFilial();
    loadTipoOperacao();
    loadTipoCarga();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        $("#liRestricao").show();
        $("#liRestricao").attr("class", "active");
        $("#liFiliais").show();
        $("#liTipoCarga").show();
        $("#liTipoOperacao").show();
    }
}

function atualizarClick(e, sender) {
    preencherListasSelecao();
    Salvar(e, "RestricaoVisualizacaoCanhotos/Salvar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Operacional.RestricaoVisualizacaoCanhotos.Sucesso, Localization.Resources.Operacional.RestricaoVisualizacaoCanhotos.AtualizadoSucesso);
                limparCamposRestricaoVisualizacaoCanhotos();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Operacional.RestricaoVisualizacaoCanhotos.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Operacional.RestricaoVisualizacaoCanhotos.Falha, arg.Msg);
        }
    }, sender);
}

function cancelarClick(e) {
    limparCamposRestricaoVisualizacaoCanhotos();
}

function preencherListasSelecao() {
    _operador.TiposOperacao.val(JSON.stringify(_tipoOperacao.Operacao.basicTable.BuscarRegistros()));
    _operador.Filiais.val(JSON.stringify(_filial.Filial.basicTable.BuscarRegistros()));
    //_operador.TiposCarga.val(JSON.stringify(_tipoCarga.TipoCarga.basicTable.BuscarRegistros()));

    _operador.RestricaoAtiva.val(_restricao.RestricaoAtiva.val());
    _operador.PossuiFiltroTipoOperacao.val(_tipoOperacao.PossuiFiltroTipoOperacao.val());
    _operador.VisualizaCargasSemTipoOperacao.val(_tipoOperacao.VisualizaCargasSemTipoOperacao.val());
}

function editarOperador(e) {
    $("#wid-id-4").show();
    _operador.Codigo.val(e.Codigo);

    BuscarPorCodigo(_operador, "RestricaoVisualizacaoCanhotos/BuscarPorOperador", function (arg) {
        recarregarGridFilial();
        recarregarGridTipoOperacao();
        recarregarTiposCarga();

        _restricao.RestricaoAtiva.val(_operador.RestricaoAtiva.val());

        _pesquisaOperador.ExibirFiltros.visibleFade(false);
        _operador.Atualizar.visible(true);
        _operador.Cancelar.visible(true);
    });
}

//*******MÉTODOS*******

function buscarOperadores() {
    var editar = { descricao: Localization.Resources.Operacional.RestricaoVisualizacaoCanhotos.Configurar, id: "clasEditar", evento: "onclick", metodo: editarOperador, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridOperador = new GridView(_pesquisaOperador.Pesquisar.idGrid, "Usuario/PesquisarOperadores", _pesquisaOperador, menuOpcoes, null);
    _gridOperador.CarregarGrid();
}

function limparCamposRestricaoVisualizacaoCanhotos() {
    _operador.Atualizar.visible(false);
    _operador.Cancelar.visible(false);
    _pesquisaOperador.ExibirFiltros.visibleFade(true);

    LimparCampos(_operador);
    limparCamposTipoOperacao();
    limparCamposFilial();

    recarregarGridTipoOperacao();
    recarregarGridFilial();

    $("#wid-id-4").hide();
    $("#divTiposCargaParent").hide();
    Global.ResetarAbas();
}