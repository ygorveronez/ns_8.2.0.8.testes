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

var _gridInstanciaBase;
var instanciaBase;
var _pesquisaInstanciaBase;

var PesquisaInstanciaBase = function () {
    this.Servidor = PropertyEntity({ text: "Servidor: " });
    this.Usuario = PropertyEntity({ text: "Usuário: " });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridInstanciaBase.CarregarGrid();
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

var InstanciaBase = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Servidor = PropertyEntity({ text: "*Servidor: ", required: true });
    this.Usuario = PropertyEntity({ text: "*Usuário: ", required: true });
    this.Senha = PropertyEntity({ text: "*Senha: ", required: true });
    this.Porta = PropertyEntity({ text: "*Porta: ", required: true, getType: typesKnockout.int });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadInstanciaBase() {

    instanciaBase = new InstanciaBase();
    KoBindings(instanciaBase, "knockoutCadastroInstanciaBase");

    _pesquisaInstanciaBase = new PesquisaInstanciaBase();
    KoBindings(_pesquisaInstanciaBase, "knockoutPesquisaInstanciaBase", false, _pesquisaInstanciaBase.Pesquisar.id);

    buscarInstanciaBase();

    HeaderAuditoria("InstanciaBase", instanciaBase);
}

function adicionarClick(e, sender) {
    instanciaBase.Senha.required = true;
    Salvar(e, "InstanciaBase/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado");
                _gridInstanciaBase.CarregarGrid();
                limparInstanciaBase();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    instanciaBase.Senha.required = false;
    Salvar(e, "InstanciaBase/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridInstanciaBase.CarregarGrid();
                limparInstanciaBase();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir?", function () {
        ExcluirPorCodigo(instanciaBase, "InstanciaBase/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridInstanciaBase.CarregarGrid();
                limparInstanciaBase();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparInstanciaBase();
}

//*******MÉTODOS*******


function buscarInstanciaBase() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarInstanciaBase, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridInstanciaBase = new GridView(_pesquisaInstanciaBase.Pesquisar.idGrid, "InstanciaBase/Pesquisa", _pesquisaInstanciaBase, menuOpcoes, null);
    _gridInstanciaBase.CarregarGrid();
}

function editarInstanciaBase(instanciaBaseAcessoGrid) {
    limparInstanciaBase();
    instanciaBase.Codigo.val(instanciaBaseAcessoGrid.Codigo);
    BuscarPorCodigo(instanciaBase, "InstanciaBase/BuscarPorCodigo", function (arg) {
        //_gridInstanciaBase.ExibirFiltros.visibleFade(false);
        instanciaBase.Atualizar.visible(true);
        instanciaBase.Cancelar.visible(true);
        instanciaBase.Excluir.visible(true);
        instanciaBase.Adicionar.visible(false);
    }, null);
}

function limparInstanciaBase() {
    instanciaBase.Atualizar.visible(false);
    instanciaBase.Cancelar.visible(false);
    instanciaBase.Excluir.visible(false);
    instanciaBase.Adicionar.visible(true);
    LimparCampos(instanciaBase);
}
