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


//*******MAPEAMENTO KNOUCKOUT*******

var _gridTipoMovimentoArquivoContabil;
var _tipoMovimentoArquivoContabil;
var _pesquisaTipoMovimentoArquivoContabil;
var _crudTipoMovimentoArquivoContabil;

var PesquisaTipoMovimentoArquivoContabil = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTipoMovimentoArquivoContabil.CarregarGrid();
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
};

var TipoMovimentoArquivoContabil = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 500 });
    this.Ativo = PropertyEntity({ text: "*Status: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true), enable: ko.observable(true) });

    this.TiposMovimentos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
};

var CRUDTipoMovimentoArquivoContabil = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadTipoMovimentoArquivoContabil() {
    _tipoMovimentoArquivoContabil = new TipoMovimentoArquivoContabil();
    KoBindings(_tipoMovimentoArquivoContabil, "knockoutCadastroTipoMovimentoArquivoContabil");

    HeaderAuditoria("TipoMovimentoArquivoContabil", _tipoMovimentoArquivoContabil);

    _crudTipoMovimentoArquivoContabil = new CRUDTipoMovimentoArquivoContabil();
    KoBindings(_crudTipoMovimentoArquivoContabil, "knockoutCRUDTipoMovimentoArquivoContabil");

    _pesquisaTipoMovimentoArquivoContabil = new PesquisaTipoMovimentoArquivoContabil();
    KoBindings(_pesquisaTipoMovimentoArquivoContabil, "knockoutPesquisaTipoMovimentoArquivoContabil", false, _pesquisaTipoMovimentoArquivoContabil.Pesquisar.id);

    LoadTipoMovimento();

    buscarTipoMovimentoArquivoContabil();
}

function adicionarClick(e, sender) {
    preencherListasSelecaoTipoMovimentoArquivoContabil();
    Salvar(_tipoMovimentoArquivoContabil, "TipoMovimentoArquivoContabil/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridTipoMovimentoArquivoContabil.CarregarGrid();
                limparCamposTipoMovimentoArquivoContabil();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    preencherListasSelecaoTipoMovimentoArquivoContabil();
    Salvar(_tipoMovimentoArquivoContabil, "TipoMovimentoArquivoContabil/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTipoMovimentoArquivoContabil.CarregarGrid();
                limparCamposTipoMovimentoArquivoContabil();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o tipo " + _tipoMovimentoArquivoContabil.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_tipoMovimentoArquivoContabil, "TipoMovimentoArquivoContabil/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridTipoMovimentoArquivoContabil.CarregarGrid();
                    limparCamposTipoMovimentoArquivoContabil();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposTipoMovimentoArquivoContabil();
}

function preencherListasSelecaoTipoMovimentoArquivoContabil() {
    _tipoMovimentoArquivoContabil.TiposMovimentos.val(JSON.stringify(_tipoMovimento.TipoMovimento.basicTable.BuscarRegistros()));
}

//*******MÉTODOS*******


function buscarTipoMovimentoArquivoContabil() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarTipoMovimentoArquivoContabil, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridTipoMovimentoArquivoContabil = new GridView(_pesquisaTipoMovimentoArquivoContabil.Pesquisar.idGrid, "TipoMovimentoArquivoContabil/Pesquisa", _pesquisaTipoMovimentoArquivoContabil, menuOpcoes, null);
    _gridTipoMovimentoArquivoContabil.CarregarGrid();
}

function editarTipoMovimentoArquivoContabil(tipoMovimentoArquivoContabilGrid) {
    limparCamposTipoMovimentoArquivoContabil();
    _tipoMovimentoArquivoContabil.Codigo.val(tipoMovimentoArquivoContabilGrid.Codigo);
    BuscarPorCodigo(_tipoMovimentoArquivoContabil, "TipoMovimentoArquivoContabil/BuscarPorCodigo", function (arg) {
        _pesquisaTipoMovimentoArquivoContabil.ExibirFiltros.visibleFade(false);
        _crudTipoMovimentoArquivoContabil.Atualizar.visible(true);
        _crudTipoMovimentoArquivoContabil.Cancelar.visible(true);
        _crudTipoMovimentoArquivoContabil.Excluir.visible(true);
        _crudTipoMovimentoArquivoContabil.Adicionar.visible(false);

        RecarregarGridTipoMovimento();
    }, null);
}

function limparCamposTipoMovimentoArquivoContabil() {
    _crudTipoMovimentoArquivoContabil.Atualizar.visible(false);
    _crudTipoMovimentoArquivoContabil.Cancelar.visible(false);
    _crudTipoMovimentoArquivoContabil.Excluir.visible(false);
    _crudTipoMovimentoArquivoContabil.Adicionar.visible(true);
    LimparCampos(_tipoMovimentoArquivoContabil);
    LimparCamposTipoMovimento();
    Global.ResetarAbas();
}