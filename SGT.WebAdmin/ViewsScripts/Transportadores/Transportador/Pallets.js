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
/// <reference path="../../Consultas/ModelosVeicularesCargas.js" />
/// <reference path="../../Enumeradores/EnumTipoSerie.js" />
/// <reference path="Transportador.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _pallets;

var Pallets = function () {

    this.DiasRotatividadePallets = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Transportadores.Transportador.DiasRotatividade.getFieldDescription(), def: "", getType: typesKnockout.int, maxlength: 2 });

    this.DiasRotatividadePallets.val.subscribe(function (novoValor) {
        _transportador.DiasRotatividadePallets.val(novoValor);
    });
}


//*******EVENTOS*******

function loadPallets() {

    _pallets = new Pallets();
    KoBindings(_pallets, "knockoutPallets");

}

//*******METODOS*******

function alterarEstadoCadastroPallets() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador && _transportador.Codigo.val() > 0)
        $("#liTabPallets").removeClass("d-none");
    else
        $("#liTabPallets").addClass("d-none");
}