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
/// <reference path="Usuario.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _permissaoUsuario;
var _paginaUsuario;

var tipoPermissaoUsuario = {
    Acesso: 1,
    Incluir: 2,
    Alterar: 3,
    Excluir: 4
};

function PermissaoUsuarioModel() {
    this.Permissoes = ko.observableArray();
}

function PaginaUsuarioModel() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Acesso = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.Incluir = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.Alterar = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.Excluir = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
}

//*******EVENTOS*******

function loadPermissaoUsuario() {
    _paginaUsuario = new PaginaUsuarioModel();
    if (_permissaoUsuario == null) {
        _permissaoUsuario = new PermissaoUsuarioModel();
        KoBindings(_permissaoUsuario, "knockoutPermissaoUsuario");
    } else {
        _permissaoUsuario.Permissoes.removeAll();
    }
    
    recarregarPermissoesUsuario();
}

function selecionarTodosDoUsuarioPorGrupo(permissao) {
    var checked = $("#checkUncheckUsuario_" + permissao.id).prop("checked");

    $.each(permissao.Paginas, function (i, pg) {
        $("#chkAcessoUsuario_" + pg.Codigo).prop({ checked: checked });

        selecionarPaginaUsuario(pg, tipoPermissaoUsuario.Acesso);
    });

    return true;
}

function selecionarPaginaUsuario(pagina, tipo) {
    if (tipo == tipoPermissaoUsuario.Acesso) {
        var checked = $("#chkAcessoUsuario_" + pagina.Codigo).prop("checked");

        $("#chkIncluirUsuario_" + pagina.Codigo).prop({ checked: checked, disabled: !checked });
        $("#chkAlterarUsuario_" + pagina.Codigo).prop({ checked: checked, disabled: !checked });
        $("#chkExcluirUsuario_" + pagina.Codigo).prop({ checked: checked, disabled: !checked });
    }

    setarPermissaoUsuario(pagina);

    return true;
}

function setarPermissaoUsuario(pagina) {
    var existe = false;
    var acessoChecked = $("#chkAcessoUsuario_" + pagina.Codigo).prop("checked");
    var indicePermissao;

    for (var i = 0; i < _usuario.Permissoes.list.length; i++) {
        if (_usuario.Permissoes.list[i].Codigo.val == pagina.Codigo) {
            if (!acessoChecked) {
                _usuario.Permissoes.list.splice(i, 1);
                break;
            } else {
                existe = true;
                indicePermissao = i;
                break;
            }
        }
    }

    if (acessoChecked && !existe) {
        _paginaUsuario.Codigo.val(pagina.Codigo);
        _paginaUsuario.Descricao.val(pagina.Descricao);
        _paginaUsuario.Acesso.val(true);
        _paginaUsuario.Alterar.val($("#chkAlterarUsuario_" + pagina.Codigo).prop("checked"));
        _paginaUsuario.Excluir.val($("#chkExcluirUsuario_" + pagina.Codigo).prop("checked"));
        _paginaUsuario.Incluir.val($("#chkIncluirUsuario_" + pagina.Codigo).prop("checked"));

        _usuario.Permissoes.list.push(SalvarListEntity(_paginaUsuario));
    } else if (existe) {
        _usuario.Permissoes.list[indicePermissao].Alterar.val = $("#chkAlterarUsuario_" + pagina.Codigo).prop("checked");
        _usuario.Permissoes.list[indicePermissao].Excluir.val = $("#chkExcluirUsuario_" + pagina.Codigo).prop("checked");
        _usuario.Permissoes.list[indicePermissao].Incluir.val = $("#chkIncluirUsuario_" + pagina.Codigo).prop("checked");
    }
}

function limparPermissoesUsuario() {
    $("#divPermissoesPaginasUsuario input[type=checkbox]").each(function () {
        $(this).prop("checked", false);
    });
}

function recarregarPermissoesUsuario() {
    
    if (_transportador.Codigo.val() <= 0)
        return;

    limparPermissoesUsuario();

    _permissaoUsuario.Permissoes().splice(0, _permissaoUsuario.Permissoes().length)
    for (var i = 0; i < _permissao.Permissoes().length; i++) {
        var permissaoUsuario = { Menu: _permissao.Permissoes()[i].Menu, Paginas: new Array() };

        for (var x = 0; x < _permissao.Permissoes()[i].Paginas.length; x++) {
            permissaoUsuario.Paginas.push({
                Codigo: _permissao.Permissoes()[i].Paginas[x].Codigo,
                Descricao: _permissao.Permissoes()[i].Paginas[x].Descricao
            });
        }

        var permissaoExiste = false;

        for (var j = permissaoUsuario.Paginas.length - 1; j >= 0 ; j--) {
            for (var k = 0; k < _transportador.Permissoes.list.length; k++) {
                if (_transportador.Permissoes.list[k].Codigo.val == permissaoUsuario.Paginas[j].Codigo) {
                    permissaoExiste = true;
                    break;
                }
            }

            if (!permissaoExiste)
                permissaoUsuario.Paginas.splice(j, 1);

            permissaoExiste = false;
        }

        if (permissaoUsuario.Paginas.length > 0)
            _permissaoUsuario.Permissoes.push({ Menu: permissaoUsuario.Menu, Paginas: permissaoUsuario.Paginas, id: guid() });
    }

    for (var i = (_usuario.Permissoes.list.length - 1) ; i >= 0 ; i--) {
        if ($("#chkAcessoUsuario_" + _usuario.Permissoes.list[i].Codigo.val).length > 0) {
            $("#chkAcessoUsuario_" + _usuario.Permissoes.list[i].Codigo.val).prop({ checked: _usuario.Permissoes.list[i].Acesso.val });
            $("#chkIncluirUsuario_" + _usuario.Permissoes.list[i].Codigo.val).prop({ checked: _usuario.Permissoes.list[i].Incluir.val, disabled: !_usuario.Permissoes.list[i].Acesso.val });
            $("#chkAlterarUsuario_" + _usuario.Permissoes.list[i].Codigo.val).prop({ checked: _usuario.Permissoes.list[i].Alterar.val, disabled: !_usuario.Permissoes.list[i].Acesso.val });
            $("#chkExcluirUsuario_" + _usuario.Permissoes.list[i].Codigo.val).prop({ checked: _usuario.Permissoes.list[i].Excluir.val, disabled: !_usuario.Permissoes.list[i].Acesso.val });
        } else {
            _usuario.Permissoes.list.splice(i, 1);
        }
    }

    for (var i = 0; i < _permissaoUsuario.Permissoes().length; i++) {
        if ($("#" + _permissaoUsuario.Permissoes()[i].id + " input:checkbox:not(:checked)").length == 0)
            $("#checkUncheckUsuario_" + _permissaoUsuario.Permissoes()[i].id).prop("checked", true);
    }
}