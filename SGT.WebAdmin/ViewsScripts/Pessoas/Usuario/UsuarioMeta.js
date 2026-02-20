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
/// <reference path="Usuario.js" />

var _gridUsuarioMeta;
//*******MAPEAMENTO KNOUCKOUT*******

var UsuarioMetaMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.DataInicial = PropertyEntity({ type: types.map, val: "" });
    this.DataFinal = PropertyEntity({ type: types.map, val: "" });
    this.Ativo = PropertyEntity({ type: types.map, val: "" });
    this.TipoMetaVendaDireta = PropertyEntity({ type: types.map, val: "" });
    this.PercentualMeta = PropertyEntity({ type: types.map, val: "" });
    this.DescricaoTipoMetaVendaDireta = PropertyEntity({ type: types.map, val: "" });
    this.DescricaoAtivo = PropertyEntity({ type: types.map, val: "" });
    this.DescricaoVigencia = PropertyEntity({ type: types.map, val: "" });
};

//*******EVENTOS*******

function loadUsuarioMetas() {

    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: editarUsuarioMeta, tamanho: "10", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    var header = [
        { data: "Codigo", visible: false },
        { data: "DataInicial", visible: false },
        { data: "DataFinal", visible: false },
        { data: "Ativo", visible: false },
        { data: "TipoMetaVendaDireta", visible: false },
        { data: "DescricaoTipoMetaVendaDireta", title: "Tipo da Meta", width: "35%", className: "text-align-left" },
        { data: "DescricaoAtivo", title: "Status", width: "25%", className: "text-align-left" },
        { data: "DescricaoVigencia", title: "Vigência", width: "15%", className: "text-align-left" },
        { data: "PercentualMeta", title: "Meta", width: "15%", className: "text-align-right" }
    ];

    _gridUsuarioMeta = new BasicDataTable(_usuario.GridMetas.idGrid, header, menuOpcoes);
    recarregarGridMetas();
}

