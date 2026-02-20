/// <reference path="Permissoes.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Auditoria.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="PermissaoUsuario.js" />



//*******MAPEAMENTO KNOUCKOUT*******


var _gridPerfil;
var _perfil;
var _pesquisaPerfilAcesso;

var PesquisaPerfilAcesso = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridPerfil.CarregarGrid();
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

var PerfilAcesso = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", issue: 586,  required: true });
    this.Status = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: ", issue: 557, required: true });

    this.PerfilAdministrador = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: "Este perfil tem permissões de administrador? (acesso completo ao sistema)", issue: 599, visible: ko.observable(true) });

    this.FormulariosPerfil = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0) });
    this.ModulosPerfil = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadPerfilAcesso() {
    _perfil = new PerfilAcesso();
    KoBindings(_perfil, "knockoutCadastroPerfil");

    _pesquisaPerfilAcesso = new PesquisaPerfilAcesso();
    KoBindings(_pesquisaPerfilAcesso, "knockoutPesquisaPerfil");
    HeaderAuditoria("PerfilAcessoTransportador", _perfil)
    buscarPerfis();
    buscarPaginas();
}

function adicionarClick(e, sender) {
    buscarPermissoesFormularios();
    Salvar(e, "PerfilAcessoTransportador/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Perfil de Acesso cadastrado com sucesso!");
                _gridPerfil.CarregarGrid();
                limparCamposPerfil();
            } else {
                exibirMensagem("aviso", "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function atualizarClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja alterar o perfil de acesso? Isso implica na alteração de todos os transportadores que possuem esse perfil de acesso.", function () {
        buscarPermissoesFormularios();
        Salvar(e, "PerfilAcessoTransportador/Atualizar", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridPerfil.CarregarGrid();
                limparCamposPerfil();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender, exibirCamposObrigatorio);
    });
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o perfil de acesso " + _perfil.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_perfil, "PerfilAcessoTransportador/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridPerfil.CarregarGrid();
                    limparCamposPerfil();
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
    resetarTabs();
    limparCamposPerfil();
}

//*******MÉTODOS*******


function buscarPerfis() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarPerfil, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridPerfil = new GridView(_pesquisaPerfilAcesso.Pesquisar.idGrid, "PerfilAcessoTransportador/Pesquisa", _pesquisaPerfilAcesso, menuOpcoes, null);
    _gridPerfil.CarregarGrid();
}

function editarPerfil(itemGrid) {
    limparCamposPerfil();

    _perfil.Codigo.val(itemGrid.Codigo);

    BuscarPorCodigo(_perfil, "PerfilAcessoTransportador/BuscarPorCodigo", function (arg) {
        setarPermissoesModulosFormularios();
        _pesquisaPerfilAcesso.ExibirFiltros.visibleFade(false);

        _perfil.Atualizar.visible(true);
        _perfil.Cancelar.visible(true);
        _perfil.Excluir.visible(true);
        _perfil.Adicionar.visible(false);
    }, null);
}

function limparCamposPerfil() {
    limparPermissoesModulosFormularios();

    _perfil.Atualizar.visible(false);
    _perfil.Cancelar.visible(false);
    _perfil.Excluir.visible(false);
    _perfil.Adicionar.visible(true);

    LimparCampos(_perfil);
}

function exibirCamposObrigatorio() {
    resetarTabs();
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}

function resetarTabs() {

    $("#myTab a:first").tab("show");
}

