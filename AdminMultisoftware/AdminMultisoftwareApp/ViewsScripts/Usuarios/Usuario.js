/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/Global/Rest.js" />
/// <reference path="../../js/Global/Mensagem.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/bootstrap/bootstrap.js" />
/// <reference path="../../js/libs/jquery.blockui.js" />
/// <reference path="../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridUsuario;
var _usuario;
var _pesquisaUsuario;

var PesquisaUsuario = function () {
    this.Nome = PropertyEntity({ text: "Nome: " });
    this.Login = PropertyEntity({ text: "Usuário:" });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridUsuario.CarregarGrid();
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

var Usuario = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Senha = PropertyEntity({ text: "*Senha: ", maxlength: 50 });
    this.Login = PropertyEntity({ text: "*Usuário: ", required: true, maxlength: 50 });
    this.Nome = PropertyEntity({ text: "*Nome: ", required: true, maxlength: 50 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadUsuario() {

    _usuario = new Usuario();
    KoBindings(_usuario, "knockoutCadastroUsuario");

    _pesquisaUsuario = new PesquisaUsuario();
    KoBindings(_pesquisaUsuario, "knockoutPesquisaUsuario", false, _pesquisaUsuario.Pesquisar.id);;

    buscarUsuarios();

    HeaderAuditoria("Usuario", _usuario);
}

function adicionarClick(e, sender) {
    Salvar(e, "Usuario/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado");
                _gridUsuario.CarregarGrid();
                limparCamposUsuario();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "Usuario/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridUsuario.CarregarGrid();
                limparCamposUsuario();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a formulario " + _usuario.Nome.val() + "?", function () {
        ExcluirPorCodigo(_usuario, "Usuario/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridUsuario.CarregarGrid();
                limparCamposUsuario();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposUsuario();
}

//*******MÉTODOS*******


function buscarUsuarios() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarUsuario, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridUsuario = new GridView(_pesquisaUsuario.Pesquisar.idGrid, "Usuario/Pesquisa", _pesquisaUsuario, menuOpcoes, null);
    _gridUsuario.CarregarGrid();
}

function editarUsuario(usuarioGrid) {
    limparCamposUsuario();
    _usuario.Codigo.val(usuarioGrid.Codigo);
    BuscarPorCodigo(_usuario, "Usuario/BuscarPorCodigo", function (arg) {
        _pesquisaUsuario.ExibirFiltros.visibleFade(false);
        _usuario.Atualizar.visible(true);
        _usuario.Cancelar.visible(true);
        _usuario.Excluir.visible(true);
        _usuario.Adicionar.visible(false);
    }, null);
}

function limparCamposUsuario() {
    _usuario.Atualizar.visible(false);
    _usuario.Cancelar.visible(false);
    _usuario.Excluir.visible(false);
    _usuario.Adicionar.visible(true);
    LimparCampos(_usuario);
}
