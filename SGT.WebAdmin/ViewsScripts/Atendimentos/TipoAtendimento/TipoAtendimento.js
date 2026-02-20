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

var _gridTipoAtendimento;
var _tipoAtendimento;
var _pesquisaTipoAtendimento;

var PesquisaTipoAtendimento = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Status = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Status: " });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTipoAtendimento.CarregarGrid();
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

var TipoAtendimento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true, maxlength: 300 });
    this.Status = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Status: " });
    this.EnvioEmail = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, text: "Enviar por e-mail quando a tarefa for concluída?", def: true, visible: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadTipoAtendimento() {
    _tipoAtendimento = new TipoAtendimento();
    KoBindings(_tipoAtendimento, "knockoutCadastroTipoAtendimento");

    _pesquisaTipoAtendimento = new PesquisaTipoAtendimento();
    KoBindings(_pesquisaTipoAtendimento, "knockoutPesquisaTipoAtendimento", false, _pesquisaTipoAtendimento.Pesquisar.id);

    buscarTipoAtendimentos();
}

function adicionarClick(e, sender) {
    Salvar(e, "TipoAtendimento/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridTipoAtendimento.CarregarGrid();
                limparCamposTipoAtendimento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "TipoAtendimento/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTipoAtendimento.CarregarGrid();
                limparCamposTipoAtendimento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o Tipo do Atendimento " + _tipoAtendimento.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_tipoAtendimento, "TipoAtendimento/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridTipoAtendimento.CarregarGrid();
                limparCamposTipoAtendimento();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposTipoAtendimento();
}

//*******MÉTODOS*******


function buscarTipoAtendimentos() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarTipoAtendimento, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridTipoAtendimento = new GridView(_pesquisaTipoAtendimento.Pesquisar.idGrid, "TipoAtendimento/Pesquisa", _pesquisaTipoAtendimento, menuOpcoes, null);
    _gridTipoAtendimento.CarregarGrid();
}

function editarTipoAtendimento(tipoAtendimentoGrid) {
    limparCamposTipoAtendimento();
    _tipoAtendimento.Codigo.val(tipoAtendimentoGrid.Codigo);
    BuscarPorCodigo(_tipoAtendimento, "TipoAtendimento/BuscarPorCodigo", function (arg) {
        _pesquisaTipoAtendimento.ExibirFiltros.visibleFade(false);
        _tipoAtendimento.Atualizar.visible(true);
        _tipoAtendimento.Cancelar.visible(true);
        _tipoAtendimento.Excluir.visible(true);
        _tipoAtendimento.Adicionar.visible(false);
    }, null);
}

function limparCamposTipoAtendimento() {
    _tipoAtendimento.Atualizar.visible(false);
    _tipoAtendimento.Cancelar.visible(false);
    _tipoAtendimento.Excluir.visible(false);
    _tipoAtendimento.Adicionar.visible(true);
    LimparCampos(_tipoAtendimento);
}