function adicionarUsuarioMetaClick() {
    var tudoCerto = validarCamposObrigatoriosUsuarioMeta();

    if (tudoCerto) {
        var usuarioMeta = new UsuarioMetaMap();
        usuarioMeta.Codigo.val = guid();
        usuarioMeta.DataInicial.val = _usuario.DataInicialMeta.val();
        usuarioMeta.DataFinal.val = _usuario.DataFinalMeta.val();
        usuarioMeta.Ativo.val = _usuario.StatusMeta.val();
        usuarioMeta.TipoMetaVendaDireta.val = _usuario.TipoMetaVendaDireta.val();
        usuarioMeta.PercentualMeta.val = _usuario.PercentualMeta.val();
        usuarioMeta.DescricaoTipoMetaVendaDireta.val = EnumTipoMetaVendaDireta.obterDescricao(_usuario.TipoMetaVendaDireta.val());
        usuarioMeta.DescricaoAtivo.val = _usuario.PercentualMeta.val() ? "Ativo" : "Inativo";
        usuarioMeta.DescricaoVigencia.val = _usuario.DataInicialMeta.val() + " até " + _usuario.DataFinalMeta.val();

        _usuario.GridMetas.list.push(usuarioMeta);
        recarregarGridMetas();
        $("#" + _usuario.DataInicialMeta.id).focus();
        LimparCamposUsuarioMeta();
    } else {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function atualizarUsuarioMetaClick() {
    var tudoCerto = validarCamposObrigatoriosUsuarioMeta();

    if (tudoCerto) {
        $.each(_usuario.GridMetas.list, function (i, usuarioMeta) {
            if (usuarioMeta.Codigo.val == _usuario.CodigoMeta.val()) {

                usuarioMeta.DataInicial.val = _usuario.DataInicialMeta.val();
                usuarioMeta.DataFinal.val = _usuario.DataFinalMeta.val();
                usuarioMeta.Ativo.val = _usuario.StatusMeta.val();
                usuarioMeta.TipoMetaVendaDireta.val = _usuario.TipoMetaVendaDireta.val();
                usuarioMeta.PercentualMeta.val = _usuario.PercentualMeta.val();
                usuarioMeta.DescricaoTipoMetaVendaDireta.val = EnumTipoMetaVendaDireta.obterDescricao(_usuario.TipoMetaVendaDireta.val());
                usuarioMeta.DescricaoAtivo.val = _usuario.PercentualMeta.val() ? "Ativo" : "Inativo";
                usuarioMeta.DescricaoVigencia.val = _usuario.DataInicialMeta.val() + " até " + _usuario.DataFinalMeta.val();

                return false;
            }
        });
        recarregarGridMetas();
        LimparCamposUsuarioMeta();
    } else {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function excluirUsuarioMetaClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, "Realmente deseja excluir a meta selecionada?", function () {
        var listaAtualizada = new Array();
        $.each(_usuario.GridMetas.list, function (i, usuarioMeta) {
            if (usuarioMeta.Codigo.val != _usuario.CodigoMeta.val()) {
                listaAtualizada.push(usuarioMeta);
            }
        });
        _usuario.GridMetas.list = listaAtualizada;
        recarregarGridMetas();
        LimparCamposUsuarioMeta();
    });
}

//*******MÉTODOS*******

function recarregarGridMetas() {
    var data = new Array();
    $.each(_usuario.GridMetas.list, function (i, epi) {
        var usuarioMeta = new Object();

        usuarioMeta.Codigo = epi.Codigo.val;
        usuarioMeta.DataInicial = epi.DataInicial.val;
        usuarioMeta.DataFinal = epi.DataFinal.val;
        usuarioMeta.Ativo = epi.Ativo.val;
        usuarioMeta.TipoMetaVendaDireta = epi.TipoMetaVendaDireta.val;
        usuarioMeta.DescricaoTipoMetaVendaDireta = epi.DescricaoTipoMetaVendaDireta.val;
        usuarioMeta.DescricaoAtivo = epi.DescricaoAtivo.val;
        usuarioMeta.DescricaoVigencia = epi.DescricaoVigencia.val;
        usuarioMeta.PercentualMeta = epi.PercentualMeta.val;

        data.push(usuarioMeta);
    });
    _gridUsuarioMeta.CarregarGrid(data);
}

function editarUsuarioMeta(data) {
    LimparCamposUsuarioMeta();
    $.each(_usuario.GridMetas.list, function (i, usuarioMeta) {
        if (usuarioMeta.Codigo.val == data.Codigo) {

            _usuario.CodigoMeta.val(usuarioMeta.Codigo.val);
            _usuario.DataInicialMeta.val(usuarioMeta.DataInicial.val);
            _usuario.DataFinalMeta.val(usuarioMeta.DataFinal.val);
            _usuario.PercentualMeta.val(usuarioMeta.PercentualMeta.val);
            _usuario.TipoMetaVendaDireta.val(usuarioMeta.TipoMetaVendaDireta.val);
            _usuario.StatusMeta.val(usuarioMeta.Ativo.val);

            return false;
        }
    });

    _usuario.AdicionarMeta.visible(false);
    _usuario.AtualizarMeta.visible(true);
    _usuario.ExcluirMeta.visible(true);
    _usuario.CancelarMeta.visible(true);
}

function LimparCamposUsuarioMeta() {
    _usuario.CodigoMeta.val(0);   
    _usuario.DataInicialMeta.val("");
    _usuario.DataFinalMeta.val("");
    LimparCampo(_usuario.PercentualMeta);
    LimparCampo(_usuario.TipoMetaVendaDireta);
    LimparCampo(_usuario.StatusMeta);

    _usuario.AdicionarMeta.visible(true);
    _usuario.AtualizarMeta.visible(false);
    _usuario.ExcluirMeta.visible(false);
    _usuario.CancelarMeta.visible(false);
}

function validarCamposObrigatoriosUsuarioMeta() {
    var valido = true;

    if (_usuario.PercentualMeta.val() == 0) {
        _usuario.PercentualMeta.requiredClass("form-control is-invalid");
        valido = false;
    } else {
        _usuario.PercentualMeta.requiredClass("form-control");
    }

    if (_usuario.DataInicialMeta.val() == "") {
        _usuario.DataInicialMeta.requiredClass("form-control is-invalid");
        valido = false;
    } else {
        _usuario.DataInicialMeta.requiredClass("form-control");
    }

    if (_usuario.DataFinalMeta.val() == "") {
        _usuario.DataFinalMeta.requiredClass("form-control is-invalid");
        valido = false;
    } else {
        _usuario.DataFinalMeta.requiredClass("form-control");
    }

    return valido;
}