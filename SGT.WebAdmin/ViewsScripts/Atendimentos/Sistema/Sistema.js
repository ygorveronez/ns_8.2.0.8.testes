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

//*******MAPEAMENTO KNOUCKOUT*******

var _gridSistema;
var _sistema;
var _pesquisaSistema;

var PesquisaSistema = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Status = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Status: " });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridSistema.CarregarGrid();
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

var Sistema = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true, maxlength: 300 });
    this.Status = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Status: " });
    this.HorarioAtendimento = PropertyEntity({ text: "Descrição do Horário de Atendimento", maxlength: 5000 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadSistema() {
    _sistema = new Sistema();
    KoBindings(_sistema, "knockoutCadastroSistema");

    _pesquisaSistema = new PesquisaSistema();
    KoBindings(_pesquisaSistema, "knockoutPesquisaSistema", false, _pesquisaSistema.Pesquisar.id);

    buscarSistemas();
}

function adicionarClick(e, sender) {
    Salvar(e, "Sistema/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridSistema.CarregarGrid();
                limparCamposSistema();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "Sistema/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridSistema.CarregarGrid();
                limparCamposSistema();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o Sistema " + _sistema.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_sistema, "Sistema/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridSistema.CarregarGrid();
                limparCamposSistema();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposSistema();
}

//*******MÉTODOS*******


function buscarSistemas() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarSistema, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridSistema = new GridView(_pesquisaSistema.Pesquisar.idGrid, "Sistema/Pesquisa", _pesquisaSistema, menuOpcoes, null);
    _gridSistema.CarregarGrid();
}

function editarSistema(sistemaGrid) {
    limparCamposSistema();
    _sistema.Codigo.val(sistemaGrid.Codigo);
    BuscarPorCodigo(_sistema, "Sistema/BuscarPorCodigo", function (arg) {
        _pesquisaSistema.ExibirFiltros.visibleFade(false);
        _sistema.Atualizar.visible(true);
        _sistema.Cancelar.visible(true);
        _sistema.Excluir.visible(true);
        _sistema.Adicionar.visible(false);
    }, null);
}

function limparCamposSistema() {
    _sistema.Atualizar.visible(false);
    _sistema.Cancelar.visible(false);
    _sistema.Excluir.visible(false);
    _sistema.Adicionar.visible(true);
    LimparCampos(_sistema);
}
