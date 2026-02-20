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
/// <reference path="../../Consultas/EPI.js" />
/// <reference path="Usuario.js" />

var _gridUsuarioEPI;
//*******MAPEAMENTO KNOUCKOUT*******

var UsuarioEPIMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.CodigoEPI = PropertyEntity({ type: types.map, val: "" });
    this.DescricaoEPI = PropertyEntity({ type: types.map, val: "" });
    this.DataRepasse = PropertyEntity({ type: types.map, val: "" });
    this.SerieEPI = PropertyEntity({ type: types.map, val: "" });
    this.Quantidade = PropertyEntity({ type: types.map, val: "" });
};

//*******EVENTOS*******

function loadUsuarioEPIs() {
    new BuscarEPI(_usuario.EPI);

    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: editarUsuarioEPI, tamanho: "10", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoEPI", visible: false },
        { data: "DescricaoEPI", title: Localization.Resources.Pessoas.Usuario.EPI, width: "45%", className: "text-align-left" },
        { data: "DataRepasse", title: Localization.Resources.Pessoas.Usuario.DataRepasseEPI, width: "15%", className: "text-align-center" },
        { data: "SerieEPI", title: Localization.Resources.Pessoas.Usuario.SerieEPI, width: "15%", className: "text-align-left" },
        { data: "Quantidade", title: Localization.Resources.Pessoas.Usuario.QuantidadeEPI, width: "15%", className: "text-align-right" }
    ];

    _gridUsuarioEPI = new BasicDataTable(_usuario.GridEPIs.idGrid, header, menuOpcoes);
    recarregarGridEPIs();
}

function adicionarUsuarioEPIClick() {
    var tudoCerto = validarCamposObrigatoriosUsuarioEPI();

    if (tudoCerto) {
        var usuarioEPI = new UsuarioEPIMap();
        usuarioEPI.Codigo.val = guid();
        usuarioEPI.CodigoEPI.val = _usuario.EPI.codEntity();
        usuarioEPI.DescricaoEPI.val = _usuario.EPI.val();
        usuarioEPI.DataRepasse.val = _usuario.DataRepasse.val();
        usuarioEPI.SerieEPI.val = _usuario.SerieEPI.val();
        usuarioEPI.Quantidade.val = _usuario.Quantidade.val();

        _usuario.GridEPIs.list.push(usuarioEPI);
        recarregarGridEPIs();
        $("#" + _usuario.Descricao.id).focus();
        LimparCamposUsuarioEPI();
    } else {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function atualizarUsuarioEPIClick() {
    var tudoCerto = validarCamposObrigatoriosUsuarioEPI();

    if (tudoCerto) {
        $.each(_usuario.GridEPIs.list, function (i, usuarioEPI) {
            if (usuarioEPI.Codigo.val == _usuario.CodigoEPI.val()) {

                usuarioEPI.CodigoEPI.val = _usuario.EPI.codEntity();
                usuarioEPI.DescricaoEPI.val = _usuario.EPI.val();
                usuarioEPI.DataRepasse.val = _usuario.DataRepasse.val();
                usuarioEPI.SerieEPI.val = _usuario.SerieEPI.val();
                usuarioEPI.Quantidade.val = _usuario.Quantidade.val();

                return false;
            }
        });
        recarregarGridEPIs();
        LimparCamposUsuarioEPI();
    } else {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function excluirUsuarioEPIClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Pessoas.Usuario.RealmenteDesejaExcluirEPISelecionado, function () {
        var listaAtualizada = new Array();
        $.each(_usuario.GridEPIs.list, function (i, usuarioEPI) {
            if (usuarioEPI.Codigo.val != _usuario.CodigoEPI.val()) {
                listaAtualizada.push(usuarioEPI);
            }
        });
        _usuario.GridEPIs.list = listaAtualizada;
        recarregarGridEPIs();
        LimparCamposUsuarioEPI();
    });
}

//*******MÉTODOS*******

function recarregarGridEPIs() {
    var data = new Array();
    $.each(_usuario.GridEPIs.list, function (i, epi) {
        var usuarioEPI = new Object();

        usuarioEPI.Codigo = epi.Codigo.val;
        usuarioEPI.CodigoEPI = epi.CodigoEPI.val;
        usuarioEPI.DescricaoEPI = epi.DescricaoEPI.val;
        usuarioEPI.DataRepasse = epi.DataRepasse.val;
        usuarioEPI.SerieEPI = epi.SerieEPI.val;
        usuarioEPI.Quantidade = epi.Quantidade.val;

        data.push(usuarioEPI);
    });
    _gridUsuarioEPI.CarregarGrid(data);
}

function editarUsuarioEPI(data) {
    LimparCamposUsuarioEPI();
    $.each(_usuario.GridEPIs.list, function (i, usuarioEPI) {
        if (usuarioEPI.Codigo.val == data.Codigo) {
            _usuario.CodigoEPI.val(usuarioEPI.Codigo.val);
            _usuario.EPI.codEntity(usuarioEPI.CodigoEPI.val);
            _usuario.EPI.val(usuarioEPI.DescricaoEPI.val);
            _usuario.DataRepasse.val(usuarioEPI.DataRepasse.val);
            _usuario.SerieEPI.val(usuarioEPI.SerieEPI.val);
            _usuario.Quantidade.val(usuarioEPI.Quantidade.val);

            return false;
        }
    });

    _usuario.AdicionarEPI.visible(false);
    _usuario.AtualizarEPI.visible(true);
    _usuario.ExcluirEPI.visible(true);
    _usuario.CancelarEPI.visible(true);
}

function LimparCamposUsuarioEPI() {
    _usuario.CodigoEPI.val(0);
    LimparCampoEntity(_usuario.EPI);
    _usuario.EPI.requiredClass("form-control");
    _usuario.DataRepasse.val("");
    _usuario.SerieEPI.val("");
    LimparCampo(_usuario.Quantidade);

    _usuario.AdicionarEPI.visible(true);
    _usuario.AtualizarEPI.visible(false);
    _usuario.ExcluirEPI.visible(false);
    _usuario.CancelarEPI.visible(false);
}

function validarCamposObrigatoriosUsuarioEPI() {
    var valido = true;
    if (_usuario.EPI.codEntity() == 0 || _usuario.EPI.val() == "") {
        _usuario.EPI.requiredClass("form-control is-invalid");
        valido = false;
    } else {
        _usuario.EPI.requiredClass("form-control");
    }

    if (_usuario.Quantidade.val() == 0) {
        _usuario.Quantidade.requiredClass("form-control is-invalid");
        valido = false;
    } else {
        _usuario.Quantidade.requiredClass("form-control");
    }

    return valido;
}