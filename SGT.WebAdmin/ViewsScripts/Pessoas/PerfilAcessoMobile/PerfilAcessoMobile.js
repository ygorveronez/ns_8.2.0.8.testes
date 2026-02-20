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
/// <reference path="PerfilAcessoMobilePermissoes.js" />


//*******MAPEAMENTO KNOUCKOUT*******


var _gridPerfilAcessoMobile;
var _perfilAcessoMobile;
var _pesquisaPerfilAcessoMobile;

var PesquisaPerfilAcessoMobile = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridPerfilAcessoMobile.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() === true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var PerfilAcessoMobile = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", issue: 586, required: true });
    this.Status = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: ", issue: 557, required: true });

    this.PerfilAdministrador = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: "Este perfil tem permissões para todos os menus do Aplicativo?", visible: ko.observable(true) });

    this.FormulariosPerfilMobile = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0) });
    this.ModulosPerfilMobile = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******


function loadPerfilAcessoMobile() {
    _perfilAcessoMobile = new PerfilAcessoMobile();
    KoBindings(_perfilAcessoMobile, "knockoutCadastroPerfil");

    _pesquisaPerfilAcessoMobile = new PesquisaPerfilAcessoMobile();
    KoBindings(_pesquisaPerfilAcessoMobile, "knockoutPesquisaPerfil", false, _pesquisaPerfilAcessoMobile.Pesquisar.id);

    HeaderAuditoria("PerfilAcessoMobile", _perfilAcessoMobile);

    buscarPerfisMobile();
    buscarPaginasMobile();
}

function adicionarClick(e, sender) {
    buscarPermissoesFormulariosMobile();
    Salvar(e, "PerfilAcessoMobile/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso!");
                _gridPerfilAcessoMobile.CarregarGrid();
                limparCamposPerfilAcessoMobile();
            } else {
                exibirMensagem("aviso", "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function atualizarClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja alterar o perfil de acesso mobile? Isso implica na alteração de todos os usuários que possuem esse perfil de acesso mobile.", function () {
        buscarPermissoesFormulariosMobile();
        Salvar(e, "PerfilAcessoMobile/Atualizar", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridPerfilAcessoMobile.CarregarGrid();
                limparCamposPerfilAcessoMobile();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender, exibirCamposObrigatorio);
    });
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o perfil de acesso mobile " + _perfilAcessoMobile.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_perfilAcessoMobile, "PerfilAcessoMobile/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridPerfilAcessoMobile.CarregarGrid();
                    limparCamposPerfilAcessoMobile();
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
    limparCamposPerfilAcessoMobile();
}

//*******MÉTODOS*******


function buscarPerfisMobile() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarPerfil, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridPerfilAcessoMobile = new GridView(_pesquisaPerfilAcessoMobile.Pesquisar.idGrid, "PerfilAcessoMobile/Pesquisa", _pesquisaPerfilAcessoMobile, menuOpcoes, null);
    _gridPerfilAcessoMobile.CarregarGrid();
}

function editarPerfil(itemGrid) {
    limparCamposPerfilAcessoMobile();

    _perfilAcessoMobile.Codigo.val(itemGrid.Codigo);

    BuscarPorCodigo(_perfilAcessoMobile, "PerfilAcessoMobile/BuscarPorCodigo", function (arg) {
        setarPermissoesModulosFormulariosMobile();
        _pesquisaPerfilAcessoMobile.ExibirFiltros.visibleFade(false);

        _perfilAcessoMobile.Atualizar.visible(true);
        _perfilAcessoMobile.Cancelar.visible(true);
        _perfilAcessoMobile.Excluir.visible(true);
        _perfilAcessoMobile.Adicionar.visible(false);
    }, null);
}

function limparCamposPerfilAcessoMobile() {
    limparPermissoesModulosFormulariosMobile();

    _perfilAcessoMobile.Atualizar.visible(false);
    _perfilAcessoMobile.Cancelar.visible(false);
    _perfilAcessoMobile.Excluir.visible(false);
    _perfilAcessoMobile.Adicionar.visible(true);

    LimparCampos(_perfilAcessoMobile);
}

function exibirCamposObrigatorio() {
    resetarTabs();
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}

function resetarTabs() {
    $("#myTab a:first").tab("show");
}