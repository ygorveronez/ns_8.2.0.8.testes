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

var _gridAreaEnvolvidaMotivoChamado;
var _areaEnvolvidaMotivoChamado;
var _crudAreaEnvolvidaMotivoChamado
var _pesquisaAreaEnvolvidaMotivoChamado;

var PesquisaAreaEnvolvidaMotivoChamado = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridAreaEnvolvidaMotivoChamado.CarregarGrid();
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

var AreaEnvolvidaMotivoChamado = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 200 });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de Integração: ", required: false, maxlength: 500 });
    this.Status = PropertyEntity({ text: "*Status: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true), enable: ko.observable(true) });
};

var CRUDAreaEnvolvidaMotivoChamado = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadAreaEnvolvidaMotivoChamado() {
    _areaEnvolvidaMotivoChamado = new AreaEnvolvidaMotivoChamado();
    KoBindings(_areaEnvolvidaMotivoChamado, "knockoutCadastroAreaEnvolvidaMotivoChamado");

    HeaderAuditoria("AreaEnvolvidaMotivoChamado", _areaEnvolvidaMotivoChamado);

    _crudAreaEnvolvidaMotivoChamado = new CRUDAreaEnvolvidaMotivoChamado();
    KoBindings(_crudAreaEnvolvidaMotivoChamado, "knockoutCRUDAreaEnvolvidaMotivoChamado");

    _pesquisaAreaEnvolvidaMotivoChamado = new PesquisaAreaEnvolvidaMotivoChamado();
    KoBindings(_pesquisaAreaEnvolvidaMotivoChamado, "knockoutPesquisaAreaEnvolvidaMotivoChamado", false, _pesquisaAreaEnvolvidaMotivoChamado.Pesquisar.id);

    buscarAreaEnvolvidaMotivoChamado();
}

function adicionarClick(e, sender) {
    Salvar(_areaEnvolvidaMotivoChamado, "AreaEnvolvidaMotivoChamado/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridAreaEnvolvidaMotivoChamado.CarregarGrid();
                limparCamposAreaEnvolvidaMotivoChamado();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_areaEnvolvidaMotivoChamado, "AreaEnvolvidaMotivoChamado/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridAreaEnvolvidaMotivoChamado.CarregarGrid();
                limparCamposAreaEnvolvidaMotivoChamado();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a àrea envolvida " + _areaEnvolvidaMotivoChamado.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_areaEnvolvidaMotivoChamado, "AreaEnvolvidaMotivoChamado/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridAreaEnvolvidaMotivoChamado.CarregarGrid();
                    limparCamposAreaEnvolvidaMotivoChamado();
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
    limparCamposAreaEnvolvidaMotivoChamado();
}

//*******MÉTODOS*******

function buscarAreaEnvolvidaMotivoChamado() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarAreaEnvolvidaMotivoChamado, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridAreaEnvolvidaMotivoChamado = new GridView(_pesquisaAreaEnvolvidaMotivoChamado.Pesquisar.idGrid, "AreaEnvolvidaMotivoChamado/Pesquisa", _pesquisaAreaEnvolvidaMotivoChamado, menuOpcoes);
    _gridAreaEnvolvidaMotivoChamado.CarregarGrid();
}

function editarAreaEnvolvidaMotivoChamado(registroSelecionado) {
    limparCamposAreaEnvolvidaMotivoChamado();

    _areaEnvolvidaMotivoChamado.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_areaEnvolvidaMotivoChamado, "AreaEnvolvidaMotivoChamado/BuscarPorCodigo", function (arg) {
        _pesquisaAreaEnvolvidaMotivoChamado.ExibirFiltros.visibleFade(false);
        _crudAreaEnvolvidaMotivoChamado.Atualizar.visible(true);
        _crudAreaEnvolvidaMotivoChamado.Cancelar.visible(true);
        _crudAreaEnvolvidaMotivoChamado.Excluir.visible(true);
        _crudAreaEnvolvidaMotivoChamado.Adicionar.visible(false);
    }, null);
}

function limparCamposAreaEnvolvidaMotivoChamado() {
    _crudAreaEnvolvidaMotivoChamado.Atualizar.visible(false);
    _crudAreaEnvolvidaMotivoChamado.Cancelar.visible(false);
    _crudAreaEnvolvidaMotivoChamado.Excluir.visible(false);
    _crudAreaEnvolvidaMotivoChamado.Adicionar.visible(true);
    LimparCampos(_areaEnvolvidaMotivoChamado);
}