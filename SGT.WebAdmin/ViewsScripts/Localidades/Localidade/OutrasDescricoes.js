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
/// <reference path="Regiao.js" />



var _gridOutrasDescricoes;
var _outraDescricao;

//*******MAPEAMENTO KNOUCKOUT*******

var OutraDescricaoMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ type: types.map, val: "" });
}

//*******EVENTOS*******


function loadOutraDescricao() {
    var editar = { descricao: Localization.Resources.Localidades.Localidade.Remover, id: guid(), evento: "onclick", metodo: excluirOutraDescricaoClick, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);
    var header = [{ data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "85%", className: "text-align-left" }];
    _gridOutrasDescricoes = new BasicDataTable(_localidade.OutrasDescricoes.idGrid, header, menuOpcoes);
    recarregarGridOutrasDescricoes();
}

function adicionarOutraDescricaoClick() {
    _localidade.OutrasDescricoes.codEntity(_localidade.OutrasDescricoes.val());
    var tudoCerto = ValidarCampoObrigatorioEntity(_localidade.OutrasDescricoes);
    if (tudoCerto) {
        _outraDescricao = new OutraDescricaoMap();
        _outraDescricao.Codigo.val = _localidade.OutrasDescricoes.val();
        !string.IsNullOrWhiteSpace(_localidade.OutrasDescricoes.val())? _outraDescricao.Descricao.val = _localidade.OutrasDescricoes.val(): "";
        var existe = false;
        $.each(_localidade.OutrasDescricoes.list, function (i, OutraDescricao) {
            if (OutraDescricao.Codigo.val == _localidade.OutrasDescricoes.codEntity()) {
                existe = true;
                return false;
            }
        });
        if (!existe) {
            _localidade.OutrasDescricoes.list.push(_outraDescricao);
            recarregarGridOutrasDescricoes();
            $("#" + _localidade.OutrasDescricoes.id).focus();
        } else {
            exibirMensagem("aviso", Localization.Resources.Localidades.Localidade.OutraDescricaoJaInformada, Localization.Resources.Localidades.Localidade.AOutraDescricao + _localidade.OutrasDescricoes.val() + Localization.Resources.Localidades.Localidade.JaFoiInformadaParaEssaLocalidade);
        }
        LimparCampoEntity(_localidade.OutrasDescricoes);
    } else {
        exibirMensagem("atencao", Localization.Resources.Localidades.Localidade.CamposObrigatorios, Localization.Resources.Localidades.Localidade.InformeOsCamposObrigatorios);
    }
}


function excluirOutraDescricaoClick(data) {
    exibirConfirmacao(Localization.Resources.Localidades.Localidade.Confirmacao, Localization.Resources.Localidades.Localidade.RealmenteDesejaExcluirOutraDescricao + data.Descricao + "?", function () {
        var listaAtualizada = new Array();
        $.each(_localidade.OutrasDescricoes.list, function (i, OutraDescricao) {
            if (OutraDescricao.Descricao.val != data.Descricao) {
                listaAtualizada.push(OutraDescricao);
            }
        });
        _localidade.OutrasDescricoes.list = listaAtualizada;
        recarregarGridOutrasDescricoes();
    });
}


//*******MÉTODOS*******

function recarregarGridOutrasDescricoes() {
    var data = new Array();
    $.each(_localidade.OutrasDescricoes.list, function (i, OutraDescricao) {
        var obj = new Object();
        obj.Descricao = OutraDescricao.Descricao.val;
        data.push(obj);
    });
    _gridOutrasDescricoes.CarregarGrid(data);
}


