//*******MAPEAMENTO KNOUCKOUT*******

var DownloadCONEMB = function () {
    this.LayoutEDI = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Layout:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: true });

    this.Download = PropertyEntity({
        eventClick: function (e) {
            DownloadCONEMBClick();
        }, type: types.event, text: "Download", idGrid: guid(), icon: "fa fa-download"
    });
};
var _modalKnockoutDownloadCONEMB;
//*******EVENTOS*******

function LoadDownloadCONEMB() {
    _downloadCONEMB = new DownloadCONEMB();
    KoBindings(_downloadCONEMB, "knockoutDownloadCONEMB");

    new BuscarLayoutsEDI(_downloadCONEMB.LayoutEDI, null, null, null, null, [EnumTipoLayoutEDI.CONEMB, EnumTipoLayoutEDI.CONEMB_CT_IMP, EnumTipoLayoutEDI.CONEMB_CT_EXP, EnumTipoLayoutEDI.CONEMB_MB]);

    _pesquisaCTe.LayoutCONEMB = _downloadCONEMB.LayoutEDI;
    _modalKnockoutDownloadCONEMB = new bootstrap.Modal(document.getElementById("knockoutDownloadCONEMB"), { backdrop: true, keyboard: true });
}

function AbrirTelaDownloadCONEMB() {
    _modalKnockoutDownloadCONEMB.show();
}

function DownloadCONEMBClick() {
    var dados = RetornarObjetoPesquisa(_pesquisaCTe);

    executarDownload("ConsultaCTe/DownloadCONEMB", dados, function () {
        _modalKnockoutDownloadCONEMB.hide();
    });
}