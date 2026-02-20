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
/// <reference path="Transportador.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _permissao;
var _pagina;

function PermissaoModel() {
    this.Permissoes = ko.observableArray();
}

function PaginaModel() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
}

//*******EVENTOS*******
var _Menu = new Array();

function loadPermissao() {

    _pagina = new PaginaModel();
    _permissao = new PermissaoModel();

    executarReST("Pagina/BuscarPaginas", { TipoAcesso: 0 }, function (r) {
        if (r.Success) {
            _PaginasGlobal = r.Data;
            $.each(r.Data, function (i, menu) {
                _permissao.Permissoes.push({ Menu: menu.menu, Paginas: menu.paginas, id: guid() });
                _Menu.push(menu);
            });
            adicionarPermissoesPadroesPaginas();
            KoBindings(_permissao, "knockoutCadastroPermissao");
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function adicionarPermissoesPadroesPaginas() {
    $.each(_Menu, function (i, menu) {
        $.each(menu.paginas, function (i, pg) {
            adicionarPermissao(pg);
        });
    });
}

function selecionarTodosPorGrupo(permissao) {
    var checked = $("#checkUncheck_" + permissao.id).prop("checked");

    $.each(permissao.Paginas, function (i, pg) {
        $("#chkPermissaoEmpresa_" + pg.Codigo).prop("checked", checked);

        if (checked)
            adicionarPermissao(pg);
        else
            removerPermissao(pg);
    });

    recarregarPermissoesUsuario();

    return true;
}

function selecionarPagina(pagina) {
    if ($("#chkPermissaoEmpresa_" + pagina.Codigo).prop("checked")) {
        adicionarPermissao(pagina);
    } else {
        removerPermissao(pagina);
    }

    recarregarPermissoesUsuario();

    return true;
}

function adicionarPermissao(pagina) {
    var existe = false;

    for (var i = 0; i < _transportador.Permissoes.list.length; i++) {
        if (_transportador.Permissoes.list[i].Codigo.val == pagina.Codigo) {
            existe = true;
            break;
        }
    }

    if (!existe) {
        _pagina.Codigo.val(pagina.Codigo);
        _pagina.Descricao.val(pagina.Descricao);

        _transportador.Permissoes.list.push(SalvarListEntity(_pagina));
    }
}

function removerPermissao(pagina) {
    for (var i = 0; i < _transportador.Permissoes.list.length; i++) {
        if (_transportador.Permissoes.list[i].Codigo.val == pagina.Codigo) {
            _transportador.Permissoes.list.splice(i, 1);
            break;
        }
    }
}

function limparPermissoes() {
    $("#divPermissoesPaginas input[type=checkbox]").each(function () {
        $(this).prop("checked", true);
    });
    adicionarPermissoesPadroesPaginas();
}


function desmarcarPermissoesTransportador() {
    $("#divPermissoesPaginas input[type=checkbox]").each(function () {
        $(this).prop("checked", false);
    });
}

function recarregarPermissoesTransportador() {
    
    desmarcarPermissoesTransportador();

    for (var i = 0; i < _transportador.Permissoes.list.length; i++) {
        $("#chkPermissaoEmpresa_" + _transportador.Permissoes.list[i].Codigo.val).prop("checked", true);
    }

    for (var i = 0; i < _permissao.Permissoes().length; i++) {
        if ($("#" + _permissao.Permissoes()[i].id + " input:checkbox:not(:checked)").length == 0)
            $("#checkUncheck_" + _permissao.Permissoes()[i].id).prop("checked", true);
    }
}