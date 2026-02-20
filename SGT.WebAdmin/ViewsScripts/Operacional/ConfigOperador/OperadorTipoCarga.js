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
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" /
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="OperadorTipoOperacao.js" />
/// <reference path="OperadorFilial.js" />
/// <reference path="ConfigOperador.js" />
/// <reference path="OperadorModeloVeicularCarga.js" />
/// <reference path="TabelaFrete.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _listaTipoCarga;
var _HTMLOperadorTipoCarga;
var _operadorTipoCarga;

var OperadorTipoCarga = function () {
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Operacional.ConfigOperador.TipoCarga.getRequiredFieldDescription(), idBtnSearch: guid() });
    this.OperadorTipoCargaModelosVeicular = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0) });
    this.Adicionar = PropertyEntity({ type: types.event, eventClick: adicionarTipoCargaClick, text: Localization.Resources.Operacional.ConfigOperador.Adicionar, visible: ko.observable(true) });
    this.Excluir = PropertyEntity({ type: types.event, eventClick: excluirTipoCargaClick, text: Localization.Resources.Operacional.ConfigOperador.Excluir, visible: ko.observable(false) });
}


//*******EVENTOS*******

function loadOperadorTipoCarga() {
    _operadorTipoCarga = new OperadorTipoCarga();
    KoBindings(_operadorTipoCarga, "knockoutOperadorTipoCarga");

    new BuscarTiposdeCarga(_operadorTipoCarga.TipoCarga, undefined, undefined, undefined, undefined, undefined, false);
    $.get("Content/Static/Operacional/OperadorTipoCarga.html?dyn=" + guid(), function (data) {
        _HTMLOperadorTipoCarga = data;
    });

    $("#" + _operadorTipoCarga.Excluir.id).droppable({
        drop: excluirTipoCargaClick,
        hoverClass: "drop-hover",
    });

}

function excluirTipoCargaClick(event, ui) {
    var idTipoCarga;
    $.each(_listaTipoCarga, function (j, item) {
        if (ui.draggable[0].id == item.id) {
            idTipoCarga = item.codTipoCarga;
            return false;
        }
    });

    $.each(_operador.OperadorTiposCarga.list, function (i, tipoCarga) {
        if (tipoCarga.TipoCarga.codEntity == idTipoCarga) {
            exibirConfirmacao(Localization.Resources.Operacional.ConfigOperador.Confirmacao, Localization.Resources.Operacional.ConfigOperador.RealmenteDesejaRemoverTipoCargaX.format(tipoCarga.TipoCarga.val), function () {
                _operador.OperadorTiposCarga.list.splice(i, 1);
                recarregarTiposCarga();
            });
            return false;
        }
    });

}

function adicionarTipoCargaClick(e) {
    var tudoCerto = ValidarCamposObrigatorios(_operadorTipoCarga);
    if (tudoCerto) {
        var existe = false;
        $.each(_operador.OperadorTiposCarga.list, function (i, tipoCarga) {
            if (tipoCarga.TipoCarga.codEntity == _operadorTipoCarga.TipoCarga.codEntity()) {
                existe = true;
                return false;
            }
        });
        if (!existe) {
            _operador.OperadorTiposCarga.list.push(SalvarListEntity(_operadorTipoCarga));
            //criarHTMLTipoCarga(_operadorTipoCarga);
            recarregarTiposCarga();
            $("#" + _operadorTipoCarga.TipoCarga.id).focus();
        } else {
            exibirMensagem("aviso", Localization.Resources.Operacional.ConfigOperador.TipoCargaJaInformado, Localization.Resources.Operacional.ConfigOperador.TipoCargaJaInformadoOperador);
        }
        LimparCampoEntity(_operadorTipoCarga.TipoCarga);
    } else {
        exibirMensagem("atencao", Localization.Resources.Operacional.ConfigOperador.CamposObrigatorios, Localization.Resources.Operacional.ConfigOperador.InformeCamposObrigatorios);
    }
}

//*******MÉTODOS*******

function recarregarTiposCarga() {
    $("#DivTiposDeCarga").html("");
    _listaTipoCarga = new Array();
    $.each(_operador.OperadorTiposCarga.list, function (i, operadorTipoCarga) {
        criarHTMLTipoCarga(operadorTipoCarga);
    });
}

function criarHTMLTipoCarga(operadorTipoCarga) {
    var idDivOperadorTipoCarga = guid();
    var idCollapTipoCarga = guid();
   
    $("#DivTiposDeCarga").prepend(
        _HTMLOperadorTipoCarga.replace(/#idDivOperadorTipoCarga/g, idDivOperadorTipoCarga)
        .replace(/#idCollapTipoCarga/g, idCollapTipoCarga)
        .replace(/#DescricaoTipoCarga/g, operadorTipoCarga.TipoCarga.val)
    );
    $('.collapse').attr("class", "panel-collapse collapse");
    $("#" + idCollapTipoCarga).draggable({ revert: true });
    $("#" + idCollapTipoCarga).on("dragstop", function (event, ui) {
        $("#" + idCollapTipoCarga).css('height','auto');
    });

    _listaTipoCarga.push({ id: idCollapTipoCarga, codTipoCarga: operadorTipoCarga.TipoCarga.codEntity });
    loadOperadorModeloVeicular(idDivOperadorTipoCarga, operadorTipoCarga);
    $("#divTiposCargaParent").show();
}
