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

/******* MAPEAMENTO KNOCKOUT ******/

var _pesquisaControleThread;
var _controleThread;
var _crudControleThread;
var _gridControleThread;

var PesquisaControleThread = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ text: "Situação: ", val: ko.observable(true), options: Global.ObterOpcoesPesquisaBooleano("Ativo", "Inativo"), def: true });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridControleThread.CarregarGrid();
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
}

var ControleThread = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "Descrição: ", required: false, maxlength: 500, enable: ko.observable(true) });
    this.Ativo = PropertyEntity({ text: "Situação: ", val: ko.observable(true), options: Global.ObterOpcoesBooleano("Ativo", "Inativo"), def: true});
    this.DataCadastro = PropertyEntity({ text: "Data cadastro: ", getType: typesKnockout.date, def: "", val: ko.observable(), enable: ko.observable(true) });
    this.DataInicio = PropertyEntity({ text: "Data início: ", getType: typesKnockout.date, def: "", val: ko.observable(), enable: ko.observable(true) });
    this.DataFim = PropertyEntity({ text: "Data fim: ", getType: typesKnockout.date, def: "", val: ko.observable(), enable: ko.observable(true) });
    this.Tempo = PropertyEntity({ text: "Tempo: ", getType: typesKnockout.int, def: "", val: ko.observable()});
}

var CRUDControleThread = function () {
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

/******* EVENTOS *******/

function loadControleThread() {
    _pesquisaControleThread = new PesquisaControleThread();
    KoBindings(_pesquisaControleThread, "knockoutPesquisaControleThread", false, _pesquisaControleThread.Pesquisar.id);

    _controleThread = new ControleThread();
    KoBindings(_controleThread, "knockoutCadastroControleThread");

    _crudControleThread = new CRUDControleThread();
    KoBindings(_crudControleThread, "knockoutCRUDControleThread");

    HeaderAuditoria("ControleThread", _controleThread);

    buscarControleThread();
}

function atualizarClick(e, sender) {
    Salvar(_controleThread, "ControleThread/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridControleThread.CarregarGrid();
                limparCamposControleThread();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function cancelarClick(e, sender) {
    limparCamposControleThread();
}

/******* MÉTODOS *******/

function buscarControleThread() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarControleThread, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridControleThread = new GridView(_pesquisaControleThread.Pesquisar.idGrid, "ControleThread/Pesquisa", _pesquisaControleThread, menuOpcoes);
    _gridControleThread.CarregarGrid();
}

function editarControleThread(controleThreadGrid) {
    limparCamposControleThread();
    SetarEnableCamposKnockout(_controleThread, false);
    _controleThread.Codigo.val(controleThreadGrid.Codigo);
    BuscarPorCodigo(_controleThread, "ControleThread/BuscarPorCodigo", function (arg) {
        _pesquisaControleThread.ExibirFiltros.visibleFade(false);
        _crudControleThread.Atualizar.visible(true);
        _crudControleThread.Cancelar.visible(true);
    }, null);
}

function limparCamposControleThread() {
    _crudControleThread.Atualizar.visible(false);
    _crudControleThread.Cancelar.visible(false);
    SetarEnableCamposKnockout(_controleThread, true);
    LimparCampos(_controleThread);
}
