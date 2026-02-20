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

//*******MAPEAMENTO KNOUCKOUT*******

var _empresaEmissora;

var EmpresaEmissora = function () {
    this.EmpresaEmissora = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Filiais.Filial.EmpresaEmissora.getFieldDescription(), idBtnSearch: guid() });
    this.EmiteMDFeFilialEmissora = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.EmitirMDFeCargasPorEstaFilialEmissora, issue: 1296, def: false });
    this.UtilizarCtesAnterioresComoCteFilialEmissora = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.UtilizarCtesAnterioresComoCteFilialEmissora, def: false, visible: ko.observable(false) });

    this.mostrarUtilizarCtesAnterioresComoCteFilialEmissora = ko.computed(function () {
        if (this.EmpresaEmissora.val() && this.EmiteMDFeFilialEmissora.val())
            return true;
        this.UtilizarCtesAnterioresComoCteFilialEmissora.val(false);
        return false;
    }, this);
}


//*******EVENTOS*******
function loadEmpresaEmissora() {
    _empresaEmissora = new EmpresaEmissora();
    KoBindings(_empresaEmissora, "knockoutEmpresaEmissora");

    BuscarTransportadores(_empresaEmissora.EmpresaEmissora);
}

//*******METODOS*******
function limparCamposEmpresaEmissora() {
    LimparCampos(_empresaEmissora);
}